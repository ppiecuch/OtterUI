using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Otter.Editor.Exporting
{
    public partial class ProgressWindow : Form
    {
        #region Events and Delegates
        public delegate bool ObjectDelegate(object sender, object source);
        public delegate void ProcressingDelegate(object sender, object source);
        public delegate void ProcressedDelegate(object sender, object source, bool success);
        public delegate void ProgressDelegate(object sender);

        public event ProgressDelegate OnProgressComplete = null;
        public event ProgressDelegate OnProgressCancelled = null;
        #endregion

        #region Data
        private Thread mProcessThread = null;
        private bool mCanceledProcess = false;
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the delegate to process the object.  This delegate will be invoked
        /// once for every object in the object list.
        /// </summary>
        public ObjectDelegate ProcessObjectDelegate { get; set; }

        /// <summary>
        /// List of objects to process.
        /// </summary>
        public ArrayList ObjectList { get; set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public ProgressWindow()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// Called when the progress window is loaded.
        /// Kick off the thread that'll process items in the background.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProgressWindow_Load(object sender, EventArgs e)
        {
            // Set up the progress bar
            mProgressBar.Minimum = 0;
            mProgressBar.Maximum = ObjectList.Count;
            mProgressBar.Step = 1;

            mProcessThread = new Thread(new ParameterizedThreadStart(ProcessThread));
            mProcessThread.Start(null);
        }

        /// <summary>
        /// Called when an object is processing (or at least has started to process)
        /// </summary>
        /// <param name="obj"></param>
        private void ObjectProcessing(object sender, object obj)
        {
            mRichTextBox.AppendText("Processing: " + obj.ToString() + " ");
        }

        /// <summary>
        /// Called when an object has completed processing.
        /// </summary>
        /// <param name="obj"></param>
        private void ObjectProcessed(object sender, object obj, bool success)
        {
            string result = "[success]";
            if(!success)
            {
                result = "[FAILURE]";
            }

            mRichTextBox.AppendText(result + "\n");
            int resultIndex = mRichTextBox.Text.LastIndexOf(result);

            mRichTextBox.SelectionStart = resultIndex;
            mRichTextBox.SelectionLength = result.Length;

            if(success)
            {
                mRichTextBox.SelectionColor = Color.Green;                
            }
            else
            {
                mRichTextBox.SelectionColor = Color.Red;
                mRichTextBox.SelectionFont = new Font(mRichTextBox.Font, FontStyle.Bold);
            }
            mRichTextBox.SelectionLength = 0;
            mRichTextBox.ScrollToCaret();

            mProgressBar.Value += 1;
        }

        /// <summary>
        /// Called when the thread has started processing data
        /// </summary>
        private void ProcessingStarted()
        {
            mCloseButton.Text = "Cancel";
        }

        /// <summary>
        /// Called when the thread has ended processing data
        /// </summary>
        private void ProcessingEnded()
        {
            mCloseButton.Text = "Close";
            mCloseButton.Enabled = true;
            mProcessThread = null;

            if (!mCanceledProcess)
            {
                if (OnProgressComplete != null)
                    OnProgressComplete(this);
            }
            else
            {
                if(OnProgressCancelled != null)
                    OnProgressCancelled(this);
            }
        }

        /// <summary>
        /// Processes the object list.
        /// </summary>
        /// <param name="obj"></param>
        private void ProcessThread(object obj)
        {
            // We use the same object delegate to notify ourselves that an object has been updated.
            ProcressingDelegate objectProcessingDelegate = ObjectProcessing;
            ProcressedDelegate objectProcessedDelegate = ObjectProcessed;

            this.BeginInvoke(new MethodInvoker(ProcessingStarted));

            foreach (object o in ObjectList)
            {
                if (mCanceledProcess)
                    break;

                // Notify ourselves that we're starting to process an object
                this.BeginInvoke(objectProcessingDelegate, new object[]{this, o});

                // Call the delegate!  Yay!
                bool result = ProcessObjectDelegate(this, o);

                // Notify ourselves that we've finished processing an object
                this.BeginInvoke(objectProcessedDelegate, new object[] { this, o, result });
            }

            this.BeginInvoke(new MethodInvoker(ProcessingEnded));
        }

        /// <summary>
        /// Closes the sceneView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mCloseButton_Click(object sender, EventArgs e)
        {
            // If no thread is currently running, just close the dialog box
            // otherwise cancel processing.
            if (mProcessThread == null)
            {
                this.DialogResult = mCanceledProcess ? DialogResult.Cancel : DialogResult.OK;
                Close();
            }
            else
            {
                mCanceledProcess = true;
                mCloseButton.Enabled = false;
            }
        }
    }
}