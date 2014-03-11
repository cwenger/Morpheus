namespace Morpheus
{
    public class Modification
    {
        public string Description { get; private set; }

        public ModificationType Type { get; private set; }

        public char AminoAcid { get; private set; }

        public double MonoisotopicMassShift { get; private set; }

        public double AverageMassShift { get; private set; }

        public bool DefaultFixed { get; private set; }

        public bool DefaultVariable { get; private set; }

        public bool Known { get; private set; }

        public Modification(string description, ModificationType type, char aminoAcid, double monoisotopicMassShift,
            double averageMassShift, bool defaultFixed, bool defaultVariable, bool known)
        {
            Description = description;
            Type = type;
            AminoAcid = aminoAcid;
            MonoisotopicMassShift = monoisotopicMassShift;
            AverageMassShift = averageMassShift;
            DefaultFixed = defaultFixed;
            DefaultVariable = defaultVariable;
            Known = known;
        }

        public override string ToString()
        {
            return Description;
        }
    }
}