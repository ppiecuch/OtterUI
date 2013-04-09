namespace Otter.CustomControls
{
    partial class TextureList
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextureList));
            this.mTexturesListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.mImageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // mTexturesListView
            // 
            this.mTexturesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.mTexturesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mTexturesListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.mTexturesListView.HideSelection = false;
            this.mTexturesListView.LargeImageList = this.mImageList;
            this.mTexturesListView.Location = new System.Drawing.Point(0, 0);
            this.mTexturesListView.MultiSelect = false;
            this.mTexturesListView.Name = "mTexturesListView";
            this.mTexturesListView.Size = new System.Drawing.Size(283, 546);
            this.mTexturesListView.TabIndex = 0;
            this.mTexturesListView.UseCompatibleStateImageBehavior = false;
            this.mTexturesListView.View = System.Windows.Forms.View.Tile;
            this.mTexturesListView.SelectedIndexChanged += new System.EventHandler(this.mTexturesListView_SelectedIndexChanged);
            // 
            // mImageList
            // 
            this.mImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("mImageList.ImageStream")));
            this.mImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.mImageList.Images.SetKeyName(0, "X.png");
            // 
            // TextureList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.mTexturesListView);
            this.Name = "TextureList";
            this.Size = new System.Drawing.Size(283, 546);
            this.Load += new System.EventHandler(this.TextureList_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView mTexturesListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ImageList mImageList;



    }
}
