using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Morpheus_Peptide_Summarizer
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_DragEnter(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filepaths = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach(string filepath in filepaths)
                {
                    if(Path.GetExtension(filepath).Equals(".tsv", StringComparison.InvariantCultureIgnoreCase) && !lstMorpheusPsmTsv.Items.Contains(filepath))
                    {
                        e.Effect = DragDropEffects.Link;
                        break;
                    }
                }
            }
        }

        private void frmMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] filepaths = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach(string filepath in filepaths)
            {
                if(Path.GetExtension(filepath).Equals(".tsv", StringComparison.InvariantCultureIgnoreCase) && !lstMorpheusPsmTsv.Items.Contains(filepath))
                {
                    lstMorpheusPsmTsv.Items.Add(filepath);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if(ofdMorpheusPsmTsv.ShowDialog() == DialogResult.OK)
            {
                foreach(string data_filepath in ofdMorpheusPsmTsv.FileNames)
                {
                    if(!lstMorpheusPsmTsv.Items.Contains(data_filepath))
                    {
                        lstMorpheusPsmTsv.Items.Add(data_filepath);
                    }
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            while(lstMorpheusPsmTsv.SelectedIndices.Count > 0)
            {
                lstMorpheusPsmTsv.Items.Remove(lstMorpheusPsmTsv.SelectedItem);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lstMorpheusPsmTsv.Items.Clear();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if(fbdOutput.ShowDialog() == DialogResult.OK)
            {
                txtOutputFolder.Text = fbdOutput.SelectedPath;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            List<string> psm_tsv_filepaths = new List<string>(lstMorpheusPsmTsv.Items.Count);
            foreach(string protein_group_tsv_filepath in lstMorpheusPsmTsv.Items)
            {
                psm_tsv_filepaths.Add(protein_group_tsv_filepath);
            }
            string output_folder = txtOutputFolder.Text;
            SummarizePeptides(psm_tsv_filepaths, output_folder);
            MessageBox.Show("Finished!");
        }

        private const double MAXIMUM_FDR_PERCENT = 1.0;

        private void SummarizePeptides(IEnumerable<string> psmTsvFilepaths, string outputFolder)
        {
            Dictionary<string, Dictionary<string, int>> peptides = new Dictionary<string, Dictionary<string, int>>();
            HashSet<string> distinct_peptides = new HashSet<string>();
            foreach(string filepath in psmTsvFilepaths)
            {
                string dataset = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(filepath));

                peptides.Add(dataset, new Dictionary<string, int>());

                using(StreamReader input = new StreamReader(filepath))
                {
                    string header = input.ReadLine();
                    string[] header_fields = header.Split('\t');
                    int base_peptide_sequence_index = Array.FindIndex(header_fields, s => s == "Base Peptide Sequence");
                    int decoy_index = Array.FindIndex(header_fields, s => s == "Decoy?");
                    int q_value_index = Array.FindIndex(header_fields, s => s == "Q-Value (%)");

                    while(input.Peek() != -1)
                    {
                        string line = input.ReadLine();
                        string[] fields = line.Split('\t');

                        if(double.Parse(fields[q_value_index]) <= MAXIMUM_FDR_PERCENT && !bool.Parse(fields[decoy_index]))
                        {
                            distinct_peptides.Add(fields[base_peptide_sequence_index]);
                            if(!peptides[dataset].ContainsKey(fields[base_peptide_sequence_index]))
                            {
                                peptides[dataset].Add(fields[base_peptide_sequence_index], 1);
                            }
                            else
                            {
                                peptides[dataset][fields[base_peptide_sequence_index]]++;
                            }
                        }
                    }
                }
            }

            using(StreamWriter output = new StreamWriter(Path.Combine(outputFolder, "peptides.tsv")))
            {
                output.Write('\t');
                foreach(string dataset in peptides.Keys)
                {
                    output.Write(dataset + '\t');
                }
                output.WriteLine();

                foreach(string peptide in distinct_peptides)
                {
                    output.Write(peptide + '\t');
                    foreach(string filepath in peptides.Keys)
                    {
                        int psms = 0;
                        peptides[filepath].TryGetValue(peptide, out psms);
                        output.Write(psms.ToString() + '\t');
                    }
                    output.WriteLine();
                }
            }
        }
    }
}
