namespace DXFRendering
{
    partial class Form1
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
            if (disposing && (components != null))
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxScaleFactor = new System.Windows.Forms.TextBox();
            this.labelScaleFactor = new System.Windows.Forms.Label();
            this.FolderBtn = new System.Windows.Forms.Button();
            this.listBoxDxfFiles = new System.Windows.Forms.ListBox();
            this.textBoxFolderPath = new System.Windows.Forms.TextBox();
            this.userControlForPaint1 = new DXFRendering.UserControlForPaint();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.textBoxScaleFactor);
            this.groupBox1.Controls.Add(this.labelScaleFactor);
            this.groupBox1.Controls.Add(this.FolderBtn);
            this.groupBox1.Controls.Add(this.listBoxDxfFiles);
            this.groupBox1.Controls.Add(this.textBoxFolderPath);
            this.groupBox1.Location = new System.Drawing.Point(1, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(259, 448);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "All good things";
            // 
            // textBoxScaleFactor
            // 
            this.textBoxScaleFactor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxScaleFactor.Location = new System.Drawing.Point(66, 411);
            this.textBoxScaleFactor.Name = "textBoxScaleFactor";
            this.textBoxScaleFactor.Size = new System.Drawing.Size(53, 20);
            this.textBoxScaleFactor.TabIndex = 5;
            this.textBoxScaleFactor.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxScaleFactor_Validating);
            // 
            // labelScaleFactor
            // 
            this.labelScaleFactor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelScaleFactor.AutoSize = true;
            this.labelScaleFactor.Location = new System.Drawing.Point(0, 415);
            this.labelScaleFactor.Name = "labelScaleFactor";
            this.labelScaleFactor.Size = new System.Drawing.Size(64, 13);
            this.labelScaleFactor.TabIndex = 4;
            this.labelScaleFactor.Text = "Scale factor";
            // 
            // FolderBtn
            // 
            this.FolderBtn.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FolderBtn.Location = new System.Drawing.Point(207, 11);
            this.FolderBtn.Name = "FolderBtn";
            this.FolderBtn.Size = new System.Drawing.Size(46, 23);
            this.FolderBtn.TabIndex = 3;
            this.FolderBtn.Text = "Folder";
            this.FolderBtn.UseVisualStyleBackColor = true;
            this.FolderBtn.Click += new System.EventHandler(this.FolderBtn_Click);
            // 
            // listBoxDxfFiles
            // 
            this.listBoxDxfFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxDxfFiles.FormattingEnabled = true;
            this.listBoxDxfFiles.Location = new System.Drawing.Point(0, 40);
            this.listBoxDxfFiles.Name = "listBoxDxfFiles";
            this.listBoxDxfFiles.Size = new System.Drawing.Size(253, 368);
            this.listBoxDxfFiles.TabIndex = 1;
            this.listBoxDxfFiles.SelectedValueChanged += new System.EventHandler(this.ListBoxDxfFiles_SelectedValueChanged);
            // 
            // textBoxFolderPath
            // 
            this.textBoxFolderPath.Location = new System.Drawing.Point(0, 14);
            this.textBoxFolderPath.Name = "textBoxFolderPath";
            this.textBoxFolderPath.Size = new System.Drawing.Size(201, 20);
            this.textBoxFolderPath.TabIndex = 0;
            this.textBoxFolderPath.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBoxFolderPath_KeyUp);
            // 
            // userControlForPaint1
            // 
            this.userControlForPaint1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.userControlForPaint1.AutoScroll = true;
            this.userControlForPaint1.BackColor = System.Drawing.Color.White;
            this.userControlForPaint1.internalScaleFactor = 0D;
            this.userControlForPaint1.Location = new System.Drawing.Point(266, 1);
            this.userControlForPaint1.Name = "userControlForPaint1";
            this.userControlForPaint1.Size = new System.Drawing.Size(531, 437);
            this.userControlForPaint1.TabIndex = 0;
            this.userControlForPaint1.internalScaleFactorChangedInternally += new DXFRendering.internalScaleFactorChangedInternallyCallbackDelegate(this.UserControlForPaint1_internalScaleFactorChangedInternally);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.userControlForPaint1);
            this.Name = "Form1";
            this.Text = "DemonstrationForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private UserControlForPaint userControlForPaint1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxFolderPath;
        private System.Windows.Forms.ListBox listBoxDxfFiles;
        private System.Windows.Forms.Button FolderBtn;
        private System.Windows.Forms.TextBox textBoxScaleFactor;
        private System.Windows.Forms.Label labelScaleFactor;
    }
}

