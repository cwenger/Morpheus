using System;
using System.Collections.Generic;

namespace Morpheus
{
    public class Protein : AminoAcidPolymer, IComparable<Protein>
    {
        public string Description { get; private set; }

        public const string DECOY_IDENTIFIER = "DECOY_";

        public bool Decoy
        {
            get { return Description.Contains(DECOY_IDENTIFIER); }
        }

        public bool Target
        {
            get { return !Decoy; }
        }

        // just used for calculating sequence coverage
        public Dictionary<string, List<Peptide>> IdentifiedPeptides { get; private set; }

        public Protein(string sequence, string description)
            : base(sequence)
        {
            Description = description;
            IdentifiedPeptides = new Dictionary<string, List<Peptide>>();
        }

        public IEnumerable<Peptide> Digest(Protease protease, int maximumMissedCleavages, InitiatorMethionineBehavior initiatorMethionineBehavior,
            int? minimumPeptideLength, int? maximumPeptideLength)
        {
            if(Length > 0)
            {
                if(protease.CleavageSpecificity != CleavageSpecificity.None)
                {
                    // these are the 0-based residue indices the protease cleaves AFTER
                    List<int> indices = protease.GetDigestionSiteIndices(this);
                    indices.Insert(0, -1);
                    indices.Add(Length - 1);

                    if(protease.CleavageSpecificity == CleavageSpecificity.Full)
                    {
                        for(int missed_cleavages = 0; missed_cleavages <= maximumMissedCleavages; missed_cleavages++)
                        {
                            for(int i = 0; i < indices.Count - missed_cleavages - 1; i++)
                            {
                                if(initiatorMethionineBehavior != InitiatorMethionineBehavior.Cleave || indices[i] + 1 != 0 || this[0] != 'M')
                                {
                                    int length = indices[i + missed_cleavages + 1] - indices[i];
                                    if((!minimumPeptideLength.HasValue || length >= minimumPeptideLength.Value) && (!maximumPeptideLength.HasValue || length <= maximumPeptideLength.Value))
                                    {
                                        // start residue number: +1 for starting at the next residue after the cleavage, +1 for 0->1 indexing
                                        // end residue number: +1 for 0->1 indexing
                                        Peptide peptide = new Peptide(this, indices[i] + 1 + 1, indices[i + missed_cleavages + 1] + 1, missed_cleavages);
                                        yield return peptide;
                                    }
                                }

                                if(initiatorMethionineBehavior != InitiatorMethionineBehavior.Retain && indices[i] + 1 == 0 && this[0] == 'M')
                                {
                                    int length = indices[i + missed_cleavages + 1] - indices[i] - 1;
                                    if(length > 0 && (!minimumPeptideLength.HasValue || length >= minimumPeptideLength.Value) && (!maximumPeptideLength.HasValue || length <= maximumPeptideLength.Value))
                                    {
                                        // start residue number: +1 for skipping initiator methionine, +1 for starting at the next residue after the cleavage, +1 for 0->1 indexing
                                        // end residue number: +1 for 0->1 indexing
                                        Peptide peptide_without_initiator_methionine = new Peptide(this, indices[i] + 1 + 1 + 1, indices[i + missed_cleavages + 1] + 1, missed_cleavages);
                                        yield return peptide_without_initiator_methionine;
                                    }
                                }
                            }
                        }
                    }
                    else  // protease.CleavageSpecificity == CleavageSpecificity.Semi || protease.CleavageSpecificity == CleavageSpecificity.SemiN || protease.CleavageSpecificity == CleavageSpecificity.SemiC
                    {
                        if(protease.CleavageSpecificity == CleavageSpecificity.Semi || protease.CleavageSpecificity == CleavageSpecificity.SemiN)
                        {
                            for(int missed_cleavages = 0; missed_cleavages <= maximumMissedCleavages; missed_cleavages++)
                            {
                                for(int i = 0; i < indices.Count - missed_cleavages - 1; i++)
                                {
                                    if(initiatorMethionineBehavior != InitiatorMethionineBehavior.Cleave || indices[i] + 1 != 0 || this[0] != 'M')
                                    {
                                        // conditional ensures that we are generating peptides at their lowest missed cleavage state
                                        for(int length = indices[i + missed_cleavages + 1] - indices[i]; length > (indices[i + missed_cleavages + 1] - indices[i]) - (indices[i + missed_cleavages + 1] - indices[(i + missed_cleavages + 1) - 1]); length--)
                                        {
                                            if((indices[i] + 1 + 1) + length - 1 <= Length)
                                            {
                                                if((!minimumPeptideLength.HasValue || length >= minimumPeptideLength.Value) && (!maximumPeptideLength.HasValue || length <= maximumPeptideLength.Value))
                                                {
                                                    // start residue number: +1 for starting at the next residue after the cleavage, +1 for 0->1 indexing
                                                    // end residue number: start residue number + length - 1
                                                    Peptide peptide = new Peptide(this, indices[i] + 1 + 1, (indices[i] + 1 + 1) + length - 1, missed_cleavages);
                                                    yield return peptide;
                                                }
                                            }
                                        }
                                    }

                                    if(initiatorMethionineBehavior != InitiatorMethionineBehavior.Retain && indices[i] + 1 == 0 && this[0] == 'M')
                                    {
                                        // conditional ensures that we are generating peptides at their lowest missed cleavage state
                                        for(int length = indices[i + missed_cleavages + 1] - indices[i]; length > (indices[i + missed_cleavages + 1] - indices[i]) - (indices[i + missed_cleavages + 1] - indices[(i + missed_cleavages + 1) - 1]); length--)
                                        {
                                            if((indices[i] + 1 + 1 + 1) + length - 1 <= Length)
                                            {
                                                if((!minimumPeptideLength.HasValue || length >= minimumPeptideLength.Value) && (!maximumPeptideLength.HasValue || length <= maximumPeptideLength.Value))
                                                {
                                                    // start residue number: +1 for skipping initiator methionine, +1 for starting at the next residue after the cleavage, +1 for 0->1 indexing
                                                    // end residue number: start residue number + length - 1
                                                    Peptide peptide_without_initiator_methionine = new Peptide(this, indices[i] + 1 + 1 + 1, (indices[i] + 1 + 1 + 1) + length - 1, missed_cleavages);
                                                    yield return peptide_without_initiator_methionine;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if(protease.CleavageSpecificity == CleavageSpecificity.Semi || protease.CleavageSpecificity == CleavageSpecificity.SemiC)
                        {
                            for(int missed_cleavages = 0; missed_cleavages <= maximumMissedCleavages; missed_cleavages++)
                            {
                                for(int i = 0; i < indices.Count - missed_cleavages - 1; i++)
                                {
                                    // handling for initiator methionine not required

                                    // - (protease.CleavageSpecificity == CleavageSpecificity.Semi ? 1 : 0) ensures that we don't repeat the same peptides we generated above in the SemiN digestion
                                    // conditional ensures that we are generating peptides at their lowest missed cleavage state
                                    for(int length = indices[i + missed_cleavages + 1] - indices[i] - (protease.CleavageSpecificity == CleavageSpecificity.Semi ? 1 : 0); length > (indices[i + missed_cleavages + 1] - indices[i]) - (indices[i + 1] - indices[i]); length--)
                                    {
                                        if((indices[i + missed_cleavages + 1] + 1) - length + 1 >= 1)
                                        {
                                            if((!minimumPeptideLength.HasValue || length >= minimumPeptideLength.Value) && (!maximumPeptideLength.HasValue || length <= maximumPeptideLength.Value))
                                            {
                                                // start residue number: end residue number - length + 1
                                                // end residue number: +1 for 0->1 indexing
                                                Peptide peptide = new Peptide(this, (indices[i + missed_cleavages + 1] + 1) - length + 1, indices[i + missed_cleavages + 1] + 1, missed_cleavages);
                                                yield return peptide;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else  // protease.CleavageSpecificity == CleavageSpecificity.None
                {
                    if(initiatorMethionineBehavior != InitiatorMethionineBehavior.Cleave || this[0] != 'M')
                    {
                        if((!minimumPeptideLength.HasValue || Length >= minimumPeptideLength.Value) && (!maximumPeptideLength.HasValue || Length <= maximumPeptideLength.Value))
                        {
                            Peptide peptide = new Peptide(this, 1, Length, -1);
                            yield return peptide;
                        }
                    }

                    if(initiatorMethionineBehavior != InitiatorMethionineBehavior.Retain && this[0] == 'M')
                    {
                        if(Length > 1 && (!minimumPeptideLength.HasValue || Length - 1 >= minimumPeptideLength.Value) && (!maximumPeptideLength.HasValue || Length - 1 <= maximumPeptideLength.Value))
                        {
                            Peptide peptide_without_initiator_methionine = new Peptide(this, 2, Length, -1);
                            yield return peptide_without_initiator_methionine;
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

        private const bool ONLY_COUNT_DUPLICATE_PEPTIDES_ONCE_IN_SEQUENCE_COVERAGE = false;

        public double CalculateSequenceCoverage()
        {
            HashSet<int> covered_residues = new HashSet<int>();
            foreach(KeyValuePair<string, List<Peptide>> kvp in IdentifiedPeptides)
            {
                for(int i = 0; i < (ONLY_COUNT_DUPLICATE_PEPTIDES_ONCE_IN_SEQUENCE_COVERAGE ? 1 : kvp.Value.Count); i++)
                {
                    for(int r = kvp.Value[i].StartResidueNumber; r <= kvp.Value[i].EndResidueNumber; r++)
                    {
                        covered_residues.Add(r);
                    }
                }
            }
            return (double)covered_residues.Count / Length;
        }

        public int CompareTo(Protein other)
        {
            int comparison = -(Length.CompareTo(other.Length));
            if(comparison == 0)
            {
                comparison = Description.Length.CompareTo(other.Description.Length);
                if(comparison == 0)
                {
                    comparison = Description.CompareTo(other.Description);
                }
            }
            return comparison;
        }
    }
}