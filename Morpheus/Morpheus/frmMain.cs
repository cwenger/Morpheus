using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Morpheus
{
    public partial class frmMain : Form
    {
        private string CASE_SENSITIVE_EXTENSION = "*.mzML;*.mzml;*.MZML";
        private string CASE_INSENSITIVE_EXTENSION = "*.mzML";
        private const string CASE_SENSITIVE_FASTA_EXTENSIONS = "*.fa;*.FA;*.faa;*.FAA;*.fas;*.FAS;*.fasta;*.FASTA;*.fsa;*.FSA";
        private const string CASE_INSENSITIVE_FASTA_EXTENSIONS = "*.fa;*.faa;*.fas;*.fasta;*.fsa";
        private bool DIRECTORY = false;
        private string BASE_LABEL = "mzML data files";
        private string FORM_LABEL { get { return BASE_LABEL + " (" + CASE_INSENSITIVE_EXTENSION + ')'; } }
        private string DIALOG_LABEL { get { return BASE_LABEL + (Environment.OSVersion.Platform == PlatformID.Unix || Application.ProductName.Contains("Agilent") ? " (" + CASE_INSENSITIVE_EXTENSION + ')' : null); } }
        private string DIALOG_FILTER { get { return DIALOG_LABEL + '|' + (Environment.OSVersion.Platform == PlatformID.Unix ? CASE_SENSITIVE_EXTENSION : CASE_INSENSITIVE_EXTENSION); } }

        public frmMain()
        {
            if(Application.ProductName.Contains("Agilent"))
            {
                CASE_INSENSITIVE_EXTENSION = "*.d";
                DIRECTORY = true;
                BASE_LABEL = "Agilent data directories";
            }
            else if(Application.ProductName.Contains("Thermo"))
            {
                CASE_INSENSITIVE_EXTENSION = "*.raw";
                DIRECTORY = false;
                BASE_LABEL = "Thermo data files";
            }

            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            Text = Program.GetProductNameAndVersion();
            label1.Text = FORM_LABEL;
            ofdData.Filter = DIALOG_FILTER;
            ofdFasta.Filter = Environment.OSVersion.Platform == PlatformID.Unix 
                ? "FASTA proteome database files (" + CASE_INSENSITIVE_FASTA_EXTENSIONS + ")|" + CASE_SENSITIVE_FASTA_EXTENSIONS + "|UniProt XML proteome database files (*.xml)|*.xml;*.XML"
                : "FASTA proteome database files|" + CASE_INSENSITIVE_FASTA_EXTENSIONS + "|UniProt XML proteome database files|*.xml";
            if(Application.ProductName.Contains("Thermo"))
            {
                chkDeisotope.Enabled = false;
                chkDeisotope.Checked = false;
            }

            ProteaseDictionary proteases = null;
            try
            {
                proteases = ProteaseDictionary.Instance;
                foreach(Protease protease in proteases.Values)
                {
                    cboProtease.Items.Add(protease);
                }
            }
            catch
            {
                MessageBox.Show("Your proteases file (" + Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), "proteases.tsv") + ") is likely corrupt. Please correct it. Program will now exit.");
                Application.Exit();
            }

            foreach(string initiatior_methionine_behavior in Enum.GetNames(typeof(InitiatorMethionineBehavior)))
            {
                cboInitiatorMethionineBehavior.Items.Add(initiatior_methionine_behavior.ToLower());
            }

            ModificationDictionary modifications = null;
            try
            {
                modifications = ModificationDictionary.Instance;
                clbFixedModifications.BeginUpdate();
                clbVariableModifications.BeginUpdate();
                foreach(Modification modification in modifications.Values)
                {
                    clbFixedModifications.Items.Add(modification, modification.DefaultFixed);
                    clbVariableModifications.Items.Add(modification, modification.DefaultVariable);
                }
                clbFixedModifications.EndUpdate();
                clbVariableModifications.EndUpdate();
            }
            catch
            {
                MessageBox.Show("Your modifications file (" + Path.Combine(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]), "modifications.tsv") + ") is likely corrupt. Please correct it.");
            }

            cboPrecursorMassToleranceUnits.Items.AddRange(Enum.GetNames(typeof(MassToleranceUnits)));
            cboProductMassToleranceUnits.Items.AddRange(Enum.GetNames(typeof(MassToleranceUnits)));

            foreach(string mass_type in Enum.GetNames(typeof(MassType)))
            {
                cboPrecursorMassType.Items.Add(mass_type.ToLower());
                cboProductMassType.Items.Add(mass_type.ToLower());
            }

            numMaxThreads.Maximum = Environment.ProcessorCount;

            string settings_filepath = Path.Combine(Application.UserAppDataPath, "settings.tsv");
            if(File.Exists(settings_filepath))
            {
                try
                {
                    using(StreamReader settings = new StreamReader(settings_filepath))
                    {
                        while(settings.Peek() != -1)
                        {
                            string line = settings.ReadLine();
                            string[] fields = line.Split('\t');
                            string name = fields[0];
                            string value = fields[1];
                            string[] value_fields;

                            switch(name)
                            {
                                case "Minimum Assumed Precursor Charge State":
                                    numMinimumAssumedPrecursorChargeState.Value = int.Parse(value);
                                    break;
                                case "Maximum Assumed Precursor Charge State":
                                    numMaximumAssumedPrecursorChargeState.Value = int.Parse(value);
                                    break;
                                case "Absolute MS/MS Peak Threshold":
                                    value_fields = value.Split(',');
                                    chkAbsoluteThreshold.Checked = bool.Parse(value_fields[0]);
                                    txtAbsoluteThreshold.Text = value_fields[1];
                                    break;
                                case "Relative MS/MS Peak Threshold (%)":
                                    value_fields = value.Split(',');
                                    chkRelativeThreshold.Checked = bool.Parse(value_fields[0]);
                                    txtRelativeThresholdPercent.Text = value_fields[1];
                                    break;
                                case "Maximum Number of MS/MS Peaks":
                                    value_fields = value.Split(',');
                                    chkMaxNumPeaks.Checked = bool.Parse(value_fields[0]);
                                    numMaxPeaks.Value = (decimal)int.Parse(value_fields[1]);
                                    break;
                                case "Assign Charge States":
                                    chkAssignChargeStates.Checked = bool.Parse(value);
                                    break;
                                case "De-isotope":
                                    if(!Application.ProductName.Contains("Thermo"))
                                    {
                                        chkDeisotope.Checked = bool.Parse(value);
                                    }
                                    break;
                                case "Protease":
                                    cboProtease.SelectedItem = proteases[value];
                                    break;
                                case "Maximum Missed Cleavages":
                                    numMaxMissedCleavages.Value = int.Parse(value);
                                    break;
                                case "Initiator Methionine Behavior":
                                    cboInitiatorMethionineBehavior.SelectedIndex = (int)Enum.Parse(typeof(InitiatorMethionineBehavior), value, true);
                                    break;
                                case "Maximum Variable Modification Isoforms per Peptide":
                                    numMaxVariableModIsoforms.Value = int.Parse(value);
                                    break;
                                case "Precursor Mass Tolerance":
                                    numPrecursorMassTolerance.Value = decimal.Parse(value, CultureInfo.InvariantCulture);
                                    break;
                                case "Precursor Mass Tolerance Units":
                                    cboPrecursorMassToleranceUnits.SelectedIndex = (int)Enum.Parse(typeof(MassToleranceUnits), value, true);
                                    break;
                                case "Precursor Mass Type":
                                    cboPrecursorMassType.SelectedIndex = (int)Enum.Parse(typeof(MassType), value, true);
                                    break;
                                case "Accepted Precursor Mass Errors (Da)":
                                    txtAcceptedPrecursorMassErrors.Text = value;
                                    break;
                                case "Product Mass Tolerance":
                                    numProductMassTolerance.Value = decimal.Parse(value, CultureInfo.InvariantCulture);
                                    break;
                                case "Product Mass Tolerance Units":
                                    cboProductMassToleranceUnits.SelectedIndex = (int)Enum.Parse(typeof(MassToleranceUnits), value, true);
                                    break;
                                case "Product Mass Type":
                                    cboProductMassType.SelectedIndex = (int)Enum.Parse(typeof(MassType), value, true);
                                    break;
                                case "Maximum FDR (%)":
                                    numMaximumFalseDiscoveryRatePercent.Value = decimal.Parse(value, CultureInfo.InvariantCulture);
                                    break;
                                case "Consider Modified Forms as Unique Peptides":
                                    chkConsiderModifiedUnique.Checked = bool.Parse(value);
                                    break;
                                case "Maximum Threads":
                                    if(int.Parse(value) > numMaxThreads.Maximum)
                                    {
                                        numMaxThreads.Value = numMaxThreads.Maximum;
                                    }
                                    else
                                    {
                                        numMaxThreads.Value = int.Parse(value);
                                    }
                                    break;
                                case "Minimize Memory Usage":
                                    chkMinimizeMemoryUsage.Checked = bool.Parse(value);
                                    break;
                            }
                        }
                    }
                    return;
                }
                catch
                {
                    MessageBox.Show("Your settings file (" + settings_filepath + ") is likely corrupt. Defaults will be used.");
                }
            }

            cboProtease.SelectedItem = proteases["trypsin (no proline rule)"];
            cboInitiatorMethionineBehavior.SelectedIndex = (int)InitiatorMethionineBehavior.Variable;
            cboPrecursorMassToleranceUnits.SelectedIndex = (int)MassToleranceUnits.Da;
            cboPrecursorMassType.SelectedIndex = (int)MassType.Monoisotopic;
            cboProductMassToleranceUnits.SelectedIndex = (int)MassToleranceUnits.Da;
            cboProductMassType.SelectedIndex = (int)MassType.Monoisotopic;
            numMaxThreads.Value = numMaxThreads.Maximum;
        }

        private void frmMain_DragEnter(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filepaths = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach(string filepath in filepaths)
                {
                    if((((!DIRECTORY && File.Exists(filepath)) || (DIRECTORY && Directory.Exists(filepath))) && Path.GetExtension(filepath).Length > 0 && CASE_INSENSITIVE_EXTENSION.IndexOf(Path.GetExtension(filepath), StringComparison.OrdinalIgnoreCase) >= 0 && !lstData.Items.Contains(filepath))
                        || File.Exists(filepath) && CASE_INSENSITIVE_FASTA_EXTENSIONS.IndexOf(Path.GetExtension(filepath).ToLower(), StringComparison.OrdinalIgnoreCase) >= 0 
                        || Path.GetExtension(filepath).Equals(".xml", StringComparison.OrdinalIgnoreCase)
                        || Directory.Exists(filepath))
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
                if(((!DIRECTORY && File.Exists(filepath)) || (DIRECTORY && Directory.Exists(filepath))) && Path.GetExtension(filepath).Length > 0 && CASE_INSENSITIVE_EXTENSION.IndexOf(Path.GetExtension(filepath), StringComparison.OrdinalIgnoreCase) >= 0 && !lstData.Items.Contains(filepath))
                {
                    lstData.Items.Add(filepath);
                    tspbProgress.Value = tspbProgress.Minimum;
                }
                else if(File.Exists(filepath) && CASE_INSENSITIVE_FASTA_EXTENSIONS.IndexOf(Path.GetExtension(filepath), StringComparison.OrdinalIgnoreCase) >= 0
                    || Path.GetExtension(filepath).Equals(".xml", StringComparison.OrdinalIgnoreCase))
                {
                    txtFastaFile.Text = filepath;
                }
                else if(Directory.Exists(filepath))
                {
                    if(!AddDataFromDirectory(filepath) && !lstData.Items.Contains(filepath))
                    {
                        txtOutputFolder.Text = filepath;
                    }
                }
            }

            if(lstData.Items.Count > 0)
            {
                btnClear.Enabled = true;
                if(txtOutputFolder.Text == string.Empty)
                {
                    txtOutputFolder.Text = Path.GetDirectoryName((string)lstData.Items[0]);
                }
            }
        }

        private bool AddDataFromDirectory(string directory)
        {
            bool added = false;

            if(DIRECTORY)
            {
                string[] directories = Directory.GetDirectories(directory, "*.*", SearchOption.AllDirectories).Where(x => Path.GetExtension(x).Length > 0 && CASE_INSENSITIVE_EXTENSION.IndexOf(Path.GetExtension(x), StringComparison.OrdinalIgnoreCase) >= 0).ToArray();
                if(directories.Length > 0)
                {
                    foreach(string subdirectory in directories)
                    {
                        if(!lstData.Items.Contains(subdirectory))
                        {
                            lstData.Items.Add(subdirectory);
                            added = true;
                            tspbProgress.Value = tspbProgress.Minimum;
                        }
                    }
                }
            }
            else
            {
                string[] files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories).Where(x => Path.GetExtension(x).Length > 0 && CASE_INSENSITIVE_EXTENSION.IndexOf(Path.GetExtension(x), StringComparison.OrdinalIgnoreCase) >= 0).ToArray();
                if(files.Length > 0)
                {
                    foreach(string file in files)
                    {
                        if(!lstData.Items.Contains(file))
                        {
                            lstData.Items.Add(file);
                            added = true;
                            tspbProgress.Value = tspbProgress.Minimum;
                        }
                    }
                }
            }

            return added;
        }

        partial void AddAgilent();

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if(DIRECTORY)
            {
                if(Text.Contains("Agilent"))
                {
                    AddAgilent();
                }
                else
                {
                    if(fbdData.ShowDialog() == DialogResult.OK)
                    {
                        string data_filepath = fbdData.SelectedPath;
                        if(!lstData.Items.Contains(data_filepath))
                        {
                            lstData.Items.Add(data_filepath);
                            tspbProgress.Value = tspbProgress.Minimum;
                        }
                    }
                }
            }
            else
            {
                if(ofdData.ShowDialog() == DialogResult.OK)
                {
                    foreach(string data_filepath in ofdData.FileNames)
                    {
                        if(!lstData.Items.Contains(data_filepath))
                        {
                            lstData.Items.Add(data_filepath);
                            tspbProgress.Value = tspbProgress.Minimum;
                        }
                    }
                    ofdData.InitialDirectory = Path.GetDirectoryName(ofdData.FileName);
                }
            }

            if(lstData.Items.Count > 0)
            {
                btnClear.Enabled = true;
                if(txtOutputFolder.Text == string.Empty)
                {
                    txtOutputFolder.Text = Path.GetDirectoryName((string)lstData.Items[0]);
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            while(lstData.SelectedIndices.Count > 0)
            {
                lstData.Items.Remove(lstData.SelectedItem);
                tspbProgress.Value = tspbProgress.Minimum;
            }

            if(lstData.Items.Count == 0)
            {
                btnClear.Enabled = false;
                txtOutputFolder.Clear();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lstData.Items.Clear();
            btnRemove.Enabled = false;
            btnClear.Enabled = false;
            txtOutputFolder.Text = "";
            tspbProgress.Value = tspbProgress.Minimum;
        }

        private void lstData_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lstData.SelectedItems.Count > 0)
            {
                btnRemove.Enabled = true;
            }
            else
            {
                btnRemove.Enabled = false;
            }
        }

        private void chkAbsoluteThreshold_CheckedChanged(object sender, EventArgs e)
        {
            if(chkAbsoluteThreshold.Checked)
            {
                txtAbsoluteThreshold.Enabled = true;
            }
            else
            {
                txtAbsoluteThreshold.Enabled = false;
            }
            tspbProgress.Value = tspbProgress.Minimum;
        }

        private void chkRelativeThresholdPercent_CheckedChanged(object sender, EventArgs e)
        {
            if(chkRelativeThreshold.Checked)
            {
                txtRelativeThresholdPercent.Enabled = true;
                label15.Enabled = true;
            }
            else
            {
                txtRelativeThresholdPercent.Enabled = false;
                label15.Enabled = false;
            }
            tspbProgress.Value = tspbProgress.Minimum;
        }

        private void chkMaxNumPeaks_CheckedChanged(object sender, EventArgs e)
        {
            if(chkMaxNumPeaks.Checked)
            {
                numMaxPeaks.Enabled = true;
            }
            else
            {
                numMaxPeaks.Enabled = false;
            }
            tspbProgress.Value = tspbProgress.Minimum;
        }

        private void chkAssignChargeStates_CheckedChanged(object sender, EventArgs e)
        {
            if(chkAssignChargeStates.Checked && !Text.Contains("Thermo"))
            {
                chkDeisotope.Enabled = true;
            }
            else
            {
                chkDeisotope.Enabled = false;
                chkDeisotope.Checked = false;
            }
            tspbProgress.Value = tspbProgress.Minimum;
        }

        private void btnBrowseFasta_Click(object sender, EventArgs e)
        {
            if(ofdFasta.ShowDialog() == DialogResult.OK)
            {
                txtFastaFile.Text = ofdFasta.FileName;
                ofdFasta.InitialDirectory = Path.GetDirectoryName(ofdFasta.FileName);
                tspbProgress.Value = tspbProgress.Minimum;
            }
        }

        private void txtFastaFile_TextChanged(object sender, EventArgs e)
        {
            if(File.Exists(txtFastaFile.Text))
            {
                CheckDatabase();
            }
            tspbProgress.Value = tspbProgress.Minimum;
        }

        private void CheckDatabase()
        {
            AllowDrop = false;
            pnlMain.Enabled = false;
            tsslStatus.Text = "Parsing database...";
            tspbProgress.Style = ProgressBarStyle.Marquee;
            Cursor = Cursors.WaitCursor;

            int i = 0;
            clbVariableModifications.BeginUpdate();
            while(i < clbVariableModifications.Items.Count)
            {
                Modification modification = (Modification)clbVariableModifications.Items[i];
                if(modification.Known)
                {
                    clbVariableModifications.Items.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
            clbVariableModifications.EndUpdate();

            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if(Path.GetExtension(txtFastaFile.Text).Equals(".xml", StringComparison.OrdinalIgnoreCase))
            {
                e.Result = new KeyValuePair<IEnumerable<Modification>, bool>(ProteomeDatabaseReader.ReadUniProtXmlModifications(txtFastaFile.Text).Values, false);
            }
            else
            {
                e.Result = new KeyValuePair<IEnumerable<Modification>, bool>(null, ProteomeDatabaseReader.HasDecoyProteins(txtFastaFile.Text));
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            KeyValuePair<IEnumerable<Modification>, bool> result = (KeyValuePair<IEnumerable<Modification>, bool>)e.Result;

            if(result.Key != null)
            {
                clbVariableModifications.BeginUpdate();
                foreach(Modification modification in result.Key)
                {
                    clbVariableModifications.Items.Add(modification, modification.DefaultVariable);
                }
                clbVariableModifications.TopIndex = clbVariableModifications.Items.Count - 1;
                clbVariableModifications.EndUpdate();
            }

            chkOnTheFlyDecoys.Checked = !result.Value;

            tsslStatus.Text = "Ready";
            tspbProgress.Style = ProgressBarStyle.Blocks;
            Cursor = Cursors.Default;
            AllowDrop = true;
            pnlMain.Enabled = true;
        }

        private void cboProtease_SelectedIndexChanged(object sender, EventArgs e)
        {
            if((Protease)cboProtease.SelectedItem == ProteaseDictionary.Instance["top-down"])
            {
                numMaxMissedCleavages.Enabled = false;
            }
            else
            {
                numMaxMissedCleavages.Enabled = true;
            }
            tspbProgress.Value = tspbProgress.Minimum;
        }

        private void cboPrecursorMassType_SelectedIndexChanged(object sender, EventArgs e)
        {
            tspbProgress.Value = tspbProgress.Minimum;
        }

        private void btnBrowseOutput_Click(object sender, EventArgs e)
        {
            if(fbdOutput.ShowDialog() == DialogResult.OK)
            {
                txtOutputFolder.Text = fbdOutput.SelectedPath;
                tspbProgress.Value = tspbProgress.Minimum;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if(lstData.Items.Count == 0)
            {
                MessageBox.Show("No data files selected");
                btnAdd.Focus();
                return;
            }
            List<string> data_filepaths = new List<string>(lstData.Items.Count);
            foreach(string data_filepath in lstData.Items)
            {
                data_filepaths.Add(data_filepath);
            }
            bool assign_charge_states = chkAssignChargeStates.Checked;
            bool deisotope = chkDeisotope.Checked;
            string fasta_filepath = txtFastaFile.Text;
            if(!File.Exists(fasta_filepath))
            {
                if(fasta_filepath.Length > 0)
                {
                    MessageBox.Show("Invalid protein database file: " + fasta_filepath);
                }
                else
                {
                    MessageBox.Show("Invalid protein database file");
                }
                txtFastaFile.Focus();
                return;
            }
            bool on_the_fly_decoys = chkOnTheFlyDecoys.Checked;
            Protease protease = (Protease)cboProtease.SelectedItem;
            int max_missed_cleavages = (int)numMaxMissedCleavages.Value;
            InitiatorMethionineBehavior initiator_methionine_behavior = (InitiatorMethionineBehavior)Enum.Parse(typeof(InitiatorMethionineBehavior), cboInitiatorMethionineBehavior.Text, true);
            List<Modification> fixed_modifications = new List<Modification>(clbFixedModifications.CheckedItems.Count);
            foreach(object fixed_modification in clbFixedModifications.CheckedItems)
            {
                fixed_modifications.Add((Modification)fixed_modification);
            }
            List<Modification> variable_modifications = new List<Modification>(clbVariableModifications.CheckedItems.Count);
            foreach(object variable_modification in clbVariableModifications.CheckedItems)
            {
                variable_modifications.Add((Modification)variable_modification);
            }
            int max_variable_mod_isoforms = (int)numMaxVariableModIsoforms.Value;
            int min_assumed_precursor_charge_state = (int)numMinimumAssumedPrecursorChargeState.Value;
            int max_assumed_precursor_charge_state = (int)numMaximumAssumedPrecursorChargeState.Value;
            double absolute_threshold = -1.0;
            if(chkAbsoluteThreshold.Checked)
            {
                if(!double.TryParse(txtAbsoluteThreshold.Text, out absolute_threshold))
                {
                    MessageBox.Show("Cannot parse absolute MS/MS peak threshold: " + txtAbsoluteThreshold.Text);
                    txtAbsoluteThreshold.Focus();
                    return;
                }
            }
            double relative_threshold_percent = -1.0;
            if(chkRelativeThreshold.Checked)
            {
                if(!double.TryParse(txtRelativeThresholdPercent.Text, out relative_threshold_percent))
                {
                    MessageBox.Show("Cannot parse relative MS/MS peak threshold: " + txtRelativeThresholdPercent.Text);
                    txtRelativeThresholdPercent.Focus();
                    return;
                }
            }
            int max_peaks = -1;
            if(chkMaxNumPeaks.Checked)
            {
                max_peaks = (int)numMaxPeaks.Value;
            }
            MassTolerance precursor_mass_tolerance = new MassTolerance((double)numPrecursorMassTolerance.Value, (MassToleranceUnits)cboPrecursorMassToleranceUnits.SelectedIndex);
            MassType precursor_mass_type = (MassType)cboPrecursorMassType.SelectedIndex;
            List<double> accepted_precursor_mass_errors = new List<double>();
            if(txtAcceptedPrecursorMassErrors.Text.Length > 0)
            {
                foreach(string accepted_precursor_mass_error_text in txtAcceptedPrecursorMassErrors.Text.Split(','))
                {
                    double accepted_precursor_mass_error;
                    if(!double.TryParse(accepted_precursor_mass_error_text, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out accepted_precursor_mass_error))
                    {
                        MessageBox.Show("Cannot parse accepted precursor mass errors: " + txtRelativeThresholdPercent.Text);
                        txtAcceptedPrecursorMassErrors.Focus();
                        return;
                    }
                    accepted_precursor_mass_errors.Add(accepted_precursor_mass_error);
                }
            }
            else
            {
                accepted_precursor_mass_errors.Add(0.0);
            }
            MassTolerance product_mass_tolerance = new MassTolerance((double)numProductMassTolerance.Value, (MassToleranceUnits)cboProductMassToleranceUnits.SelectedIndex);
            MassType product_mass_type = (MassType)cboProductMassType.SelectedIndex;
            double max_false_discovery_rate = (double)numMaximumFalseDiscoveryRatePercent.Value / 100.0;
            bool consider_modified_unique = chkConsiderModifiedUnique.Checked;
            int max_threads = (int)numMaxThreads.Value;
            bool minimize_memory_usage = chkMinimizeMemoryUsage.Checked;
            string output_folder = txtOutputFolder.Text;
            if(!Directory.Exists(output_folder))
            {
                try
                {
                    Directory.CreateDirectory(output_folder);
                }
                catch
                {
                    if(output_folder.Length > 0)
                    {
                        MessageBox.Show("Invalid output folder: " + output_folder);
                    }
                    else
                    {
                        MessageBox.Show("Invalid output folder");
                    }
                    txtOutputFolder.Focus();
                    return;
                }
            }

            DatabaseSearcher database_searcher = new DatabaseSearcher(data_filepaths,
                min_assumed_precursor_charge_state, max_assumed_precursor_charge_state,
                absolute_threshold, relative_threshold_percent, max_peaks,
                assign_charge_states, deisotope,
                fasta_filepath, on_the_fly_decoys,
                protease, max_missed_cleavages, initiator_methionine_behavior,
                fixed_modifications, variable_modifications, max_variable_mod_isoforms,
                precursor_mass_tolerance, precursor_mass_type,
                accepted_precursor_mass_errors,
                product_mass_tolerance, product_mass_type,
                max_false_discovery_rate, consider_modified_unique,
                max_threads, minimize_memory_usage,
                output_folder);

            database_searcher.Starting += HandleStarting;
            database_searcher.StartingFile += HandleStartingFile;
            database_searcher.UpdateStatus += HandleUpdateStatus;
            database_searcher.ReportTaskWithoutProgress += HandleReportTaskWithoutProgress;
            database_searcher.ReportTaskWithProgress += HandleReportTaskWithProgress;
            database_searcher.UpdateProgress += HandleUpdateProgress;
            database_searcher.ThrowException += HandleThrowException;
            database_searcher.FinishedFile += HandleFinishedFile;
            database_searcher.Finished += HandleFinished;

            lstData.SelectedItem = null;
            tspbProgress.Value = tspbProgress.Minimum;

            Thread thread = new Thread(new ThreadStart(database_searcher.Search));
            thread.IsBackground = true;
            thread.Start();
        }

        private void SetDropAllowed(Form form, bool allowDrop)
        {
            if(form.InvokeRequired)
            {
                form.Invoke(new Action<Form, bool>(SetDropAllowed), new object[] { form, allowDrop });
            }
            else
            {
                form.AllowDrop = allowDrop;
            }
        }

        private void SetControlEnabled(Control control, bool enabled)
        {
            if(control.InvokeRequired)
            {
                control.Invoke(new Action<Control, bool>(SetControlEnabled), new object[] { control, enabled });
            }
            else
            {
                control.Enabled = enabled;
            }
        }

        private void HandleStarting(object sender, EventArgs e)
        {
            SetDropAllowed(this, false);
            SetControlEnabled(pnlMain, false);
        }

        private void SetListBoxSelectedItem(ListBox listBox, string selectedItem)
        {
            if(listBox.InvokeRequired)
            {
                listBox.Invoke(new Action<ListBox, string>(SetListBoxSelectedItem), new object[] { listBox, selectedItem });
            }
            else
            {
                listBox.SelectedItem = selectedItem;
            }
        }

        private void HandleStartingFile(object sender, FilepathEventArgs e)
        {
            SetListBoxSelectedItem(lstData, e.Filepath);
        }

        private void SetToolStripItemText(ToolStripItem toolStripItem, string text)
        {
            if(toolStripItem.GetCurrentParent().InvokeRequired)
            {
                toolStripItem.GetCurrentParent().Invoke(new Action<ToolStripItem, string>(SetToolStripItemText), new object[] { toolStripItem, text });
            }
            else
            {
                toolStripItem.Text = text;
            }
        }

        private void HandleUpdateStatus(object sender, StatusEventArgs e)
        {
            SetToolStripItemText(tsslStatus, e.Status);
        }

        private void SetToolStripProgressBarStyle(ToolStripProgressBar toolStripProgressBar, ProgressBarStyle style)
        {
            if(toolStripProgressBar.GetCurrentParent().InvokeRequired)
            {
                toolStripProgressBar.GetCurrentParent().Invoke(new Action<ToolStripProgressBar, ProgressBarStyle>(SetToolStripProgressBarStyle), new object[] { toolStripProgressBar, style });
            }
            else
            {
                toolStripProgressBar.Style = style;
            }
        }

        private void HandleReportTaskWithProgress(object sender, EventArgs e)
        {
            SetToolStripProgressBarStyle(tspbProgress, ProgressBarStyle.Continuous);
        }

        private void HandleReportTaskWithoutProgress(object sender, EventArgs e)
        {
            SetToolStripProgressBarStyle(tspbProgress, ProgressBarStyle.Marquee);
        }

        private void SetToolStripProgressBarValue(ToolStripProgressBar toolStripProgressBar, int value)
        {
            if(toolStripProgressBar.GetCurrentParent().InvokeRequired)
            {
                toolStripProgressBar.GetCurrentParent().Invoke(new Action<ToolStripProgressBar, int>(SetToolStripProgressBarValue), new object[] { toolStripProgressBar, value });
            }
            else
            {
                toolStripProgressBar.Value = value;
            }
        }

        private void HandleUpdateProgress(object sender, ProgressEventArgs e)
        {
            SetToolStripProgressBarValue(tspbProgress, e.Progress);
        }

        private void HandleThrowException(object sender, ExceptionEventArgs e)
        {
            ShowGridErrorDialog(null, e.Exception.Message, e.Exception.ToString());
            SetListBoxSelectedItem(lstData, null);
            SetToolStripItemText(tsslStatus, "Ready");
            SetToolStripProgressBarStyle(tspbProgress, ProgressBarStyle.Blocks);
            SetToolStripProgressBarValue(tspbProgress, 0);
            SetDropAllowed(this, true);
            SetControlEnabled(pnlMain, true);
        }

        // from http://stackoverflow.com/a/8653764/60067
        private static DialogResult ShowGridErrorDialog(string title, string message, string details)
        {
            Type type = typeof(Form).Assembly.GetType("System.Windows.Forms.PropertyGridInternal.GridErrorDlg");
            Form dialog = (Form)Activator.CreateInstance(type, new PropertyGrid());
            dialog.Text = title;
            type.GetProperty("Message").SetValue(dialog, message, null);
            type.GetProperty("Details").SetValue(dialog, details, null);
            return dialog.ShowDialog();
        }

        private void HandleFinishedFile(object sender, FilepathEventArgs e)
        {
            SetListBoxSelectedItem(lstData, null);
        }

        private void HandleFinished(object sender, EventArgs e)
        {
            SetToolStripItemText(tsslStatus, "Ready");
            SetToolStripProgressBarStyle(tspbProgress, ProgressBarStyle.Blocks);
            SetToolStripProgressBarValue(tspbProgress, 100);
            SetDropAllowed(this, true);
            SetControlEnabled(pnlMain, true);
            GC.Collect();
        }

        private void ResetProgressBar(object sender, EventArgs e)
        {
            tspbProgress.Value = tspbProgress.Minimum;
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            using(StreamWriter settings = new StreamWriter(Path.Combine(Application.UserAppDataPath, "settings.tsv")))
            {
                settings.WriteLine("Minimum Assumed Precursor Charge State" + '\t' + ((int)numMinimumAssumedPrecursorChargeState.Value).ToString());
                settings.WriteLine("Maximum Assumed Precursor Charge State" + '\t' + ((int)numMaximumAssumedPrecursorChargeState.Value).ToString());
                settings.WriteLine("Absolute MS/MS Peak Threshold" + '\t' + chkAbsoluteThreshold.Checked.ToString().ToLower() + ',' + txtAbsoluteThreshold.Text);
                settings.WriteLine("Relative MS/MS Peak Threshold (%)" + '\t' + chkRelativeThreshold.Checked.ToString().ToLower() + ',' + txtRelativeThresholdPercent.Text);
                settings.WriteLine("Maximum Number of MS/MS Peaks" + '\t' + chkMaxNumPeaks.Checked.ToString().ToLower() + ',' + ((int)numMaxPeaks.Value).ToString());
                settings.WriteLine("Assign Charge States" + '\t' + chkAssignChargeStates.Checked.ToString().ToLower());
                settings.WriteLine("De-isotope" + '\t' + chkDeisotope.Checked.ToString().ToLower());
                settings.WriteLine("Protease" + '\t' + cboProtease.Text);
                settings.WriteLine("Maximum Missed Cleavages" + '\t' + ((int)numMaxMissedCleavages.Value).ToString());
                settings.WriteLine("Initiator Methionine Behavior" + '\t' + cboInitiatorMethionineBehavior.Text);
                settings.WriteLine("Maximum Variable Modification Isoforms per Peptide" + '\t' + ((int)numMaxVariableModIsoforms.Value).ToString());
                settings.WriteLine("Precursor Mass Tolerance" + '\t' + numPrecursorMassTolerance.Value.ToString(CultureInfo.InvariantCulture));
                settings.WriteLine("Precursor Mass Tolerance Units" + '\t' + cboPrecursorMassToleranceUnits.Text);
                settings.WriteLine("Precursor Mass Type" + '\t' + cboPrecursorMassType.Text);
                settings.WriteLine("Accepted Precursor Mass Errors (Da)" + '\t' + txtAcceptedPrecursorMassErrors.Text);
                settings.WriteLine("Product Mass Tolerance" + '\t' + numProductMassTolerance.Value.ToString(CultureInfo.InvariantCulture));
                settings.WriteLine("Product Mass Tolerance Units" + '\t' + cboProductMassToleranceUnits.Text);
                settings.WriteLine("Product Mass Type" + '\t' + cboProductMassType.Text);
                settings.WriteLine("Maximum FDR (%)" + '\t' + numMaximumFalseDiscoveryRatePercent.Value.ToString(CultureInfo.InvariantCulture));
                settings.WriteLine("Consider Modified Forms as Unique Peptides" + '\t' + chkConsiderModifiedUnique.Checked.ToString().ToLower());
                settings.WriteLine("Maximum Threads" + '\t' + ((int)numMaxThreads.Value).ToString());
                settings.WriteLine("Minimize Memory Usage" + '\t' + chkMinimizeMemoryUsage.Checked.ToString().ToLower());
            }
        }
    }
}