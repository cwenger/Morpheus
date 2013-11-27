using System;
using System.Collections.Generic;
using System.IO;

namespace Morpheus
{
    public class ProteaseDictionary : Dictionary<string, Protease>
    {
        private static readonly ProteaseDictionary instance = new ProteaseDictionary();

        private ProteaseDictionary()
            : base()
        {
            using(StreamReader proteases = new StreamReader(Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), "proteases.tsv")))
            {
                string header = proteases.ReadLine();

                while(proteases.Peek() != -1)
                {
                    string line = proteases.ReadLine();
                    string[] fields = line.Split('\t');

                    string name = fields[0];
                    string[] sequences_inducing_cleavage = fields[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string[] sequences_preventing_cleavage = fields[2].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    Terminus cleavage_terminus = (Terminus)Enum.Parse(typeof(Terminus), fields[3], true);
                    CleavageSpecificity cleavage_specificity = (CleavageSpecificity)Enum.Parse(typeof(CleavageSpecificity), fields[4], true);
                    Protease protease = new Protease(name, sequences_inducing_cleavage, sequences_preventing_cleavage, cleavage_terminus, cleavage_specificity);
                    Add(protease);
                }
            }
        }

        public static ProteaseDictionary Instance
        {
            get { return instance; }
        }

        public void Add(Protease protease)
        {
            base.Add(protease.Name, protease);
        }
    }
}