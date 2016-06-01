namespace Morpheus
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.btnClear = new System.Windows.Forms.Button();
            this.lstData = new System.Windows.Forms.ListBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtFastaFile = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBrowseFasta = new System.Windows.Forms.Button();
            this.ofdFasta = new System.Windows.Forms.OpenFileDialog();
            this.cboProtease = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.numMaxMissedCleavages = new System.Windows.Forms.NumericUpDown();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnBrowseOutput = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.fbdOutput = new System.Windows.Forms.FolderBrowserDialog();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.chkMinimizeMemoryUsage = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkDeisotope = new System.Windows.Forms.CheckBox();
            this.chkAssignChargeStates = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txtRelativeThresholdPercent = new System.Windows.Forms.TextBox();
            this.txtAbsoluteThreshold = new System.Windows.Forms.TextBox();
            this.chkMaxNumPeaks = new System.Windows.Forms.CheckBox();
            this.numMaxPeaks = new System.Windows.Forms.NumericUpDown();
            this.chkRelativeThreshold = new System.Windows.Forms.CheckBox();
            this.chkAbsoluteThreshold = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.numMinimumAssumedPrecursorChargeState = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.numMaximumAssumedPrecursorChargeState = new System.Windows.Forms.NumericUpDown();
            this.label21 = new System.Windows.Forms.Label();
            this.numMaxThreads = new System.Windows.Forms.NumericUpDown();
            this.label19 = new System.Windows.Forms.Label();
            this.chkPrecursorMonoisotopicPeakCorrection = new System.Windows.Forms.CheckBox();
            this.label20 = new System.Windows.Forms.Label();
            this.numMaxPrecursorMonoPeakOffset = new System.Windows.Forms.NumericUpDown();
            this.numMinPrecursorMonoPeakOffset = new System.Windows.Forms.NumericUpDown();
            this.chkOnTheFlyDecoys = new System.Windows.Forms.CheckBox();
            this.chkConsiderModifiedUnique = new System.Windows.Forms.CheckBox();
            this.numMaxVariableModIsoforms = new System.Windows.Forms.NumericUpDown();
            this.cboProductMassType = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.numProductMassTolerance = new System.Windows.Forms.NumericUpDown();
            this.cboProductMassToleranceUnits = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cboPrecursorMassType = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.numPrecursorMassTolerance = new System.Windows.Forms.NumericUpDown();
            this.cboPrecursorMassToleranceUnits = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.numMaximumFalseDiscoveryRatePercent = new System.Windows.Forms.NumericUpDown();
            this.label16 = new System.Windows.Forms.Label();
            this.clbVariableModifications = new System.Windows.Forms.CheckedListBox();
            this.clbFixedModifications = new System.Windows.Forms.CheckedListBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.cboInitiatorMethionineBehavior = new System.Windows.Forms.ComboBox();
            this.txtOutputFolder = new System.Windows.Forms.TextBox();
            this.ofdData = new System.Windows.Forms.OpenFileDialog();
            this.fbdData = new System.Windows.Forms.FolderBrowserDialog();
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.ssStatusStrip = new System.Windows.Forms.StatusStrip();
            this.tsslStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tspbProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxMissedCleavages)).BeginInit();
            this.pnlMain.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxPeaks)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMinimumAssumedPrecursorChargeState)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaximumAssumedPrecursorChargeState)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxThreads)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxPrecursorMonoPeakOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinPrecursorMonoPeakOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxVariableModIsoforms)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numProductMassTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPrecursorMassTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaximumFalseDiscoveryRatePercent)).BeginInit();
            this.ssStatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClear
            // 
            this.btnClear.Enabled = false;
            this.btnClear.Location = new System.Drawing.Point(625, 139);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lstData
            // 
            this.lstData.FormattingEnabled = true;
            this.lstData.HorizontalScrollbar = true;
            this.lstData.Location = new System.Drawing.Point(12, 25);
            this.lstData.Name = "lstData";
            this.lstData.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstData.Size = new System.Drawing.Size(688, 108);
            this.lstData.TabIndex = 1;
            this.lstData.SelectedIndexChanged += new System.EventHandler(this.lstData_SelectedIndexChanged);
            // 
            // btnRemove
            // 
            this.btnRemove.Enabled = false;
            this.btnRemove.Location = new System.Drawing.Point(318, 139);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 1;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Data";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(12, 139);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtFastaFile
            // 
            this.txtFastaFile.Location = new System.Drawing.Point(12, 309);
            this.txtFastaFile.Name = "txtFastaFile";
            this.txtFastaFile.Size = new System.Drawing.Size(607, 20);
            this.txtFastaFile.TabIndex = 4;
            this.txtFastaFile.TextChanged += new System.EventHandler(this.txtFastaFile_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 293);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(210, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Proteome Database (FASTA, UniProt XML)";
            // 
            // btnBrowseFasta
            // 
            this.btnBrowseFasta.Location = new System.Drawing.Point(625, 307);
            this.btnBrowseFasta.Name = "btnBrowseFasta";
            this.btnBrowseFasta.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseFasta.TabIndex = 5;
            this.btnBrowseFasta.Text = "Browse";
            this.btnBrowseFasta.UseVisualStyleBackColor = true;
            this.btnBrowseFasta.Click += new System.EventHandler(this.btnBrowseFasta_Click);
            // 
            // ofdFasta
            // 
            this.ofdFasta.Multiselect = true;
            // 
            // cboProtease
            // 
            this.cboProtease.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProtease.FormattingEnabled = true;
            this.cboProtease.Location = new System.Drawing.Point(12, 385);
            this.cboProtease.Name = "cboProtease";
            this.cboProtease.Size = new System.Drawing.Size(186, 21);
            this.cboProtease.TabIndex = 8;
            this.cboProtease.SelectedIndexChanged += new System.EventHandler(this.cboProtease_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 369);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Protease";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(219, 368);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(140, 13);
            this.label8.TabIndex = 9;
            this.label8.Text = "Maximum Missed Cleavages";
            // 
            // numMaxMissedCleavages
            // 
            this.numMaxMissedCleavages.Location = new System.Drawing.Point(222, 385);
            this.numMaxMissedCleavages.Name = "numMaxMissedCleavages";
            this.numMaxMissedCleavages.Size = new System.Drawing.Size(46, 20);
            this.numMaxMissedCleavages.TabIndex = 10;
            this.numMaxMissedCleavages.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numMaxMissedCleavages.ValueChanged += new System.EventHandler(this.ResetProgressBar);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(12, 697);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 42;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnBrowseOutput
            // 
            this.btnBrowseOutput.Location = new System.Drawing.Point(625, 669);
            this.btnBrowseOutput.Name = "btnBrowseOutput";
            this.btnBrowseOutput.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseOutput.TabIndex = 41;
            this.btnBrowseOutput.Text = "Browse";
            this.btnBrowseOutput.UseVisualStyleBackColor = true;
            this.btnBrowseOutput.Click += new System.EventHandler(this.btnBrowseOutput_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 655);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(71, 13);
            this.label9.TabIndex = 39;
            this.label9.Text = "Output Folder";
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.textBox1);
            this.pnlMain.Controls.Add(this.label22);
            this.pnlMain.Controls.Add(this.chkMinimizeMemoryUsage);
            this.pnlMain.Controls.Add(this.groupBox3);
            this.pnlMain.Controls.Add(this.groupBox2);
            this.pnlMain.Controls.Add(this.lstData);
            this.pnlMain.Controls.Add(this.label1);
            this.pnlMain.Controls.Add(this.groupBox1);
            this.pnlMain.Controls.Add(this.numMaxThreads);
            this.pnlMain.Controls.Add(this.label19);
            this.pnlMain.Controls.Add(this.chkPrecursorMonoisotopicPeakCorrection);
            this.pnlMain.Controls.Add(this.label20);
            this.pnlMain.Controls.Add(this.numMaxPrecursorMonoPeakOffset);
            this.pnlMain.Controls.Add(this.numMinPrecursorMonoPeakOffset);
            this.pnlMain.Controls.Add(this.chkOnTheFlyDecoys);
            this.pnlMain.Controls.Add(this.chkConsiderModifiedUnique);
            this.pnlMain.Controls.Add(this.numMaxVariableModIsoforms);
            this.pnlMain.Controls.Add(this.cboProductMassType);
            this.pnlMain.Controls.Add(this.label6);
            this.pnlMain.Controls.Add(this.numProductMassTolerance);
            this.pnlMain.Controls.Add(this.cboProductMassToleranceUnits);
            this.pnlMain.Controls.Add(this.label7);
            this.pnlMain.Controls.Add(this.cboPrecursorMassType);
            this.pnlMain.Controls.Add(this.label5);
            this.pnlMain.Controls.Add(this.numPrecursorMassTolerance);
            this.pnlMain.Controls.Add(this.cboPrecursorMassToleranceUnits);
            this.pnlMain.Controls.Add(this.label4);
            this.pnlMain.Controls.Add(this.label18);
            this.pnlMain.Controls.Add(this.label17);
            this.pnlMain.Controls.Add(this.numMaximumFalseDiscoveryRatePercent);
            this.pnlMain.Controls.Add(this.label16);
            this.pnlMain.Controls.Add(this.clbVariableModifications);
            this.pnlMain.Controls.Add(this.clbFixedModifications);
            this.pnlMain.Controls.Add(this.label12);
            this.pnlMain.Controls.Add(this.label11);
            this.pnlMain.Controls.Add(this.label10);
            this.pnlMain.Controls.Add(this.cboInitiatorMethionineBehavior);
            this.pnlMain.Controls.Add(this.btnBrowseOutput);
            this.pnlMain.Controls.Add(this.label9);
            this.pnlMain.Controls.Add(this.txtOutputFolder);
            this.pnlMain.Controls.Add(this.btnSearch);
            this.pnlMain.Controls.Add(this.numMaxMissedCleavages);
            this.pnlMain.Controls.Add(this.label8);
            this.pnlMain.Controls.Add(this.label3);
            this.pnlMain.Controls.Add(this.cboProtease);
            this.pnlMain.Controls.Add(this.btnBrowseFasta);
            this.pnlMain.Controls.Add(this.label2);
            this.pnlMain.Controls.Add(this.txtFastaFile);
            this.pnlMain.Controls.Add(this.btnClear);
            this.pnlMain.Controls.Add(this.btnRemove);
            this.pnlMain.Controls.Add(this.btnAdd);
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(712, 723);
            this.pnlMain.TabIndex = 2;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(441, 539);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(261, 20);
            this.textBox1.TabIndex = 53;
            this.textBox1.Text = "0";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(438, 523);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(112, 13);
            this.label22.TabIndex = 52;
            this.label22.Text = "Mass errors to accept:";
            // 
            // chkMinimizeMemoryUsage
            // 
            this.chkMinimizeMemoryUsage.AutoSize = true;
            this.chkMinimizeMemoryUsage.Location = new System.Drawing.Point(222, 623);
            this.chkMinimizeMemoryUsage.Name = "chkMinimizeMemoryUsage";
            this.chkMinimizeMemoryUsage.Size = new System.Drawing.Size(140, 17);
            this.chkMinimizeMemoryUsage.TabIndex = 49;
            this.chkMinimizeMemoryUsage.Text = "Minimize Memory Usage";
            this.chkMinimizeMemoryUsage.UseVisualStyleBackColor = true;
            this.chkMinimizeMemoryUsage.CheckedChanged += new System.EventHandler(this.ResetProgressBar);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkDeisotope);
            this.groupBox3.Controls.Add(this.chkAssignChargeStates);
            this.groupBox3.Location = new System.Drawing.Point(502, 177);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(198, 103);
            this.groupBox3.TabIndex = 48;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "MS/MS Analysis";
            // 
            // chkDeisotope
            // 
            this.chkDeisotope.AutoSize = true;
            this.chkDeisotope.Checked = true;
            this.chkDeisotope.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDeisotope.Location = new System.Drawing.Point(10, 65);
            this.chkDeisotope.Name = "chkDeisotope";
            this.chkDeisotope.Size = new System.Drawing.Size(77, 17);
            this.chkDeisotope.TabIndex = 1;
            this.chkDeisotope.Text = "De-isotope";
            this.chkDeisotope.UseVisualStyleBackColor = true;
            this.chkDeisotope.CheckedChanged += new System.EventHandler(this.ResetProgressBar);
            // 
            // chkAssignChargeStates
            // 
            this.chkAssignChargeStates.AutoSize = true;
            this.chkAssignChargeStates.Checked = true;
            this.chkAssignChargeStates.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAssignChargeStates.Location = new System.Drawing.Point(10, 33);
            this.chkAssignChargeStates.Name = "chkAssignChargeStates";
            this.chkAssignChargeStates.Size = new System.Drawing.Size(127, 17);
            this.chkAssignChargeStates.TabIndex = 0;
            this.chkAssignChargeStates.Text = "Assign Charge States";
            this.chkAssignChargeStates.UseVisualStyleBackColor = true;
            this.chkAssignChargeStates.CheckedChanged += new System.EventHandler(this.chkAssignChargeStates_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.txtRelativeThresholdPercent);
            this.groupBox2.Controls.Add(this.txtAbsoluteThreshold);
            this.groupBox2.Controls.Add(this.chkMaxNumPeaks);
            this.groupBox2.Controls.Add(this.numMaxPeaks);
            this.groupBox2.Controls.Add(this.chkRelativeThreshold);
            this.groupBox2.Controls.Add(this.chkAbsoluteThreshold);
            this.groupBox2.Location = new System.Drawing.Point(204, 177);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(292, 103);
            this.groupBox2.TabIndex = 47;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "MS/MS Peak Filtering";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Enabled = false;
            this.label15.Location = new System.Drawing.Point(236, 47);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(15, 13);
            this.label15.TabIndex = 4;
            this.label15.Text = "%";
            // 
            // txtRelativeThresholdPercent
            // 
            this.txtRelativeThresholdPercent.Enabled = false;
            this.txtRelativeThresholdPercent.Location = new System.Drawing.Point(171, 44);
            this.txtRelativeThresholdPercent.Name = "txtRelativeThresholdPercent";
            this.txtRelativeThresholdPercent.Size = new System.Drawing.Size(65, 20);
            this.txtRelativeThresholdPercent.TabIndex = 3;
            this.txtRelativeThresholdPercent.Text = "1";
            this.txtRelativeThresholdPercent.TextChanged += new System.EventHandler(this.ResetProgressBar);
            // 
            // txtAbsoluteThreshold
            // 
            this.txtAbsoluteThreshold.Enabled = false;
            this.txtAbsoluteThreshold.Location = new System.Drawing.Point(171, 18);
            this.txtAbsoluteThreshold.Name = "txtAbsoluteThreshold";
            this.txtAbsoluteThreshold.Size = new System.Drawing.Size(80, 20);
            this.txtAbsoluteThreshold.TabIndex = 1;
            this.txtAbsoluteThreshold.Text = "10";
            this.txtAbsoluteThreshold.TextChanged += new System.EventHandler(this.ResetProgressBar);
            // 
            // chkMaxNumPeaks
            // 
            this.chkMaxNumPeaks.AutoSize = true;
            this.chkMaxNumPeaks.Checked = true;
            this.chkMaxNumPeaks.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMaxNumPeaks.Location = new System.Drawing.Point(10, 74);
            this.chkMaxNumPeaks.Name = "chkMaxNumPeaks";
            this.chkMaxNumPeaks.Size = new System.Drawing.Size(155, 17);
            this.chkMaxNumPeaks.TabIndex = 5;
            this.chkMaxNumPeaks.Text = "Maximum Number of Peaks";
            this.chkMaxNumPeaks.UseVisualStyleBackColor = true;
            this.chkMaxNumPeaks.CheckedChanged += new System.EventHandler(this.chkMaxNumPeaks_CheckedChanged);
            // 
            // numMaxPeaks
            // 
            this.numMaxPeaks.Location = new System.Drawing.Point(171, 71);
            this.numMaxPeaks.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numMaxPeaks.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaxPeaks.Name = "numMaxPeaks";
            this.numMaxPeaks.Size = new System.Drawing.Size(65, 20);
            this.numMaxPeaks.TabIndex = 6;
            this.numMaxPeaks.Value = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.numMaxPeaks.ValueChanged += new System.EventHandler(this.ResetProgressBar);
            // 
            // chkRelativeThreshold
            // 
            this.chkRelativeThreshold.AutoSize = true;
            this.chkRelativeThreshold.Location = new System.Drawing.Point(10, 48);
            this.chkRelativeThreshold.Name = "chkRelativeThreshold";
            this.chkRelativeThreshold.Size = new System.Drawing.Size(115, 17);
            this.chkRelativeThreshold.TabIndex = 2;
            this.chkRelativeThreshold.Text = "Relative Threshold";
            this.chkRelativeThreshold.UseVisualStyleBackColor = true;
            this.chkRelativeThreshold.CheckedChanged += new System.EventHandler(this.chkRelativeThresholdPercent_CheckedChanged);
            // 
            // chkAbsoluteThreshold
            // 
            this.chkAbsoluteThreshold.AutoSize = true;
            this.chkAbsoluteThreshold.Location = new System.Drawing.Point(10, 22);
            this.chkAbsoluteThreshold.Name = "chkAbsoluteThreshold";
            this.chkAbsoluteThreshold.Size = new System.Drawing.Size(117, 17);
            this.chkAbsoluteThreshold.TabIndex = 0;
            this.chkAbsoluteThreshold.Text = "Absolute Threshold";
            this.chkAbsoluteThreshold.UseVisualStyleBackColor = true;
            this.chkAbsoluteThreshold.CheckedChanged += new System.EventHandler(this.chkAbsoluteThreshold_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.numMinimumAssumedPrecursorChargeState);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.numMaximumAssumedPrecursorChargeState);
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Location = new System.Drawing.Point(12, 177);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(186, 103);
            this.groupBox1.TabIndex = 46;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Assumed Precursor Charge States";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 67);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(51, 13);
            this.label13.TabIndex = 2;
            this.label13.Text = "Maximum";
            // 
            // numMinimumAssumedPrecursorChargeState
            // 
            this.numMinimumAssumedPrecursorChargeState.Location = new System.Drawing.Point(63, 35);
            this.numMinimumAssumedPrecursorChargeState.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numMinimumAssumedPrecursorChargeState.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMinimumAssumedPrecursorChargeState.Name = "numMinimumAssumedPrecursorChargeState";
            this.numMinimumAssumedPrecursorChargeState.Size = new System.Drawing.Size(52, 20);
            this.numMinimumAssumedPrecursorChargeState.TabIndex = 1;
            this.numMinimumAssumedPrecursorChargeState.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numMinimumAssumedPrecursorChargeState.ValueChanged += new System.EventHandler(this.ResetProgressBar);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 37);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(48, 13);
            this.label14.TabIndex = 0;
            this.label14.Text = "Minimum";
            // 
            // numMaximumAssumedPrecursorChargeState
            // 
            this.numMaximumAssumedPrecursorChargeState.Location = new System.Drawing.Point(63, 65);
            this.numMaximumAssumedPrecursorChargeState.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numMaximumAssumedPrecursorChargeState.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaximumAssumedPrecursorChargeState.Name = "numMaximumAssumedPrecursorChargeState";
            this.numMaximumAssumedPrecursorChargeState.Size = new System.Drawing.Size(52, 20);
            this.numMaximumAssumedPrecursorChargeState.TabIndex = 3;
            this.numMaximumAssumedPrecursorChargeState.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numMaximumAssumedPrecursorChargeState.ValueChanged += new System.EventHandler(this.ResetProgressBar);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(6, 13);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(88, 13);
            this.label21.TabIndex = 50;
            this.label21.Text = "(Unknowns Only)";
            // 
            // numMaxThreads
            // 
            this.numMaxThreads.Location = new System.Drawing.Point(108, 622);
            this.numMaxThreads.Name = "numMaxThreads";
            this.numMaxThreads.Size = new System.Drawing.Size(51, 20);
            this.numMaxThreads.TabIndex = 44;
            this.numMaxThreads.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaxThreads.ValueChanged += new System.EventHandler(this.ResetProgressBar);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(9, 624);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(93, 13);
            this.label19.TabIndex = 43;
            this.label19.Text = "Maximum Threads";
            // 
            // chkPrecursorMonoisotopicPeakCorrection
            // 
            this.chkPrecursorMonoisotopicPeakCorrection.AutoSize = true;
            this.chkPrecursorMonoisotopicPeakCorrection.Location = new System.Drawing.Point(441, 471);
            this.chkPrecursorMonoisotopicPeakCorrection.Name = "chkPrecursorMonoisotopicPeakCorrection";
            this.chkPrecursorMonoisotopicPeakCorrection.Size = new System.Drawing.Size(216, 17);
            this.chkPrecursorMonoisotopicPeakCorrection.TabIndex = 26;
            this.chkPrecursorMonoisotopicPeakCorrection.Text = "Precursor Monoisotopic Peak Correction";
            this.chkPrecursorMonoisotopicPeakCorrection.UseVisualStyleBackColor = true;
            this.chkPrecursorMonoisotopicPeakCorrection.CheckedChanged += new System.EventHandler(this.chkMonoisotopicPeakCorrection_CheckedChanged);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Enabled = false;
            this.label20.Location = new System.Drawing.Point(500, 496);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(16, 13);
            this.label20.TabIndex = 28;
            this.label20.Text = "to";
            // 
            // numMaxPrecursorMonoPeakOffset
            // 
            this.numMaxPrecursorMonoPeakOffset.Enabled = false;
            this.numMaxPrecursorMonoPeakOffset.Location = new System.Drawing.Point(522, 494);
            this.numMaxPrecursorMonoPeakOffset.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numMaxPrecursorMonoPeakOffset.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.numMaxPrecursorMonoPeakOffset.Name = "numMaxPrecursorMonoPeakOffset";
            this.numMaxPrecursorMonoPeakOffset.Size = new System.Drawing.Size(54, 20);
            this.numMaxPrecursorMonoPeakOffset.TabIndex = 29;
            this.numMaxPrecursorMonoPeakOffset.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaxPrecursorMonoPeakOffset.ValueChanged += new System.EventHandler(this.ResetProgressBar);
            // 
            // numMinPrecursorMonoPeakOffset
            // 
            this.numMinPrecursorMonoPeakOffset.Enabled = false;
            this.numMinPrecursorMonoPeakOffset.Location = new System.Drawing.Point(440, 494);
            this.numMinPrecursorMonoPeakOffset.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numMinPrecursorMonoPeakOffset.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.numMinPrecursorMonoPeakOffset.Name = "numMinPrecursorMonoPeakOffset";
            this.numMinPrecursorMonoPeakOffset.Size = new System.Drawing.Size(54, 20);
            this.numMinPrecursorMonoPeakOffset.TabIndex = 27;
            this.numMinPrecursorMonoPeakOffset.Value = new decimal(new int[] {
            3,
            0,
            0,
            -2147483648});
            this.numMinPrecursorMonoPeakOffset.ValueChanged += new System.EventHandler(this.ResetProgressBar);
            // 
            // chkOnTheFlyDecoys
            // 
            this.chkOnTheFlyDecoys.AutoSize = true;
            this.chkOnTheFlyDecoys.Location = new System.Drawing.Point(12, 335);
            this.chkOnTheFlyDecoys.Name = "chkOnTheFlyDecoys";
            this.chkOnTheFlyDecoys.Size = new System.Drawing.Size(231, 17);
            this.chkOnTheFlyDecoys.TabIndex = 6;
            this.chkOnTheFlyDecoys.Text = "Create Target–Decoy Database On The Fly";
            this.chkOnTheFlyDecoys.UseVisualStyleBackColor = true;
            this.chkOnTheFlyDecoys.CheckedChanged += new System.EventHandler(this.ResetProgressBar);
            // 
            // chkConsiderModifiedUnique
            // 
            this.chkConsiderModifiedUnique.AutoSize = true;
            this.chkConsiderModifiedUnique.Location = new System.Drawing.Point(560, 629);
            this.chkConsiderModifiedUnique.Name = "chkConsiderModifiedUnique";
            this.chkConsiderModifiedUnique.Size = new System.Drawing.Size(141, 30);
            this.chkConsiderModifiedUnique.TabIndex = 38;
            this.chkConsiderModifiedUnique.Text = "Consider Modified Forms\r\nas Unique Peptides";
            this.chkConsiderModifiedUnique.UseVisualStyleBackColor = true;
            this.chkConsiderModifiedUnique.CheckedChanged += new System.EventHandler(this.ResetProgressBar);
            // 
            // numMaxVariableModIsoforms
            // 
            this.numMaxVariableModIsoforms.Location = new System.Drawing.Point(266, 589);
            this.numMaxVariableModIsoforms.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numMaxVariableModIsoforms.Name = "numMaxVariableModIsoforms";
            this.numMaxVariableModIsoforms.Size = new System.Drawing.Size(82, 20);
            this.numMaxVariableModIsoforms.TabIndex = 18;
            this.numMaxVariableModIsoforms.Value = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.numMaxVariableModIsoforms.ValueChanged += new System.EventHandler(this.ResetProgressBar);
            // 
            // cboProductMassType
            // 
            this.cboProductMassType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProductMassType.FormattingEnabled = true;
            this.cboProductMassType.Location = new System.Drawing.Point(595, 583);
            this.cboProductMassType.Name = "cboProductMassType";
            this.cboProductMassType.Size = new System.Drawing.Size(106, 21);
            this.cboProductMassType.TabIndex = 34;
            this.cboProductMassType.SelectedIndexChanged += new System.EventHandler(this.ResetProgressBar);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(437, 586);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(13, 13);
            this.label6.TabIndex = 31;
            this.label6.Text = "±";
            // 
            // numProductMassTolerance
            // 
            this.numProductMassTolerance.DecimalPlaces = 3;
            this.numProductMassTolerance.Location = new System.Drawing.Point(450, 584);
            this.numProductMassTolerance.Name = "numProductMassTolerance";
            this.numProductMassTolerance.Size = new System.Drawing.Size(74, 20);
            this.numProductMassTolerance.TabIndex = 32;
            this.numProductMassTolerance.Value = new decimal(new int[] {
            25,
            0,
            0,
            196608});
            this.numProductMassTolerance.ValueChanged += new System.EventHandler(this.ResetProgressBar);
            // 
            // cboProductMassToleranceUnits
            // 
            this.cboProductMassToleranceUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProductMassToleranceUnits.FormattingEnabled = true;
            this.cboProductMassToleranceUnits.Location = new System.Drawing.Point(530, 583);
            this.cboProductMassToleranceUnits.Name = "cboProductMassToleranceUnits";
            this.cboProductMassToleranceUnits.Size = new System.Drawing.Size(59, 21);
            this.cboProductMassToleranceUnits.TabIndex = 33;
            this.cboProductMassToleranceUnits.SelectedIndexChanged += new System.EventHandler(this.ResetProgressBar);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(437, 568);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(123, 13);
            this.label7.TabIndex = 30;
            this.label7.Text = "Product Mass Tolerance";
            // 
            // cboPrecursorMassType
            // 
            this.cboPrecursorMassType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPrecursorMassType.FormattingEnabled = true;
            this.cboPrecursorMassType.Location = new System.Drawing.Point(595, 438);
            this.cboPrecursorMassType.Name = "cboPrecursorMassType";
            this.cboPrecursorMassType.Size = new System.Drawing.Size(106, 21);
            this.cboPrecursorMassType.TabIndex = 25;
            this.cboPrecursorMassType.SelectedIndexChanged += new System.EventHandler(this.cboPrecursorMassType_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(437, 441);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(13, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "±";
            // 
            // numPrecursorMassTolerance
            // 
            this.numPrecursorMassTolerance.DecimalPlaces = 3;
            this.numPrecursorMassTolerance.Location = new System.Drawing.Point(450, 439);
            this.numPrecursorMassTolerance.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numPrecursorMassTolerance.Name = "numPrecursorMassTolerance";
            this.numPrecursorMassTolerance.Size = new System.Drawing.Size(74, 20);
            this.numPrecursorMassTolerance.TabIndex = 23;
            this.numPrecursorMassTolerance.Value = new decimal(new int[] {
            21,
            0,
            0,
            65536});
            this.numPrecursorMassTolerance.ValueChanged += new System.EventHandler(this.ResetProgressBar);
            // 
            // cboPrecursorMassToleranceUnits
            // 
            this.cboPrecursorMassToleranceUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPrecursorMassToleranceUnits.FormattingEnabled = true;
            this.cboPrecursorMassToleranceUnits.Location = new System.Drawing.Point(530, 438);
            this.cboPrecursorMassToleranceUnits.Name = "cboPrecursorMassToleranceUnits";
            this.cboPrecursorMassToleranceUnits.Size = new System.Drawing.Size(59, 21);
            this.cboPrecursorMassToleranceUnits.TabIndex = 24;
            this.cboPrecursorMassToleranceUnits.SelectedIndexChanged += new System.EventHandler(this.ResetProgressBar);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(437, 423);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(131, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Precursor Mass Tolerance";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(9, 591);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(251, 13);
            this.label18.TabIndex = 17;
            this.label18.Text = "Maximum Variable Modification Isoforms per Peptide";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(494, 641);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(15, 13);
            this.label17.TabIndex = 37;
            this.label17.Text = "%";
            // 
            // numMaximumFalseDiscoveryRatePercent
            // 
            this.numMaximumFalseDiscoveryRatePercent.DecimalPlaces = 2;
            this.numMaximumFalseDiscoveryRatePercent.Location = new System.Drawing.Point(440, 639);
            this.numMaximumFalseDiscoveryRatePercent.Name = "numMaximumFalseDiscoveryRatePercent";
            this.numMaximumFalseDiscoveryRatePercent.Size = new System.Drawing.Size(54, 20);
            this.numMaximumFalseDiscoveryRatePercent.TabIndex = 36;
            this.numMaximumFalseDiscoveryRatePercent.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMaximumFalseDiscoveryRatePercent.ValueChanged += new System.EventHandler(this.ResetProgressBar);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(438, 623);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(76, 13);
            this.label16.TabIndex = 35;
            this.label16.Text = "Maximum FDR";
            // 
            // clbVariableModifications
            // 
            this.clbVariableModifications.CheckOnClick = true;
            this.clbVariableModifications.FormattingEnabled = true;
            this.clbVariableModifications.HorizontalScrollbar = true;
            this.clbVariableModifications.IntegralHeight = false;
            this.clbVariableModifications.Location = new System.Drawing.Point(222, 439);
            this.clbVariableModifications.Name = "clbVariableModifications";
            this.clbVariableModifications.Size = new System.Drawing.Size(204, 141);
            this.clbVariableModifications.TabIndex = 16;
            this.clbVariableModifications.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ResetProgressBar);
            // 
            // clbFixedModifications
            // 
            this.clbFixedModifications.CheckOnClick = true;
            this.clbFixedModifications.FormattingEnabled = true;
            this.clbFixedModifications.HorizontalScrollbar = true;
            this.clbFixedModifications.IntegralHeight = false;
            this.clbFixedModifications.Location = new System.Drawing.Point(12, 439);
            this.clbFixedModifications.Name = "clbFixedModifications";
            this.clbFixedModifications.Size = new System.Drawing.Size(204, 141);
            this.clbFixedModifications.TabIndex = 14;
            this.clbFixedModifications.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ResetProgressBar);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(219, 423);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(110, 13);
            this.label12.TabIndex = 15;
            this.label12.Text = "Variable Modifications";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(9, 423);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(97, 13);
            this.label11.TabIndex = 13;
            this.label11.Text = "Fixed Modifications";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(437, 369);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(141, 13);
            this.label10.TabIndex = 11;
            this.label10.Text = "Initiator Methionine Behavior";
            // 
            // cboInitiatorMethionineBehavior
            // 
            this.cboInitiatorMethionineBehavior.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboInitiatorMethionineBehavior.FormattingEnabled = true;
            this.cboInitiatorMethionineBehavior.Location = new System.Drawing.Point(440, 385);
            this.cboInitiatorMethionineBehavior.Name = "cboInitiatorMethionineBehavior";
            this.cboInitiatorMethionineBehavior.Size = new System.Drawing.Size(138, 21);
            this.cboInitiatorMethionineBehavior.TabIndex = 12;
            this.cboInitiatorMethionineBehavior.SelectedIndexChanged += new System.EventHandler(this.ResetProgressBar);
            // 
            // txtOutputFolder
            // 
            this.txtOutputFolder.Location = new System.Drawing.Point(12, 671);
            this.txtOutputFolder.Name = "txtOutputFolder";
            this.txtOutputFolder.Size = new System.Drawing.Size(607, 20);
            this.txtOutputFolder.TabIndex = 40;
            this.txtOutputFolder.TextChanged += new System.EventHandler(this.ResetProgressBar);
            // 
            // ofdData
            // 
            this.ofdData.Multiselect = true;
            // 
            // fbdData
            // 
            this.fbdData.ShowNewFolderButton = false;
            // 
            // BottomToolStripPanel
            // 
            this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // TopToolStripPanel
            // 
            this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // RightToolStripPanel
            // 
            this.RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // LeftToolStripPanel
            // 
            this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // ContentPanel
            // 
            this.ContentPanel.Size = new System.Drawing.Size(150, 150);
            // 
            // ssStatusStrip
            // 
            this.ssStatusStrip.AutoSize = false;
            this.ssStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslStatus,
            this.tspbProgress});
            this.ssStatusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.ssStatusStrip.Location = new System.Drawing.Point(0, 726);
            this.ssStatusStrip.Name = "ssStatusStrip";
            this.ssStatusStrip.Size = new System.Drawing.Size(712, 22);
            this.ssStatusStrip.SizingGrip = false;
            this.ssStatusStrip.TabIndex = 3;
            this.ssStatusStrip.Text = "statusStrip1";
            // 
            // tsslStatus
            // 
            this.tsslStatus.Name = "tsslStatus";
            this.tsslStatus.Size = new System.Drawing.Size(39, 17);
            this.tsslStatus.Text = "Ready";
            // 
            // tspbProgress
            // 
            this.tspbProgress.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tspbProgress.Name = "tspbProgress";
            this.tspbProgress.Size = new System.Drawing.Size(400, 16);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // frmMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(712, 748);
            this.Controls.Add(this.ssStatusStrip);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "Morpheus";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.frmMain_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.frmMain_DragEnter);
            ((System.ComponentModel.ISupportInitialize)(this.numMaxMissedCleavages)).EndInit();
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxPeaks)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMinimumAssumedPrecursorChargeState)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaximumAssumedPrecursorChargeState)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxThreads)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxPrecursorMonoPeakOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinPrecursorMonoPeakOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxVariableModIsoforms)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numProductMassTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPrecursorMassTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaximumFalseDiscoveryRatePercent)).EndInit();
            this.ssStatusStrip.ResumeLayout(false);
            this.ssStatusStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.ListBox lstData;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TextBox txtFastaFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnBrowseFasta;
        private System.Windows.Forms.OpenFileDialog ofdFasta;
        private System.Windows.Forms.ComboBox cboProtease;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown numMaxMissedCleavages;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnBrowseOutput;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.FolderBrowserDialog fbdOutput;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cboInitiatorMethionineBehavior;
        private System.Windows.Forms.CheckedListBox clbVariableModifications;
        private System.Windows.Forms.CheckedListBox clbFixedModifications;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.NumericUpDown numMaximumFalseDiscoveryRatePercent;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtOutputFolder;
        private System.Windows.Forms.OpenFileDialog ofdData;
        private System.Windows.Forms.FolderBrowserDialog fbdData;
        private System.Windows.Forms.ComboBox cboProductMassType;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numProductMassTolerance;
        private System.Windows.Forms.ComboBox cboProductMassToleranceUnits;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cboPrecursorMassType;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numPrecursorMassTolerance;
        private System.Windows.Forms.ComboBox cboPrecursorMassToleranceUnits;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripPanel BottomToolStripPanel;
        private System.Windows.Forms.ToolStripPanel TopToolStripPanel;
        private System.Windows.Forms.ToolStripPanel RightToolStripPanel;
        private System.Windows.Forms.ToolStripPanel LeftToolStripPanel;
        private System.Windows.Forms.ToolStripContentPanel ContentPanel;
        private System.Windows.Forms.StatusStrip ssStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel tsslStatus;
        private System.Windows.Forms.ToolStripProgressBar tspbProgress;
        private System.Windows.Forms.NumericUpDown numMaxVariableModIsoforms;
        private System.Windows.Forms.CheckBox chkConsiderModifiedUnique;
        private System.Windows.Forms.CheckBox chkOnTheFlyDecoys;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.NumericUpDown numMaxPrecursorMonoPeakOffset;
        private System.Windows.Forms.NumericUpDown numMinPrecursorMonoPeakOffset;
        private System.Windows.Forms.CheckBox chkPrecursorMonoisotopicPeakCorrection;
        private System.Windows.Forms.NumericUpDown numMaxThreads;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkDeisotope;
        private System.Windows.Forms.CheckBox chkAssignChargeStates;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtRelativeThresholdPercent;
        private System.Windows.Forms.TextBox txtAbsoluteThreshold;
        private System.Windows.Forms.CheckBox chkMaxNumPeaks;
        private System.Windows.Forms.NumericUpDown numMaxPeaks;
        private System.Windows.Forms.CheckBox chkRelativeThreshold;
        private System.Windows.Forms.CheckBox chkAbsoluteThreshold;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.NumericUpDown numMinimumAssumedPrecursorChargeState;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.NumericUpDown numMaximumAssumedPrecursorChargeState;
        private System.Windows.Forms.CheckBox chkMinimizeMemoryUsage;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label22;
    }
}