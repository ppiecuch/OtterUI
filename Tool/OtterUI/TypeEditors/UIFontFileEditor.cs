using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Otter.UI;
using Otter.Project;

namespace Otter.TypeEditors
{
    class UIFontFileEditor : UITypeEditor
    {
        private IWindowsFormsEditorService edSvc;

        /// <summary>
        /// Specifies that the editor will create a modal control with which to edit values.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return System.Drawing.Design.UITypeEditorEditStyle.Modal;
        }

        /// <summary>
        /// Called when we want to edit the value of a property. Displays the Open File Dialog to open TTF files.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc != null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Font files (*.ttf)|*.ttf|All files (*.*)|*.*";
                openFileDialog.InitialDirectory = GUIProject.CurrentProject.ProjectDirectory;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    return openFileDialog.FileName;
                }
            }

            return value;
        }
    }
}
