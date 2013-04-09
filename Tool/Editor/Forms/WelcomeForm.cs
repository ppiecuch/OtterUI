using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Otter.Editor.Forms
{
    public partial class WelcomeForm : Form
    {
        public delegate void LoadRecentProjectDelegate(object sender, string path);
        public delegate void WelcomeActionDelegate(object sender);

        public event LoadRecentProjectDelegate OnLoadRecentProject;
        public event WelcomeActionDelegate OnLoadProject;
        public event WelcomeActionDelegate OnNewProject;

        /// <summary>
        /// Constructor
        /// </summary>
        public WelcomeForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when the form has loaded - sets up the recent projects list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WelcomeForm_Load(object sender, EventArgs e)
        {
            RefreshRecentProjects();
        }

        /// <summary>
        /// Refreshes and repopulates recent projects list
        /// </summary>
        public void RefreshRecentProjects()
        {
            mRecentProjectsPanel.Controls.Clear();

            try
            {
                int count = 0;
                foreach (string str in Otter.Editor.Properties.Settings.Default.RecentProjects)
                {
                    string filename = System.IO.Path.GetFileName(str);

                    Button button = new Button();

                    button.FlatAppearance.BorderSize = 0;
                    button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.LightSteelBlue;
                    button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                    button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    button.ImageIndex = 2;
                    button.ImageList = this.imageList1;
                    button.Location = new System.Drawing.Point(0, count * 36);
                    button.Name = "button_" + filename;
                    button.Size = new System.Drawing.Size(159, 36);
                    button.TabIndex = 20;
                    button.Text = filename;
                    button.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    button.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
                    button.UseVisualStyleBackColor = true;
                    button.Tag = str;

                    button.Click += new EventHandler(RecentButton_Click);

                    mRecentProjectsPanel.Controls.Add(button);

                    if (++count >= 3)
                        break;
                }
            }
            catch(Exception)
            {
                mRecentProjectsPanel.Controls.Clear();
            }
        }

        /// <summary>
        /// Called when the user clicked on any of the "recent" items.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RecentButton_Click(object sender, EventArgs e)
        {
            if (OnLoadRecentProject != null)
                OnLoadRecentProject(this, ((Button)sender).Tag as string);
        }

        /// <summary>
        /// "Show Welcome" checkbox changed - save the user settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mShowWelcomeCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Otter.Editor.Properties.Settings.Default.ShowWelcome = mShowWelcomeCheckbox.Checked;
            Otter.Editor.Properties.Settings.Default.Save();
        }

        /// <summary>
        /// User clicked on the "New Project..." button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mNewProjectButton_Click(object sender, EventArgs e)
        {
            if (OnNewProject != null)
                OnNewProject(this);
        }

        /// <summary>
        /// User clicked on the "Load Project..." button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mLoadProjectButton_Click(object sender, EventArgs e)
        {
            if (OnLoadProject != null)
                OnLoadProject(this);
        }

    }
}
