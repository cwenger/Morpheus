using System;
using System.Collections.Generic;
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
                string header = mods.ReadLine();

                while(mods.Peek() != -1)
                {
                    string line = mods.ReadLine();
                    string[] fields = line.Split('\t');

                    string description = fields[0];
                    ModificationType modification_type;
                    switch(fields[1])
                    {
                        case "amino acid residue":
                            modification_type = ModificationType.AminoAcidResidue;
                            break;
                        case "protein N-terminus":
                            modification_type = ModificationType.ProteinNTerminus;
                            break;
                        case "protein C-terminus":
                            modification_type = ModificationType.ProteinCTerminus;
                            break;
                        case "peptide N-terminus":
                            modification_type = ModificationType.PeptideNTerminus;
                            break;
                        case "peptide C-terminus":
                            modification_type = ModificationType.PeptideCTerminus;
                            break;
                        default:
                            modification_type = ModificationType.AminoAcidResidue;
                            break;
                    }
                    char amino_acid;
                    char.TryParse(fields[2], out amino_acid);
                    double monoisotopic_mass_shift = double.Parse(fields[3]);
                    double average_mass_shift = double.Parse(fields[4]);
                    string default_mod = fields[5];
                    bool default_fixed = default_mod.Equals("fixed", StringComparison.OrdinalIgnoreCase);
                    bool default_variable = default_mod.Equals("variable", StringComparison.OrdinalIgnoreCase);
                    Modification modification = new Modification(description, modification_type, amino_acid, monoisotopic_mass_shift, average_mass_shift, default_fixed, default_variable);
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