using System;
using System.Text;

namespace Morpheus
{
    public class PeptideSpectrumMatch : ITargetDecoy
    {
        private static MassType precursorMassType = MassType.Monoisotopic;

        public TandemMassSpectrum Spectrum { get; private set; }

        public Peptide Peptide { get; private set; }

        public double PrecursorMassErrorDa { get; private set; }

        public double PrecursorMassErrorPpm { get; private set; }

        public int MatchingProducts { get; private set; }

        public int TotalProducts { get; private set; }

        public double MatchingProductsFraction { get; private set; }

        public double MatchingIntensity { get; private set; }

        public double MatchingIntensityFraction { get; private set; }

        public double MorpheusScore { get; private set; }

        double ITargetDecoy.Score { get { return MorpheusScore; } }

        public bool Decoy
        {
            get { return Peptide.Parent.Decoy; }
        }

        public bool Target
        {
            get { return !Decoy; }
        }

        private static readonly ProductTypes PRODUCT_TYPES = ProductTypes.Instance;

        public static void SetPrecursorMassType(MassType precursorMassType)
        {
            PeptideSpectrumMatch.precursorMassType = precursorMassType;
        }

        public PeptideSpectrumMatch(TandemMassSpectrum spectrum, Peptide peptide, MassTolerance productMassTolerance)
        {
            Spectrum = spectrum;
            Peptide = peptide;

            PrecursorMassErrorDa = spectrum.PrecursorMass - (precursorMassType == MassType.Average ? peptide.AverageMass : peptide.MonoisotopicMass);
            PrecursorMassErrorPpm = PrecursorMassErrorDa / (precursorMassType == MassType.Average ? peptide.AverageMass : peptide.MonoisotopicMass) * 1e6;

            ScoreMatch(productMassTolerance);
        }

        private void ScoreMatch(MassTolerance productMassTolerance)
        {
            double[] theoretical_product_masses = Peptide.CalculateProductMasses(PRODUCT_TYPES[Spectrum.FragmentationMethod]).ToArray();
            TotalProducts = theoretical_product_masses.Length;

            // speed optimizations
            int num_theoretical_products = theoretical_product_masses.Length;
            double[] experimental_masses = Spectrum.Masses;
            double[] experimental_intensities = Spectrum.Intensities;
            int num_experimental_peaks = experimental_masses.Length;
            double product_mass_tolerance_value = productMassTolerance.Value;
            MassToleranceUnits product_mass_tolerance_units = productMassTolerance.Units;

            MatchingProducts = 0;
            int t = 0;
            int e = 0;
            while(t < num_theoretical_products && e < num_experimental_peaks)
            {
                double mass_difference = experimental_masses[e] - theoretical_product_masses[t];
                if(product_mass_tolerance_units == MassToleranceUnits.ppm)
                {
                    mass_difference = mass_difference / theoretical_product_masses[t] * 1e6;
                }
                if(Math.Abs(mass_difference) <= product_mass_tolerance_value)
                {
                    MatchingProducts++;
                    t++;
                }
                else if(mass_difference < 0)
                {
                    e++;
                }
                else if(mass_difference > 0)
                {
                    t++;
                }
            }
            MatchingProductsFraction = (double)MatchingProducts / TotalProducts;

            MatchingIntensity = 0.0;
            int e2 = 0;
            int t2 = 0;
            while(e2 < num_experimental_peaks && t2 < num_theoretical_products)
            {
                double mass_difference = experimental_masses[e2] - theoretical_product_masses[t2];
                if(product_mass_tolerance_units == MassToleranceUnits.ppm)
                {
                    mass_difference = mass_difference / theoretical_product_masses[t2] * 1e6;
                }
                if(Math.Abs(mass_difference) <= product_mass_tolerance_value)
                {
                    MatchingIntensity += experimental_intensities[e2];
                    e2++;
                }
                else if(mass_difference < 0)
                {
                    e2++;
                }
                else if(mass_difference > 0)
                {
                    t2++;
                }
            }
            MatchingIntensityFraction = MatchingIntensity / Spectrum.TotalIntensity;

            MorpheusScore = MatchingProducts + MatchingIntensityFraction;
        }

        public static int AscendingSpectrumNumberComparison(PeptideSpectrumMatch left, PeptideSpectrumMatch right)
        {
            return left.Spectrum.SpectrumNumber.CompareTo(right.Spectrum.SpectrumNumber);
        }

        public static int DescendingMorpheusScoreComparison(PeptideSpectrumMatch left, PeptideSpectrumMatch right)
        {
            int comparison = -(left.MorpheusScore.CompareTo(right.MorpheusScore));
            if(comparison != 0)
            {
                return comparison;
            }
            else
            {
                return left.Target.CompareTo(right.Target);
            }
        }

        public static readonly string Header = "Filename\tSpectrum Number\tSpectrum ID\tSpectrum Title\tRetention Time (minutes)\tPrecursor m/z\tPrecursor Intensity\tPrecursor Charge\tPrecursor Mass (Da)\tExperimental Peaks\tTotal Intensity"
            + "\tPeptide Sequence\tBase Peptide Sequence\tProtein Description\tStart Residue Number\tStop Residue Number\tMissed Cleavages"
            + "\tTheoretical Mass (Da)\tPrecursor Mass Error (Da)\tPrecursor Mass Error (ppm)"
            + "\tMatching Products\tTotal Products\tRatio of Matching Products\tMatching Intensity\tFraction of Intensity Matching\tMorpheus Score";

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Spectrum.Filename + '\t');
            sb.Append(Spectrum.SpectrumNumber.ToString() + '\t');
            sb.Append(Spectrum.SpectrumId + '\t');
            sb.Append(Spectrum.SpectrumTitle + '\t');
            sb.Append(Spectrum.RetentionTimeMinutes.ToString() + '\t');
            sb.Append(Spectrum.PrecursorMZ.ToString() + '\t');
            sb.Append(Spectrum.PrecursorIntensity.ToString() + '\t');
            sb.Append(Spectrum.PrecursorCharge.ToString() + '\t');
            sb.Append(Spectrum.PrecursorMass.ToString() + '\t');
            sb.Append(Spectrum.Masses.Length.ToString() + '\t');
            sb.Append(Spectrum.TotalIntensity.ToString() + '\t');
            sb.Append(Peptide.ExtendedSequence + '\t');
            sb.Append(Peptide.BaseSequence + '\t');
            sb.Append(Peptide.Parent.Description + '\t');
            sb.Append(Peptide.StartResidueNumber.ToString() + '\t');
            sb.Append(Peptide.EndResidueNumber.ToString() + '\t');
            sb.Append(Peptide.MissedCleavages.ToString() + '\t');
            sb.Append((precursorMassType == MassType.Average ? Peptide.AverageMass : Peptide.MonoisotopicMass).ToString() + '\t');
            sb.Append(PrecursorMassErrorDa.ToString() + '\t');
            sb.Append(PrecursorMassErrorPpm.ToString() + '\t');
            sb.Append(MatchingProducts.ToString() + '\t');
            sb.Append(TotalProducts.ToString() + '\t');
            sb.Append(MatchingProductsFraction.ToString() + '\t');
            sb.Append(MatchingIntensity.ToString() + '\t');
            sb.Append(MatchingIntensityFraction.ToString() + '\t');
            sb.Append(MorpheusScore.ToString());

            return sb.ToString();
        }
    }
}