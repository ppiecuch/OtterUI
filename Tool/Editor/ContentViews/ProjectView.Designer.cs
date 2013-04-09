namespace Otter.Editor.ContentViews
{
    partial class ProjectView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectView));
            this.mImageList = new System.Windows.Forms.ImageList(this.components);
            this.mScenesListView = new System.Windows.Forms.ListView();
            this.mProjectViewContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mAddSceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mOpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mCloseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mExportSceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.mDeleteSceneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mRenameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mProjectViewContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mImageList
            // 
            this.mImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("mImageList.ImageStream")));
            this.mImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.mImageList.Images.SetKeyName(0, "Button_Scene.png");
            // 
            // mScenesListView
            // 
            this.mScenesListView.ContextMenuStrip = this.mProjectViewContextMenuStrip;
            this.mScenesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mScenesListView.LargeImageList = this.mImageList;
            this.mScenesListView.Location = new System.Drawing.Point(0, 0);
            this.mScenesListView.Name = "mScenesListView";
            this.mScenesListView.Size = new System.Drawing.Size(273, 659);
            this.mScenesListView.TabIndex = 2;
            this.mScenesListView.UseCompatibleStateImageBehavior = false;
            this.mScenesListView.View = System.Windows.Forms.View.Tile;
            this.mScenesListView.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.mScenesListView_AfterLabelEdit);
            this.mScenesListView.SelectedIndexChanged += new System.EventHandler(this.mScenesListView_SelectedIndexChanged);
            this.mScenesListView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.mScenesListView_KeyDown);
            this.mScenesListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.mScenesListView_MouseDoubleClick);
            // 
            // mProjectViewContextMenuStrip
            // 
            this.mProjectViewContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mAddSceneToolStripMenuItem,
            this.toolStripMenuItem1,
            this.mOpenToolStripMenuItem,
            this.mCloseToolStripMenuItem,
            this.mExportSceneToolStripMenuItem,
            this.toolStripMenuItem2,
            this.mDeleteSceneToolStripMenuItem,
            this.mRenameToolStripMenuItem});
            this.mProjectViewContextMenuStrip.Name = "mProjectViewContextMenuStrip";
            this.mProjectViewContextMenuStrip.Size = new System.Drawing.Size(118, 148);
            this.mProjectViewContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.mProjectViewContextMenuStrip_Opening);
            // 
            // mAddSceneToolStripMenuItem
            // 
            this.mAddSceneToolStripMenuItem.Name = "mAddSceneToolStripMenuItem";
            this.mAddSceneToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.mAddSceneToolStripMenuItem.Text = "Add";
            this.mAddSceneToolStripMenuItem.Click += new System.EventHandler(this.mAddSceneToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(114, 6);
            // 
            // mOpenToolStripMenuItem
            // 
            this.mOpenToolStripMenuItem.Name = "mOpenToolStripMenuItem";
            this.mOpenToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.mOpenToolStripMenuItem.Text = "Open";
            this.mOpenToolStripMenuItem.Click += new System.EventHandler(this.mOpenToolStripMenuItem_Click);
            // 
            // mCloseToolStripMenuItem
            // 
            this.mCloseToolStripMenuItem.Name = "mCloseToolStripMenuItem";
            this.mCloseToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.mCloseToolStripMenuItem.Text = "Close";
            this.mCloseToolStripMenuItem.Click += new System.EventHandler(this.mCloseToolStripMenuItem_Click);
            // 
            // mExportSceneToolStripMenuItem
            // 
            this.mExportSceneToolStripMenuItem.Name = "mExportSceneToolStripMenuItem";
            this.mExportSceneToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.mExportSceneToolStripMenuItem.Text = "Export";
            this.mExportSceneToolStripMenuItem.Click += new System.EventHandler(this.mExportSceneToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(114, 6);
            // 
            // mDeleteSceneToolStripMenuItem
            // 
            this.mDeleteSceneToolStripMenuItem.Name = "mDeleteSceneToolStripMenuItem";
            this.mDeleteSceneToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.mDeleteSceneToolStripMenuItem.Text = "Delete";
            this.mDeleteSceneToolStripMenuItem.Click += new System.EventHandler(this.mDeleteSceneToolStripMenuItem_Click);
            // 
            // mRenameToolStripMenuItem
            // 
            this.mRenameToolStripMenuItem.Name = "mRenameToolStripMenuItem";
            this.mRenameToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.mRenameToolStripMenuItem.Text = "Rename";
            this.mRenameToolStripMenuItem.Click += new System.EventHandler(this.mRenameToolStripMenuItem_Click);
            // 
            // ProjectView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(273, 659);
            this.Controls.Add(this.mScenesListView);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ProjectView";
            this.TabText = "Project";
            this.Text = "Project";
            this.mProjectViewContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList mImageList;
        private System.Windows.Forms.ListView mScenesListView;
        private System.Windows.Forms.ContextMenuStrip mProjectViewContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem mAddSceneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mDeleteSceneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mExportSceneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mRenameToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mOpenToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem mCloseToolStripMenuItem;

    }
}