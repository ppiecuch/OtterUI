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
    class UIFontEditor : UITypeEditor
    {
        private IWindowsFormsEditorService edSvc;

        /// <summary>
        /// Specifies that the editor will create a dropdown control with which to edit values.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return System.Drawing.Design.UITypeEditorEditStyle.DropDown;
        }

        /// <summary>
        /// Called when we want to edit the value of a property.  Displays a listbox with the available fonts.
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
                ListBox listBox = new ListBox();
                foreach (GUIFont font in GUIProject.CurrentProject.Fonts)
                {
                    listBox.Items.Add(font);
                }

                listBox.SelectedItem = GUIProject.CurrentProject.GetFont((int)value);
                listBox.SelectedIndexChanged += (s, e) => { edSvc.CloseDropDown(); };

                edSvc.DropDownControl(listBox);

                return listBox.SelectedItem != null ? (listBox.SelectedItem as GUIFont).ID : -1;
            }

            return value;
        }
    }
}
