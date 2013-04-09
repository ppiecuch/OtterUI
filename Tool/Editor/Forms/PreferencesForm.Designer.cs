namespace Otter.Editor.Forms
{
    partial class PreferencesForm
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
            this.mCancelButton = new System.Windows.Forms.Button();
            this.mOKButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mBoundsColorPanel = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.mSnapTolerance = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.mGridIncrement = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.mSelectColorPanel = new System.Windows.Forms.Panel();
            this.mInnerSelectColorPanel = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.mImagePanel = new System.Windows.Forms.Panel();
            this.mImageBackgroundRadioButton = new System.Windows.Forms.RadioButton();
            this.mColorBackgroundRadioButton = new System.Windows.Forms.RadioButton();
            this.mColorPanel = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.mShowWelcomeCheckbox = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.mSaveOnExportCheckbox = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.mAnchorEditor = new Otter.CustomControls.AnchorEditor();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mSnapTolerance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mGridIncrement)).BeginInit();
            this.mSelectColorPanel.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // mCancelButton
            // 
            this.mCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mCancelButton.Location = new System.Drawing.Point(190, 490);
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.Size = new System.Drawing.Size(75, 23);
            this.mCancelButton.TabIndex = 0;
            this.mCancelButton.Text = "Cancel";
            this.mCancelButton.UseVisualStyleBackColor = true;
            this.mCancelButton.Click += new System.EventHandler(this.mCancelButton_Click);
            // 
            // mOKButton
            // 
            this.mOKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mOKButton.Location = new System.Drawing.Point(109, 490);
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.Size = new System.Drawing.Size(75, 23);
            this.mOKButton.TabIndex = 1;
            this.mOKButton.Text = "OK";
            this.mOKButton.UseVisualStyleBackColor = true;
            this.mOKButton.Click += new System.EventHandler(this.mOKButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.mBoundsColorPanel);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.mSnapTolerance);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.mGridIncrement);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.mSelectColorPanel);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.mImagePanel);
            this.groupBox1.Controls.Add(this.mImageBackgroundRadioButton);
            this.groupBox1.Controls.Add(this.mColorBackgroundRadioButton);
            this.groupBox1.Controls.Add(this.mColorPanel);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(12, 91);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(253, 217);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Views";
            // 
            // mBoundsColorPanel
            // 
            this.mBoundsColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mBoundsColorPanel.Location = new System.Drawing.Point(97, 97);
            this.mBoundsColorPanel.Name = "mBoundsColorPanel";
            this.mBoundsColorPanel.Size = new System.Drawing.Size(60, 33);
            this.mBoundsColorPanel.TabIndex = 10;
            this.mBoundsColorPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.mBoundsColorPanel_Paint);
            this.mBoundsColorPanel.Click += new System.EventHandler(this.mBoundsColorPanel_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 107);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Bounds:";
            // 
            // mSnapTolerance
            // 
            this.mSnapTolerance.Location = new System.Drawing.Point(97, 162);
            this.mSnapTolerance.Name = "mSnapTolerance";
            this.mSnapTolerance.Size = new System.Drawing.Size(60, 20);
            this.mSnapTolerance.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 164);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Snap Tolerance:";
            // 
            // mGridIncrement
            // 
            this.mGridIncrement.Location = new System.Drawing.Point(97, 136);
            this.mGridIncrement.Name = "mGridIncrement";
            this.mGridIncrement.Size = new System.Drawing.Size(60, 20);
            this.mGridIncrement.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 138);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Grid Increment:";
            // 
            // mSelectColorPanel
            // 
            this.mSelectColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mSelectColorPanel.Controls.Add(this.mInnerSelectColorPanel);
            this.mSelectColorPanel.Location = new System.Drawing.Point(97, 58);
            this.mSelectColorPanel.Name = "mSelectColorPanel";
            this.mSelectColorPanel.Size = new System.Drawing.Size(60, 33);
            this.mSelectColorPanel.TabIndex = 3;
            this.mSelectColorPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.mSelectColorPanel_Paint);
            this.mSelectColorPanel.Click += new System.EventHandler(this.mSelectColorPanel_Click);
            // 
            // mInnerSelectColorPanel
            // 
            this.mInnerSelectColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mInnerSelectColorPanel.Location = new System.Drawing.Point(8, 7);
            this.mInnerSelectColorPanel.Name = "mInnerSelectColorPanel";
            this.mInnerSelectColorPanel.Size = new System.Drawing.Size(42, 18);
            this.mInnerSelectColorPanel.TabIndex = 4;
            this.mInnerSelectColorPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.mInnerSelectColorPanel_Paint);
            this.mInnerSelectColorPanel.Click += new System.EventHandler(this.mInnerSelectColorPanel_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Selection:";
            // 
            // mImagePanel
            // 
            this.mImagePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mImagePanel.Location = new System.Drawing.Point(183, 19);
            this.mImagePanel.Name = "mImagePanel";
            this.mImagePanel.Size = new System.Drawing.Size(60, 33);
            this.mImagePanel.TabIndex = 4;
            this.mImagePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.ImagePanel_Paint);
            this.mImagePanel.Click += new System.EventHandler(this.mImagePanel_Click);
            // 
            // mImageBackgroundRadioButton
            // 
            this.mImageBackgroundRadioButton.AutoSize = true;
            this.mImageBackgroundRadioButton.Location = new System.Drawing.Point(163, 28);
            this.mImageBackgroundRadioButton.Name = "mImageBackgroundRadioButton";
            this.mImageBackgroundRadioButton.Size = new System.Drawing.Size(14, 13);
            this.mImageBackgroundRadioButton.TabIndex = 3;
            this.mImageBackgroundRadioButton.TabStop = true;
            this.mImageBackgroundRadioButton.UseVisualStyleBackColor = true;
            // 
            // mColorBackgroundRadioButton
            // 
            this.mColorBackgroundRadioButton.AutoSize = true;
            this.mColorBackgroundRadioButton.Location = new System.Drawing.Point(77, 28);
            this.mColorBackgroundRadioButton.Name = "mColorBackgroundRadioButton";
            this.mColorBackgroundRadioButton.Size = new System.Drawing.Size(14, 13);
            this.mColorBackgroundRadioButton.TabIndex = 2;
            this.mColorBackgroundRadioButton.TabStop = true;
            this.mColorBackgroundRadioButton.UseVisualStyleBackColor = true;
            // 
            // mColorPanel
            // 
            this.mColorPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mColorPanel.Location = new System.Drawing.Point(97, 19);
            this.mColorPanel.Name = "mColorPanel";
            this.mColorPanel.Size = new System.Drawing.Size(60, 33);
            this.mColorPanel.TabIndex = 1;
            this.mColorPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.mColorPanel_Paint);
            this.mColorPanel.Click += new System.EventHandler(this.mColorPanel_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Background:";
            // 
            // mShowWelcomeCheckbox
            // 
            this.mShowWelcomeCheckbox.AutoSize = true;
            this.mShowWelcomeCheckbox.Location = new System.Drawing.Point(8, 22);
            this.mShowWelcomeCheckbox.Name = "mShowWelcomeCheckbox";
            this.mShowWelcomeCheckbox.Size = new System.Drawing.Size(190, 17);
            this.mShowWelcomeCheckbox.TabIndex = 4;
            this.mShowWelcomeCheckbox.Text = "Show Welcome Screen on Startup";
            this.mShowWelcomeCheckbox.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.mSaveOnExportCheckbox);
            this.groupBox2.Controls.Add(this.mShowWelcomeCheckbox);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(253, 73);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "General";
            // 
            // mSaveOnExportCheckbox
            // 
            this.mSaveOnExportCheckbox.AutoSize = true;
            this.mSaveOnExportCheckbox.Location = new System.Drawing.Point(8, 45);
            this.mSaveOnExportCheckbox.Name = "mSaveOnExportCheckbox";
            this.mSaveOnExportCheckbox.Size = new System.Drawing.Size(99, 17);
            this.mSaveOnExportCheckbox.TabIndex = 5;
            this.mSaveOnExportCheckbox.Text = "Save on Export";
            this.mSaveOnExportCheckbox.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.mAnchorEditor);
            this.groupBox3.Location = new System.Drawing.Point(12, 288);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(253, 196);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Controls";
            // 
            // mAnchorEditor
            // 
            this.mAnchorEditor.AnchorFlags = Otter.UI.AnchorFlags.None;
            this.mAnchorEditor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mAnchorEditor.Location = new System.Drawing.Point(93, 19);
            this.mAnchorEditor.Name = "mAnchorEditor";
            this.mAnchorEditor.Size = new System.Drawing.Size(150, 150);
            this.mAnchorEditor.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 83);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Anchor Defaults:";
            // 
            // PreferencesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(277, 525);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.mOKButton);
            this.Controls.Add(this.mCancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PreferencesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Preferences";
            this.Load += new System.EventHandler(this.PreferencesForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mSnapTolerance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mGridIncrement)).EndInit();
            this.mSelectColorPanel.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel mColorPanel;
        private System.Windows.Forms.RadioButton mColorBackgroundRadioButton;
        private System.Windows.Forms.RadioButton mImageBackgroundRadioButton;
        private System.Windows.Forms.Panel mImagePanel;
        private System.Windows.Forms.CheckBox mShowWelcomeCheckbox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel mSelectColorPanel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel mInnerSelectColorPanel;
        private System.Windows.Forms.NumericUpDown mGridIncrement;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown mSnapTolerance;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel mBoundsColorPanel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox mSaveOnExportCheckbox;
        private System.Windows.Forms.GroupBox groupBox3;
        private Otter.CustomControls.AnchorEditor mAnchorEditor;
        private System.Windows.Forms.Label label6;
    }
}