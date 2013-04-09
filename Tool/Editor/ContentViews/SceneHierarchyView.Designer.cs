namespace Otter.Editor.ContentViews
{
    partial class SceneHierarchyView
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
            this.mContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mCreateViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mDuplicateViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mRenameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mDeleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mSceneTreeView = new Aga.Controls.Tree.TreeViewAdv();
            this.mContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mContextMenuStrip
            // 
            this.mContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mCreateViewToolStripMenuItem,
            this.mDuplicateViewToolStripMenuItem,
            this.toolStripMenuItem1,
            this.mRenameToolStripMenuItem,
            this.mDeleteToolStripMenuItem});
            this.mContextMenuStrip.Name = "mContextMenuStrip";
            this.mContextMenuStrip.Size = new System.Drawing.Size(153, 120);
            this.mContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.mContextMenuStrip_Opening);
            // 
            // mCreateViewToolStripMenuItem
            // 
            this.mCreateViewToolStripMenuItem.Name = "mCreateViewToolStripMenuItem";
            this.mCreateViewToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.mCreateViewToolStripMenuItem.Text = "Create View";
            this.mCreateViewToolStripMenuItem.Click += new System.EventHandler(this.mCreateViewToolStripMenuItem_Click);
            // 
            // mDuplicateViewToolStripMenuItem
            // 
            this.mDuplicateViewToolStripMenuItem.Name = "mDuplicateViewToolStripMenuItem";
            this.mDuplicateViewToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.mDuplicateViewToolStripMenuItem.Text = "Duplicate View";
            this.mDuplicateViewToolStripMenuItem.Click += new System.EventHandler(this.mDuplicateViewToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
            // 
            // mRenameToolStripMenuItem
            // 
            this.mRenameToolStripMenuItem.Name = "mRenameToolStripMenuItem";
            this.mRenameToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.mRenameToolStripMenuItem.Text = "Rename";
            this.mRenameToolStripMenuItem.Click += new System.EventHandler(this.mRenameToolStripMenuItem_Click);
            // 
            // mDeleteToolStripMenuItem
            // 
            this.mDeleteToolStripMenuItem.Name = "mDeleteToolStripMenuItem";
            this.mDeleteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.mDeleteToolStripMenuItem.Text = "Delete";
            this.mDeleteToolStripMenuItem.Click += new System.EventHandler(this.mDeleteToolStripMenuItem_Click);
            // 
            // mSceneTreeView
            // 
            this.mSceneTreeView.AllowDrop = true;
            this.mSceneTreeView.BackColor = System.Drawing.SystemColors.Window;
            this.mSceneTreeView.ContextMenuStrip = this.mContextMenuStrip;
            this.mSceneTreeView.DefaultToolTipProvider = null;
            this.mSceneTreeView.DisplayDraggingNodes = true;
            this.mSceneTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mSceneTreeView.DragDropMarkColor = System.Drawing.Color.Black;
            this.mSceneTreeView.LineColor = System.Drawing.SystemColors.ControlDark;
            this.mSceneTreeView.Location = new System.Drawing.Point(0, 0);
            this.mSceneTreeView.Model = null;
            this.mSceneTreeView.Name = "mSceneTreeView";
            this.mSceneTreeView.SelectedNode = null;
            this.mSceneTreeView.SelectionMode = Aga.Controls.Tree.TreeSelectionMode.MultiSameParent;
            this.mSceneTreeView.Size = new System.Drawing.Size(333, 639);
            this.mSceneTreeView.TabIndex = 0;
            this.mSceneTreeView.SelectionChanged += new System.EventHandler(this.mSceneTreeView_SelectionChanged);
            this.mSceneTreeView.DragOver += new System.Windows.Forms.DragEventHandler(this.mSceneTreeView_DragOver);
            this.mSceneTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.mSceneTreeView_DragDrop);
            this.mSceneTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.mSceneTreeView_ItemDrag);
            // 
            // SceneHierarchyView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(333, 639);
            this.Controls.Add(this.mSceneTreeView);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SceneHierarchyView";
            this.Text = "Scene Hierarchy";
            this.mContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Aga.Controls.Tree.TreeViewAdv mSceneTreeView;
        private System.Windows.Forms.ContextMenuStrip mContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem mRenameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mDeleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mCreateViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mDuplicateViewToolStripMenuItem;
    }
}