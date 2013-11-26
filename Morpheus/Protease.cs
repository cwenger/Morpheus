using System;
using System.Collections.Generic;
using System.Text;

namespace Morpheus
{
    public class Protease
    {
        public string Name { get; private set; }

        public Terminus CleavageTerminus { get; private set; }

        public string Sense
        {
            get { return CleavageTerminus.ToString(); }
        }

        public IEnumerable<string> SequencesInducingCleavage { get; private set; }

        public string Cut
        {
            get
            {
                StringBuilder cut = new StringBuilder();

                foreach(string c in SequencesInducingCleavage)
                {
                    cut.Append(c);
                }

                return cut.ToString();
            }
        }

        public IEnumerable<string> SequencesPreventingCleavage { get; private set; }

        public string NoCut
        {
            get
            {
                StringBuilder no_cut = new StringBuilder();

                foreach(string c in SequencesPreventingCleavage)
                {
                    no_cut.Append(c);
                }

                return no_cut.ToString();
            }
        }

        public CleavageSpecificity CleavageSpecificity { get; private set; }

        public Protease(string name, IEnumerable<string> sequencesInducingCleavage, IEnumerable<string> sequencesPreventingCleavage, Terminus cleavageTerminus, CleavageSpecificity cleavageSpecificity)
        {
            Name = name;
            SequencesInducingCleavage = sequencesInducingCleavage;
            SequencesPreventingCleavage = sequencesPreventingCleavage;
            CleavageTerminus = cleavageTerminus;
            CleavageSpecificity = cleavageSpecificity;
        }

        public override string ToString()
        {
            return Name;
        }

        public List<int> GetDigestionSiteIndices(AminoAcidPolymer aminoAcidPolymer)
        {
            List<int> indices = new List<int>();

            for(int i = 0; i < aminoAcidPolymer.Length - 1; i++)
            {
                foreach(string c in SequencesInducingCleavage)
                {
                    if((CleavageTerminus != Terminus.N && i - c.Length + 1 >= 0 && aminoAcidPolymer.BaseSequence.Substring(i - c.Length + 1, c.Length).Equals(c, StringComparison.InvariantCultureIgnoreCase))
                        || (CleavageTerminus == Terminus.N && i + 1 + c.Length <= aminoAcidPolymer.Length && aminoAcidPolymer.BaseSequence.Substring(i + 1, c.Length).Equals(c, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        bool cleave = true;
                        foreach(string nc in SequencesPreventingCleavage)
                        {
                            if((CleavageTerminus != Terminus.N && i + 1 + nc.Length <= aminoAcidPolymer.Length && aminoAcidPolymer.BaseSequence.Substring(i + 1, nc.Length).Equals(nc, StringComparison.InvariantCultureIgnoreCase)) 
                                || (CleavageTerminus == Terminus.N && i - nc.Length + 1 >= 0 && aminoAcidPolymer.BaseSequence.Substring(i - nc.Length + 1, nc.Length).Equals(nc, StringComparison.InvariantCultureIgnoreCase)))
                            {
                                cleave = false;
                                break;
                            }
                        }
                        if(cleave)
                        {
                            indices.Add(i);
                        }
                    }
                }
            }

            return indices;
        }
    }
}