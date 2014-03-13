using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Agilent.MassSpectrometry;
using Agilent.MassSpectrometry.DataAnalysis;

namespace Morpheus
{
    public partial class TandemMassSpectra : List<TandemMassSpectrum>
    {
        private const bool GET_PRECURSOR_MZ_AND_INTENSITY_FROM_MS1 = true;
        private const bool ALWAYS_USE_PRECURSOR_CHARGE_STATE_RANGE = false;
        private const double ACCURACY_C0 = 0.0025;
        private const double ACCURACY_C1 = 7.0 / 1000000;
        private const string FRAGMENTATION_METHOD = "collision-induced dissociation";

        public void Load(string agilentDFolderPath, int minimumAssumedPrecursorChargeState, int maximumAssumedPrecursorChargeState,
            double absoluteThreshold, double relativeThresholdPercent, int maximumNumberOfPeaks,
            bool assignChargeStates, bool deisotope, MassTolerance isotopicMZTolerance, int maximumThreads)
        {
            Load(agilentDFolderPath, minimumAssumedPrecursorChargeState, maximumAssumedPrecursorChargeState,
                absoluteThreshold, relativeThresholdPercent, maximumNumberOfPeaks,
                assignChargeStates, deisotope, maximumThreads);
        }

        public void Load(string agilentDFolderPath, int minimumAssumedPrecursorChargeState, int maximumAssumedPrecursorChargeState,
            double absoluteThreshold, double relativeThresholdPercent, int maximumNumberOfPeaks,
            bool assignChargeStates, bool deisotope, int maximumThreads)
        {
            OnReportTaskWithoutProgress(new EventArgs());

            IMsdrDataReader agilent_d = new MassSpecDataReader();

            agilent_d.OpenDataFile(agilentDFolderPath);

            Dictionary<int, IBDASpecData> ms1s;
            if(GET_PRECURSOR_MZ_AND_INTENSITY_FROM_MS1)
            {
                ms1s = new Dictionary<int, IBDASpecData>();
            }

            IMsdrPeakFilter ms1_peak_filter = new MsdrPeakFilter();
            IMsdrPeakFilter ms2_peak_filter = new MsdrPeakFilter();
            ms2_peak_filter.AbsoluteThreshold = absoluteThreshold;
            ms2_peak_filter.RelativeThreshold = relativeThresholdPercent;
            ms2_peak_filter.MaxNumPeaks = maximumNumberOfPeaks;
            ChargeStateAssignmentWrapper csaw = new ChargeStateAssignmentWrapper();

            OnReportTaskWithProgress(new EventArgs());
            object progress_lock = new object();
            int spectra_processed = 0;
            int old_progress = 0;

            ParallelOptions parallel_options = new ParallelOptions();
            parallel_options.MaxDegreeOfParallelism = maximumThreads;
            Parallel.For(0, (int)agilent_d.MSScanFileInformation.TotalScansPresent, parallel_options, row_number =>
                {
                    IMSScanRecord scan_record;
                    lock(agilent_d)
                    {
                        scan_record = agilent_d.GetScanRecord(row_number);
                    }

                    if(scan_record.MSLevel == MSLevel.MSMS)
                    {
                        IBDASpecData agilent_spectrum;
                        lock(agilent_d)
                        {
                            agilent_spectrum = agilent_d.GetSpectrum(row_number, ms1_peak_filter, ms2_peak_filter);
                        }

                        if(agilent_spectrum.TotalDataPoints > 0)
                        {
                            int spectrum_number = row_number + 1;
                            string scan_id = "scanId=" + agilent_spectrum.ScanId.ToString();

                            double precursor_mz = agilent_spectrum.MZOfInterest[0].Start;
                            double precursor_intensity;
                            if(GET_PRECURSOR_MZ_AND_INTENSITY_FROM_MS1)
                            {
                                GetAccurateMZAndIntensity(ms1s, agilent_d, agilent_spectrum.ParentScanId, ref precursor_mz, out precursor_intensity);
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

                            for(int c = (ALWAYS_USE_PRECURSOR_CHARGE_STATE_RANGE || charge == 0 ? minimumAssumedPrecursorChargeState : charge);
                                c <= (ALWAYS_USE_PRECURSOR_CHARGE_STATE_RANGE || charge == 0 ? maximumAssumedPrecursorChargeState : charge); c++)
                            {
                                if(assignChargeStates)
                                {
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

                                    int num_peaks;
                                    lock(csaw)
                                    {
                                        csaw.SetParameters(IsotopeModel.Peptidic, 1, (short)c, false, ACCURACY_C0, ACCURACY_C1);
                                        num_peaks = csaw.AssignChargeStates(peak_ids, peak_mzs, peak_intensities, peak_charge_states, peak_clusters);
                                    }

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

                                TandemMassSpectrum spectrum = new TandemMassSpectrum(agilentDFolderPath, spectrum_number, scan_id, null, scan_record.RetentionTime, FRAGMENTATION_METHOD, precursor_mz, precursor_intensity, c, precursor_mass, peaks);
                                lock(this)
                                {
                                    Add(spectrum);
                                }
                            }
                        }
                    }

                    lock(progress_lock)
                    {
                        spectra_processed++;
                        int new_progress = (int)((double)spectra_processed / agilent_d.MSScanFileInformation.TotalScansPresent * 100);
                        if(new_progress > old_progress)
                        {
                            OnUpdateProgress(new ProgressEventArgs(new_progress));
                            old_progress = new_progress;
                        }
                    }
                }
            );

            agilent_d.CloseDataFile();
        }

        private static bool GetAccurateMZAndIntensity(IDictionary<int, IBDASpecData> ms1s, IMsdrDataReader agilentD, int parentScanId, ref double mz, out double intensity)
        {
            IBDASpecData ms1;
            lock(ms1s)
            {
                if(!ms1s.TryGetValue(parentScanId, out ms1))
                {
                    IBDASpecFilter spec_filter = new BDASpecFilter();
                    spec_filter.SpectrumType = SpecType.MassSpectrum;
                    spec_filter.ScanIds = new int[] { parentScanId };
                    lock(agilentD)
                    {
                        ms1 = agilentD.GetSpectrum(spec_filter)[0];
                    }
                    ms1s.Add(parentScanId, ms1);
                }
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