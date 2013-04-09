namespace Otter.Forms
{
    partial class ActionsEditor
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Message", 0);
            System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Sound", 1);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActionsEditor));
            this.mPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.label2 = new System.Windows.Forms.Label();
            this.mRemoveButton = new System.Windows.Forms.Button();
            this.mItemsListView = new System.Windows.Forms.ListView();
            this.mImageList = new System.Windows.Forms.ImageList(this.components);
            this.mOKButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.mActionTypesComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // mPropertyGrid
            // 
            this.mPropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mPropertyGrid.Location = new System.Drawing.Point(242, 8);
            this.mPropertyGrid.Name = "mPropertyGrid";
            this.mPropertyGrid.Size = new System.Drawing.Size(262, 455);
            this.mPropertyGrid.TabIndex = 14;
            this.mPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.mPropertyGrid_PropertyValueChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(234, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(2, 450);
            this.label2.TabIndex = 13;
            // 
            // mRemoveButton
            // 
            this.mRemoveButton.Enabled = false;
            this.mRemoveButton.Location = new System.Drawing.Point(153, 9);
            this.mRemoveButton.Name = "mRemoveButton";
            this.mRemoveButton.Size = new System.Drawing.Size(75, 23);
            this.mRemoveButton.TabIndex = 12;
            this.mRemoveButton.Text = "Remove";
            this.mRemoveButton.UseVisualStyleBackColor = true;
            this.mRemoveButton.Click += new System.EventHandler(this.mRemoveButton_Click);
            // 
            // mItemsListView
            // 
            this.mItemsListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.mItemsListView.FullRowSelect = true;
            this.mItemsListView.HideSelection = false;
            this.mItemsListView.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2});
            this.mItemsListView.LargeImageList = this.mImageList;
            this.mItemsListView.Location = new System.Drawing.Point(13, 37);
            this.mItemsListView.Name = "mItemsListView";
            this.mItemsListView.Size = new System.Drawing.Size(215, 426);
            this.mItemsListView.SmallImageList = this.mImageList;
            this.mItemsListView.TabIndex = 10;
            this.mItemsListView.TileSize = new System.Drawing.Size(200, 24);
            this.mItemsListView.UseCompatibleStateImageBehavior = false;
            this.mItemsListView.View = System.Windows.Forms.View.Tile;
            this.mItemsListView.SelectedIndexChanged += new System.EventHandler(this.mItemsListView_SelectedIndexChanged);
            // 
            // mImageList
            // 
            this.mImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("mImageList.ImageStream")));
            this.mImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.mImageList.Images.SetKeyName(0, "Icon_Message.png");
            this.mImageList.Images.SetKeyName(1, "Icon_Sound.png");
            // 
            // mOKButton
            // 
            this.mOKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOKButton.Location = new System.Drawing.Point(429, 479);
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.Size = new System.Drawing.Size(75, 23);
            this.mOKButton.TabIndex = 9;
            this.mOKButton.Text = "OK";
            this.mOKButton.UseVisualStyleBackColor = true;
            this.mOKButton.Click += new System.EventHandler(this.mOKButton_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(12, 466);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(492, 2);
            this.label1.TabIndex = 8;
            // 
            // mActionTypesComboBox
            // 
            this.mActionTypesComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mActionTypesComboBox.FormattingEnabled = true;
            this.mActionTypesComboBox.Items.AddRange(new object[] {
            "Create...",
            "Message",
            "Sound"});
            this.mActionTypesComboBox.Location = new System.Drawing.Point(13, 11);
            this.mActionTypesComboBox.Name = "mActionTypesComboBox";
            this.mActionTypesComboBox.Size = new System.Drawing.Size(121, 21);
            this.mActionTypesComboBox.TabIndex = 15;
            this.mActionTypesComboBox.SelectedIndexChanged += new System.EventHandler(this.mActionTypesComboBox_SelectedIndexChanged);
            // 
            // ActionsEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(516, 514);
            this.Controls.Add(this.mActionTypesComboBox);
            this.Controls.Add(this.mPropertyGrid);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mRemoveButton);
            this.Controls.Add(this.mItemsListView);
            this.Controls.Add(this.mOKButton);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ActionsEditor";
            this.Text = "Actions Editor";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid mPropertyGrid;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button mRemoveButton;
        private System.Windows.Forms.ListView mItemsListView;
        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox mActionTypesComboBox;
        private System.Windows.Forms.ImageList mImageList;

    }
}