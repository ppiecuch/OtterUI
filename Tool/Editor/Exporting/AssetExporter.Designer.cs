namespace Otter.Editor.Exporting
{
    partial class AssetExporter
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
            this.mExportButton = new System.Windows.Forms.Button();
            this.mPlatformsListBox = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.mFilesListBox = new System.Windows.Forms.CheckedListBox();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // mExportButton
            // 
            this.mExportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mExportButton.Location = new System.Drawing.Point(572, 523);
            this.mExportButton.Name = "mExportButton";
            this.mExportButton.Size = new System.Drawing.Size(75, 23);
            this.mExportButton.TabIndex = 0;
            this.mExportButton.Text = "Export";
            this.mExportButton.UseVisualStyleBackColor = true;
            this.mExportButton.Click += new System.EventHandler(this.mExportButton_Click);
            // 
            // mPlatformsListBox
            // 
            this.mPlatformsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mPlatformsListBox.FormattingEnabled = true;
            this.mPlatformsListBox.IntegralHeight = false;
            this.mPlatformsListBox.Location = new System.Drawing.Point(6, 19);
            this.mPlatformsListBox.Name = "mPlatformsListBox";
            this.mPlatformsListBox.Size = new System.Drawing.Size(156, 478);
            this.mPlatformsListBox.TabIndex = 1;
            this.mPlatformsListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.mPlatformsListBox_ItemCheck);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(13, 518);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(715, 2);
            this.label1.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.mPlatformsListBox);
            this.groupBox1.Location = new System.Drawing.Point(13, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(171, 503);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Target Platforms";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.mFilesListBox);
            this.groupBox2.Location = new System.Drawing.Point(191, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(537, 502);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Files to Export";
            // 
            // mFilesListBox
            // 
            this.mFilesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mFilesListBox.FormattingEnabled = true;
            this.mFilesListBox.IntegralHeight = false;
            this.mFilesListBox.Location = new System.Drawing.Point(6, 18);
            this.mFilesListBox.Name = "mFilesListBox";
            this.mFilesListBox.Size = new System.Drawing.Size(524, 478);
            this.mFilesListBox.TabIndex = 0;
            // 
            // mCancelButton
            // 
            this.mCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mCancelButton.Location = new System.Drawing.Point(653, 523);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(75, 23);
            this.mCancelButton.TabIndex = 5;
            this.mCancelButton.Text = "Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            this.mCancelButton.Click += new System.EventHandler(this.mCancelButton_Click);
            // 
            // AssetExporter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 558);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mExportButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AssetExporter";
            this.Text = "Asset Exporter";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mExportButton;
        private System.Windows.Forms.CheckedListBox mPlatformsListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.CheckedListBox mFilesListBox;
    }
}