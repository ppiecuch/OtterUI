using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Otter.UI;
using Otter.Interface;
using Otter.UI.Animation;

namespace Otter.Editor.Commands.ControlCommands
{
    /// <summary>
    /// Changes the control order of a set of controls
    /// </summary>
    public class ChangeOrderCommand : Otter.Commands.Command
    {
        #region Data
        private List<GUIControl> mControls = new List<GUIControl>();
        private List<int> mOriginalIndices = new List<int>();
        private int mStartIndex = -1;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="controls"></param>
        /// <param name="startIndex"></param>
        public ChangeOrderCommand(List<GUIControl> controls, int startIndex)
        {
            foreach (GUIControl control in controls)
            {
                if(control is GUIView)
                    mOriginalIndices.Add(control.Scene.Views.IndexOf((GUIView)control));
                else
                    mOriginalIndices.Add(control.Parent.Controls.IndexOf(control));
            }

            mControls.AddRange(controls);
            mStartIndex = startIndex;
        }

        /// <summary>
        /// Executes the command.  Sets the control's new parent to the one provided in the constructor
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            int index = mStartIndex;
            foreach (GUIControl control in mControls)
            {
                GUIView view = control as GUIView;
                if (view != null)
                {
                    GUIScene scene = view.Scene;

                    int prevIndex = scene.Views.IndexOf(view);
                    int targetIndex = index;

                    scene.RemoveView(view);

                    if (targetIndex == -1)
                    {
                        scene.AddView(view);
                    }
                    else
                    {
                        if (prevIndex < targetIndex)
                            targetIndex--;

                        scene.AddView(view, targetIndex);
                        index++;
                    }

                }
                else if(control.Parent != null)
                {
                    GUIControl parent = control.Parent;

                    int prevIndex = parent.Controls.IndexOf(control);
                    int targetIndex = index;

                    parent.Controls.Remove(control, false);

                    if (targetIndex == -1)
                    {
                        parent.Controls.Add(control);
                    }
                    else
                    {
                        if (prevIndex < targetIndex)
                            targetIndex--;

                        parent.Controls.Insert(targetIndex, control);
                        index++;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Undoes the set-parent command.  Instead of doing the opposite of the
        /// Execute function, we simply restore everything to their original values
        /// </summary>
        /// <returns></returns>
        public override bool Undo()
        {
            for (int i = mControls.Count - 1; i >= 0; i--)
            {
                GUIControl control = mControls[i];

                if (control is GUIView)
                {
                    GUIView view = control as GUIView;
                    GUIScene scene = view.Scene;
                    scene.Views.Remove(view);
                    scene.Views.Insert(mOriginalIndices[i], view);
                }
                else
                {
                    GUIControl parent = control.Parent;
                    parent.Controls.Remove(control, false);
                    parent.Controls.Insert(mOriginalIndices[i], control);
                }
            }       

            return true;
        }

        public override string ToString()
        {
            return "Reorder: " + mControls.Count + " Controls";
        }
    }
}
