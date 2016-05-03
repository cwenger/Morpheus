namespace Morpheus
{
    public class Modification
    {
        public string Description { get; private set; }

        public ModificationType Type { get; private set; }

        public char AminoAcid { get; private set; }

        public double MonoisotopicMassShift { get; private set; }

        public double AverageMassShift { get; private set; }

        public double MonoisotopicNeutralLossMass { get; private set; }

        public double AverageNeutralLossMass { get; private set; }

        public bool DefaultFixed { get; private set; }

        public bool DefaultVariable { get; private set; }

        public string Database { get; private set; }

        public int DatabaseAccessionNumber { get; private set; }

        public string DatabaseName { get; private set; }

        public bool Known { get; private set; }

        public Modification(string description, ModificationType type, char aminoAcid, double monoisotopicMassShift,
            double averageMassShift, double monoisotopicNeutralLossMass, double averageNeutralLossMass, bool defaultFixed, bool defaultVariable, string database, int databaseAccessionNumber, string databaseName, bool known)
        {
            Description = description;
            Type = type;
            AminoAcid = aminoAcid;
            MonoisotopicMassShift = monoisotopicMassShift;
            AverageMassShift = averageMassShift;
            MonoisotopicNeutralLossMass = monoisotopicNeutralLossMass;
            AverageNeutralLossMass = averageNeutralLossMass;
            DefaultFixed = defaultFixed;
            DefaultVariable = defaultVariable;
            Database = database;
            DatabaseAccessionNumber = databaseAccessionNumber;
            DatabaseName = databaseName;
            Known = known;
        }

        public override string ToString()
        {
            return Description;
        }
    }
}