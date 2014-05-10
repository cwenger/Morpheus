using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Morpheus
{
    public class ModificationDictionary : Dictionary<string, Modification>
    {
        private static readonly ModificationDictionary instance = new ModificationDictionary();

        private ModificationDictionary()
            : base()
        {
            using(StreamReader mods = new StreamReader(Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), "modifications.tsv")))
            {
                mods.ReadLine();

                while(mods.Peek() != -1)
                {
                    string line = mods.ReadLine();
                    string[] fields = line.Split('\t');

                    string description = fields[0];
                    ModificationType modification_type;
                    if(fields[1].Equals("amino acid residue", StringComparison.OrdinalIgnoreCase))
                    {
                        modification_type = ModificationType.AminoAcidResidue;
                    }
                    else if(fields[1].Equals("protein N-terminus", StringComparison.OrdinalIgnoreCase))
                    {
                        modification_type = ModificationType.ProteinNTerminus;
                    }
                    else if(fields[1].Equals("protein C-terminus", StringComparison.OrdinalIgnoreCase))
                    {
                        modification_type = ModificationType.ProteinCTerminus;
                    }
                    else if(fields[1].Equals("peptide N-terminus", StringComparison.OrdinalIgnoreCase))
                    {
                        modification_type = ModificationType.PeptideNTerminus;
                    }
                    else if(fields[1].Equals("peptide C-terminus", StringComparison.OrdinalIgnoreCase))
                    {
                        modification_type = ModificationType.PeptideCTerminus;
                    }
                    else
                    {
                        throw new ArgumentException("Modification type '" + fields[1] + "' is invalid.");
                    }
                    char amino_acid;
                    char.TryParse(fields[2], out amino_acid);
                    double monoisotopic_mass_shift = double.Parse(fields[3], CultureInfo.InvariantCulture);
                    double average_mass_shift = double.Parse(fields[4], CultureInfo.InvariantCulture);
                    string default_mod = fields[5];
                    bool default_fixed = default_mod.Equals("fixed", StringComparison.OrdinalIgnoreCase);
                    bool default_variable = default_mod.Equals("variable", StringComparison.OrdinalIgnoreCase);
                    Modification modification = new Modification(description, modification_type, amino_acid, monoisotopic_mass_shift, average_mass_shift, default_fixed, default_variable, false);
                    Add(modification);
                }
            }
        }

        public static ModificationDictionary Instance
        {
            get { return instance; }
        }

        public void Add(Modification modification)
        {
            Add(modification.Description, modification);
        }
    }
}