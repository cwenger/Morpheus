using System;
using System.Collections.Generic;
using Agilent.MassSpectrometry;
using Agilent.MassSpectrometry.DataAnalysis;

namespace Morpheus
{
    public partial class ProductSpectra : List<ProductSpectrum>
    {
        private const double ACCURACY_C0 = 0.0025;
        private const double ACCURACY_C1 = 7.0 / 1000000;

        private const string FRAGMENTATION_METHOD = "collision-induced dissociation";

        private const bool GET_PRECURSOR_MZ_AND_INTENSITY_FROM_MS1 = true;

        public ProductSpectra() : base() { }

        public static ProductSpectra Load(string agilentDFolderPath, int minimumAssumedPrecursorChargeState, int maximumAssumedPrecursorChargeState,
            double absoluteThreshold, double relativeThresholdPercent, int maximumNumberOfPeaks,
            bool assignChargeStates, bool deisotope, MassTolerance isotopicMzTolerance)
        {
            return Load(agilentDFolderPath, minimumAssumedPrecursorChargeState, maximumAssumedPrecursorChargeState,
                absoluteThreshold, relativeThresholdPercent, maximumNumberOfPeaks,
                assignChargeStates, deisotope);
        }

        public static ProductSpectra Load(string agilentDFolderPath, int minimumAssumedPrecursorChargeState, int maximumAssumedPrecursorChargeState,
            double absoluteThreshold, double relativeThresholdPercent, int maximumNumberOfPeaks,
            bool assignChargeStates, bool deisotope)
        {
            OnReportTaskWithoutProgress(new EventArgs());

            IMsdrDataReader agilent_d = new MassSpecDataReader();

            agilent_d.OpenDataFile(agilentDFolderPath);

            IMsdrPeakFilter ms1_peak_filter = new MsdrPeakFilter();
            IMsdrPeakFilter ms2_peak_filter = new MsdrPeakFilter();
            ms2_peak_filter.AbsoluteThreshold = absoluteThreshold;
            ms2_peak_filter.RelativeThreshold = relativeThresholdPercent;
            ms2_peak_filter.MaxNumPeaks = maximumNumberOfPeaks;
            ChargeStateAssignmentWrapper csaw = new ChargeStateAssignmentWrapper();

            ProductSpectra spectra = new ProductSpectra();

            OnReportTaskWithProgress(new EventArgs());
            int old_progress = 0;

            for(int row_number = 0; row_number < agilent_d.MSScanFileInformation.TotalScansPresent; row_number++)
            {
                IMSScanRecord scan_record = agilent_d.GetScanRecord(row_number);

                if(scan_record.MSLevel == MSLevel.MSMS)
                {
                    IBDASpecData agilent_spectrum = agilent_d.GetSpectrum(row_number, ms1_peak_filter, ms2_peak_filter);

                    int scan_number = row_number + 1;
                    string scan_id = "scanId=" + agilent_spectrum.ScanId.ToString();

                    double precursor_mz = agilent_spectrum.MZOfInterest[0].Start;
                    double precursor_intensity;
                    if(GET_PRECURSOR_MZ_AND_INTENSITY_FROM_MS1)
                    {
                        GetAccurateMzAndIntensity(agilent_d, agilent_spectrum.ParentScanId, ref precursor_mz, out precursor_intensity);
                    }
                    else
                    {
                        agilent_spectrum.GetPrecursorIntensity(out precursor_intensity);
                    }

                    int charge;
                    agilent_spectrum.GetPrecursorCharge(out charge);

                    List<MSPeak> peaks = null;
                    if(!assignChargeStates)
                    {
                        peaks = new List<MSPeak>(agilent_spectrum.TotalDataPoints);
                        for(int i = 0; i < agilent_spectrum.TotalDataPoints; i++)
                        {
                            peaks.Add(new MSPeak(agilent_spectrum.XArray[i], agilent_spectrum.YArray[i], 0));
                        }
                    }

                    if(charge == 0)
                    {
                        for(int c = minimumAssumedPrecursorChargeState; c <= maximumAssumedPrecursorChargeState; c++)
                        {
                            if(assignChargeStates)
                            {
                                csaw.SetParameters(IsotopeModel.Peptidic, 1, (short)c, false, ACCURACY_C0, ACCURACY_C1);

                                int[] peak_ids = new int[agilent_spectrum.TotalDataPoints];
                                double[] peak_mzs = new double[agilent_spectrum.TotalDataPoints];
                                double[] peak_intensities = new double[agilent_spectrum.TotalDataPoints];
                                for(int i = 0; i < agilent_spectrum.TotalDataPoints; i++)
                                {
                                    peak_ids[i] = i;
                                    peak_mzs[i] = agilent_spectrum.XArray[i];
                                    peak_intensities[i] = agilent_spectrum.YArray[i];
                                }
                                int[] peak_charge_states = new int[agilent_spectrum.TotalDataPoints];
                                int[] peak_clusters = new int[agilent_spectrum.TotalDataPoints];

                                int num_peaks = csaw.AssignChargeStates(peak_ids, peak_mzs, peak_intensities, peak_charge_states, peak_clusters);

                                peaks = new List<MSPeak>(num_peaks);
                                HashSet<int> observed_peak_clusters = new HashSet<int>();
                                for(int i = 0; i < num_peaks; i++)
                                {
                                    bool isotopic_peak = observed_peak_clusters.Contains(peak_clusters[i]);
                                    if(!deisotope || !isotopic_peak)
                                    {
                                        peaks.Add(new MSPeak(peak_mzs[i], peak_intensities[i], peak_charge_states[i]));
                                        if(peak_clusters[i] > 0 && !isotopic_peak)
                                        {
                                            observed_peak_clusters.Add(peak_clusters[i]);
                                        }
                                    }
                                }
                            }

                            double precursor_mass = Utilities.MassFromMZ(precursor_mz, c);

                            ProductSpectrum spectrum = new ProductSpectrum(agilentDFolderPath, scan_id, scan_number, scan_record.RetentionTime, FRAGMENTATION_METHOD, precursor_mz, precursor_intensity, c, precursor_mass, peaks);
                            spectra.Add(spectrum);
                        }
                    }
                    else
                    {
                        if(assignChargeStates)
                        {
                            csaw.SetParameters(IsotopeModel.Peptidic, 1, (short)charge, false, ACCURACY_C0, ACCURACY_C1);

                            int[] peak_ids = new int[agilent_spectrum.TotalDataPoints];
                            double[] peak_mzs = new double[agilent_spectrum.TotalDataPoints];
                            double[] peak_intensities = new double[agilent_spectrum.TotalDataPoints];
                            for(int i = 0; i < agilent_spectrum.TotalDataPoints; i++)
                            {
                                peak_ids[i] = i;
                                peak_mzs[i] = agilent_spectrum.XArray[i];
                                peak_intensities[i] = agilent_spectrum.YArray[i];
                            }
                            int[] peak_charge_states = new int[agilent_spectrum.TotalDataPoints];
                            int[] peak_clusters = new int[agilent_spectrum.TotalDataPoints];

                            int num_peaks = csaw.AssignChargeStates(peak_ids, peak_mzs, peak_intensities, peak_charge_states, peak_clusters);

                            peaks = new List<MSPeak>(num_peaks);
                            HashSet<int> observed_peak_clusters = new HashSet<int>();
                            for(int i = 0; i < num_peaks; i++)
                            {
                                bool isotopic_peak = observed_peak_clusters.Contains(peak_clusters[i]);
                                if(!deisotope || !isotopic_peak)
                                {
                                    peaks.Add(new MSPeak(peak_mzs[i], peak_intensities[i], peak_charge_states[i]));
                                    if(peak_clusters[i] > 0 && !isotopic_peak)
                                    {
                                        observed_peak_clusters.Add(peak_clusters[i]);
                                    }
                                }
                            }
                        }

                        double precursor_mass = Utilities.MassFromMZ(precursor_mz, charge);

                        ProductSpectrum spectrum = new ProductSpectrum(agilentDFolderPath, scan_id, scan_number, scan_record.RetentionTime, FRAGMENTATION_METHOD, precursor_mz, precursor_intensity, charge, precursor_mass, peaks);
                        spectra.Add(spectrum);
                    }
                }

                int new_progress = (int)((double)(row_number + 1) / agilent_d.MSScanFileInformation.TotalScansPresent * 100);
                if(new_progress > old_progress)
                {
                    OnUpdateProgress(new ProgressEventArgs(new_progress));
                    old_progress = new_progress;
                }
            }

            agilent_d.CloseDataFile();

            return spectra;
        }

        private static int ms1ScanId = -1;
        private static IBDASpecData ms1 = null;

        private static bool GetAccurateMzAndIntensity(IMsdrDataReader msdr, int parentScanId, ref double mz, out double intensity)
        {
            if(parentScanId != ms1ScanId)
            {
                IBDASpecFilter spec_filter = new BDASpecFilter();
                spec_filter.SpectrumType = SpecType.MassSpectrum;
                spec_filter.ScanIds = new int[] { parentScanId };
                ms1 = msdr.GetSpectrum(spec_filter)[0];
                ms1ScanId = parentScanId;
            }

            int index = -1;
            for(int i = 0; i < ms1.TotalDataPoints; i++)
            {
                if(index < 0 || Math.Abs(ms1.XArray[i] - mz) < Math.Abs(ms1.XArray[index] - mz))
                {
                    index = i;
                }
            }

            if(index >= 0)
            {
                mz = ms1.XArray[index];
                intensity = ms1.YArray[index];
                return true;
            }

            intensity = double.NaN;
            return false;
        }
    }
}