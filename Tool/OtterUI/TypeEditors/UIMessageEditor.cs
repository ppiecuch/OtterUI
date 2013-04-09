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
    class UIMessageEditor : UITypeEditor
    {
        private IWindowsFormsEditorService edSvc;

        /// <summary>
        /// Specifies that the editor will create a dropdown control with which to
        /// edit values.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return System.Drawing.Design.UITypeEditorEditStyle.DropDown;
        }

        /// <summary>
        /// Called when we want to edit the value of a property.
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
                UI.Actions.MessageAction messageAction = context.Instance as UI.Actions.MessageAction;
                if (messageAction != null)
                {
                    ListBox listBox = new ListBox();
                    foreach (UI.Message message in messageAction.Scene.Messages)
                    {
                        listBox.Items.Add(message);
                    }

                    listBox.SelectedItem = messageAction.Scene.GetMessage((int)value);
                    listBox.SelectedIndexChanged += ((s, e) => { edSvc.CloseDropDown(); });

                    edSvc.DropDownControl(listBox);

                    return listBox.SelectedItem != null ? (listBox.SelectedItem as UI.Message).ID : -1;
                }
            }

            return value;
        }
    }
}
