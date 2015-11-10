namespace Morpheus
{
    public class MSPeak
    {
        public double MZ { get; private set; }

        public double Intensity { get; private set; }

        public int Charge { get; private set; }

        public double Mass { get; private set; }

        public MSPeak(double mz, double intensity, int charge, int polarity)
        {
            MZ = mz;
            Intensity = intensity;
            Charge = charge;
            CalculateMass(charge, polarity);
        }

        private void CalculateMass(int charge, int polarity)
        {
            if(charge == 0)
            {
                charge = polarity;
            }
            Mass = Utilities.MassFromMZ(MZ, charge);
        }

        public static int AscendingMZComparison(MSPeak left, MSPeak right)
        {
            return left.MZ.CompareTo(right.MZ);
        }

        public static int DescendingIntensityComparison(MSPeak left, MSPeak right)
        {
            return -(left.Intensity.CompareTo(right.Intensity));
        }

        public static int AscendingMassComparison(MSPeak left, MSPeak right)
        {
            return left.Mass.CompareTo(right.Mass);
        }
    }
}