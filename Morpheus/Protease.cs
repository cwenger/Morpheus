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

        public IEnumerable<char> AminoAcidsInducingCleavage { get; private set; }

        public string Cut
        {
            get
            {
                StringBuilder cut = new StringBuilder();

                foreach(char c in AminoAcidsInducingCleavage)
                {
                    cut.Append(c);
                }

                return cut.ToString();
            }
        }

        public IEnumerable<char> AminoAcidsPreventingCleavage { get; private set; }

        public string NoCut
        {
            get
            {
                StringBuilder no_cut = new StringBuilder();

                foreach(char c in AminoAcidsPreventingCleavage)
                {
                    no_cut.Append(c);
                }

                return no_cut.ToString();
            }
        }

        public CleavageSpecificity CleavageSpecificity { get; private set; }

        public Protease(string name, IEnumerable<char> aminoAcidsInducingCleavage, IEnumerable<char> aminoAcidsPreventingCleavage, Terminus cleavageTerminus, CleavageSpecificity cleavageSpecificity)
        {
            Name = name;
            AminoAcidsInducingCleavage = aminoAcidsInducingCleavage;
            AminoAcidsPreventingCleavage = aminoAcidsPreventingCleavage;
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
                foreach(char c in AminoAcidsInducingCleavage)
                {
                    if((CleavageTerminus != Terminus.N && aminoAcidPolymer[i] == c)
                        || (CleavageTerminus == Terminus.N && i + 1 < aminoAcidPolymer.Length && aminoAcidPolymer[i + 1] == c))
                    {
                        bool cleave = true;
                        foreach(char nc in AminoAcidsPreventingCleavage)
                        {
                            if((CleavageTerminus != Terminus.N && i + 1 < aminoAcidPolymer.Length && aminoAcidPolymer[i + 1] == nc) 
                                || (CleavageTerminus == Terminus.N && i - 1 >= 0 && aminoAcidPolymer[i - 1] == nc))
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