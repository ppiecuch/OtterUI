using System.Collections.Generic;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Otter.UI;
using Otter.Forms;
using Otter.UI.Animation;
using Otter.Containers;

namespace Otter.TypeEditors
{
    class UIActionListEditor : UITypeEditor
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
        /// Called when we want to edit the value of a property.  Shows the ActionsEditor form.
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
                List<Otter.UI.Actions.Action> actions = value as List<Otter.UI.Actions.Action>;

                if (actions == null)
                    return value;

                GUIScene scene = null;
                
                GUIControl control = context.Instance as GUIControl;
                if (control != null)
                    scene = control.Scene;

                MainChannelFrame mainChannelFrame = context.Instance as MainChannelFrame;
                if (mainChannelFrame != null)
                    scene = mainChannelFrame.Animation.Scene;

                if(scene == null)
                    return value;

                ActionsEditor actionsEditor = new ActionsEditor(scene);
                actionsEditor.Actions = actions;

                if (actionsEditor.ShowDialog() == DialogResult.OK)
                {
                    if(control != null)
                        return actionsEditor.Actions;

                    NotifyingList<Otter.UI.Actions.Action> list = new NotifyingList<UI.Actions.Action>();
                    list.AddRange(actionsEditor.Actions);

                    return list;
                }

                return actions;
            }

            return value;
        }
    }
}
