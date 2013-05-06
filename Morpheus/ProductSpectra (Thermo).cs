using System;
using System.Collections.Generic;
using MSFileReaderLib;

namespace Morpheus
{
    internal enum PrecursorMassType
    {
        Isolation,
        Monoisotopic
    }

    public partial class ProductSpectra : List<ProductSpectrum>
    {
        private const PrecursorMassType PRECURSOR_MASS_TYPE = PrecursorMassType.Monoisotopic;

        public ProductSpectra() : base() { }

        public static ProductSpectra Load(string rawFilepath, int minimumAssumedPrecursorChargeState, int maximumAssumedPrecursorChargeState,
            double absoluteThreshold, double relativeThresholdPercent, int maximumNumberOfPeaks,
            bool assignChargeStates, bool deisotope, MassTolerance isotopicMzTolerance)
        {
            return Load(rawFilepath, minimumAssumedPrecursorChargeState, maximumAssumedPrecursorChargeState,
                absoluteThreshold, relativeThresholdPercent, maximumNumberOfPeaks, 
                assignChargeStates);
        }

        public static ProductSpectra Load(string rawFilepath, int minimumAssumedPrecursorChargeState, int maximumAssumedPrecursorChargeState,
            double absoluteThreshold, double relativeThresholdPercent, int maximumNumberOfPeaks,
            bool assignChargeStates)
        {
            OnReportTaskWithoutProgress(new EventArgs());

            IXRawfile2 raw = (IXRawfile2)new MSFileReader_XRawfile();

            raw.Open(rawFilepath);
            raw.SetCurrentController(0, 1);

            int first_scan_number = -1;
            raw.GetFirstSpectrumNumber(ref first_scan_number);
            int last_scan_number = -1;
            raw.GetLastSpectrumNumber(ref last_scan_number);

            ProductSpectra spectra = new ProductSpectra();

            OnReportTaskWithProgress(new EventArgs());
            int old_progress = 0;

            for(int scan_number = first_scan_number; scan_number <= last_scan_number; scan_number++)
            {
                string scan_filter = null;
                raw.GetFilterForScanNum(scan_number, ref scan_filter);

                if(!scan_filter.Contains(" ms "))
                {
                    double retention_time = double.NaN;
                    raw.RTFromScanNum(scan_number, ref retention_time);

                    string fragmentation_method = GetFragmentationMethod(scan_filter);

                    double precursor_mz;
                    double precursor_intensity;
                    GetPrecursor(raw, scan_number, scan_filter, first_scan_number, out precursor_mz, out precursor_intensity);

                    int charge = DeterminePrecursorCharge(raw, scan_number);

                    double[,] label_data = GetFragmentationData(raw, scan_number);
                    List<MSPeak> peaks = new List<MSPeak>(label_data.GetLength(1));
                    for(int peak_index = label_data.GetLowerBound(1); peak_index <= label_data.GetUpperBound(1); peak_index++)
                    {
                        peaks.Add(new MSPeak(label_data[(int)RawLabelDataColumn.MZ, peak_index],
                            label_data[(int)RawLabelDataColumn.Intensity, peak_index],
                            assignChargeStates ? (int)label_data[(int)RawLabelDataColumn.Charge, peak_index] : 0));
                    }

                    peaks = FilterPeaks(peaks, absoluteThreshold, relativeThresholdPercent, maximumNumberOfPeaks);

                    if(charge == 0)
                    {
                        for(int c = minimumAssumedPrecursorChargeState; c <= maximumAssumedPrecursorChargeState; c++)
                        {
                            double precursor_mass = Utilities.MassFromMZ(precursor_mz, c);

                            ProductSpectrum spectrum = new ProductSpectrum(rawFilepath, scan_number, retention_time, fragmentation_method, precursor_mz, precursor_intensity, c, precursor_mass, peaks);
                            spectra.Add(spectrum);
                        }
                    }
                    else
                    {
                        double isolation_mass = Utilities.MassFromMZ(precursor_mz, charge);

                        ProductSpectrum spectrum = new ProductSpectrum(rawFilepath, scan_number, retention_time, fragmentation_method, precursor_mz, precursor_intensity, charge, isolation_mass, peaks);
                        spectra.Add(spectrum);
                    }
                }

                int new_progress = (int)((double)(scan_number + 1) / (last_scan_number - first_scan_number + 1) * 100);
                if(new_progress > old_progress)
                {
                    OnUpdateProgress(new ProgressEventArgs(new_progress));
                    old_progress = new_progress;
                }
            }

            raw.Close();

            return spectra;
        }

        private static string GetFragmentationMethod(string scanFilter)
        {
            if(scanFilter.Contains("cid"))
            {
                return "collision-induced dissociation";
            }
            else if(scanFilter.Contains("mpd"))
            {
                return "infrared multiphoton dissociation";
            }
            else if(scanFilter.Contains("pqd"))
            {
                return "pulsed q dissociation";
            }
            else if(scanFilter.Contains("hcd"))
            {
                return "high-energy collision-induced dissociation";
            }
            else if(scanFilter.Contains("ecd"))
            {
                return "electron capture dissociation";
            }
            else if(scanFilter.Contains("etd"))
            {
                return "electron transfer dissociation";
            }
            else
            {
                return null;
            }
        }

        private static void GetPrecursor(IXRawfile2 raw, int scanNumber, string scanFilter, int firstScanNumber, out double mz, out double intensity)
        {
            if(PRECURSOR_MASS_TYPE == PrecursorMassType.Isolation)
            {
                mz = GetIsolationMZ(scanFilter);
            }
            else if(PRECURSOR_MASS_TYPE == PrecursorMassType.Monoisotopic)
            {
                mz = GetMonoisotopicMZ(raw, scanNumber, scanFilter);
            }
            GetAccurateMzAndIntensity(raw, scanNumber, firstScanNumber, ref mz, out intensity);
        }

        private static bool GetAccurateMzAndIntensity(IXRawfile2 raw, int scanNumber, int firstScanNumber, ref double mz, out double intensity)
        {
            scanNumber--;
            while(scanNumber >= firstScanNumber)
            {
                string scan_filter = null;
                raw.GetFilterForScanNum(scanNumber, ref scan_filter);

                if(scan_filter.Contains(" ms "))
                {
                    double[,] ms1;
                    if(scan_filter.Contains("FTMS"))
                    {
                        object labels_obj = null;
                        object flags_obj = null;
                        raw.GetLabelData(ref labels_obj, ref flags_obj, ref scanNumber);
                        ms1 = (double[,])labels_obj;
                    }
                    else
                    {
                        double centroid_peak_width = double.NaN;
                        object mass_list = null;
                        object peak_flags = null;
                        int array_size = -1;
                        raw.GetMassListFromScanNum(scanNumber, null, 0, 0, 0, 1, ref centroid_peak_width, ref mass_list, ref peak_flags, ref array_size);
                        ms1 = (double[,])mass_list;
                    }

                    int index = -1;
                    for(int i = ms1.GetLowerBound(1); i <= ms1.GetUpperBound(1); i++)
                    {
                        if(index < 0 || Math.Abs(ms1[0, i] - mz) < Math.Abs(ms1[0, index] - mz))
                        {
                            index = i;
                        }
                    }
                    if(index >= 0)
                    {
                        mz = ms1[0, index];
                        intensity = ms1[1, index];
                        return true;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    scanNumber--;
                }
            }

            intensity = double.NaN;
            return false;
        }

        private static double[,] GetFragmentationData(IXRawfile2 raw, int scanNumber)
        {
            object labels_obj = null;
            object flags_obj = null;
            raw.GetLabelData(ref labels_obj, ref flags_obj, ref scanNumber);
            double[,] labels = (double[,])labels_obj;

            return labels;
        }

        private static int DeterminePrecursorCharge(IXRawfile2 raw, int scanNumber)
        {
            object trailer_labels_obj = null;
            object trailer_values_obj = null;
            int trailer_array_size = -1;
            raw.GetTrailerExtraForScanNum(scanNumber, ref trailer_labels_obj, ref trailer_values_obj, ref trailer_array_size);
            string[] trailer_labels = (string[])trailer_labels_obj;
            string[] trailer_values = (string[])trailer_values_obj;

            int charge = -1;
            for(int trailer_index = trailer_labels.GetLowerBound(0); trailer_index <= trailer_labels.GetUpperBound(0); trailer_index++)
            {
                if(trailer_labels[trailer_index].StartsWith("Charge"))
                {
                    charge = int.Parse(trailer_values[trailer_index]);
                }
            }
            return charge;
        }

        private static double GetIsolationMZ(string scanFilter)
        {
            string temp_scan_filter = scanFilter.Substring(0, scanFilter.LastIndexOf('@'));
            double isolation_mz = double.Parse(temp_scan_filter.Substring(temp_scan_filter.LastIndexOf(' ') + 1));

            return isolation_mz;
        }

        private static double GetMonoisotopicMZ(IXRawfile2 raw, int scanNumber, string scanFilter)
        {
            object labels_obj = null;
            object values_obj = null;
            int array_size = -1;
            raw.GetTrailerExtraForScanNum(scanNumber, ref labels_obj, ref values_obj, ref array_size);
            string[] labels = (string[])labels_obj;
            string[] values = (string[])values_obj;
            for(int i = labels.GetLowerBound(0); i <= labels.GetUpperBound(0); i++)
            {
                if(labels[i].StartsWith("Monoisotopic M/Z"))
                {
                    double monoisotopic_mz = double.Parse(values[i]);
                    if(monoisotopic_mz > 0.0)
                    {
                        return monoisotopic_mz;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return GetIsolationMZ(scanFilter);
        }
    }
}