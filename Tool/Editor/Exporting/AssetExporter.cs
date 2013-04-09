using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;

using Otter.Export;
using Otter.Project;

namespace Otter.Editor.Exporting
{
    public partial class AssetExporter : Form
    {
        #region Data
        private GUIProject mProject = null;
        private ArrayList mItemsToExport = null;

        private ImporterExporter mExporter = null;
        #endregion

        /// <summary>
        /// Constructs this sceneView with a list of files for exporting.
        /// </summary>
        /// <param name="filesToExport"></param>
        public AssetExporter(GUIProject project, ArrayList itemsToExport)
        {
            mProject = project;
            mItemsToExport = itemsToExport;

            InitializeComponent();
            
            // Add the platforms to the listbox.
            for (int i = 0; i < mProject.Platforms.Count; i++)
                mPlatformsListBox.Items.Add(mProject.Platforms[i], true);

            // Add the items that we want to export.  Can be of any type.
            for (int i = 0; i < itemsToExport.Count; i++)
                mFilesListBox.Items.Add(itemsToExport[i], true);
        }

        /// <summary>
        /// Exports the selected files for the selected platforms
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mExportButton_Click(object sender, EventArgs e)
        {
            mExporter = new ImporterExporter();

            ArrayList list = new ArrayList();
            foreach (object obj in mFilesListBox.CheckedItems)
                list.Add(obj);

            // The Progress Window kicks off a thread that will individually process
            // each of our items.
            ProgressWindow progressWindow = new ProgressWindow();
            progressWindow.ObjectList = list;
            progressWindow.ProcessObjectDelegate = new ProgressWindow.ObjectDelegate(ProcessFileExport);

            progressWindow.OnProgressComplete += new ProgressWindow.ProgressDelegate(progressWindow_OnProgressComplete);

            DialogResult result = progressWindow.ShowDialog();

            this.Close();
        }

        /// <summary>
        /// Called when the export process has completed successfully.
        /// </summary>
        /// <param name="sender"></param>
        void progressWindow_OnProgressComplete(object sender)
        {
            List<Platform> platforms = new List<Platform>(mPlatformsListBox.CheckedItems.Cast<Platform>());
            foreach (Platform platform in platforms)
            {
                string script = platform.PostExportScript;
                if (script == "")
                    continue;

                string[] lines = script.Split("\n\r".ToCharArray());
                foreach (string line in lines)
                {
                    try
                    {
                        // Poor man's variables
                        string l = line.Replace("${OutputDir}", platform.OutputDirectory);

                        // create the ProcessStartInfo using "cmd" as the program to be run,
                        // and "/c " as the parameters.
                        // Incidentally, /c tells cmd that we want it to execute the command that follows,
                        // and then exit.
                        System.Diagnostics.ProcessStartInfo procStartInfo =new System.Diagnostics.ProcessStartInfo("cmd", "/c " + l);

                        // The following commands are needed to redirect the standard output.
                        // This means that it will be redirected to the Process.StandardOutput StreamReader.
                        procStartInfo.RedirectStandardOutput = true;
                        procStartInfo.UseShellExecute = false;
                        procStartInfo.WorkingDirectory = Otter.Project.GUIProject.CurrentProject.ProjectDirectory;

                        // Do not create the black window.
                        procStartInfo.CreateNoWindow = true;

                        // Now we create a process, assign its ProcessStartInfo and start it
                        System.Diagnostics.Process proc = new System.Diagnostics.Process();
                        proc.StartInfo = procStartInfo;
                        proc.Start();

                        // Get the output into a string
                        string result = proc.StandardOutput.ReadToEnd();

                        // Display the command output.
                        Console.WriteLine(result);
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("Exception : " + ex);
                    }
                }
            }
        }

        /// <summary>
        /// Processes a file for export.  "source" is a string that will
        /// contain a fileName (full path)  
        /// 
        /// NOTE! : Called from the Progress Window on a SEPARATE THREAD!  Meaning, we cannot 
        /// update any controls directly from this function!  Use MethodInvoker or delegate.BeginInvoke
        /// instead!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        private bool ProcessFileExport(object sender, object source)
        {
            List<Platform> platforms = new List<Platform>(mPlatformsListBox.CheckedItems.Cast<Platform>());

            if(platforms.Count == 0)
                return false;

            return mExporter.Export(source, platforms);
        }

        /// <summary>
        /// Occurs when an item is about the have it's check state changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mPlatformsListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            int numChecked = mPlatformsListBox.CheckedIndices.Count + (e.NewValue == CheckState.Checked ? 1 : -1);
            mExportButton.Enabled = (numChecked != 0);
        }

        /// <summary>
        /// Closes the sceneView
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mCancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
