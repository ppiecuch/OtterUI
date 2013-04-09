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
    public partial class LoadingForm : Form
    {
        public delegate void ActionDelegate();

        public Label Status
        {
            get { return mStatusLabel; }
        }

        public ProgressBar ProgressBar
        {
            get { return mProgressBar; }
        }

        public ActionDelegate Action
        {
            get;
            set;
        }

        public LoadingForm()
        {
            InitializeComponent();

            this.Shown += new EventHandler(LoadingForm_Shown);
        }

        void LoadingForm_Shown(object sender, EventArgs e)
        {
            if (Action != null)
                Action();

            this.Close();
        }
    }
}
