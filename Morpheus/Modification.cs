namespace Morpheus
{
    public class Modification
    {
        public string Description { get; private set; }

        public ModificationType Type { get; private set; }

        public char AminoAcid { get; private set; }

        public double MonoisotopicMassShift { get; private set; }

        public double AverageMassShift { get; private set; }

        public bool DefaultFixedModification { get; private set; }

        public bool DefaultVariableModification { get; private set; }

        public Modification(string description, ModificationType type, char aminoAcid, double monoisotopicMassShift,
            double averageMassShift, bool defaultFixedModification, bool defaultVariableModification)
        {
            Description = description;
            Type = type;
            AminoAcid = aminoAcid;
            MonoisotopicMassShift = monoisotopicMassShift;
            AverageMassShift = averageMassShift;
            DefaultFixedModification = defaultFixedModification;
            DefaultVariableModification = defaultVariableModification;
        }

        public override string ToString()
        {
            return Description;
        }
    }
}