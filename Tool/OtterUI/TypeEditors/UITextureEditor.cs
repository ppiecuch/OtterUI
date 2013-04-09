using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Otter.UI;
using Otter.Forms;

namespace Otter.TypeEditors
{
    public class UITextureEditor : UITypeEditor
    {
        private IWindowsFormsEditorService edSvc;

        /// <summary>
        /// Specifies that the editor will create a modal control with which to
        /// edit values.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return System.Drawing.Design.UITypeEditorEditStyle.Modal;
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
                GUIControl control = context.Instance as GUIControl;
                if (control != null)
                {
                    TextureManagerForm form = new TextureManagerForm(control.Scene, true);

                    form.SelectedTexture = control.Scene.GetTextureInfo((int)value);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        return (form.SelectedTexture != null ? form.SelectedTexture.ID : -1);
                    }

                    return (int)value;
                }
            }

            return value;
        }
    }
}
