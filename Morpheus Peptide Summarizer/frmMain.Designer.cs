namespace Morpheus_Peptide_Summarizer
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
            this.lstMorpheusPsmTsv = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.txtOutputFolder = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.ofdMorpheusPsmTsv = new System.Windows.Forms.OpenFileDialog();
            this.fbdOutput = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // lstMorpheusPsmTsv
            // 
            this.lstMorpheusPsmTsv.FormattingEnabled = true;
            this.lstMorpheusPsmTsv.HorizontalScrollbar = true;
            this.lstMorpheusPsmTsv.Location = new System.Drawing.Point(12, 25);
            this.lstMorpheusPsmTsv.Name = "lstMorpheusPsmTsv";
            this.lstMorpheusPsmTsv.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstMorpheusPsmTsv.Size = new System.Drawing.Size(770, 147);
            this.lstMorpheusPsmTsv.TabIndex = 56;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(210, 13);
            this.label1.TabIndex = 55;
            this.label1.Text = "Morpheus PSM or unique peptide TSV files";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(707, 178);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 58;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(359, 178);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 57;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(12, 178);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 54;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(707, 233);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 64;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 220);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(71, 13);
            this.label9.TabIndex = 62;
            this.label9.Text = "Output Folder";
            // 
            // txtOutputFolder
            // 
            this.txtOutputFolder.Location = new System.Drawing.Point(12, 236);
            this.txtOutputFolder.Name = "txtOutputFolder";
            this.txtOutputFolder.Size = new System.Drawing.Size(689, 20);
            this.txtOutputFolder.TabIndex = 63;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(12, 271);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 65;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // ofdMorpheusPsmTsv
            // 
            this.ofdMorpheusPsmTsv.Filter = "Morpheus PSM or unique peptide TSV files|*.tsv";
            this.ofdMorpheusPsmTsv.Multiselect = true;
            // 
            // frmMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 307);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtOutputFolder);
            this.Controls.Add(this.lstMorpheusPsmTsv);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.Text = "Morpheus Peptide Summarizer";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.frmMain_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.frmMain_DragEnter);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstMorpheusPsmTsv;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtOutputFolder;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.OpenFileDialog ofdMorpheusPsmTsv;
        private System.Windows.Forms.FolderBrowserDialog fbdOutput;
    }
}

