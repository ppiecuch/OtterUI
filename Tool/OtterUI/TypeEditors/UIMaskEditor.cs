
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Otter.UI;

namespace Otter.TypeEditors
{
    class UIMaskEditor : UITypeEditor
    {
        private IWindowsFormsEditorService edSvc;

        /// <summary>
        /// Specifies that the AnimationEditor will create a dropdown control with which to
        /// edit values.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return System.Drawing.Design.UITypeEditorEditStyle.DropDown;
        }

        /// <summary>
        /// Called when we want to edit the value of the property.  Brings up the a drop down list box of masks.
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
                GUIControl controlContext = context.Instance as GUIControl;

                ListBox listBox = new ListBox();
                listBox.Items.Add("None");
                AddControls(listBox, controlContext.ParentView.Controls);

                listBox.SelectedItem = value;
                listBox.SelectedIndexChanged += (s, e) => { edSvc.CloseDropDown(); };

                edSvc.DropDownControl(listBox);

                //no valid item selected; return previous value
                if (listBox.SelectedIndex == -1 || listBox.SelectedItem == null)
                    return value;

                //"None" selected
                if (listBox.SelectedIndex == 0)
                    return -1;

                return (listBox.SelectedItem as GUIMask).ID;
            }

            return value;
        }

        private void AddControls(ListBox listBox, GUIControlCollection controls)
        {
            foreach (GUIControl control in controls)
            {
                if (control.GetType() == typeof(GUIMask))
                    listBox.Items.Add(control);

                AddControls(listBox, control.Controls);
            }
        }
    }
}
