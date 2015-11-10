using System;

namespace Morpheus
{
    public static class Utilities
    {
        public static double MassFromMZ(double mz, int charge)
        {
            return charge == 0 ? mz : mz * Math.Abs(charge) - charge * Constants.PROTON_MASS;
        }

        public static double MZFromMass(double mass, int charge)
        {
            if(charge == 0)
            {
                throw new ArgumentOutOfRangeException("Charge cannot be zero.");
            }
            return (mass + charge * Constants.PROTON_MASS) / Math.Abs(charge);
        }
    }
}