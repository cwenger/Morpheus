using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace Morpheus
{
    public static class ProteomeDatabaseReader
    {
        public static bool HasDecoyProteins(string fastaProteomeDatabaseFilepath)
        {
            using(StreamReader fasta = new StreamReader(fastaProteomeDatabaseFilepath))
            {
                while(!fasta.EndOfStream)
                {
                    string line = fasta.ReadLine();
                    if(line.StartsWith(">") && line.Contains(Protein.DECOY_IDENTIFIER))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static Dictionary<string, string> proteinExistenceCodes;
        private static Dictionary<string, ModificationType> modificationTypeCodes;
        private static Dictionary<string, char> aminoAcidCodes;

        private static void InitializeDictionaries()
        {
            proteinExistenceCodes = new Dictionary<string, string>();
            proteinExistenceCodes.Add("evidence at protein level", "1");
            proteinExistenceCodes.Add("evidence at transcript level", "2");
            proteinExistenceCodes.Add("inferred from homology", "3");
            proteinExistenceCodes.Add("predicted", "4");
            proteinExistenceCodes.Add("uncertain", "5");

            modificationTypeCodes = new Dictionary<string, ModificationType>();
            modificationTypeCodes.Add("N-terminal.", ModificationType.ProteinNTerminus);
            modificationTypeCodes.Add("C-terminal.", ModificationType.ProteinCTerminus);

            aminoAcidCodes = new Dictionary<string, char>();
            aminoAcidCodes.Add("Alanine", 'A');
            aminoAcidCodes.Add("Arginine", 'R');
            aminoAcidCodes.Add("Asparagine", 'N');
            aminoAcidCodes.Add("Aspartate", 'D');
            aminoAcidCodes.Add("Cysteine", 'C');
            aminoAcidCodes.Add("Glutamate", 'E');
            aminoAcidCodes.Add("Glutamine", 'Q');
            aminoAcidCodes.Add("Glycine", 'G');
            aminoAcidCodes.Add("Histidine", 'H');
            aminoAcidCodes.Add("Isoleucine", 'I');
            aminoAcidCodes.Add("Leucine", 'L');
            aminoAcidCodes.Add("Lysine", 'K');
            aminoAcidCodes.Add("Methionine", 'M');
            aminoAcidCodes.Add("Phenylalanine", 'F');
            aminoAcidCodes.Add("Proline", 'P');
            aminoAcidCodes.Add("Serine", 'S');
            aminoAcidCodes.Add("Threonine", 'T');
            aminoAcidCodes.Add("Tryptophan", 'W');
            aminoAcidCodes.Add("Tyrosine", 'Y');
            aminoAcidCodes.Add("Valine", 'V');
        }

        private const bool ONLY_CONSIDER_MODIFICATIONS_WITH_EVIDENCE = false;
        private static readonly Regex PSI_MOD_ACCESSION_NUMBER_REGEX = new Regex(@"(.+); (\d+)\.");

        public static Dictionary<string, Modification> ReadUniProtXmlModifications(string uniProtXmlProteomeDatabaseFilepath)
        {
            if(proteinExistenceCodes == null)
            {
                InitializeDictionaries();
            }

            List<string> modifications_in_database = new List<string>();
            using(XmlReader xml = XmlReader.Create(uniProtXmlProteomeDatabaseFilepath))
            {
                while(xml.ReadToFollowing("feature"))
                {
                    if(xml.GetAttribute("type") == "modified residue" && (!ONLY_CONSIDER_MODIFICATIONS_WITH_EVIDENCE || xml.GetAttribute("evidence") != null))
                    {
                        string description = xml.GetAttribute("description");
                        if(!description.Contains("variant"))
                        {
                            int semicolon_index = description.IndexOf(';');
                            if(semicolon_index >= 0)
                            {
                                description = description.Substring(0, semicolon_index);
                            }
                            modifications_in_database.Add(description);
                        }
                    }
                }
            }

            Dictionary<string, Modification> modifications = new Dictionary<string, Modification>();

            ModificationDictionary user_modifications = ModificationDictionary.Instance;
            int i = 0;
            while(i < modifications_in_database.Count)
            {
                string modification_name = modifications_in_database[i];
                Modification modification;
                if(user_modifications.TryGetValue(modification_name, out modification))
                {
                    modifications.Add("UniProt: " + modification_name, new Modification("UniProt: " + modification_name, modification.Type, modification.AminoAcid, modification.MonoisotopicMassShift, modification.AverageMassShift, modification.MonoisotopicNeutralLossMass, modification.AverageNeutralLossMass, false, true, modification.Database, modification.DatabaseAccessionNumber, modification.DatabaseName, true));
                    modifications_in_database.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            if(modifications_in_database.Count > 0)
            {
                string old_ptmlist_filepath = Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), "ptmlist.txt");
                try
                {
                    string new_ptmlist_filepath = Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), "ptmlist.new.txt");
                    using(WebClient client = new WebClient())
                    {
                        client.DownloadFile("http://www.uniprot.org/docs/ptmlist.txt", new_ptmlist_filepath);
                    }
                    string old_ptmlist = File.ReadAllText(old_ptmlist_filepath);
                    string new_ptmlist = File.ReadAllText(new_ptmlist_filepath);
                    if(string.Equals(old_ptmlist, new_ptmlist))
                    {
                        File.Delete(new_ptmlist_filepath);
                    }
                    else
                    {
                        File.Delete(old_ptmlist_filepath);
                        File.Move(new_ptmlist_filepath, old_ptmlist_filepath);
                    }
                }
                catch
                {

                }

                XmlDocument psi_mod_temp = new XmlDocument();
                psi_mod_temp.Load(Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), "PSI-MOD.obo.xml"));
                XPathNavigator psi_mod = psi_mod_temp.CreateNavigator();

                using(StreamReader uniprot_mods = new StreamReader(old_ptmlist_filepath))
                {
                    string description = null;
                    string feature_type = null;
                    ModificationType modification_type = ModificationType.AminoAcidResidue;
                    char amino_acid_residue = '\0';
                    double monoisotopic_mass_shift = double.NaN;
                    double average_mass_shift = double.NaN;
                    string database = null;
                    int database_accession_number = -1;
                    string database_name = null;
                    while(uniprot_mods.Peek() != -1)
                    {
                        string line = uniprot_mods.ReadLine();
                        if(line.Length >= 2)
                        {
                            switch(line.Substring(0, 2))
                            {
                                case "ID":
                                    description = line.Substring(5);
                                    break;
                                case "FT":
                                    feature_type = line.Substring(5);
                                    break;
                                case "TG":
                                    if(feature_type == "MOD_RES")
                                    {
                                        string amino_acid = line.Substring(5);
                                        aminoAcidCodes.TryGetValue(char.ToUpperInvariant(amino_acid[0]) + amino_acid.Substring(1).TrimEnd('.'), out amino_acid_residue);
                                    }
                                    break;
                                case "PP":
                                    if(feature_type == "MOD_RES")
                                    {
                                        modificationTypeCodes.TryGetValue(line.Substring(5), out modification_type);
                                    }
                                    break;
                                case "MM":
                                    monoisotopic_mass_shift = double.Parse(line.Substring(5));
                                    break;
                                case "MA":
                                    average_mass_shift = double.Parse(line.Substring(5));
                                    break;
                                case "DR":
                                    if(line.Contains("PSI-MOD"))
                                    {
                                        Match match = PSI_MOD_ACCESSION_NUMBER_REGEX.Match(line.Substring(5));
                                        if(match.Success)
                                        {
                                            database = match.Groups[1].Value;
                                            database_accession_number = int.Parse(match.Groups[2].Value);
                                            XPathNavigator term = psi_mod.SelectSingleNode(@"/obo/term[id='MOD:" + database_accession_number.ToString("00000") + "']");
                                            database_name = term.SelectSingleNode("name").Value;
                                        }
                                    }
                                    break;
                                case "//":
                                    if(feature_type == "MOD_RES" && modifications_in_database.Contains(description) && (!double.IsNaN(monoisotopic_mass_shift) || !double.IsNaN(average_mass_shift)))
                                    {
                                        Modification modification = new Modification("UniProt: " + description, ModificationType.AminoAcidResidue, amino_acid_residue, monoisotopic_mass_shift, average_mass_shift, 0.0, 0.0, false, true, database, database_accession_number, database_name, true);
                                        modifications.Add(modification.Description, modification);
                                    }
                                    description = null;
                                    feature_type = null;
                                    modification_type = ModificationType.AminoAcidResidue;
                                    amino_acid_residue = '\0';
                                    monoisotopic_mass_shift = double.NaN;
                                    average_mass_shift = double.NaN;
                                    break;
                            }
                        }
                    }
                }
            }

            return modifications;
        }

        public static int CountProteins(FileStream proteomeDatabase, bool onTheFlyDecoys, out int targetProteins, out int decoyProteins, out int onTheFlyDecoyProteins)
        {
            if(Path.GetExtension(proteomeDatabase.Name).Equals(".xml", StringComparison.InvariantCultureIgnoreCase))
            {
                return CountUniProtXmlProteins(proteomeDatabase, onTheFlyDecoys, out targetProteins, out decoyProteins, out onTheFlyDecoyProteins);
            }
            else
            {
                return CountFastaProteins(proteomeDatabase, onTheFlyDecoys, out targetProteins, out decoyProteins, out onTheFlyDecoyProteins);
            }
        }

        private static int CountFastaProteins(FileStream fastaProteomeDatabase, bool onTheFlyDecoys, out int targetProteins, out int decoyProteins, out int onTheFlyDecoyProteins)
        {
            targetProteins = 0;
            decoyProteins = 0;
            onTheFlyDecoyProteins = 0;

            StreamReader fasta = new StreamReader(fastaProteomeDatabase);

            string description = null;

            while(true)
            {
                string line = fasta.ReadLine();

                if(line.StartsWith(">"))
                {
                    description = line.Substring(1);
                }

                if(fasta.Peek() == '>' || fasta.Peek() == -1)
                {
                    if(description.Contains(Protein.DECOY_IDENTIFIER))
                    {
                        if(onTheFlyDecoys)
                        {
                            throw new ArgumentException(fastaProteomeDatabase.Name + " contains decoy proteins; database should not contain decoy proteins when \"create target–decoy database on the fly\" option is enabled");
                        }
                        decoyProteins++;
                    }
                    else
                    {
                        targetProteins++;
                        if(onTheFlyDecoys)
                        {
                            onTheFlyDecoyProteins++;
                        }
                    }

                    description = null;

                    if(fasta.Peek() == -1)
                    {
                        break;
                    }
                }
            }

            fastaProteomeDatabase.Seek(0, SeekOrigin.Begin);

            return targetProteins + decoyProteins + onTheFlyDecoyProteins;
        }

        private static int CountUniProtXmlProteins(FileStream uniProtXmlProteomeDatabase, bool onTheFlyDecoys, out int targetProteins, out int decoyProteins, out int onTheFlyDecoyProteins)
        {
            targetProteins = 0;
            decoyProteins = 0;
            onTheFlyDecoyProteins = 0;

            XmlReader xml = XmlReader.Create(uniProtXmlProteomeDatabase);

            string[] nodes = new string[6];

            string dataset = null;
            string accession = null;
            string name = null;
            string full_name = null;
            bool fragment = false;
            string organism = null;
            string gene_name = null;
            string protein_existence = null;
            string sequence_version = null;
            while(xml.Read())
            {
                switch(xml.NodeType)
                {
                    case XmlNodeType.Element:
                        nodes[xml.Depth] = xml.Name;
                        switch(xml.Name)
                        {
                            case "entry":
                                dataset = xml.GetAttribute("dataset");
                                break;
                            case "accession":
                                if(accession == null)
                                {
                                    accession = xml.ReadElementString();
                                }
                                break;
                            case "name":
                                if(xml.Depth == 2)
                                {
                                    name = xml.ReadElementString();
                                }
                                else if(nodes[2] == "gene")
                                {
                                    if(gene_name == null)
                                    {
                                        gene_name = xml.ReadElementString();
                                    }
                                }
                                else if(nodes[2] == "organism")
                                {
                                    if(organism == null)
                                    {
                                        organism = xml.ReadElementString();
                                    }
                                }
                                break;
                            case "fullName":
                                if(full_name == null)
                                {
                                    full_name = xml.ReadElementString();
                                }
                                break;
                            case "proteinExistence":
                                protein_existence = xml.GetAttribute("type");
                                break;
                            case "sequence":
                                fragment = xml.GetAttribute("fragment") != null;
                                sequence_version = xml.GetAttribute("version");
                                break;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        switch(xml.Name)
                        {
                            case "entry":
                                string dataset_abbreviation = dataset.Equals("Swiss-Prot", StringComparison.InvariantCultureIgnoreCase) ? "sp" : "tr";
                                string protein_existence_code = proteinExistenceCodes[protein_existence];
                                string description = dataset_abbreviation + '|' + accession + '|' + name + ' ' + full_name + (fragment ? " (Fragment)" : null) + " OS=" + organism + (gene_name != null ? " GN=" + gene_name : null) + " PE=" + protein_existence_code + " SV=" + sequence_version;
                                if(description.Contains(Protein.DECOY_IDENTIFIER))
                                {
                                    if(onTheFlyDecoys)
                                    {
                                        throw new ArgumentException(uniProtXmlProteomeDatabase.Name + " contains decoy proteins; database should not contain decoy proteins when \"create target–decoy database on the fly\" option is enabled");
                                    }
                                    decoyProteins++;
                                }
                                else
                                {
                                    targetProteins++;
                                    if(onTheFlyDecoys)
                                    {
                                        onTheFlyDecoyProteins++;
                                    }
                                }

                                dataset = null;
                                accession = null;
                                name = null;
                                full_name = null;
                                fragment = false;
                                organism = null;
                                gene_name = null;
                                protein_existence = null;
                                sequence_version = null;

                                break;
                        }
                        break;
                }
            }

            uniProtXmlProteomeDatabase.Seek(0, SeekOrigin.Begin);

            return targetProteins + decoyProteins + onTheFlyDecoyProteins;
        }

        public static IEnumerable<Protein> ReadProteins(FileStream proteomeDatabase, bool onTheFlyDecoys, IDictionary<string, Modification> knownVariableModifications)
        {
            if(Path.GetExtension(proteomeDatabase.Name).Equals(".xml", StringComparison.InvariantCultureIgnoreCase))
            {
                return ReadUniProtXmlProteins(proteomeDatabase, onTheFlyDecoys, knownVariableModifications);
            }
            else
            {
                return ReadFastaProteins(proteomeDatabase, onTheFlyDecoys);
            }
        }

        private static IEnumerable<Protein> ReadFastaProteins(FileStream fastaProteomeDatabase, bool onTheFlyDecoys)
        {
            StreamReader fasta = new StreamReader(fastaProteomeDatabase);

            string description = null;
            string sequence = null;

            while(true)
            {
                string line = fasta.ReadLine();

                if(line.StartsWith(">"))
                {
                    description = line.Substring(1);
                }
                else
                {
                    sequence += line.Trim();
                }

                if(fasta.Peek() == '>' || fasta.Peek() == -1)
                {
                    Protein protein = new Protein(sequence, description);

                    yield return protein;

                    if(onTheFlyDecoys)
                    {
                        if(protein.Decoy)
                        {
                            throw new ArgumentException(fastaProteomeDatabase.Name + " contains decoy proteins; database should not contain decoy proteins when \"create target–decoy database on the fly\" option is enabled");
                        }
                        char[] sequence_array = sequence.ToCharArray();
                        if(sequence.StartsWith("M"))
                        {
                            Array.Reverse(sequence_array, 1, sequence.Length - 1);
                        }
                        else
                        {
                            Array.Reverse(sequence_array);
                        }
                        string reversed_sequence = new string(sequence_array);
                        Protein decoy_protein = new Protein(reversed_sequence, description[2] == '|' ? description.Insert(3, Protein.DECOY_IDENTIFIER) : Protein.DECOY_IDENTIFIER + description);
                        yield return decoy_protein;
                    }

                    description = null;
                    sequence = null;

                    if(fasta.Peek() == -1)
                    {
                        break;
                    }
                }
            }

            fastaProteomeDatabase.Seek(0, SeekOrigin.Begin);
        }

        private static IEnumerable<Protein> ReadUniProtXmlProteins(FileStream uniProtXmlProteomeDatabase, bool onTheFlyDecoys, IDictionary<string, Modification> knownVariableModifications)
        {
            XmlReader xml = XmlReader.Create(uniProtXmlProteomeDatabase);

            string[] nodes = new string[6];

            string dataset = null;
            string accession = null;
            string name = null;
            string full_name = null;
            bool fragment = false;
            string organism = null;
            string gene_name = null;
            string protein_existence = null;
            string sequence_version = null;
            string sequence = null;
            string feature_type = null;
            string feature_description = null;
            int feature_position = -1;
            Dictionary<int, List<Modification>> modifications = new Dictionary<int, List<Modification>>();
            while(xml.Read())
            {
                switch(xml.NodeType)
                {
                    case XmlNodeType.Element:
                        nodes[xml.Depth] = xml.Name;
                        switch(xml.Name)
                        {
                            case "entry":
                                dataset = xml.GetAttribute("dataset");
                                break;
                            case "accession":
                                if(accession == null)
                                {
                                    accession = xml.ReadElementString();
                                }
                                break;
                            case "name":
                                if(xml.Depth == 2)
                                {
                                    name = xml.ReadElementString();
                                }
                                else if(nodes[2] == "gene")
                                {
                                    if(gene_name == null)
                                    {
                                        gene_name = xml.ReadElementString();
                                    }
                                }
                                else if(nodes[2] == "organism")
                                {
                                    if(organism == null)
                                    {
                                        organism = xml.ReadElementString();
                                    }
                                }
                                break;
                            case "fullName":
                                if(full_name == null)
                                {
                                    full_name = xml.ReadElementString();
                                }
                                break;
                            case "proteinExistence":
                                protein_existence = xml.GetAttribute("type");
                                break;
                            case "feature":
                                feature_type = xml.GetAttribute("type");
                                feature_description = xml.GetAttribute("description");
                                break;
                            case "position":
                                feature_position = int.Parse(xml.GetAttribute("position")) - 1;
                                break;
                            case "sequence":
                                fragment = xml.GetAttribute("fragment") != null;
                                sequence_version = xml.GetAttribute("version");
                                sequence = xml.ReadElementString().Replace("\n", null);
                                break;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        switch(xml.Name)
                        {
                            case "feature":
                                if(feature_type == "modified residue" && knownVariableModifications != null && !feature_description.Contains("variant"))
                                {
                                    List<Modification> residue_modifications;
                                    if(!modifications.TryGetValue(feature_position + 2, out residue_modifications))
                                    {
                                        residue_modifications = new List<Modification>();
                                        modifications.Add(feature_position + 2, residue_modifications);
                                    }
                                    int semicolon_index = feature_description.IndexOf(';');
                                    if(semicolon_index >= 0)
                                    {
                                        feature_description = feature_description.Substring(0, semicolon_index);
                                    }
                                    Modification modification;
                                    if(knownVariableModifications.TryGetValue("UniProt: " + feature_description, out modification))
                                    {
                                        residue_modifications.Add(modification);
                                    }
                                }
                                break;
                            case "entry":
                                string dataset_abbreviation = dataset.Equals("Swiss-Prot", StringComparison.InvariantCultureIgnoreCase) ? "sp" : "tr";
                                string protein_existence_code = proteinExistenceCodes[protein_existence];
                                string description = dataset_abbreviation + '|' + accession + '|' + name + ' ' + full_name + (fragment ? " (Fragment)" : null) + " OS=" + organism + (gene_name != null ? " GN=" + gene_name : null) + " PE=" + protein_existence_code + " SV=" + sequence_version;
                                Protein protein;
                                if(modifications.Count > 0)
                                {
                                    protein = new Protein(sequence, description, modifications);
                                }
                                else
                                {
                                    protein = new Protein(sequence, description);
                                }

                                yield return protein;

                                if(onTheFlyDecoys)
                                {
                                    if(protein.Decoy)
                                    {
                                        throw new ArgumentException(uniProtXmlProteomeDatabase.Name + " contains decoy proteins; database should not contain decoy proteins when \"create target–decoy database on the fly\" option is enabled");
                                    }
                                    char[] sequence_array = sequence.ToCharArray();
                                    Dictionary<int, List<Modification>> decoy_modifications = null;
                                    if(sequence.StartsWith("M"))
                                    {
                                        Array.Reverse(sequence_array, 1, sequence.Length - 1);
                                        if(modifications != null)
                                        {
                                            decoy_modifications = new Dictionary<int, List<Modification>>(modifications.Count);
                                            foreach(KeyValuePair<int, List<Modification>> kvp in modifications)
                                            {
                                                if(kvp.Key == 2)
                                                {
                                                    decoy_modifications.Add(2, kvp.Value);
                                                }
                                                else if(kvp.Key > 2)
                                                {
                                                    decoy_modifications.Add(sequence.Length - (kvp.Key - 2) - 1 + 1 + 2, kvp.Value);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Array.Reverse(sequence_array);
                                        if(modifications != null)
                                        {
                                            decoy_modifications = new Dictionary<int, List<Modification>>(modifications.Count);
                                            foreach(KeyValuePair<int, List<Modification>> kvp in modifications)
                                            {
                                                decoy_modifications.Add(sequence.Length - (kvp.Key - 2) - 1 + 2, kvp.Value);
                                            }
                                        }
                                    }
                                    string reversed_sequence = new string(sequence_array);
                                    Protein decoy_protein = new Protein(reversed_sequence, description[2] == '|' ? description.Insert(3, Protein.DECOY_IDENTIFIER) : Protein.DECOY_IDENTIFIER + description, decoy_modifications);
                                    yield return decoy_protein;
                                }

                                dataset = null;
                                accession = null;
                                name = null;
                                full_name = null;
                                fragment = false;
                                organism = null;
                                gene_name = null;
                                protein_existence = null;
                                sequence_version = null;
                                sequence = null;
                                feature_type = null;
                                feature_description = null;
                                feature_position = -1;
                                modifications = new Dictionary<int, List<Modification>>();

                                break;
                        }
                        break;
                }
            }

            uniProtXmlProteomeDatabase.Seek(0, SeekOrigin.Begin);
        }
    }
}