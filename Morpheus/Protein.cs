using System.Collections.Generic;

namespace Morpheus
{
    public class Protein : AminoAcidPolymer
    {
        public string Description { get; private set; }

        public bool Decoy
        {
            get { return Description.Contains("DECOY_"); }
        }

        public bool Target
        {
            get { return !Decoy; }
        }

        public Protein(string sequence, string description)
            : base(sequence)
        {
            Description = description;
        }

        public IEnumerable<Peptide> Digest(Protease protease, int maximumMissedCleavages, InitiatorMethionineBehavior initiatorMethionineBehavior,
            int? minimumPeptideLength, int? maximumPeptideLength)
        {
            if(Length > 0)
            {
                List<int> indices = protease.GetDigestionSiteIndices(this);
                indices.Insert(0, -1);
                indices.Add(Length - 1);

                for(int missed_cleavages = 0; missed_cleavages <= maximumMissedCleavages; missed_cleavages++)
                {
                    for(int i = 0; i < indices.Count - missed_cleavages - 1; i++)
                    {
                        if(initiatorMethionineBehavior != InitiatorMethionineBehavior.Cleave || indices[i] + 1 != 0 || this[0] != 'M')
                        {
                            Peptide peptide = new Peptide(this, indices[i] + 1 + 1, indices[i + missed_cleavages + 1] + 1, missed_cleavages);

                            if((!minimumPeptideLength.HasValue || peptide.Length >= minimumPeptideLength.Value)
                                && (!maximumPeptideLength.HasValue || peptide.Length <= maximumPeptideLength.Value))
                            {
                                yield return peptide;
                            }
                        }

                        if(initiatorMethionineBehavior != InitiatorMethionineBehavior.Retain && indices[i] + 1 == 0 && this[0] == 'M')
                        {
                            if(indices[i + missed_cleavages + 1] + 1 >= indices[i] + 1 + 1 + 1)
                            {
                                Peptide peptide_without_initiator_methionine = new Peptide(this, indices[i] + 1 + 1 + 1, indices[i + missed_cleavages + 1] + 1, missed_cleavages);

                                if((!minimumPeptideLength.HasValue || peptide_without_initiator_methionine.Length >= minimumPeptideLength.Value)
                                    && (!maximumPeptideLength.HasValue || peptide_without_initiator_methionine.Length <= maximumPeptideLength.Value))
                                {
                                    yield return peptide_without_initiator_methionine;
                                }
                            }
                        }
                    }
                }
            }
        }

        public IEnumerable<Protein> GetVariablyModifiedProteins(IEnumerable<Modification> variableModifications, int maximumVariableModificationIsoforms)
        {
            Dictionary<int, List<Modification>> possible_modifications = new Dictionary<int, List<Modification>>();

            foreach(Modification variable_modification in variableModifications)
            {
                if(variable_modification.Type == ModificationType.ProteinNTerminus && (variable_modification.AminoAcid == char.MinValue || this[0] == variable_modification.AminoAcid))
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

                if(variable_modification.Type == ModificationType.ProteinCTerminus && (variable_modification.AminoAcid == char.MinValue || this[Length - 1] == variable_modification.AminoAcid))
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
                Protein protein = new Protein(BaseSequence, Description);
                protein.FixedModifications = FixedModifications;
                protein.VariableModifications = kvp;
                yield return protein;
                variable_modification_isoforms++;
                if(variable_modification_isoforms == maximumVariableModificationIsoforms)
                {
                    yield break;
                }
            }
        }

        public double CalculateSequenceCoverage(IEnumerable<PeptideSpectrumMatch> peptideSpectrumMatches)
        {
            HashSet<int> covered_residues = new HashSet<int>();
            foreach(PeptideSpectrumMatch psm in peptideSpectrumMatches)
            {
                for(int r = psm.Peptide.StartResidueNumber; r <= psm.Peptide.EndResidueNumber; r++)
                {
                    covered_residues.Add(r);
                }
            }
            return (double)covered_residues.Count / Length;
        }
    }
}