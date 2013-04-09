namespace Otter.Forms
{
    partial class SoundManagerForm
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
            this.mSoundList = new System.Windows.Forms.ListView();
            this.mRemoveButton = new System.Windows.Forms.Button();
            this.mAddButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.mCloseButton = new System.Windows.Forms.Button();
            this.mDetailsGroupBox = new System.Windows.Forms.GroupBox();
            this.mPlayButton = new System.Windows.Forms.Button();
            this.mReferencesListView = new System.Windows.Forms.ListView();
            this.mBrowseButton = new System.Windows.Forms.Button();
            this.mFilenameTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.mSizeTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.mOKButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.mDetailsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.mSoundList);
            this.groupBox1.Controls.Add(this.mRemoveButton);
            this.groupBox1.Controls.Add(this.mAddButton);
            this.groupBox1.Location = new System.Drawing.Point(12, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(270, 524);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sound List";
            // 
            // mSoundList
            // 
            this.mSoundList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.mSoundList.HideSelection = false;
            this.mSoundList.LabelEdit = true;
            this.mSoundList.Location = new System.Drawing.Point(6, 48);
            this.mSoundList.MultiSelect = false;
            this.mSoundList.Name = "mSoundList";
            this.mSoundList.Size = new System.Drawing.Size(258, 470);
            this.mSoundList.TabIndex = 3;
            this.mSoundList.UseCompatibleStateImageBehavior = false;
            this.mSoundList.View = System.Windows.Forms.View.List;
            this.mSoundList.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.mSoundList_AfterLabelEdit);
            this.mSoundList.SelectedIndexChanged += new System.EventHandler(this.mSoundList_SelectedIndexChanged);
            this.mSoundList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mSoundList_KeyDown);
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
            this.label1.Location = new System.Drawing.Point(12, 540);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(683, 2);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // mCloseButton
            // 
            this.mCloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mCloseButton.Location = new System.Drawing.Point(620, 553);
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
            this.mDetailsGroupBox.Controls.Add(this.mPlayButton);
            this.mDetailsGroupBox.Controls.Add(this.mReferencesListView);
            this.mDetailsGroupBox.Controls.Add(this.mBrowseButton);
            this.mDetailsGroupBox.Controls.Add(this.mFilenameTextBox);
            this.mDetailsGroupBox.Controls.Add(this.label9);
            this.mDetailsGroupBox.Controls.Add(this.label7);
            this.mDetailsGroupBox.Controls.Add(this.mSizeTextBox);
            this.mDetailsGroupBox.Controls.Add(this.label6);
            this.mDetailsGroupBox.Controls.Add(this.label2);
            this.mDetailsGroupBox.Enabled = false;
            this.mDetailsGroupBox.Location = new System.Drawing.Point(282, 13);
            this.mDetailsGroupBox.Name = "mDetailsGroupBox";
            this.mDetailsGroupBox.Size = new System.Drawing.Size(413, 524);
            this.mDetailsGroupBox.TabIndex = 3;
            this.mDetailsGroupBox.TabStop = false;
            this.mDetailsGroupBox.Text = "Details";
            // 
            // mPlayButton
            // 
            this.mPlayButton.Enabled = false;
            this.mPlayButton.Location = new System.Drawing.Point(328, 46);
            this.mPlayButton.Name = "mPlayButton";
            this.mPlayButton.Size = new System.Drawing.Size(75, 23);
            this.mPlayButton.TabIndex = 16;
            this.mPlayButton.Text = "Play";
            this.mPlayButton.UseVisualStyleBackColor = true;
            this.mPlayButton.Click += new System.EventHandler(this.mPlayButton_Click);
            // 
            // mReferencesListView
            // 
            this.mReferencesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mReferencesListView.Location = new System.Drawing.Point(63, 88);
            this.mReferencesListView.Name = "mReferencesListView";
            this.mReferencesListView.Size = new System.Drawing.Size(340, 430);
            this.mReferencesListView.TabIndex = 15;
            this.mReferencesListView.TileSize = new System.Drawing.Size(300, 20);
            this.mReferencesListView.UseCompatibleStateImageBehavior = false;
            this.mReferencesListView.View = System.Windows.Forms.View.SmallIcon;
            // 
            // mBrowseButton
            // 
            this.mBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mBrowseButton.Location = new System.Drawing.Point(328, 20);
            this.mBrowseButton.Name = "mBrowseButton";
            this.mBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.mBrowseButton.TabIndex = 14;
            this.mBrowseButton.Text = "Browse";
            this.mBrowseButton.UseVisualStyleBackColor = true;
            this.mBrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // mFilenameTextBox
            // 
            this.mFilenameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mFilenameTextBox.Location = new System.Drawing.Point(63, 22);
            this.mFilenameTextBox.Name = "mFilenameTextBox";
            this.mFilenameTextBox.ReadOnly = true;
            this.mFilenameTextBox.Size = new System.Drawing.Size(259, 20);
            this.mFilenameTextBox.TabIndex = 13;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 13);
            this.label9.TabIndex = 12;
            this.label9.Text = "Filename:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 88);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(49, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Used by:";
            // 
            // mSizeTextBox
            // 
            this.mSizeTextBox.Location = new System.Drawing.Point(63, 53);
            this.mSizeTextBox.Name = "mSizeTextBox";
            this.mSizeTextBox.ReadOnly = true;
            this.mSizeTextBox.Size = new System.Drawing.Size(108, 20);
            this.mSizeTextBox.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 56);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "Size:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(7, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(397, 1);
            this.label2.TabIndex = 0;
            // 
            // mOKButton
            // 
            this.mOKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mOKButton.Location = new System.Drawing.Point(539, 553);
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.Size = new System.Drawing.Size(75, 23);
            this.mOKButton.TabIndex = 4;
            this.mOKButton.Text = "OK";
            this.mOKButton.UseVisualStyleBackColor = true;
            this.mOKButton.Click += new System.EventHandler(this.mOKButton_Click);
            // 
            // SoundManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(707, 588);
            this.Controls.Add(this.mOKButton);
            this.Controls.Add(this.mDetailsGroupBox);
            this.Controls.Add(this.mCloseButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SoundManagerForm";
            this.Text = "Sound Manager";
            this.Load += new System.EventHandler(this.SoundManagerForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.mDetailsGroupBox.ResumeLayout(false);
            this.mDetailsGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button mCloseButton;
        private System.Windows.Forms.GroupBox mDetailsGroupBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox mSizeTextBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button mBrowseButton;
        private System.Windows.Forms.TextBox mFilenameTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button mRemoveButton;
        private System.Windows.Forms.Button mAddButton;
        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListView mReferencesListView;
        private System.Windows.Forms.ListView mSoundList;
        private System.Windows.Forms.Button mPlayButton;
    }
}