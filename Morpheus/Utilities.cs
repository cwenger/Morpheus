using System;

namespace Morpheus
{
    public static class Utilities
    {
        public static double MassFromMZ(double mz, int charge)
        {
            return mz * Math.Abs(charge) - charge * Constants.PROTON_MASS;
        }

        public static double MZFromMass(double mass, int charge)
        {
            return (mass + charge * Constants.PROTON_MASS) / Math.Abs(charge);
        }
    }
}