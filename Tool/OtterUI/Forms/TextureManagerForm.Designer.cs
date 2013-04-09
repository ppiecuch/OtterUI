namespace Otter.Forms
{
    partial class TextureManagerForm
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
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Group 1", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Test Item1");
            System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Test Item 2");
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mRefreshButton = new System.Windows.Forms.Button();
            this.mRemoveButton = new System.Windows.Forms.Button();
            this.mTextureList = new Otter.CustomControls.TextureList();
            this.mAddButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.mCloseButton = new System.Windows.Forms.Button();
            this.mDetailsGroupBox = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.mFilenameTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.mSizeTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.mHeightTextBox = new System.Windows.Forms.TextBox();
            this.mWidthTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.mReferencesListView = new System.Windows.Forms.ListView();
            this.mAtlasCheckbox = new System.Windows.Forms.CheckBox();
            this.mOKButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.mOverrideCheckbox = new System.Windows.Forms.CheckBox();
            this.mOverrideNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.mPaddingTypeLabel = new System.Windows.Forms.Label();
            this.mPaddingTypeComboBox = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.mDetailsGroupBox.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mOverrideNumericUpDown)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.mRefreshButton);
            this.groupBox1.Controls.Add(this.mRemoveButton);
            this.groupBox1.Controls.Add(this.mTextureList);
            this.groupBox1.Controls.Add(this.mAddButton);
            this.groupBox1.Location = new System.Drawing.Point(12, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(270, 533);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Texture List";
            // 
            // mRefreshButton
            // 
            this.mRefreshButton.Location = new System.Drawing.Point(189, 19);
            this.mRefreshButton.Name = "mRefreshButton";
            this.mRefreshButton.Size = new System.Drawing.Size(75, 23);
            this.mRefreshButton.TabIndex = 3;
            this.mRefreshButton.Text = "Refresh";
            this.mRefreshButton.UseVisualStyleBackColor = true;
            this.mRefreshButton.Click += new System.EventHandler(this.mRefreshButton_Click);
            // 
            // mRemoveButton
            // 
            this.mRemoveButton.Enabled = false;
            this.mRemoveButton.Location = new System.Drawing.Point(87, 19);
            this.mRemoveButton.Name = "mRemoveButton";
            this.mRemoveButton.Size = new System.Drawing.Size(75, 23);
            this.mRemoveButton.TabIndex = 2;
            this.mRemoveButton.Text = "Remove";
            this.mRemoveButton.UseVisualStyleBackColor = true;
            this.mRemoveButton.Click += new System.EventHandler(this.mRemoveButton_Click);
            // 
            // mTextureList
            // 
            this.mTextureList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mTextureList.BackColor = System.Drawing.Color.White;
            this.mTextureList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mTextureList.Location = new System.Drawing.Point(7, 48);
            this.mTextureList.Name = "mTextureList";
            this.mTextureList.Size = new System.Drawing.Size(257, 479);
            this.mTextureList.TabIndex = 0;
            this.mTextureList.SelectedTextureChanged += new Otter.CustomControls.TextureList.TextureInfoHandler(this.mTextureList_SelectedTextureChanged);
            // 
            // mAddButton
            // 
            this.mAddButton.Location = new System.Drawing.Point(6, 19);
            this.mAddButton.Name = "mAddButton";
            this.mAddButton.Size = new System.Drawing.Size(75, 23);
            this.mAddButton.TabIndex = 1;
            this.mAddButton.Text = "Add";
            this.mAddButton.UseVisualStyleBackColor = true;
            this.mAddButton.Click += new System.EventHandler(this.mAddButton_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(12, 549);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(648, 2);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // mCloseButton
            // 
            this.mCloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mCloseButton.Location = new System.Drawing.Point(585, 562);
            this.mCloseButton.Name = "mCloseButton";
            this.mCloseButton.Size = new System.Drawing.Size(75, 23);
            this.mCloseButton.TabIndex = 2;
            this.mCloseButton.Text = "Close";
            this.mCloseButton.UseVisualStyleBackColor = true;
            this.mCloseButton.Click += new System.EventHandler(this.mCloseButton_Click);
            // 
            // mDetailsGroupBox
            // 
            this.mDetailsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mDetailsGroupBox.Controls.Add(this.button1);
            this.mDetailsGroupBox.Controls.Add(this.mFilenameTextBox);
            this.mDetailsGroupBox.Controls.Add(this.label9);
            this.mDetailsGroupBox.Controls.Add(this.mSizeTextBox);
            this.mDetailsGroupBox.Controls.Add(this.label6);
            this.mDetailsGroupBox.Controls.Add(this.mHeightTextBox);
            this.mDetailsGroupBox.Controls.Add(this.mWidthTextBox);
            this.mDetailsGroupBox.Controls.Add(this.label4);
            this.mDetailsGroupBox.Controls.Add(this.label3);
            this.mDetailsGroupBox.Enabled = false;
            this.mDetailsGroupBox.Location = new System.Drawing.Point(282, 13);
            this.mDetailsGroupBox.Name = "mDetailsGroupBox";
            this.mDetailsGroupBox.Size = new System.Drawing.Size(378, 127);
            this.mDetailsGroupBox.TabIndex = 3;
            this.mDetailsGroupBox.TabStop = false;
            this.mDetailsGroupBox.Text = "Details";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(293, 15);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 14;
            this.button1.Text = "Browse";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // mFilenameTextBox
            // 
            this.mFilenameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mFilenameTextBox.Location = new System.Drawing.Point(63, 17);
            this.mFilenameTextBox.Name = "mFilenameTextBox";
            this.mFilenameTextBox.ReadOnly = true;
            this.mFilenameTextBox.Size = new System.Drawing.Size(224, 20);
            this.mFilenameTextBox.TabIndex = 13;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 20);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 13);
            this.label9.TabIndex = 12;
            this.label9.Text = "Filename:";
            // 
            // mSizeTextBox
            // 
            this.mSizeTextBox.Location = new System.Drawing.Point(63, 99);
            this.mSizeTextBox.Name = "mSizeTextBox";
            this.mSizeTextBox.ReadOnly = true;
            this.mSizeTextBox.Size = new System.Drawing.Size(108, 20);
            this.mSizeTextBox.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 102);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Size:";
            // 
            // mHeightTextBox
            // 
            this.mHeightTextBox.Location = new System.Drawing.Point(63, 72);
            this.mHeightTextBox.Name = "mHeightTextBox";
            this.mHeightTextBox.ReadOnly = true;
            this.mHeightTextBox.Size = new System.Drawing.Size(108, 20);
            this.mHeightTextBox.TabIndex = 4;
            // 
            // mWidthTextBox
            // 
            this.mWidthTextBox.Location = new System.Drawing.Point(63, 46);
            this.mWidthTextBox.Name = "mWidthTextBox";
            this.mWidthTextBox.ReadOnly = true;
            this.mWidthTextBox.Size = new System.Drawing.Size(108, 20);
            this.mWidthTextBox.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Height:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Width:";
            // 
            // mReferencesListView
            // 
            this.mReferencesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            listViewGroup2.Header = "Group 1";
            listViewGroup2.Name = "listViewGroup1";
            this.mReferencesListView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup2});
            listViewItem3.Group = listViewGroup2;
            listViewItem4.Group = listViewGroup2;
            this.mReferencesListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem3,
            listViewItem4});
            this.mReferencesListView.Location = new System.Drawing.Point(9, 17);
            this.mReferencesListView.Name = "mReferencesListView";
            this.mReferencesListView.Size = new System.Drawing.Size(359, 259);
            this.mReferencesListView.TabIndex = 15;
            this.mReferencesListView.TileSize = new System.Drawing.Size(200, 20);
            this.mReferencesListView.UseCompatibleStateImageBehavior = false;
            this.mReferencesListView.View = System.Windows.Forms.View.Tile;
            // 
            // mAtlasCheckbox
            // 
            this.mAtlasCheckbox.AutoSize = true;
            this.mAtlasCheckbox.Location = new System.Drawing.Point(9, 25);
            this.mAtlasCheckbox.Name = "mAtlasCheckbox";
            this.mAtlasCheckbox.Size = new System.Drawing.Size(122, 17);
            this.mAtlasCheckbox.TabIndex = 16;
            this.mAtlasCheckbox.Text = "Add to Texture Atlas";
            this.mAtlasCheckbox.UseVisualStyleBackColor = true;
            this.mAtlasCheckbox.CheckedChanged += new System.EventHandler(this.mAtlasCheckbox_CheckedChanged);
            // 
            // mOKButton
            // 
            this.mOKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mOKButton.Location = new System.Drawing.Point(504, 562);
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.Size = new System.Drawing.Size(75, 23);
            this.mOKButton.TabIndex = 4;
            this.mOKButton.Text = "OK";
            this.mOKButton.UseVisualStyleBackColor = true;
            this.mOKButton.Click += new System.EventHandler(this.mOKButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.mOverrideCheckbox);
            this.groupBox2.Controls.Add(this.mOverrideNumericUpDown);
            this.groupBox2.Controls.Add(this.mPaddingTypeLabel);
            this.groupBox2.Controls.Add(this.mPaddingTypeComboBox);
            this.groupBox2.Controls.Add(this.mAtlasCheckbox);
            this.groupBox2.Location = new System.Drawing.Point(282, 434);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(378, 106);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Texture Atlas Settings";
            // 
            // mOverrideCheckbox
            // 
            this.mOverrideCheckbox.AutoSize = true;
            this.mOverrideCheckbox.Location = new System.Drawing.Point(9, 47);
            this.mOverrideCheckbox.Name = "mOverrideCheckbox";
            this.mOverrideCheckbox.Size = new System.Drawing.Size(142, 17);
            this.mOverrideCheckbox.TabIndex = 22;
            this.mOverrideCheckbox.Text = "Override Padding Width:";
            this.mOverrideCheckbox.UseVisualStyleBackColor = true;
            this.mOverrideCheckbox.CheckedChanged += new System.EventHandler(this.mOverrideCheckbox_CheckedChanged);
            // 
            // mOverrideNumericUpDown
            // 
            this.mOverrideNumericUpDown.Location = new System.Drawing.Point(157, 46);
            this.mOverrideNumericUpDown.Name = "mOverrideNumericUpDown";
            this.mOverrideNumericUpDown.Size = new System.Drawing.Size(50, 20);
            this.mOverrideNumericUpDown.TabIndex = 21;
            this.mOverrideNumericUpDown.ValueChanged += new System.EventHandler(this.mOverrideNumericUpDown_ValueChanged);
            // 
            // mPaddingTypeLabel
            // 
            this.mPaddingTypeLabel.AutoSize = true;
            this.mPaddingTypeLabel.Location = new System.Drawing.Point(6, 75);
            this.mPaddingTypeLabel.Name = "mPaddingTypeLabel";
            this.mPaddingTypeLabel.Size = new System.Drawing.Size(102, 13);
            this.mPaddingTypeLabel.TabIndex = 19;
            this.mPaddingTypeLabel.Text = "Atlas Padding Type:";
            // 
            // mPaddingTypeComboBox
            // 
            this.mPaddingTypeComboBox.FormattingEnabled = true;
            this.mPaddingTypeComboBox.Location = new System.Drawing.Point(157, 72);
            this.mPaddingTypeComboBox.Name = "mPaddingTypeComboBox";
            this.mPaddingTypeComboBox.Size = new System.Drawing.Size(211, 21);
            this.mPaddingTypeComboBox.TabIndex = 18;
            this.mPaddingTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.mPaddingTypeComboBox_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.mReferencesListView);
            this.groupBox3.Location = new System.Drawing.Point(282, 146);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(378, 282);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "References:";
            // 
            // TextureManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(672, 597);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.mOKButton);
            this.Controls.Add(this.mDetailsGroupBox);
            this.Controls.Add(this.mCloseButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TextureManagerForm";
            this.Text = "Texture Manager";
            this.Load += new System.EventHandler(this.TextureManagerForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.mDetailsGroupBox.ResumeLayout(false);
            this.mDetailsGroupBox.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mOverrideNumericUpDown)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button mCloseButton;
        private Otter.CustomControls.TextureList mTextureList;
        private System.Windows.Forms.GroupBox mDetailsGroupBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox mHeightTextBox;
        private System.Windows.Forms.TextBox mWidthTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox mSizeTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox mFilenameTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button mRemoveButton;
        private System.Windows.Forms.Button mAddButton;
        private System.Windows.Forms.Button mRefreshButton;
        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.ListView mReferencesListView;
        private System.Windows.Forms.CheckBox mAtlasCheckbox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown mOverrideNumericUpDown;
        private System.Windows.Forms.Label mPaddingTypeLabel;
        private System.Windows.Forms.ComboBox mPaddingTypeComboBox;
        private System.Windows.Forms.CheckBox mOverrideCheckbox;
    }
}