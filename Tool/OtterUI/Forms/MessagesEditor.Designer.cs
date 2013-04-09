namespace Otter.Forms
{
    partial class MessagesEditor
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
            this.label1 = new System.Windows.Forms.Label();
            this.mMessagesDataGrid = new System.Windows.Forms.DataGridView();
            this.MessageCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DescriptionCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.mAddButton = new System.Windows.Forms.Button();
            this.mRemoveButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.mMessagesDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // mOKButton
            // 
            this.mOKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOKButton.Location = new System.Drawing.Point(602, 439);
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.Size = new System.Drawing.Size(125, 23);
            this.mOKButton.TabIndex = 4;
            this.mOKButton.Text = "OK";
            this.mOKButton.UseVisualStyleBackColor = true;
            this.mOKButton.Click += new System.EventHandler(this.mOKButton_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(12, 426);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(715, 2);
            this.label1.TabIndex = 3;
            // 
            // mMessagesDataGrid
            // 
            this.mMessagesDataGrid.AllowUserToAddRows = false;
            this.mMessagesDataGrid.AllowUserToDeleteRows = false;
            this.mMessagesDataGrid.AllowUserToResizeColumns = false;
            this.mMessagesDataGrid.AllowUserToResizeRows = false;
            this.mMessagesDataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mMessagesDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mMessagesDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MessageCol,
            this.DescriptionCol});
            this.mMessagesDataGrid.Location = new System.Drawing.Point(13, 13);
            this.mMessagesDataGrid.MultiSelect = false;
            this.mMessagesDataGrid.Name = "mMessagesDataGrid";
            this.mMessagesDataGrid.RowHeadersVisible = false;
            this.mMessagesDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.mMessagesDataGrid.ShowEditingIcon = false;
            this.mMessagesDataGrid.Size = new System.Drawing.Size(575, 410);
            this.mMessagesDataGrid.TabIndex = 5;
            this.mMessagesDataGrid.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.mMessagesDataGrid_CellEndEdit);
            this.mMessagesDataGrid.SelectionChanged += new System.EventHandler(this.mMessagesDataGrid_SelectionChanged);
            // 
            // MessageCol
            // 
            this.MessageCol.HeaderText = "Message";
            this.MessageCol.Name = "MessageCol";
            this.MessageCol.Width = 200;
            // 
            // DescriptionCol
            // 
            this.DescriptionCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DescriptionCol.HeaderText = "Description";
            this.DescriptionCol.Name = "DescriptionCol";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(594, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(2, 410);
            this.label2.TabIndex = 6;
            // 
            // mAddButton
            // 
            this.mAddButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mAddButton.Location = new System.Drawing.Point(602, 13);
            this.mAddButton.Name = "mAddButton";
            this.mAddButton.Size = new System.Drawing.Size(125, 23);
            this.mAddButton.TabIndex = 7;
            this.mAddButton.Text = "Add";
            this.mAddButton.UseVisualStyleBackColor = true;
            this.mAddButton.Click += new System.EventHandler(this.mAddButton_Click);
            // 
            // mRemoveButton
            // 
            this.mRemoveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mRemoveButton.Enabled = false;
            this.mRemoveButton.Location = new System.Drawing.Point(603, 43);
            this.mRemoveButton.Name = "mRemoveButton";
            this.mRemoveButton.Size = new System.Drawing.Size(124, 23);
            this.mRemoveButton.TabIndex = 8;
            this.mRemoveButton.Text = "Remove";
            this.mRemoveButton.UseVisualStyleBackColor = true;
            this.mRemoveButton.Click += new System.EventHandler(this.mRemoveButton_Click);
            // 
            // MessagesEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(739, 474);
            this.Controls.Add(this.mRemoveButton);
            this.Controls.Add(this.mAddButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mMessagesDataGrid);
            this.Controls.Add(this.mOKButton);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "MessagesEditor";
            this.Text = "Messages Editor";
            this.Load += new System.EventHandler(this.MessagesEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.mMessagesDataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView mMessagesDataGrid;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button mAddButton;
        private System.Windows.Forms.Button mRemoveButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn MessageCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn DescriptionCol;
    }
}