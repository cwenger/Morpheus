using System;
using System.Collections.Generic;
using System.IO;

namespace Morpheus
{
    public static class ProteinFastaReader
    {
        public static bool HasDecoyProteins(string proteinFastaDatabaseFilepath)
        {
            using(StreamReader fasta = new StreamReader(proteinFastaDatabaseFilepath))
            {
                while(!fasta.EndOfStream)
                {
                    string line = fasta.ReadLine();
                    if(line.StartsWith(">") && line.Contains(Protein.DECOY_IDENTIFIER))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static int CountProteins(FileStream proteinFastaDatabase, bool onTheFlyDecoys, out int targetProteins, out int decoyProteins, out int onTheFlyDecoyProteins)
        {
            targetProteins = 0;
            decoyProteins = 0;
            onTheFlyDecoyProteins = 0;

            StreamReader fasta = new StreamReader(proteinFastaDatabase);

            string description = null;

            while(true)
            {
                string line = fasta.ReadLine();

                if(line.StartsWith(">"))
                {
                    description = line.Substring(1);
                }

                if(fasta.Peek() == '>' || fasta.Peek() == -1)
                {
                    Protein protein = new Protein(string.Empty, description);
                    if(protein.Decoy)
                    {
                        decoyProteins++;
                        if(onTheFlyDecoys)
                        {
                            throw new ArgumentException(proteinFastaDatabase.Name + " contains decoy proteins; database should not contain decoy proteins when \"create target–decoy database on the fly\" option is enabled");
                        }
                    }
                    else
                    {
                        targetProteins++;
                        if(onTheFlyDecoys)
                        {
                            onTheFlyDecoyProteins++;
                        }
                    }

                    description = null;

                    if(fasta.Peek() == -1)
                    {
                        break;
                    }
                }
            }

            proteinFastaDatabase.Seek(0, SeekOrigin.Begin);

            return targetProteins + decoyProteins + onTheFlyDecoyProteins;
        }

        public static IEnumerable<Protein> ReadProteins(FileStream proteinFastaDatabase, bool onTheFlyDecoys)
        {
            StreamReader fasta = new StreamReader(proteinFastaDatabase);

            string description = null;
            string sequence = null;

            while(true)
            {
                string line = fasta.ReadLine();

                if(line.StartsWith(">"))
                {
                    description = line.Substring(1);
                }
                else
                {
                    sequence += line.Trim();
                }

                if(fasta.Peek() == '>' || fasta.Peek() == -1)
                {
                    Protein protein = new Protein(sequence, description);

                    yield return protein;

                    if(onTheFlyDecoys)
                    {
                        if(protein.Decoy)
                        {
                            throw new ArgumentException(proteinFastaDatabase.Name + " contains decoy proteins; database should not contain decoy proteins when \"create target–decoy database on the fly\" option is enabled");
                        }
                        char[] sequence_array = sequence.ToCharArray();
                        if(sequence.StartsWith("M"))
                        {
                            Array.Reverse(sequence_array, 1, sequence.Length - 1);
                        }
                        else
                        {
                            Array.Reverse(sequence_array);
                        }
                        string reversed_sequence = new string(sequence_array);
                        Protein decoy_protein = new Protein(reversed_sequence, description[2] == '|' ? description.Insert(3, Protein.DECOY_IDENTIFIER) : Protein.DECOY_IDENTIFIER + description);
                        yield return decoy_protein;
                    }

                    description = null;
                    sequence = null;

                    if(fasta.Peek() == -1)
                    {
                        break;
                    }
                }
            }

            proteinFastaDatabase.Seek(0, SeekOrigin.Begin);
        }
    }
}