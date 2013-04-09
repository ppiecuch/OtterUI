using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Otter.UI;

namespace Otter.TypeEditors
{
    class UIAnchorEditor : UITypeEditor
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
        /// Called when we want to edit the value of a property.  Displays the AnchorEditor.
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
                GUIControl control = context.Instance as GUIControl;
                if (control != null)
                {
                    CustomControls.AnchorEditor editor = new CustomControls.AnchorEditor();
                    editor.AnchorFlags = (AnchorFlags)value;

                    edSvc.DropDownControl(editor);

                    return editor.AnchorFlags;
                }
            }

            return value;
        }
    }
}