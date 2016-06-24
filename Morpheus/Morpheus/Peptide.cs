using System.Collections.Generic;

namespace Morpheus
{
    public class Peptide : AminoAcidPolymer
    {
        public Protein Parent { get; private set; }

        public int StartResidueNumber { get; private set; }

        public int EndResidueNumber { get; private set; }

        public int MissedCleavages { get; private set; }

        public char PreviousAminoAcid { get; private set; }

        public char NextAminoAcid { get; private set; }

        public bool Decoy
        {
            get { return Parent.Decoy; }
        }

        public bool Target
        {
            get { return !Decoy; }
        }

        public string ExtendedSequence
        {
            get
            {
                return PreviousAminoAcid.ToString() + '.' + Sequence + '.' + NextAminoAcid.ToString();
            }
        }

        public string ExtendedLeucineSequence
        {
            get
            {
                return ExtendedSequence.Replace('I', 'L');
            }
        }

        public Peptide(Protein parent, int startResidueNumber, int endResidueNumber, int missedCleavages)
            : base(parent.BaseSequence.Substring(startResidueNumber - 1, endResidueNumber - startResidueNumber + 1))
        {
            Parent = parent;
            StartResidueNumber = startResidueNumber;
            EndResidueNumber = endResidueNumber;
            MissedCleavages = missedCleavages;
            if(startResidueNumber - 1 - 1 >= 0)
            {
                PreviousAminoAcid = parent[startResidueNumber - 1 - 1];
            }
            else
            {
                PreviousAminoAcid = '-';
            }
            if(endResidueNumber - 1 + 1 < parent.Length)
            {
                NextAminoAcid = parent[endResidueNumber - 1 + 1];
            }
            else
            {
                NextAminoAcid = '-';
            }
            if(parent.KnownModifications != null && parent.KnownModifications.Count > 0)
            {
                for(int i = 0; i < endResidueNumber - startResidueNumber + 1; i++)
                {
                    List<Modification> modifications;
                    if(parent.KnownModifications.TryGetValue(startResidueNumber - 1 + i + 2, out modifications))
                    {
                        if(KnownModifications == null)
                        {
                            KnownModifications = new Dictionary<int, List<Modification>>();
                        }
                        KnownModifications.Add(i + 2, modifications);
                    }
                }
            }
        }

        private Peptide(Peptide peptide) : this(peptide.Parent, peptide.StartResidueNumber, peptide.EndResidueNumber, peptide.MissedCleavages) { }

        public IEnumerable<Peptide> GetVariablyModifiedPeptides(IEnumerable<Modification> variableModifications, int maximumVariableModificationIsoforms)
        {
            Dictionary<int, List<Modification>> possible_modifications = new Dictionary<int, List<Modification>>(Length + 4);

            foreach(Modification variable_modification in variableModifications)
            {
                if(variable_modification.Type == ModificationType.ProteinNTerminus && (StartResidueNumber == 1 || (StartResidueNumber == 2 && Parent[0] == 'M')) 
                    && (variable_modification.AminoAcid == char.MinValue || this[0] == variable_modification.AminoAcid))
                {
                    List<Modification> prot_n_term_variable_mods;
                    if(!possible_modifications.TryGetValue(0, out prot_n_term_variable_mods))
                    {
                        prot_n_term_variable_mods = new List<Modification>();
                        prot_n_term_variable_mods.Add(variable_modification);
                        possible_modifications.Add(0, prot_n_term_variable_mods);
                    }
                    else
                    {
                        prot_n_term_variable_mods.Add(variable_modification);
                    }
                }

                if(variable_modification.Type == ModificationType.PeptideNTerminus && (variable_modification.AminoAcid == char.MinValue || this[0] == variable_modification.AminoAcid))
                {
                    List<Modification> pep_n_term_variable_mods;
                    if(!possible_modifications.TryGetValue(1, out pep_n_term_variable_mods))
                    {
                        pep_n_term_variable_mods = new List<Modification>();
                        pep_n_term_variable_mods.Add(variable_modification);
                        possible_modifications.Add(1, pep_n_term_variable_mods);
                    }
                    else
                    {
                        pep_n_term_variable_mods.Add(variable_modification);
                    }
                }

                for(int r = 0; r < Length; r++)
                {
                    if(variable_modification.Type == ModificationType.AminoAcidResidue && this[r] == variable_modification.AminoAcid)
                    {
                        List<Modification> residue_variable_mods;
                        if(!possible_modifications.TryGetValue(r + 2, out residue_variable_mods))
                        {
                            residue_variable_mods = new List<Modification>();
                            residue_variable_mods.Add(variable_modification);
                            possible_modifications.Add(r + 2, residue_variable_mods);
                        }
                        else
                        {
                            residue_variable_mods.Add(variable_modification);
                        }
                    }
                }

                if(variable_modification.Type == ModificationType.PeptideCTerminus && (variable_modification.AminoAcid == char.MinValue || this[Length - 1] == variable_modification.AminoAcid))
                {
                    List<Modification> pep_c_term_variable_mods;
                    if(!possible_modifications.TryGetValue(Length + 2, out pep_c_term_variable_mods))
                    {
                        pep_c_term_variable_mods = new List<Modification>();
                        pep_c_term_variable_mods.Add(variable_modification);
                        possible_modifications.Add(Length + 2, pep_c_term_variable_mods);
                    }
                    else
                    {
                        pep_c_term_variable_mods.Add(variable_modification);
                    }
                }

                if(variable_modification.Type == ModificationType.ProteinCTerminus && (EndResidueNumber == Parent.Length - 1) 
                    && (variable_modification.AminoAcid == char.MinValue || this[Length - 1] == variable_modification.AminoAcid))
                {
                    List<Modification> prot_c_term_variable_mods;
                    if(!possible_modifications.TryGetValue(Length + 3, out prot_c_term_variable_mods))
                    {
                        prot_c_term_variable_mods = new List<Modification>();
                        prot_c_term_variable_mods.Add(variable_modification);
                        possible_modifications.Add(Length + 3, prot_c_term_variable_mods);
                    }
                    else
                    {
                        prot_c_term_variable_mods.Add(variable_modification);
                    }
                }
            }

            int variable_modification_isoforms = 0;
            foreach(Dictionary<int, Modification> kvp in GetVariableModificationPatterns(possible_modifications))
            {
                Peptide peptide = new Peptide(this);
                peptide.FixedModifications = FixedModifications;
                peptide.VariableModifications = kvp;
                yield return peptide;
                variable_modification_isoforms++;
                if(variable_modification_isoforms == maximumVariableModificationIsoforms)
                {
                    yield break;
                }
            }
        }
    }
}