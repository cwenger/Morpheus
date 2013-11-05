using System;
using System.Collections.Generic;

namespace Morpheus
{
    public partial class TandemMassSpectra
    {
        public IEnumerable<TandemMassSpectrum> GetTandemMassSpectraInMassRange(double precursorMass, MassTolerance precursorMassTolerance)
        {
            return GetTandemMassSpectraInMassRange(precursorMass, precursorMassTolerance, 0, 0);
        }

        public IEnumerable<TandemMassSpectrum> GetTandemMassSpectraInMassRange(double precursorMass, MassTolerance precursorMassTolerance, 
            int minimumMonoisotopicPeakOffset, int maximumMonoisotopicPeakOffset)
        {
            for(int i = minimumMonoisotopicPeakOffset; i <= maximumMonoisotopicPeakOffset; i++)
            {
                double precursor_mass = precursorMass + i * Constants.C12_C13_MASS_DIFFERENCE;

                double minimum_precursor_mass = precursor_mass - precursorMassTolerance;
                double maximum_precursor_mass = precursor_mass + precursorMassTolerance;

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