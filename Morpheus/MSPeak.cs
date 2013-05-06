namespace Morpheus
{
    public class MSPeak
    {
        public double MZ { get; private set; }

        public double Intensity { get; private set; }

        public int Charge { get; private set; }

        public double Mass { get; private set; }

        public MSPeak(double mz, double intensity, int charge)
        {
            MZ = mz;
            Intensity = intensity;
            Charge = charge;
            CalculateMass();
        }

        private void CalculateMass()
        {
            CalculateMass(Charge);
        }

        private void CalculateMass(int charge)
        {
            if(charge == 0)
            {
                charge = 1;
            }
            Mass = Utilities.MassFromMZ(MZ, charge);
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