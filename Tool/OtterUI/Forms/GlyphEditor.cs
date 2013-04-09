using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Otter.Project;

namespace Otter.Forms
{
    /// <summary>
    /// The Glyph Editor maintains and edits glyphs for a font
    /// </summary>
    public partial class GlyphEditor : Form
    {
        #region Data
        private ArrayList mGlyphs = new ArrayList();
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the glyphs
        /// </summary>
        public ArrayList Glyphs
        {
            get { return mGlyphs; }
            set { mGlyphs = value; }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public GlyphEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when the Glyph Editor has loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GlyphEditor_Load(object sender, EventArgs e)
        {
            PopulateDataGrid();
        }

        /// <summary>
        /// Populates the datagrid with glyph information
        /// </summary>
        private void PopulateDataGrid()
        {
            try
            {
                mGlyphDataGrid.Rows.Clear();

                Font font = new Font(Font.FontFamily, 24.0f);
                int imageW = 48;
                int imageH = 48;

                foreach (char c in mGlyphs.OfType<char>())
                {
                    int index = mGlyphDataGrid.Rows.Add();

                    // Prepare the glyph image cell
                    DataGridViewImageCell imageCell = mGlyphDataGrid.Rows[index].Cells[0] as DataGridViewImageCell;

                    Bitmap bitmap = new Bitmap(imageW, imageH);
                    Graphics graphics = Graphics.FromImage(bitmap);

                    string s = c.ToString();

                    SizeF size = graphics.MeasureString(s, font);
                    graphics.FillRectangle(Brushes.LightBlue, 0, 0, imageW, imageH);
                    graphics.DrawString(s, font, Brushes.Black, new PointF(imageW / 2 - size.Width / 2, imageH / 2 - size.Height / 2));

                    graphics.Dispose();

                    imageCell.Value = bitmap;

                    // Prepare the Character Code cell
                    DataGridViewTextBoxCell textCell = mGlyphDataGrid.Rows[index].Cells[1] as DataGridViewTextBoxCell;

                    string str = String.Format("\\u{0:0000}", (int)c);
                    textCell.Value = str;

                    // Finalize the row
                    mGlyphDataGrid.Rows[index].Tag = c;
                }

                foreach (FontBuilder.ImageGlyph imageGlyph in mGlyphs.OfType<FontBuilder.ImageGlyph>())
                {
                    int index = mGlyphDataGrid.Rows.Add();

                    // Prepare the glyph image cell
                    DataGridViewImageCell imageCell = mGlyphDataGrid.Rows[index].Cells[0] as DataGridViewImageCell;

                    string path = GUIProject.CurrentProject.ProjectDirectory + "/" + imageGlyph.ImagePath;
                    Image image = null;

                    if(System.IO.File.Exists(path))
                        image = Image.FromFile(path);

                    Bitmap bitmap = (image != null) ? new Bitmap(image, imageW, imageH) : new Bitmap(imageW, imageH);
                    imageCell.Value = bitmap;

                    // Prepare the Character Code cell
                    DataGridViewTextBoxCell textCell = mGlyphDataGrid.Rows[index].Cells[1] as DataGridViewTextBoxCell;

                    string str = String.Format("{{{0}}}", imageGlyph.ID);
                    textCell.Value = str;

                    // Finalize the row
                    mGlyphDataGrid.Rows[index].Tag = imageGlyph;

                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("EXCEPTION: " + ex);
            }
        }

        /// <summary>
        /// Brings up a popup to add new characters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mAddUnicodeCharsButton_Click(object sender, EventArgs e)
        {
            AddCharactersForm form = new AddCharactersForm();
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                // Compile new list of complete characters
                List<char> chars = form.Characters;
                chars = new List<char>(mGlyphs.OfType<char>().Union(chars).Distinct());

                // Extract previous list of images
                List<FontBuilder.ImageGlyph> images = new List<FontBuilder.ImageGlyph>(mGlyphs.OfType<FontBuilder.ImageGlyph>());

                // New list, add characters and sort
                mGlyphs = new ArrayList(chars);
                mGlyphs.Sort();

                // Add images at the end
                mGlyphs.AddRange(images);

                PopulateDataGrid();
            }
        }

        /// <summary>
        /// Called when the current selection has changed.  Enable/disable the "Remove selected"
        /// button accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mGlyphDataGrid_SelectionChanged(object sender, EventArgs e)
        {
            mRemoveSelectedButton.Enabled = (mGlyphDataGrid.SelectedRows.Count > 0);
        }

        /// <summary>
        /// User hit the "Remove Selected" button.  Remove all of the selected rows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mRemoveSelectedButton_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show(Otter.Properties.Resources.DELETE_GLYPHS,
                                                Otter.Properties.Resources.WARNING,
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Exclamation);
            if (res == DialogResult.No)
                return;
            
            foreach (DataGridViewRow row in mGlyphDataGrid.SelectedRows)
            {
                mGlyphs.Remove(row.Tag);

                mGlyphDataGrid.Rows.Remove(row);
            }
        }

        /// <summary>
        /// User hit the "Add Images" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mAddImagesButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.png, *.tga)|*.jpg;*.png;*.tga|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = GUIProject.CurrentProject.ProjectDirectory;
            openFileDialog.Multiselect = true;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            foreach (string fn in openFileDialog.FileNames)
            {
                FontBuilder.ImageGlyph glyph = new FontBuilder.ImageGlyph();
                glyph.ImagePath = Utils.GetRelativePath(GUIProject.CurrentProject.ProjectDirectory, fn);

                int count = 0;
                foreach (FontBuilder.ImageGlyph existingGlyph in mGlyphs.OfType<FontBuilder.ImageGlyph>())
                {
                    string newId = String.Format("{0:0000}", count);
                    if (existingGlyph.ID == newId)
                    {
                        count++;

                        if (count > 9999)
                        {
                            break;
                        }

                        continue;
                    }
                }

                if (count <= 9999)
                {
                    glyph.ID = String.Format("{0:0000}", count);

                    mGlyphs.Add(glyph);
                }
            }

            PopulateDataGrid();
        }

        /// <summary>
        /// Begins the cell edit if the user double-clicked on an editable cell
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mGlyphDataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = mGlyphDataGrid.Rows[e.RowIndex];
            DataGridViewCell cell = row.Cells[e.ColumnIndex];

            // We want to edit the name
            if (row.Tag is FontBuilder.ImageGlyph && e.ColumnIndex == 1)
            {
                DataGridViewTextBoxCell textCell = cell as DataGridViewTextBoxCell;
                string text = textCell.Value as string;

                if (textCell != null && text != null)
                {
                    text = text.Trim(new char[] { '{', '}' });
                    textCell.Value = text;

                    mGlyphDataGrid.CurrentCell = textCell;
                    mGlyphDataGrid.BeginEdit(false);
                }
            }
        }

        /// <summary>
        /// Editing has ended
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mGlyphDataGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = mGlyphDataGrid.Rows[e.RowIndex];
            DataGridViewCell cell = row.Cells[e.ColumnIndex];

            // We want to edit the name
            if (row.Tag is FontBuilder.ImageGlyph && e.ColumnIndex == 1)
            {
                DataGridViewTextBoxCell textCell = cell as DataGridViewTextBoxCell;
                string text = textCell.Value as string;

                if (textCell != null && text != null)
                {
                    FontBuilder.ImageGlyph glyph = row.Tag as FontBuilder.ImageGlyph;
                    text = text.Trim(new char[] { '{', '}' });

                    if (text != "")
                    {
                        // First check to see if this ID is not taken by anyone.
                        foreach (FontBuilder.ImageGlyph existingGlyph in mGlyphs.OfType<FontBuilder.ImageGlyph>())
                        {
                            if (existingGlyph.ID == text)
                            {
                                text = glyph.ID;
                                break;
                            }
                        }
                    }
                    else
                    {
                        text = glyph.ID;
                    }

                    glyph.ID = text;
                    textCell.Value = String.Format("{{{0}}}", glyph.ID);
                }
            }           
        }
    }
}
