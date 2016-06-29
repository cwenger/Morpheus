using System;
using System.Collections.Generic;

namespace Morpheus
{
    public partial class TandemMassSpectra : List<TandemMassSpectrum>
    {
        public TandemMassSpectra() : base() { }

        public event EventHandler ReportTaskWithoutProgress;

        protected virtual void OnReportTaskWithoutProgress(EventArgs e)
        {
            ReportTaskWithoutProgress?.Invoke(null, e);
        }

        public event EventHandler ReportTaskWithProgress;

        protected virtual void OnReportTaskWithProgress(EventArgs e)
        {
            ReportTaskWithProgress?.Invoke(null, e);
        }

        public event EventHandler<ProgressEventArgs> UpdateProgress;

        protected virtual void OnUpdateProgress(ProgressEventArgs e)
        {
            UpdateProgress?.Invoke(null, e);
        }

        public IEnumerable<TandemMassSpectrum> GetTandemMassSpectraInMassRanges(double precursorMass, IEnumerable<double> acceptedPrecursorMassErrors, MassTolerance precursorMassTolerance)
        {
            foreach(double accepted_precursor_mass_error in acceptedPrecursorMassErrors)
            {
                foreach(TandemMassSpectrum spectrum in GetTandemMassSpectraInMassRange(precursorMass + accepted_precursor_mass_error, precursorMassTolerance))
                {
                    yield return spectrum;
                }
            }
        }

        public IEnumerable<TandemMassSpectrum> GetTandemMassSpectraInMassRange(double precursorMass, MassTolerance precursorMassTolerance)
        {
            double minimum_precursor_mass = precursorMass - precursorMassTolerance;
            double maximum_precursor_mass = precursorMass + precursorMassTolerance;

            int index = BinarySearch(NextHigherDouble(maximum_precursor_mass));
            if(index == Count)
            {
                index--;
            }
            while(index >= 0 && this[index].PrecursorMass >= minimum_precursor_mass)
            {
                if(this[index].PrecursorMass <= maximum_precursor_mass)
                {
                    yield return this[index];
                }
                index--;
            }
        }

        private int BinarySearch(double precursorMass)
        {
            int low_index = 0;
            int high_index = Count - 1;
            while(low_index <= high_index)
            {
                int mid_index = low_index + ((high_index - low_index) / 2);
                int comparison = this[mid_index].PrecursorMass.CompareTo(precursorMass);
                if(comparison == 0)
                {
                    return mid_index;
                }
                if(comparison < 0)
                {
                    low_index = mid_index + 1;
                }
                else
                {
                    high_index = mid_index - 1;
                }
            }
            return low_index;
        }

        // only works for positive doubles; does not handle special cases
        private static double NextHigherDouble(double value)
        {
            long bits = BitConverter.DoubleToInt64Bits(value);
            long next_double_bits = bits + 1;
            return BitConverter.Int64BitsToDouble(next_double_bits);
        }
    }
}
