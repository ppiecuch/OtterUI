using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

namespace Otter.Editor.ContentViews
{
    public partial class PropertiesView : DockContent
    {
        #region Events and Handlers
        public event PropertyValueChangedEventHandler PropertyValueChanged;
        #endregion

        #region Data
        private Timer mPropertyRefreshTimer = null;
        private bool mNeedsRefresh = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the internal property grid
        /// </summary>
        public PropertyGrid PropertyGrid
        {
            get { return mPropertyGrid; }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public PropertiesView()
        {
            InitializeComponent();

            this.HideOnClose = true;
            this.FormClosing += new FormClosingEventHandler(PropertiesView_FormClosing);

            mPropertyGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(mPropertyGrid_PropertyValueChanged);

            mPropertyRefreshTimer = new Timer();
            mPropertyRefreshTimer.Interval = 1;
            mPropertyRefreshTimer.Tick += new EventHandler(mPropertyRefreshTimer_Tick);
            mPropertyRefreshTimer.Start();
        }

        /// <summary>
        /// Called when the form is closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PropertiesView_FormClosing(object sender, FormClosingEventArgs e)
        {
            mPropertyRefreshTimer.Stop();
        }

        /// <summary>
        /// Timer tick callback.  If the property grid requires a refresh, do so now.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mPropertyRefreshTimer_Tick(object sender, EventArgs e)
        {
            if (mNeedsRefresh)
            {
                mPropertyGrid.Refresh();
                mNeedsRefresh = false;
            }
        }

        /// <summary>
        /// Signals that we should refresh the property grid
        /// </summary>
        public void RefreshProperties()
        {
            mNeedsRefresh = true;
        }

        /// <summary>
        /// Called when the property grid has had a value changed
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        void mPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (PropertyValueChanged != null)
                PropertyValueChanged(this, e);
        }
    }
}
