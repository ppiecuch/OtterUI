using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Otter.UI;
using Otter.Forms;

namespace Otter.TypeEditors
{
    class UIGlyphEditor : UITypeEditor
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
        /// Called when we want to edit the value of a property.  Brings up the Glyph Editor control.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc != null && value is ArrayList)
            {
                GlyphEditor glyphEditor = new GlyphEditor();
                glyphEditor.Glyphs = value as ArrayList;

                if (edSvc.ShowDialog(glyphEditor) == DialogResult.OK)
                    return new ArrayList(glyphEditor.Glyphs);
            }

            return value;
        }
    }
}
