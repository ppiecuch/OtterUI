namespace Otter.Editor.ContentViews
{
    partial class ControlsView
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
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("Standard", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("Custom", System.Windows.Forms.HorizontalAlignment.Left);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlsView));
            this.mControlsListView = new System.Windows.Forms.ListView();
            this.mImageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // mControlsListView
            // 
            this.mControlsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mControlsListView.FullRowSelect = true;
            listViewGroup1.Header = "Standard";
            listViewGroup1.Name = "mStandardControlsGroup";
            listViewGroup2.Header = "Custom";
            listViewGroup2.Name = "mCustomControlsGroup";
            this.mControlsListView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
            this.mControlsListView.HideSelection = false;
            this.mControlsListView.LargeImageList = this.mImageList;
            this.mControlsListView.Location = new System.Drawing.Point(0, 0);
            this.mControlsListView.MultiSelect = false;
            this.mControlsListView.Name = "mControlsListView";
            this.mControlsListView.Size = new System.Drawing.Size(201, 555);
            this.mControlsListView.TabIndex = 0;
            this.mControlsListView.TileSize = new System.Drawing.Size(100, 36);
            this.mControlsListView.UseCompatibleStateImageBehavior = false;
            this.mControlsListView.View = System.Windows.Forms.View.Tile;
            this.mControlsListView.Resize += new System.EventHandler(this.mControlsListView_Resize);
            this.mControlsListView.SelectedIndexChanged += new System.EventHandler(this.mControlsListView_SelectedIndexChanged);
            // 
            // mImageList
            // 
            this.mImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("mImageList.ImageStream")));
            this.mImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.mImageList.Images.SetKeyName(0, "Button_Default.png");
            this.mImageList.Images.SetKeyName(1, "Button_Sprite.png");
            this.mImageList.Images.SetKeyName(2, "Button_Label.png");
            this.mImageList.Images.SetKeyName(3, "Button_Button.png");
            this.mImageList.Images.SetKeyName(4, "Button_Table.png");
            this.mImageList.Images.SetKeyName(5, "Button_Toggle.png");
            this.mImageList.Images.SetKeyName(6, "Button_Slider.png");
            this.mImageList.Images.SetKeyName(7, "Button_Mask.png");
            // 
            // ControlsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(201, 555);
            this.Controls.Add(this.mControlsListView);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ControlsView";
            this.TabText = "Controls";
            this.Text = "Controls";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView mControlsListView;
        private System.Windows.Forms.ImageList mImageList;
    }
}