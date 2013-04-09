using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;

using Otter.Project;

namespace Otter.Editor.Forms
{
    /// <summary>
    /// The ReportBug Form is responsible for showing the user an error
    /// when it occurs and then reporting that error by emailing it out.
    /// </summary>
    public partial class ReportBugForm : Form
    {
        private Exception mException = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ex"></param>
        public ReportBugForm(Exception ex)
        {
            mException = ex;
            InitializeComponent();
        }

        /// <summary>
        /// Called when the form is loaded.  Display 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReportBugForm_Load(object sender, EventArgs e)
        {
            if (mException != null)
            {
                string errorString = "";

                try
                {
                    errorString += "-----------------------------\r\n";
                    errorString += "Please describe what you were doing:\r\n";
                    errorString += "\r\n";
                    errorString += "<enter description here>" + "\r\n";
                    errorString += "\r\n";

                    errorString += "-----------------------------\r\n";
                    errorString += "User:\r\n";
                    errorString += "\r\n";
                    errorString += Environment.UserName + "\r\n";
                    errorString += "\r\n";

                    errorString += "-----------------------------\r\n";
                    errorString += "Project:\r\n";
                    errorString += "\r\n";
                    errorString += GUIProject.CurrentProject != null ? GUIProject.CurrentProject.FullPath : "[No Project]" + "\r\n";
                    errorString += "\r\n";

                    errorString += "-----------------------------\r\n";
                    errorString += "Exception Type:\r\n";
                    errorString += "\r\n";
                    errorString += mException.GetType() + "\r\n";
                    errorString += "\r\n";

                    errorString += "-----------------------------\r\n";
                    errorString += "Message:\r\n";
                    errorString += "\r\n";
                    errorString += mException.Message + "\r\n";
                    errorString += "\r\n";

                    errorString += "-----------------------------\r\n";
                    errorString += "Stack:\r\n";
                    errorString += "\r\n";
                    errorString += mException.StackTrace;
                    errorString += "\r\n";
                }
                catch (Exception ex)
                {
                    errorString += "***********************************************\r\n";
                    errorString += "\r\nError while constructing error information:\r\n" + ex.Message;
                }

                int from = errorString.IndexOf('<');
                int to = errorString.Substring(from).IndexOf('>');

                mErrorTextBox.Text = errorString;
                mErrorTextBox.Select(from, to + 1);
            }
        }

        /// <summary>
        /// "Report" button clicked.  Forward the error off to somebody who can deal with it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // create mail message object
                MailMessage mail = new MailMessage("support@aonyxsoftware.com", "support@aonyxsoftware.com"); ;
                mail.Subject = "OtterUI : Unexpected Error";
                mail.Body = mErrorTextBox.Text;

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.Host = "smtp.1and1.com";
                smtpClient.Port = 587;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("dummy@aonyxsoftware.com", "1r0ndummy");
                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Failed to send error report: " + ex.Message);
            }

            this.Close();
        }
    }
}
