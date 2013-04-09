namespace Otter.Forms
{
    partial class GlyphEditor
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
            this.mOKButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.mGlyphDataGrid = new System.Windows.Forms.DataGridView();
            this.Glyph = new System.Windows.Forms.DataGridViewImageColumn();
            this.CharCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mAddUnicodeCharsButton = new System.Windows.Forms.Button();
            this.mRemoveSelectedButton = new System.Windows.Forms.Button();
            this.mAddImagesButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.mGlyphDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // mOKButton
            // 
            this.mOKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOKButton.Location = new System.Drawing.Point(513, 588);
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.Size = new System.Drawing.Size(75, 23);
            this.mOKButton.TabIndex = 0;
            this.mOKButton.Text = "OK";
            this.mOKButton.UseVisualStyleBackColor = true;
            // 
            // mOKButton
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(594, 588);
            this.button1.Name = "mOKButton";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(12, 582);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(657, 3);
            this.label1.TabIndex = 2;
            // 
            // mGlyphDataGrid
            // 
            this.mGlyphDataGrid.AllowUserToAddRows = false;
            this.mGlyphDataGrid.AllowUserToDeleteRows = false;
            this.mGlyphDataGrid.AllowUserToResizeColumns = false;
            this.mGlyphDataGrid.AllowUserToResizeRows = false;
            this.mGlyphDataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mGlyphDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mGlyphDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Glyph,
            this.CharCode});
            this.mGlyphDataGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.mGlyphDataGrid.Location = new System.Drawing.Point(12, 12);
            this.mGlyphDataGrid.Name = "mGlyphDataGrid";
            this.mGlyphDataGrid.RowHeadersVisible = false;
            this.mGlyphDataGrid.RowTemplate.Height = 48;
            this.mGlyphDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.mGlyphDataGrid.Size = new System.Drawing.Size(481, 567);
            this.mGlyphDataGrid.TabIndex = 3;
            this.mGlyphDataGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.mGlyphDataGrid_CellDoubleClick);
            this.mGlyphDataGrid.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.mGlyphDataGrid_CellEndEdit);
            this.mGlyphDataGrid.SelectionChanged += new System.EventHandler(this.mGlyphDataGrid_SelectionChanged);
            // 
            // Glyph
            // 
            this.Glyph.HeaderText = "Glyph";
            this.Glyph.Name = "Glyph";
            this.Glyph.ReadOnly = true;
            this.Glyph.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Glyph.Width = 48;
            // 
            // CharCode
            // 
            this.CharCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.CharCode.HeaderText = "Character Code";
            this.CharCode.Name = "CharCode";
            this.CharCode.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // mAddUnicodeCharsButton
            // 
            this.mAddUnicodeCharsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mAddUnicodeCharsButton.Location = new System.Drawing.Point(499, 12);
            this.mAddUnicodeCharsButton.Name = "mAddUnicodeCharsButton";
            this.mAddUnicodeCharsButton.Size = new System.Drawing.Size(170, 23);
            this.mAddUnicodeCharsButton.TabIndex = 5;
            this.mAddUnicodeCharsButton.Text = "Add Unicode Characters ...";
            this.mAddUnicodeCharsButton.UseVisualStyleBackColor = true;
            this.mAddUnicodeCharsButton.Click += new System.EventHandler(this.mAddUnicodeCharsButton_Click);
            // 
            // mRemoveSelectedButton
            // 
            this.mRemoveSelectedButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mRemoveSelectedButton.Enabled = false;
            this.mRemoveSelectedButton.Location = new System.Drawing.Point(499, 70);
            this.mRemoveSelectedButton.Name = "mRemoveSelectedButton";
            this.mRemoveSelectedButton.Size = new System.Drawing.Size(170, 23);
            this.mRemoveSelectedButton.TabIndex = 6;
            this.mRemoveSelectedButton.Text = "Remove Selected";
            this.mRemoveSelectedButton.UseVisualStyleBackColor = true;
            this.mRemoveSelectedButton.Click += new System.EventHandler(this.mRemoveSelectedButton_Click);
            // 
            // mAddImagesButton
            // 
            this.mAddImagesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mAddImagesButton.Location = new System.Drawing.Point(499, 41);
            this.mAddImagesButton.Name = "mAddImagesButton";
            this.mAddImagesButton.Size = new System.Drawing.Size(170, 23);
            this.mAddImagesButton.TabIndex = 8;
            this.mAddImagesButton.Text = "Add Images ...";
            this.mAddImagesButton.UseVisualStyleBackColor = true;
            this.mAddImagesButton.Click += new System.EventHandler(this.mAddImagesButton_Click);
            // 
            // GlyphEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(681, 623);
            this.Controls.Add(this.mAddImagesButton);
            this.Controls.Add(this.mRemoveSelectedButton);
            this.Controls.Add(this.mAddUnicodeCharsButton);
            this.Controls.Add(this.mGlyphDataGrid);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.mOKButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "GlyphEditor";
            this.Text = "Glyph Editor";
            this.Load += new System.EventHandler(this.GlyphEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.mGlyphDataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView mGlyphDataGrid;
        private System.Windows.Forms.Button mAddUnicodeCharsButton;
        private System.Windows.Forms.Button mRemoveSelectedButton;
        private System.Windows.Forms.Button mAddImagesButton;
        private System.Windows.Forms.DataGridViewImageColumn Glyph;
        private System.Windows.Forms.DataGridViewTextBoxColumn CharCode;
    }
}