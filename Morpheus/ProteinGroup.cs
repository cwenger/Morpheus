using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Morpheus
{
    // sort proteins by descending length?
    public class ProteinGroup : List<Protein>, ITargetDecoy
    {
        public ProteinGroup()
            : base()
        {
            PeptideSpectrumMatches = new List<PeptideSpectrumMatch>();
        }

        public bool Target { get { return !Decoy; } }

        public bool Decoy
        {
            get
            {
                foreach(Protein protein in this)
                {
                    if(protein.Decoy)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public List<PeptideSpectrumMatch> PeptideSpectrumMatches { get; private set; }

        public List<PeptideSpectrumMatch> UniquePeptides
        {
            get
            {
                Dictionary<string, PeptideSpectrumMatch> unique_peptides = new Dictionary<string, PeptideSpectrumMatch>();

                foreach(PeptideSpectrumMatch psm in PeptideSpectrumMatches)
                {
                    if(!unique_peptides.ContainsKey(psm.Peptide.BaseLeucineSequence))
                    {
                        unique_peptides.Add(psm.Peptide.BaseLeucineSequence, psm);
                    }
                    else if(PeptideSpectrumMatch.DescendingMorpheusScoreComparison(psm, unique_peptides[psm.Peptide.BaseLeucineSequence]) < 0)
                    {
                        unique_peptides[psm.Peptide.BaseLeucineSequence] = psm;
                    }
                }

                return new List<PeptideSpectrumMatch>(unique_peptides.Values);
            }
        }

        public HashSet<string> BaseLeucinePeptideSequences
        {
            get
            {
                HashSet<string> base_leucine_peptide_sequences = new HashSet<string>();

                foreach(PeptideSpectrumMatch psm in PeptideSpectrumMatches)
                {
                    base_leucine_peptide_sequences.Add(psm.Peptide.BaseLeucineSequence);
                }

                return base_leucine_peptide_sequences;
            }
        }

        public double SummedMorpheusScore
        {
            get
            {
                double summed_morpheus_score = 0.0;

                foreach(PeptideSpectrumMatch psm in UniquePeptides)  // need option to score based on all PSMs rather than unique peptide PSMs?
                {
                    summed_morpheus_score += psm.MorpheusScore;
                }

                return summed_morpheus_score;
            }
        }

        double ITargetDecoy.Score { get { return SummedMorpheusScore; } }

        public static int DescendingSummedMorpheusScoreProteinGroupComparison(ProteinGroup left, ProteinGroup right)
        {
            int comparison = -(left.SummedMorpheusScore.CompareTo(right.SummedMorpheusScore));
            if(comparison != 0)
            {
                return comparison;
            }
            else
            {
                return left.Target.CompareTo(right.Target);
            }
        }

        public static readonly string Header = "Protein Description\tProtein Sequence\tProtein Length\tNumber of Proteins in Group\tNumber of Peptide-Spectrum Matches\tNumber of Unique Peptides\tProtein Sequence Coverage (%)\tSummed Morpheus Score";

        public override string ToString()
        {
            StringBuilder description = new StringBuilder();
            StringBuilder sequence = new StringBuilder();
            StringBuilder length = new StringBuilder();
            foreach(Protein protein in this)
            {
                description.Append(protein.Description + "; ");
                sequence.Append(protein.Sequence + "; ");
                length.Append(protein.Sequence.Length.ToString() + "; ");
            }
            description = description.Remove(description.Length - 2, 2);
            sequence = sequence.Remove(sequence.Length - 2, 2);
            length = length.Remove(length.Length - 2, 2);

            StringBuilder sb = new StringBuilder();

            sb.Append(description.ToString() + '\t');
            sb.Append(sequence.ToString() + '\t');
            sb.Append(length.ToString() + '\t');
            sb.Append(Count.ToString() + '\t');
            sb.Append(PeptideSpectrumMatches.Count.ToString() + '\t');
            sb.Append(UniquePeptides.Count.ToString() + '\t');
            StringBuilder sequence_coverage = new StringBuilder();
            foreach(Protein protein in this)
            {
                sequence_coverage.Append((protein.CalculateSequenceCoverage(UniquePeptides) * 100.0).ToString() + "; ");
            }
            sequence_coverage = sequence_coverage.Remove(sequence_coverage.Length - 2, 2);
            sb.Append(sequence_coverage.ToString() + '\t');
            sb.Append(SummedMorpheusScore.ToString());

            return sb.ToString();
        }

        public static List<ProteinGroup> ApplyProteinParsimony(IEnumerable<PeptideSpectrumMatch> peptideSpectrumMatches, double morpheusScoreThreshold, FileStream proteinFastaDatabase, bool onTheFlyDecoys, Protease protease, int maximumMissedCleavages, InitiatorMethionineBehavior initiatorMethionineBehavior, int maximumThreads)
        {
            Dictionary<string, List<Protein>> peptide_proteins = new Dictionary<string, List<Protein>>();
            foreach(PeptideSpectrumMatch psm in peptideSpectrumMatches)
            {
                if(psm.MorpheusScore >= morpheusScoreThreshold)
                {
                    if(!peptide_proteins.ContainsKey(psm.Peptide.BaseLeucineSequence))
                    {
                        peptide_proteins.Add(psm.Peptide.BaseLeucineSequence, new List<Protein>());
                    }
                }
            }

            ParallelOptions parallel_options = new ParallelOptions();
            parallel_options.MaxDegreeOfParallelism = maximumThreads;
            Parallel.ForEach(ProteinFastaReader.ReadProteins(proteinFastaDatabase, onTheFlyDecoys), parallel_options, protein =>
                {
                    foreach(Peptide peptide in protein.Digest(protease, maximumMissedCleavages, initiatorMethionineBehavior, null, null))
                    {
                        lock(peptide_proteins)
                        {
                            List<Protein> proteins;
                            if(peptide_proteins.TryGetValue(peptide.BaseLeucineSequence, out proteins))
                            {
                                proteins.Add(protein);
                            }
                        }
                    }
                }
            );

            Dictionary<string, ProteinGroup> proteins_by_description = new Dictionary<string, ProteinGroup>();
            foreach(PeptideSpectrumMatch psm in peptideSpectrumMatches)
            {
                if(psm.MorpheusScore >= morpheusScoreThreshold)
                {
                    foreach(Protein protein in peptide_proteins[psm.Peptide.BaseLeucineSequence])
                    {
                        ProteinGroup protein_group;
                        if(!proteins_by_description.TryGetValue(protein.Description, out protein_group))
                        {
                            protein_group = new ProteinGroup();
                            protein_group.Add(protein);
                            protein_group.PeptideSpectrumMatches.Add(psm);
                            proteins_by_description.Add(protein.Description, protein_group);
                        }
                        else
                        {
                            protein_group.PeptideSpectrumMatches.Add(psm);
                        }
                    }
                }
            }

            List<ProteinGroup> protein_groups = new List<ProteinGroup>(proteins_by_description.Values);
            protein_groups.Sort(ProteinGroup.DescendingSummedMorpheusScoreProteinGroupComparison);

            // merge indistinguishable proteins
            for(int i = 0; i < protein_groups.Count - 1; i++)
            {
                ProteinGroup protein_group = protein_groups[i];

                int j = i + 1;
                while(j < protein_groups.Count)
                {
                    ProteinGroup lower_protein_group = protein_groups[j];

                    if(lower_protein_group.SummedMorpheusScore < protein_group.SummedMorpheusScore)
                    {
                        break;
                    }

                    if(lower_protein_group.BaseLeucinePeptideSequences.SetEquals(protein_group.BaseLeucinePeptideSequences))
                    {
                        protein_group.AddRange(lower_protein_group);  // should only ever be one protein in the group to add
                        protein_groups.RemoveAt(j);
                    }
                    else
                    {
                        j++;
                    }
                }
            }

            // remove subset and subsumable protein groups
            int k = protein_groups.Count - 1;
            while(k >= 1)
            {
                ProteinGroup protein_group = protein_groups[k];
                HashSet<string> protein_group_peptides = new HashSet<string>(protein_group.BaseLeucinePeptideSequences);

                for(int l = 0; l < k; l++)
                {
                    ProteinGroup higher_protein_group = protein_groups[l];

                    protein_group_peptides.ExceptWith(higher_protein_group.BaseLeucinePeptideSequences);
                    if(protein_group_peptides.Count == 0)
                    {
                        break;
                    }
                }

                if(protein_group_peptides.Count == 0)
                {
                    protein_groups.RemoveAt(k);
                }
                k--;
            }

            return protein_groups;
        }
    }
}
