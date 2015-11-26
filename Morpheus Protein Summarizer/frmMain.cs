using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Morpheus_Protein_Summarizer
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
                    if(Path.GetExtension(filepath).Equals(".tsv", StringComparison.InvariantCultureIgnoreCase) && !lstMorpheusProteinGroupsTsv.Items.Contains(filepath))
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
                if(Path.GetExtension(filepath).Equals(".tsv", StringComparison.InvariantCultureIgnoreCase) && !lstMorpheusProteinGroupsTsv.Items.Contains(filepath))
                {
                    lstMorpheusProteinGroupsTsv.Items.Add(filepath);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if(ofdMorpheusProteinGroupsTsv.ShowDialog() == DialogResult.OK)
            {
                foreach(string data_filepath in ofdMorpheusProteinGroupsTsv.FileNames)
                {
                    if(!lstMorpheusProteinGroupsTsv.Items.Contains(data_filepath))
                    {
                        lstMorpheusProteinGroupsTsv.Items.Add(data_filepath);
                    }
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            while(lstMorpheusProteinGroupsTsv.SelectedIndices.Count > 0)
            {
                lstMorpheusProteinGroupsTsv.Items.Remove(lstMorpheusProteinGroupsTsv.SelectedItem);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lstMorpheusProteinGroupsTsv.Items.Clear();
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
            List<string> protein_group_tsv_filepaths = new List<string>(lstMorpheusProteinGroupsTsv.Items.Count);
            foreach(string protein_group_tsv_filepath in lstMorpheusProteinGroupsTsv.Items)
            {
                protein_group_tsv_filepaths.Add(protein_group_tsv_filepath);
            }
            string output_folder = txtOutputFolder.Text;
            SummarizeProteins(protein_group_tsv_filepaths, output_folder);
            MessageBox.Show("Finished!");
        }

        private const double MAXIMUM_FDR_PERCENT = 1.0;

        private void SummarizeProteins(IEnumerable<string> proteinGroupTsvFilepaths, string outputFolder)
        {
            Dictionary<string, Dictionary<string, KeyValuePair<int, int>>> proteins = new Dictionary<string, Dictionary<string, KeyValuePair<int, int>>>();
            HashSet<string> distinct_proteins = new HashSet<string>();
            foreach(string filepath in proteinGroupTsvFilepaths)
            {
                string dataset = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(filepath));

                proteins.Add(dataset, new Dictionary<string, KeyValuePair<int, int>>());

                using(StreamReader input = new StreamReader(filepath))
                {
                    string header = input.ReadLine();
                    string[] header_fields = header.Split('\t');
                    int protein_description_index = Array.FindIndex(header_fields, s => s == "Protein Description");
                    int decoy_index = Array.FindIndex(header_fields, s => s == "Decoy?");
                    int q_value_index = Array.FindIndex(header_fields, s => s == "Q-Value (%)");
                    int psms_index = Array.FindIndex(header_fields, s => s == "Number of Peptide-Spectrum Matches");
                    int peptides_index = Array.FindIndex(header_fields, s => s == "Number of Unique Peptides");

                    while(input.Peek() != -1)
                    {
                        string line = input.ReadLine();
                        string[] fields = line.Split('\t');

                        if(double.Parse(fields[q_value_index]) <= MAXIMUM_FDR_PERCENT && !bool.Parse(fields[decoy_index]))
                        {
                            foreach(string protein in fields[protein_description_index].Split(new string[] { ";; " }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                proteins[dataset].Add(protein, new KeyValuePair<int, int>(int.Parse(fields[psms_index]), int.Parse(fields[peptides_index])));
                                distinct_proteins.Add(protein);
                            }
                        }
                    }
                }
            }

            using(StreamWriter psms = new StreamWriter(Path.Combine(outputFolder, "protein_PSMs.tsv")))
            {
                using(StreamWriter peptides = new StreamWriter(Path.Combine(outputFolder, "protein_peptides.tsv")))
                {
                    psms.Write('\t');
                    peptides.Write('\t');
                    foreach(string dataset in proteins.Keys)
                    {
                        psms.Write(dataset + '\t');
                        peptides.Write(dataset + '\t');
                    }
                    psms.WriteLine();
                    peptides.WriteLine();

                    foreach(string protein in distinct_proteins)
                    {
                        psms.Write(protein + '\t');
                        peptides.Write(protein + '\t');
                        foreach(string filepath in proteins.Keys)
                        {
                            KeyValuePair<int, int> kvp = new KeyValuePair<int, int>(0, 0);
                            proteins[filepath].TryGetValue(protein, out kvp);
                            psms.Write(kvp.Key.ToString() + '\t');
                            peptides.Write(kvp.Value.ToString() + '\t');
                        }
                        psms.WriteLine();
                        peptides.WriteLine();
                    }
                }
            }
        }
    }
}
