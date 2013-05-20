using System.Collections.Generic;

namespace Morpheus
{
    public class ProductSpectrum
    {
        public string Filename { get; private set; }

        public string ScanID { get; private set; }

        public int ScanNumber { get; private set; }

        public double RetentionTime { get; private set; }

        public string FragmentationMethod { get; private set; }

        public double PrecursorMZ { get; private set; }

        public double PrecursorIntensity { get; private set; }

        public int PrecursorCharge { get; private set; }

        public double PrecursorMass { get; private set; }

        public double[] Masses { get; private set; }

        public double[] Intensities { get; private set; }

        public double TotalIntensity { get; private set; }

        public ProductSpectrum(string filename, string scanID, int scanNumber, double retentionTime, string fragmentationMethod, double precursorMZ, double precursorIntensity, int precursorCharge, double precursorMass, List<MSPeak> peaks)
        {
            Filename = filename;
            ScanID = scanID;
            ScanNumber = scanNumber;
            RetentionTime = retentionTime;
            FragmentationMethod = fragmentationMethod;
            PrecursorMZ = precursorMZ;
            PrecursorIntensity = precursorIntensity;
            PrecursorCharge = precursorCharge;
            PrecursorMass = precursorMass;
            TotalIntensity = 0.0;
            if(peaks != null)
            {
                peaks.Sort(MSPeak.AscendingMassComparison);
                Masses = new double[peaks.Count];
                Intensities = new double[peaks.Count];
                for(int p = 0; p < peaks.Count; p++)
                {
                    MSPeak peak = peaks[p];
                    Masses[p] = peak.Mass;
                    Intensities[p] = peak.Intensity;
                    TotalIntensity += peak.Intensity;
                }
            }
        }

        public static int AscendingPrecursorMassComparison(ProductSpectrum left, ProductSpectrum right)
        {
            return left.PrecursorMass.CompareTo(right.PrecursorMass);
        }
    }
}