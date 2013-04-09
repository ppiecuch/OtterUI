using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Otter.Forms
{
    public partial class AddCharactersForm : Form
    {
        #region Properties
        /// <summary>
        /// Retrieves the entered characters as a sorted, distinct array
        /// of characters
        /// </summary>
        public List<char> Characters
        {
            get
            {
                string text = mTextBox.Text;

                List<char> chars = new List<char>(text.Distinct());
                chars.Sort();

                return chars;
            }
        }

        #endregion

        public AddCharactersForm()
        {
            InitializeComponent();
        }
    }
}
