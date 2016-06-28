using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Morpheus
{
    public abstract class AminoAcidPolymer
    {
        private static MassType productMassType = MassType.Monoisotopic;

        private string baseSequence;

        public string BaseSequence
        {
            get
            {
                return baseSequence;
            }
            private set
            {
                baseSequence = value;
                Length = value.Length;
            }
        }

        public char this[int index]
        {
            get
            {
                return baseSequence[index];
            }
        }

        public int Length { get; private set; }

        public double MonoisotopicMass
        {
            get
            {
                double monoisotopic_mass = Constants.WATER_MONOISOTOPIC_MASS;

                foreach(char amino_acid in baseSequence)
                {
                    monoisotopic_mass += AminoAcidMasses.GetMonoisotopicMass(amino_acid);
                }
                if(fixedModifications != null)
                {
                    foreach(List<Modification> fixed_modifications in fixedModifications.Values)
                    {
                        foreach(Modification fixed_modification in fixed_modifications)
                        {
                            monoisotopic_mass += fixed_modification.MonoisotopicMassShift;
                        }
                    }
                }
                if(variableModifications != null)
                {
                    foreach(Modification variable_modification in variableModifications.Values)
                    {
                        monoisotopic_mass += variable_modification.MonoisotopicMassShift;
                    }
                }

                return monoisotopic_mass;
            }
        }

        public double AverageMass
        {
            get
            {
                double average_mass = Constants.WATER_AVERAGE_MASS;

                foreach(char amino_acid in baseSequence)
                {
                    average_mass += AminoAcidMasses.GetAverageMass(amino_acid);
                }
                if(fixedModifications != null)
                {
                    foreach(List<Modification> fixed_modifications in fixedModifications.Values)
                    {
                        foreach(Modification fixed_modification in fixed_modifications)
                        {
                            average_mass += fixed_modification.AverageMassShift;
                        }
                    }
                }
                if(variableModifications != null)
                {
                    foreach(Modification variable_modification in variableModifications.Values)
                    {
                        average_mass += variable_modification.AverageMassShift;
                    }
                }

                return average_mass;
            }
        }

        public string BaseLeucineSequence
        {
            get { return baseSequence.Replace('I', 'L'); }
        }

        public string Sequence
        {
            get
            {
                StringBuilder sequence = new StringBuilder();

                // fixed modifications on protein N-terminus
                if(fixedModifications != null)
                {
                    List<Modification> prot_n_term_fixed_mods;
                    if(fixedModifications.TryGetValue(0, out prot_n_term_fixed_mods))
                    {
                        foreach(Modification fixed_modification in prot_n_term_fixed_mods)
                        {
                            sequence.Append('[' + fixed_modification.Description + ']');
                        }
                    }
                }
                // variable modification on protein N-terminus
                if(variableModifications != null)
                {
                    Modification prot_n_term_variable_mod;
                    if(variableModifications.TryGetValue(0, out prot_n_term_variable_mod))
                    {
                        sequence.Append('(' + prot_n_term_variable_mod.Description + ')');
                    }
                }

                // fixed modifications on peptide N-terminus
                if(fixedModifications != null)
                {
                    List<Modification> pep_n_term_fixed_mods;
                    if(fixedModifications.TryGetValue(1, out pep_n_term_fixed_mods))
                    {
                        foreach(Modification fixed_modification in pep_n_term_fixed_mods)
                        {
                            sequence.Append('[' + fixed_modification.Description + ']');
                        }
                    }
                }
                // variable modification on peptide N-terminus
                if(variableModifications != null)
                {
                    Modification pep_n_term_variable_mod;
                    if(variableModifications.TryGetValue(1, out pep_n_term_variable_mod))
                    {
                        sequence.Append('(' + pep_n_term_variable_mod.Description + ')');
                    }
                }

                for(int r = 0; r < Length; r++)
                {
                    sequence.Append(this[r]);
                    // fixed modifications on this residue
                    if(fixedModifications != null)
                    {
                        List<Modification> residue_fixed_mods;
                        if(fixedModifications.TryGetValue(r + 2, out residue_fixed_mods))
                        {
                            foreach(Modification fixed_modification in residue_fixed_mods)
                            {
                                sequence.Append('[' + fixed_modification.Description + ']');
                            }
                        }
                    }
                    // variable modification on this residue
                    if(variableModifications != null)
                    {
                        Modification residue_variable_mod;
                        if(variableModifications.TryGetValue(r + 2, out residue_variable_mod))
                        {
                            sequence.Append('(' + residue_variable_mod.Description + ')');
                        }
                    }
                }

                // fixed modifications on peptide C-terminus
                if(fixedModifications != null)
                {
                    List<Modification> pep_c_term_fixed_mods;
                    if(fixedModifications.TryGetValue(Length + 2, out pep_c_term_fixed_mods))
                    {
                        foreach(Modification fixed_modification in pep_c_term_fixed_mods)
                        {
                            sequence.Append('[' + fixed_modification.Description + ']');
                        }
                    }
                }
                // variable modification on peptide C-terminus
                if(variableModifications != null)
                {
                    Modification pep_c_term_variable_mod;
                    if(variableModifications.TryGetValue(Length + 2, out pep_c_term_variable_mod))
                    {
                        sequence.Append('(' + pep_c_term_variable_mod.Description + ')');
                    }
                }

                // fixed modifications on protein C-terminus
                if(fixedModifications != null)
                {
                    List<Modification> prot_c_term_fixed_mods;
                    if(fixedModifications.TryGetValue(Length + 3, out prot_c_term_fixed_mods))
                    {
                        foreach(Modification fixed_modification in prot_c_term_fixed_mods)
                        {
                            sequence.Append('[' + fixed_modification.Description + ']');
                        }
                    }
                }
                // variable modification on protein C-terminus
                if(variableModifications != null)
                {
                    Modification prot_c_term_variable_mod;
                    if(variableModifications.TryGetValue(Length + 3, out prot_c_term_variable_mod))
                    {
                        sequence.Append('(' + prot_c_term_variable_mod.Description + ')');
                    }
                }

                return sequence.ToString();
            }
        }

        public string LeucineSequence
        {
            get { return Sequence.Replace('I', 'L'); }
        }

        protected AminoAcidPolymer(string baseSequence)
        {
            BaseSequence = baseSequence;
        }

        protected AminoAcidPolymer(string baseSequence, Dictionary<int, List<Modification>> knownModifications)
            : this(baseSequence)
        {
            KnownModifications = knownModifications;
        }

        public override string ToString()
        {
            return Sequence;
        }

        public static void SetProductMassType(MassType productMassType)
        {
            AminoAcidPolymer.productMassType = productMassType;
        }

        private bool initializeProductArrays = true;

        private Dictionary<int, List<Modification>> fixedModifications;

        public Dictionary<int, List<Modification>> FixedModifications
        {
            get { return fixedModifications; }
            set
            {
                fixedModifications = value;
                initializeProductArrays = true;
            }
        }

        private Dictionary<int, Modification> variableModifications;

        public Dictionary<int, Modification> VariableModifications
        {
            get { return variableModifications; }
            set
            {
                variableModifications = value;
                initializeProductArrays = true;
            }
        }

        public void SetFixedModifications(IEnumerable<Modification> fixedModifications)
        {
            this.fixedModifications = new Dictionary<int, List<Modification>>(Length + 4);

            foreach(Modification fixed_modification in fixedModifications)
            {
                if(fixed_modification.Type == ModificationType.ProteinNTerminus && (this is Protein ||
                    (this is Peptide && (((Peptide)this).StartResidueNumber == 1 || (((Peptide)this).StartResidueNumber == 2 && ((Peptide)this).Parent[0] == 'M'))))
                    && (fixed_modification.AminoAcid == char.MinValue || this[0] == fixed_modification.AminoAcid))
                {
                    List<Modification> prot_n_term_fixed_mods;
                    if(!this.fixedModifications.TryGetValue(0, out prot_n_term_fixed_mods))
                    {
                        prot_n_term_fixed_mods = new List<Modification>();
                        prot_n_term_fixed_mods.Add(fixed_modification);
                        this.fixedModifications.Add(0, prot_n_term_fixed_mods);
                    }
                    else
                    {
                        prot_n_term_fixed_mods.Add(fixed_modification);
                    }
                }

                if(fixed_modification.Type == ModificationType.PeptideNTerminus && (fixed_modification.AminoAcid == char.MinValue || this[0] == fixed_modification.AminoAcid))
                {
                    List<Modification> pep_n_term_fixed_mods;
                    if(!this.fixedModifications.TryGetValue(1, out pep_n_term_fixed_mods))
                    {
                        pep_n_term_fixed_mods = new List<Modification>();
                        pep_n_term_fixed_mods.Add(fixed_modification);
                        this.fixedModifications.Add(1, pep_n_term_fixed_mods);
                    }
                    else
                    {
                        pep_n_term_fixed_mods.Add(fixed_modification);
                    }
                }

                for(int r = 0; r < Length; r++)
                {
                    if(fixed_modification.Type == ModificationType.AminoAcidResidue && this[r] == fixed_modification.AminoAcid)
                    {
                        List<Modification> residue_fixed_mods;
                        if(!this.fixedModifications.TryGetValue(r + 2, out residue_fixed_mods))
                        {
                            residue_fixed_mods = new List<Modification>();
                            residue_fixed_mods.Add(fixed_modification);
                            this.fixedModifications.Add(r + 2, residue_fixed_mods);
                        }
                        else
                        {
                            residue_fixed_mods.Add(fixed_modification);
                        }
                    }
                }

                if(fixed_modification.Type == ModificationType.PeptideCTerminus && (fixed_modification.AminoAcid == char.MinValue || this[Length - 1] == fixed_modification.AminoAcid))
                {
                    List<Modification> pep_c_term_fixed_mods;
                    if(!this.fixedModifications.TryGetValue(Length + 2, out pep_c_term_fixed_mods))
                    {
                        pep_c_term_fixed_mods = new List<Modification>();
                        pep_c_term_fixed_mods.Add(fixed_modification);
                        this.fixedModifications.Add(Length + 2, pep_c_term_fixed_mods);
                    }
                    else
                    {
                        pep_c_term_fixed_mods.Add(fixed_modification);
                    }
                }

                if(fixed_modification.Type == ModificationType.ProteinCTerminus && (this is Protein || (this is Peptide && ((Peptide)this).EndResidueNumber == ((Peptide)this).Parent.Length - 1)
                    && (fixed_modification.AminoAcid == char.MinValue || this[Length - 1] == fixed_modification.AminoAcid)))
                {
                    List<Modification> prot_c_term_fixed_mods;
                    if(!this.fixedModifications.TryGetValue(Length + 3, out prot_c_term_fixed_mods))
                    {
                        prot_c_term_fixed_mods = new List<Modification>();
                        prot_c_term_fixed_mods.Add(fixed_modification);
                        this.fixedModifications.Add(Length + 3, prot_c_term_fixed_mods);
                    }
                    else
                    {
                        prot_c_term_fixed_mods.Add(fixed_modification);
                    }
                }
            }

            if(this.fixedModifications.Count == 0)
            {
                this.fixedModifications = null;
            }

            initializeProductArrays = true;
        }

        private double[] cumulativeNTerminalMass;
        private double[] cumulativeCTerminalMass;

        private void InitializeProductArrays()
        {
            double mass_shift;

            cumulativeNTerminalMass = new double[Length];

            mass_shift = 0.0;
            // fixed modifications on protein N-terminus
            if(fixedModifications != null)
            {
                List<Modification> prot_n_term_fixed_mods;
                if(fixedModifications.TryGetValue(0, out prot_n_term_fixed_mods))
                {
                    foreach(Modification fixed_modification in prot_n_term_fixed_mods)
                    {
                        mass_shift += productMassType == MassType.Average ? (fixed_modification.AverageMassShift - fixed_modification.AverageNeutralLossMass) : (fixed_modification.MonoisotopicMassShift - fixed_modification.AverageNeutralLossMass);
                    }
                }
            }
            // variable modification on the protein N-terminus
            if(variableModifications != null)
            {
                Modification protein_n_term_variable_mod;
                if(variableModifications.TryGetValue(0, out protein_n_term_variable_mod))
                {
                    mass_shift += productMassType == MassType.Average ? (protein_n_term_variable_mod.AverageMassShift - protein_n_term_variable_mod.AverageNeutralLossMass) : (protein_n_term_variable_mod.MonoisotopicMassShift - protein_n_term_variable_mod.AverageNeutralLossMass);
                }
            }
            // fixed modifications on peptide N-terminus
            if(fixedModifications != null)
            {
                List<Modification> pep_n_term_fixed_mods;
                if(fixedModifications.TryGetValue(1, out pep_n_term_fixed_mods))
                {
                    foreach(Modification fixed_modification in pep_n_term_fixed_mods)
                    {
                        mass_shift += productMassType == MassType.Average ? (fixed_modification.AverageMassShift - fixed_modification.AverageNeutralLossMass) : (fixed_modification.MonoisotopicMassShift - fixed_modification.AverageNeutralLossMass);
                    }
                }
            }
            // variable modification on peptide N-terminus
            if(variableModifications != null)
            {
                Modification pep_n_term_variable_mod;
                if(variableModifications.TryGetValue(1, out pep_n_term_variable_mod))
                {
                    mass_shift += productMassType == MassType.Average ? (pep_n_term_variable_mod.AverageMassShift - pep_n_term_variable_mod.AverageNeutralLossMass) : (pep_n_term_variable_mod.MonoisotopicMassShift - pep_n_term_variable_mod.AverageNeutralLossMass);
                }
            }
            cumulativeNTerminalMass[0] = mass_shift;

            for(int r = 1; r < Length; r++)
            {
                mass_shift = 0.0;
                // fixed modifications on this residue
                if(fixedModifications != null)
                {
                    List<Modification> residue_fixed_mods;
                    if(fixedModifications.TryGetValue(r + 1, out residue_fixed_mods))
                    {
                        foreach(Modification fixed_modification in residue_fixed_mods)
                        {
                            mass_shift += productMassType == MassType.Average ? (fixed_modification.AverageMassShift - fixed_modification.AverageNeutralLossMass) : (fixed_modification.MonoisotopicMassShift - fixed_modification.AverageNeutralLossMass);
                        }
                    }
                }
                // variable modification on this residue
                if(variableModifications != null)
                {
                    Modification residue_variable_mod;
                    if(variableModifications.TryGetValue(r + 1, out residue_variable_mod))
                    {
                        mass_shift += productMassType == MassType.Average ? (residue_variable_mod.AverageMassShift - residue_variable_mod.AverageNeutralLossMass) : (residue_variable_mod.MonoisotopicMassShift - residue_variable_mod.MonoisotopicNeutralLossMass);
                    }
                }
                cumulativeNTerminalMass[r] = cumulativeNTerminalMass[r - 1] + (productMassType == MassType.Average ? AminoAcidMasses.GetAverageMass(this[r - 1]) : AminoAcidMasses.GetMonoisotopicMass(this[r - 1])) + mass_shift;
            }

            cumulativeCTerminalMass = new double[Length];

            mass_shift = 0.0;
            // fixed modifications on protein C-terminus
            if(fixedModifications != null)
            {
                List<Modification> prot_c_term_fixed_mods;
                if(fixedModifications.TryGetValue(Length + 3, out prot_c_term_fixed_mods))
                {
                    foreach(Modification fixed_modification in prot_c_term_fixed_mods)
                    {
                        mass_shift += productMassType == MassType.Average ? (fixed_modification.AverageMassShift - fixed_modification.AverageNeutralLossMass) : (fixed_modification.MonoisotopicMassShift - fixed_modification.AverageNeutralLossMass);
                    }
                }
            }
            // variable modification on protein C-terminus
            if(variableModifications != null)
            {
                Modification prot_c_term_variable_mod;
                if(variableModifications.TryGetValue(Length + 3, out prot_c_term_variable_mod))
                {
                    mass_shift += productMassType == MassType.Average ? (prot_c_term_variable_mod.AverageMassShift - prot_c_term_variable_mod.AverageNeutralLossMass) : (prot_c_term_variable_mod.MonoisotopicMassShift - prot_c_term_variable_mod.MonoisotopicNeutralLossMass);
                }
            }
            // fixed modifications on peptide C-terminus
            if(fixedModifications != null)
            {
                List<Modification> pep_c_term_fixed_mods;
                if(fixedModifications.TryGetValue(Length + 2, out pep_c_term_fixed_mods))
                {
                    foreach(Modification fixed_modification in pep_c_term_fixed_mods)
                    {
                        mass_shift += productMassType == MassType.Average ? (fixed_modification.AverageMassShift - fixed_modification.AverageNeutralLossMass) : (fixed_modification.MonoisotopicMassShift - fixed_modification.MonoisotopicNeutralLossMass);
                    }
                }
            }
            // variable modification on peptide C-terminus
            if(variableModifications != null)
            {
                Modification pep_c_term_variable_mod;
                if(variableModifications.TryGetValue(Length + 2, out pep_c_term_variable_mod))
                {
                    mass_shift += productMassType == MassType.Average ? (pep_c_term_variable_mod.AverageMassShift - pep_c_term_variable_mod.AverageNeutralLossMass) : (pep_c_term_variable_mod.MonoisotopicMassShift - pep_c_term_variable_mod.MonoisotopicNeutralLossMass);
                }
            }
            cumulativeCTerminalMass[0] = mass_shift;

            for(int r = 1; r < Length; r++)
            {
                mass_shift = 0.0;
                // fixed modifications on this residue
                if(fixedModifications != null)
                {
                    List<Modification> residue_fixed_mods;
                    if(fixedModifications.TryGetValue(Length - r + 2, out residue_fixed_mods))
                    {
                        foreach(Modification fixed_modification in residue_fixed_mods)
                        {
                            mass_shift += productMassType == MassType.Average ? (fixed_modification.AverageMassShift - fixed_modification.AverageNeutralLossMass) : (fixed_modification.MonoisotopicMassShift - fixed_modification.MonoisotopicNeutralLossMass);
                        }
                    }
                }
                // variable modification on this residue
                if(variableModifications != null)
                {
                    Modification residue_variable_mod;
                    if(variableModifications.TryGetValue(Length - r + 2, out residue_variable_mod))
                    {
                        mass_shift += productMassType == MassType.Average ? (residue_variable_mod.AverageMassShift - residue_variable_mod.AverageNeutralLossMass) : (residue_variable_mod.MonoisotopicMassShift - residue_variable_mod.MonoisotopicNeutralLossMass);
                    }
                }

                cumulativeCTerminalMass[r] = cumulativeCTerminalMass[r - 1] + (productMassType == MassType.Average ? AminoAcidMasses.GetAverageMass(this[Length - r]) : AminoAcidMasses.GetMonoisotopicMass(this[Length - r])) + mass_shift;
            }

            initializeProductArrays = false;
        }

        private static readonly ProductCaps PRODUCT_CAPS = ProductCaps.Instance;

        public double CalculateProductMass(ProductType productType, int productNumber)
        {
            if(initializeProductArrays)
            {
                InitializeProductArrays();
            }

            switch(productType)
            {
                case ProductType.adot:
                case ProductType.b:
                case ProductType.c:
                    return cumulativeNTerminalMass[productNumber] + PRODUCT_CAPS[productType, productMassType];
                case ProductType.x:
                case ProductType.y:
                case ProductType.zdot:
                    return cumulativeCTerminalMass[productNumber] + PRODUCT_CAPS[productType, productMassType];
                default:
                    return double.NaN;
            }
        }

        public List<double> CalculateProductMasses(IEnumerable<ProductType> productTypes)
        {
            List<double> product_masses = new List<double>(2 * (Length - 1));

            for(int r = 1; r < Length; r++)
            {
                foreach(ProductType product_type in productTypes)
                {
                    if(!(product_type == ProductType.c && r < Length && this[r] == 'P') &&
                       !(product_type == ProductType.zdot && Length - r < Length && this[Length - r] == 'P'))
                    {
                        double product_mass = CalculateProductMass(product_type, r);
                        product_masses.Add(product_mass);
                    }
                }
            }

            product_masses.Sort();
            return product_masses;
        }

        public Dictionary<int, List<Modification>> KnownModifications { get; protected set; }

        protected IEnumerable<Dictionary<int, Modification>> GetVariableModificationPatterns(Dictionary<int, List<Modification>> possibleVariableModifications)
        {
            if(possibleVariableModifications.Count == 0 && (KnownModifications == null || KnownModifications.Count == 0))
            {
                yield return null;
            }
            else
            {
                Dictionary<int, List<Modification>> possible_variable_modifications = new Dictionary<int, List<Modification>>(possibleVariableModifications);
                if(KnownModifications != null)
                {
                    foreach(KeyValuePair<int, List<Modification>> kvp in KnownModifications)
                    {
                        foreach(Modification modification in kvp.Value)
                        {
                            if(modification.AminoAcid != BaseSequence[kvp.Key - 2])
                            {
                                throw new Exception("Known modification amino acid mismatch in " + ((this is Peptide) ? ((Peptide)this).Parent.Description : ((Protein)this).Description));
                            }
                        }
                        List<Modification> modifications;
                        if(!possible_variable_modifications.TryGetValue(kvp.Key, out modifications))
                        {
                            modifications = kvp.Value;
                            possible_variable_modifications.Add(kvp.Key, modifications);
                        }
                        else
                        {
                            modifications.AddRange(kvp.Value);
                        }
                    }
                }
                int[] base_variable_modification_pattern = new int[Length + 4];
                for(int variable_modifications = 0; variable_modifications <= possible_variable_modifications.Count; variable_modifications++)
                {
                    foreach(int[] variable_modification_pattern in GetVariableModificationPatterns(new List<KeyValuePair<int, List<Modification>>>(possible_variable_modifications), possible_variable_modifications.Count - variable_modifications, base_variable_modification_pattern, 0))
                    {
                        yield return GetVariableModificationPattern(variable_modification_pattern, possible_variable_modifications);
                    }
                }
            }
        }

        private static IEnumerable<int[]> GetVariableModificationPatterns(List<KeyValuePair<int, List<Modification>>> possibleVariableModifications, int unmodifiedResiduesDesired, int[] variableModificationPattern, int index)
        {
            if(index < possibleVariableModifications.Count - 1)
            {
                if(unmodifiedResiduesDesired > 0)
                {
                    variableModificationPattern[possibleVariableModifications[index].Key] = 0;
                    foreach(int[] new_variable_modification_pattern in GetVariableModificationPatterns(possibleVariableModifications, unmodifiedResiduesDesired - 1, variableModificationPattern, index + 1))
                    {
                        yield return new_variable_modification_pattern;
                    }
                }
                if(unmodifiedResiduesDesired < possibleVariableModifications.Count - index)
                {
                    for(int i = 1; i <= possibleVariableModifications[index].Value.Count; i++)
                    {
                        variableModificationPattern[possibleVariableModifications[index].Key] = i;
                        foreach(int[] new_variable_modification_pattern in GetVariableModificationPatterns(possibleVariableModifications, unmodifiedResiduesDesired, variableModificationPattern, index + 1))
                        {
                            yield return new_variable_modification_pattern;
                        }
                    }
                }
            }
            else
            {
                if(unmodifiedResiduesDesired > 0)
                {
                    variableModificationPattern[possibleVariableModifications[index].Key] = 0;
                    yield return variableModificationPattern;
                }
                else
                {
                    for(int i = 1; i <= possibleVariableModifications[index].Value.Count; i++)
                    {
                        variableModificationPattern[possibleVariableModifications[index].Key] = i;
                        yield return variableModificationPattern;
                    }
                }
            }
        }

        private static Dictionary<int, Modification> GetVariableModificationPattern(int[] variableModificationArray, IEnumerable<KeyValuePair<int, List<Modification>>> possibleVariableModifications)
        {
            Dictionary<int, Modification> modification_pattern = new Dictionary<int, Modification>();

            foreach(KeyValuePair<int, List<Modification>> kvp in possibleVariableModifications)
            {
                if(variableModificationArray[kvp.Key] > 0)
                {
                    modification_pattern.Add(kvp.Key, kvp.Value[variableModificationArray[kvp.Key] - 1]);
                }
            }

            return modification_pattern;
        }
    }
}
