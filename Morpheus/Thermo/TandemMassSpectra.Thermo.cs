using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using MSFileReaderLib;

namespace Morpheus
{
    public partial class TandemMassSpectra
    {
        private const PrecursorMassType PRECURSOR_MASS_TYPE = PrecursorMassType.Monoisotopic;
        private const bool REFINE_PRECURSOR_MZ_AND_GET_INTENSITY_FROM_MS1 = true;
        private const bool ALWAYS_USE_PRECURSOR_CHARGE_STATE_RANGE = false;

        public void Load(string rawFilepath, int minimumAssumedPrecursorChargeState, int maximumAssumedPrecursorChargeState,
            double absoluteThreshold, double relativeThresholdPercent, int maximumNumberOfPeaks,
            bool assignChargeStates, bool deisotope, MassTolerance isotopicMZTolerance, int maximumThreads)
        {
            Load(rawFilepath, minimumAssumedPrecursorChargeState, maximumAssumedPrecursorChargeState,
                absoluteThreshold, relativeThresholdPercent, maximumNumberOfPeaks,
                assignChargeStates, maximumThreads);
        }

        public void Load(string rawFilepath, int minimumAssumedPrecursorChargeState, int maximumAssumedPrecursorChargeState,
            double absoluteThreshold, double relativeThresholdPercent, int maximumNumberOfPeaks,
            bool assignChargeStates, int maximumThreads)
        {
            OnReportTaskWithoutProgress(new EventArgs());

            IXRawfile5 raw = (IXRawfile5)new MSFileReader_XRawfile();

            raw.Open(rawFilepath);
            raw.SetCurrentController(0, 1);

            Dictionary<int, double[,]> ms1s;
            if(REFINE_PRECURSOR_MZ_AND_GET_INTENSITY_FROM_MS1)
            {
                ms1s = new Dictionary<int, double[,]>();
            }

            int first_scan_number = -1;
            raw.GetFirstSpectrumNumber(ref first_scan_number);
            int last_scan_number = -1;
            raw.GetLastSpectrumNumber(ref last_scan_number);

            OnReportTaskWithProgress(new EventArgs());
            object progress_lock = new object();
            int spectra_processed = 0;
            int old_progress = 0;

            ParallelOptions parallel_options = new ParallelOptions();
            parallel_options.MaxDegreeOfParallelism = maximumThreads;
            Parallel.For(first_scan_number, last_scan_number + 1, scan_number =>
            {
                // ref parameter initializations
                string scan_filter = null;
                int MSOrder = -1;

                raw.GetFilterForScanNum(scan_number, ref scan_filter);
                raw.GetMSOrderForScanNum(scan_number, ref MSOrder);
                if (MSOrder > 1)
                {
                    string spectrum_id = "controllerType=0 controllerNumber=1 scan=" + scan_number.ToString();

                    double retention_time_minutes = double.NaN;
                    raw.RTFromScanNum(scan_number, ref retention_time_minutes);

                    int polarity = DeterminePolarity(scan_filter);

                    string fragmentation_method = GetFragmentationMethod(scan_filter);

                    double precursor_mz;
                    double precursor_intensity;
                    GetPrecursor(ms1s, raw, scan_number, scan_filter, first_scan_number, out precursor_mz, out precursor_intensity);

                    int charge = DeterminePrecursorCharge(raw, scan_number);
                    if(polarity < 0)
                    {
                        charge = -charge;
                    }

                    double[,] label_data = GetFragmentationData(raw, scan_number, scan_filter);
                    List<MSPeak> peaks = new List<MSPeak>(label_data.GetLength(1));
                    for(int peak_index = label_data.GetLowerBound(1); peak_index <= label_data.GetUpperBound(1); peak_index++)
                    {
                        peaks.Add(new MSPeak(label_data[(int)RawLabelDataColumn.MZ, peak_index],
                            label_data[(int)RawLabelDataColumn.Intensity, peak_index],
                            assignChargeStates && (int)RawLabelDataColumn.Charge < label_data.GetLength(0) ? (int)label_data[(int)RawLabelDataColumn.Charge, peak_index] : 0, polarity));
                    }

                    peaks = FilterPeaks(peaks, absoluteThreshold, relativeThresholdPercent, maximumNumberOfPeaks);
                    if(peaks.Count > 0)
                    {
                        for(int c = (ALWAYS_USE_PRECURSOR_CHARGE_STATE_RANGE || charge == 0 ? minimumAssumedPrecursorChargeState : charge);
                            c <= (ALWAYS_USE_PRECURSOR_CHARGE_STATE_RANGE || charge == 0 ? maximumAssumedPrecursorChargeState : charge); c++)
                        {
                            double precursor_mass = MSPeak.MassFromMZ(precursor_mz, c);

                            TandemMassSpectrum spectrum = new TandemMassSpectrum(rawFilepath, scan_number, spectrum_id, null, retention_time_minutes, fragmentation_method, precursor_mz, precursor_intensity, c, precursor_mass, peaks);
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
                    int new_progress = (int)((double)spectra_processed / (last_scan_number - first_scan_number + 1) * 100);
                    if(new_progress > old_progress)
                    {
                        OnUpdateProgress(new ProgressEventArgs(new_progress));
                        old_progress = new_progress;
                    }
                }
            }
            );

            raw.Close();
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
            //hack: HCD at energy of 1.50 means nETD
            //else if(scanFilter.Contains("hcd1.50"))
            //{
            //    return "negative electron transfer dissociation";
            //}
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

        private static void GetPrecursor(IDictionary<int, double[,]> ms1s, IXRawfile5 raw, int scanNumber, string scanFilter, int firstScanNumber, out double mz, out double intensity)
        {
            if(PRECURSOR_MASS_TYPE == PrecursorMassType.Isolation)
            {
                mz = GetIsolationMZ(scanFilter);
            }
            else if(PRECURSOR_MASS_TYPE == PrecursorMassType.Monoisotopic)
            {
                mz = -1;
                raw.GetPrecursorMassForScanNum(scanNumber, 2, ref mz);
            }
            if(REFINE_PRECURSOR_MZ_AND_GET_INTENSITY_FROM_MS1)
            {
                RefineMZestimateAndGetIntensity(ms1s, raw, scanNumber, firstScanNumber, ref mz, out intensity);
            }
            else
            {
                intensity = double.NaN;
            }
        }

        // For a given MSn scan, determines the precursor mz and intensity by looking at the scans backwards until an MS1 scan is found
        // In the found MS1 scan, look for peak with similar mass/charge to the input parameter mz
        // Return that peak in variables mz and intensity
        private static bool RefineMZestimateAndGetIntensity(IDictionary<int, double[,]> ms1s, IXRawfile5 raw, int scanNumber, int firstScanNumber, ref double mz, out double intensity)
        {
            scanNumber--;
            while(scanNumber >= firstScanNumber)
            {
                // ref parameter initializations
                string scan_filter = null;
                int MSOrder = -1;

                raw.GetFilterForScanNum(scanNumber, ref scan_filter);
                raw.GetMSOrderForScanNum(scanNumber, ref MSOrder);
                if (MSOrder == 1)
                {
                    double[,] ms1;
                    lock(ms1s)
                    {
                        if(!ms1s.TryGetValue(scanNumber, out ms1))
                        {
                            if(scan_filter.Contains("FTMS"))
                            {
                                object labels_obj = null;
                                object flags_obj = null;
                                // Read FT-PROFILE labels of a scan
                                // labels_obj will contain values of mass(double), intensity(double), resolution(float), baseline(float), noise(float) and charge(int).
                                raw.GetLabelData(ref labels_obj, ref flags_obj, ref scanNumber);
                                ms1 = (double[,])labels_obj;
                            }
                            else
                            {
                                double centroid_peak_width = double.NaN;
                                object mass_list = null;
                                object peak_flags = null;
                                // Only applicable to scanning devices such as MS and PDA
                                // mass_list will containan array of double precision values in mass intensity pairs in ascending mass order
                                int array_size = -1;
                                raw.GetMassListFromScanNum(scanNumber, null, 0, 0, 0, 1, ref centroid_peak_width, ref mass_list, ref peak_flags, ref array_size);
                                ms1 = (double[,])mass_list;
                            }
                            ms1s.Add(scanNumber, ms1);
                        }
                    }

                    int index = -1;
                    // In the MS1 scan, look at every peak. Find index of the one that is closest in mz value to the input parameter mz
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

        private static double[,] GetFragmentationData(IXRawfile2 raw, int scanNumber, string scanFilter)
        {
            double[,] data;
            if(scanFilter.Contains("FTMS"))
            {
                object labels_obj = null;
                object flags_obj = null;
                raw.GetLabelData(ref labels_obj, ref flags_obj, ref scanNumber);
                data = (double[,])labels_obj;
            }
            else
            {
                double centroid_peak_width = double.NaN;
                object mass_list = null;
                object peak_flags = null;
                int array_size = -1;
                raw.GetMassListFromScanNum(scanNumber, null, 0, 0, 0, 1, ref centroid_peak_width, ref mass_list, ref peak_flags, ref array_size);
                data = (double[,])mass_list;
            }

            return data;
        }

        private static int DeterminePolarity(string scanFilter)
        {
            if(scanFilter.Contains(" + "))
            {
                return 1;
            }
            else if(scanFilter.Contains(" - "))
            {
                return -1;
            }
            else
            {
                throw new ArgumentException("Unknown polarity.");
            }
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
            double isolation_mz = double.Parse(temp_scan_filter.Substring(temp_scan_filter.LastIndexOf(' ') + 1), CultureInfo.InvariantCulture);

            return isolation_mz;
        }

    }

    internal enum PrecursorMassType
    {
        Isolation,
        Monoisotopic
    }

    internal enum RawLabelDataColumn
    {
        MZ = 0,
        Intensity = 1,
        Resolution = 2,
        NoiseBaseline = 3,
        NoiseLevel = 4,
        Charge = 5
    }
}