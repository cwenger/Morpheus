﻿using System;
using System.Collections.Generic;
using System.Xml;
using Ionic.Zlib;

namespace Morpheus
{
    internal enum ArrayDataType
    {
        Unknown,
        MZ,
        Intensity
    }

    public partial class ProductSpectra : List<ProductSpectrum>
    {
        private const bool HARMONIC_CHARGE_DETECTION = false;

        private ProductSpectra() : base() { }

        public static ProductSpectra Load(string mzmlFilepath, int minimumAssumedPrecursorChargeState, int maximumAssumedPrecursorChargeState,
            double absoluteThreshold, double relativeThresholdPercent, int maximumNumberOfPeaks,
            bool assignChargeStates, bool deisotope, MassTolerance isotopicMzTolerance)
        {
            OnReportTaskWithoutProgress(new EventArgs());

            XmlDocument mzML = new XmlDocument();
            mzML.Load(mzmlFilepath);

            XmlNamespaceManager xnm = new XmlNamespaceManager(mzML.NameTable);
            xnm.AddNamespace("mzML", "http://psi.hupo.org/ms/mzml");

            Dictionary<string, XmlNodeList> referenceable_param_groups = new Dictionary<string, XmlNodeList>();
            foreach(XmlNode referenceable_param_group in mzML.SelectNodes("//mzML:mzML/mzML:referenceableParamGroupList/mzML:referenceableParamGroup", xnm))
            {
                referenceable_param_groups.Add(referenceable_param_group.Attributes["id"].Value, referenceable_param_group.ChildNodes);
            }

            int num_spectra = int.Parse(mzML.SelectSingleNode("//mzML:mzML/mzML:run/mzML:spectrumList", xnm).Attributes["count"].Value);

            ProductSpectra spectra = new ProductSpectra();

            OnReportTaskWithProgress(new EventArgs());
            int old_progress = 0;

            foreach(XmlNode node in mzML.SelectNodes("//mzML:mzML/mzML:run/mzML:spectrumList/mzML:spectrum", xnm))
            {
                int scan_index = int.Parse(node.Attributes["index"].Value);
                int scan_number = scan_index + 1;

                int ms_level = -1;
                double retention_time = double.NaN;
                double precursor_mz = double.NaN;
                int charge = 0;
                double precursor_intensity = double.NaN;
                string fragmentation_method = null;
                double[] mz = null;
                double[] intensity = null;
                foreach(XmlNode node2 in node.ChildNodes)
                {
                    if(node2.Name == "cvParam")
                    {
                        if(node2.Attributes["name"].Value == "ms level")
                        {
                            ms_level = int.Parse(node2.Attributes["value"].Value);
                        }
                    }
                    else if(node2.Name == "referenceableParamGroupRef")
                    {
                        foreach(XmlNode node3 in referenceable_param_groups[node2.Attributes["ref"].Value])
                        {
                            if(node3.Name == "cvParam")
                            {
                                if(node3.Attributes["name"].Value == "ms level")
                                {
                                    ms_level = int.Parse(node3.Attributes["value"].Value);
                                    break;
                                }
                            }
                        }
                    }
                    else if(node2.Name == "scanList")
                    {
                        foreach(XmlNode node3 in node2.SelectNodes("mzML:scan/mzML:cvParam", xnm))
                        {
                            if(node3.Attributes["name"].Value == "scan start time")
                            {
                                retention_time = double.Parse(node3.Attributes["value"].Value);
                            }
                        }
                    }
                    else if(node2.Name == "precursorList")
                    {
                        foreach(XmlNode node3 in node2.SelectNodes("mzML:precursor/mzML:selectedIonList/mzML:selectedIon/mzML:cvParam", xnm))
                        {
                            if(node3.Attributes["name"].Value == "selected ion m/z")
                            {
                                precursor_mz = double.Parse(node3.Attributes["value"].Value);
                            }
                            else if(node3.Attributes["name"].Value == "charge state")
                            {
                                charge = int.Parse(node3.Attributes["value"].Value);
                            }
                            else if(node3.Attributes["name"].Value == "peak intensity")
                            {
                                precursor_intensity = double.Parse(node3.Attributes["value"].Value);
                            }
                        }
                        fragmentation_method = node2.SelectSingleNode("mzML:precursor/mzML:activation/mzML:cvParam", xnm).Attributes["name"].Value;
                    }
                    else if(node2.Name == "binaryDataArrayList")
                    {
                        int word_length_in_bytes = 0;
                        bool zlib_compressed = false;
                        ArrayDataType array_data_type = ArrayDataType.Unknown;
                        foreach(XmlNode node3 in node2.SelectNodes("mzML:binaryDataArray/*", xnm))
                        {
                            if(node3.Name == "cvParam")
                            {
                                if(node3.Attributes["name"].Value == "32-bit float")
                                {
                                    word_length_in_bytes = 4;
                                }
                                else if(node3.Attributes["name"].Value == "64-bit float")
                                {
                                    word_length_in_bytes = 8;
                                }
                                else if(node3.Attributes["name"].Value == "zlib compression")
                                {
                                    zlib_compressed = true;
                                }
                                else if(node3.Attributes["name"].Value == "m/z array")
                                {
                                    array_data_type = ArrayDataType.MZ;
                                }
                                else if(node3.Attributes["name"].Value == "intensity array")
                                {
                                    array_data_type = ArrayDataType.Intensity;
                                }
                            }
                            else if(node3.Name == "binary")
                            {
                                if(array_data_type == ArrayDataType.MZ)
                                {
                                    mz = ReadBase64EncodedDoubleArray(node3.InnerText, word_length_in_bytes, zlib_compressed);
                                }
                                else if(array_data_type == ArrayDataType.Intensity)
                                {
                                    intensity = ReadBase64EncodedDoubleArray(node3.InnerText, word_length_in_bytes, zlib_compressed);
                                }
                            }
                        }
                    }
                    if(ms_level == 1)
                    {
                        break;
                    }
                }

                if(ms_level >= 2)
                {
                    List<MSPeak> peaks = new List<MSPeak>(mz.Length);
                    for(int i = 0; i < mz.Length; i++)
                    {
                        peaks.Add(new MSPeak(mz[i], intensity[i], 0));
                    }

                    peaks = FilterPeaks(peaks, absoluteThreshold, relativeThresholdPercent, maximumNumberOfPeaks);

                    if(charge == 0)
                    {
                        for(int c = minimumAssumedPrecursorChargeState; c <= maximumAssumedPrecursorChargeState; c++)
                        {
                            List<MSPeak> new_peaks = peaks;
                            if(assignChargeStates)
                            {
                                new_peaks = AssignChargeStates(new_peaks, c, isotopicMzTolerance);
                                if(deisotope)
                                {
                                    new_peaks = Deisotope(new_peaks, c, isotopicMzTolerance);
                                }
                            }

                            double precursor_mass = Utilities.MassFromMZ(precursor_mz, c);

                            ProductSpectrum spectrum = new ProductSpectrum(mzmlFilepath, scan_number, retention_time, fragmentation_method, precursor_mz, precursor_intensity, c, precursor_mass, new_peaks);
                            spectra.Add(spectrum);
                        }
                    }
                    else
                    {
                        if(assignChargeStates)
                        {
                            peaks = AssignChargeStates(peaks, charge, isotopicMzTolerance);
                            if(deisotope)
                            {
                                peaks = Deisotope(peaks, charge, isotopicMzTolerance);
                            }
                        }

                        double precursor_mass = Utilities.MassFromMZ(precursor_mz, charge);

                        ProductSpectrum spectrum = new ProductSpectrum(mzmlFilepath, scan_number, retention_time, fragmentation_method, precursor_mz, precursor_intensity, charge, precursor_mass, peaks);
                        spectra.Add(spectrum);
                    }
                }

                int new_progress = (int)((double)(scan_index + 1) / num_spectra * 100);
                if(new_progress > old_progress)
                {
                    OnUpdateProgress(new ProgressEventArgs(new_progress));
                    old_progress = new_progress;
                }
            }

            return spectra;
        }

        private static double[] ReadBase64EncodedDoubleArray(string s, int wordLengthInBytes, bool zlibCompressed)
        {
            byte[] bytes = Convert.FromBase64String(s);
            if(zlibCompressed)
            {
                bytes = ZlibStream.UncompressBuffer(bytes);
            }
            double[] doubles = new double[bytes.Length / wordLengthInBytes];
            for(int i = doubles.GetLowerBound(0); i <= doubles.GetUpperBound(0); i++)
            {
                if(wordLengthInBytes == 4)
                {
                    doubles[i] = BitConverter.ToSingle(bytes, i * wordLengthInBytes);
                }
                else if(wordLengthInBytes == 8)
                {
                    doubles[i] = BitConverter.ToDouble(bytes, i * wordLengthInBytes);
                }
            }
            return doubles;
        }

        private static List<MSPeak> AssignChargeStates(IList<MSPeak> peaks, int maxCharge, MassTolerance isotopicMzTolerance)
        {
            List<MSPeak> new_peaks = new List<MSPeak>();

            for(int i = 0; i < peaks.Count - 1; i++)
            {
                int j = i + 1;
                List<int> charges = new List<int>();
                while(j < peaks.Count)
                {
                    if(peaks[j].MZ > (peaks[i].MZ + Constants.C12_C13_MASS_DIFFERENCE) + isotopicMzTolerance)
                    {
                        break;
                    }

                    for(int c = maxCharge; c >= 1; c--)
                    {
                        // remove harmonic charges, e.g. don't consider peak as a +2 (0.5 Th spacing) if it could be a +4 (0.25 Th spacing)
                        if(HARMONIC_CHARGE_DETECTION)
                        {
                            bool harmonic = false;
                            foreach(int c2 in charges)
                            {
                                if(c2 % c == 0)
                                {
                                    harmonic = true;
                                    break;
                                }
                            }
                            if(harmonic)
                            {
                                continue;
                            }
                        }

                        if(Math.Abs(MassTolerance.CalculateMassError(peaks[j].MZ, peaks[i].MZ + Constants.C12_C13_MASS_DIFFERENCE / c, isotopicMzTolerance.Units)) <= isotopicMzTolerance.Value)
                        {
                            new_peaks.Add(new MSPeak(peaks[i].MZ, peaks[i].Intensity, c));
                            charges.Add(c);
                        }
                    }

                    j++;
                }
                if(charges.Count == 0)
                {
                    new_peaks.Add(new MSPeak(peaks[i].MZ, peaks[i].Intensity, 0));
                }
            }

            return new_peaks;
        }

        private static List<MSPeak> Deisotope(IEnumerable<MSPeak> peaks, int maxCharge, MassTolerance isotopicMzTolerance)
        {
            List<MSPeak> new_peaks = new List<MSPeak>(peaks);

            int p = new_peaks.Count - 1;
            while(p >= 1)
            {
                int q = p - 1;
                bool removed = false;
                while(q >= 0)
                {
                    if(new_peaks[p].MZ > (new_peaks[q].MZ + Constants.C12_C13_MASS_DIFFERENCE) + isotopicMzTolerance)
                    {
                        break;
                    }

                    if(new_peaks[p].Intensity < new_peaks[q].Intensity)
                    {
                        for(int c = 1; c <= maxCharge; c++)
                        {
                            if(Math.Abs(MassTolerance.CalculateMassError(new_peaks[p].MZ, new_peaks[q].MZ + Constants.C12_C13_MASS_DIFFERENCE / c, isotopicMzTolerance.Units)) <= isotopicMzTolerance.Value)
                            {
                                new_peaks.RemoveAt(p);
                                removed = true;
                                break;
                            }
                        }
                        if(removed)
                        {
                            break;
                        }
                    }

                    q--;
                }

                p--;
            }

            return new_peaks;
        }
    }
}