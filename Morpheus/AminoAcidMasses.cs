using System;
using System.Globalization;
using System.IO;

namespace Morpheus
{
    public static class AminoAcidMasses
    {
        private static readonly double[] MONOISOTOPIC_AMINO_ACID_MASSES = new double['Z' - 'A' + 1];
        private static readonly double[] AVERAGE_AMINO_ACID_MASSES = new double['Z' - 'A' + 1];

        static AminoAcidMasses()
        {
            using(StreamReader amino_acids = new StreamReader(Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), "amino_acids.tsv")))
            {
                string header = amino_acids.ReadLine();

                while(amino_acids.Peek() != -1)
                {
                    string line = amino_acids.ReadLine();
                    string[] fields = line.Split('\t');

                    char one_letter_code = char.Parse(fields[0]);
                    if(!char.IsUpper(one_letter_code))
                    {
                        throw new ArgumentOutOfRangeException("Invalid amino acid abbreviation: " + one_letter_code);
                    }
                    double monoisotopic_mass = double.Parse(fields[1], CultureInfo.InvariantCulture);
                    MONOISOTOPIC_AMINO_ACID_MASSES[one_letter_code - 'A'] = monoisotopic_mass;
                    double average_mass = double.Parse(fields[2], CultureInfo.InvariantCulture);
                    AVERAGE_AMINO_ACID_MASSES[one_letter_code - 'A'] = average_mass;
                }
            }
        }

        public static double GetMonoisotopicMass(char aminoAcid)
        {
            return MONOISOTOPIC_AMINO_ACID_MASSES[aminoAcid - 'A'];
        }

        public static double GetAverageMass(char aminoAcid)
        {
            return AVERAGE_AMINO_ACID_MASSES[aminoAcid - 'A'];
        }
    }
}