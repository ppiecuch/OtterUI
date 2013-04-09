using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using Otter.UI;
using Otter.UI.Resources;

namespace Otter.TypeConverters
{
    public class TextureConverter : StringConverter
    {
        /// <summary>
        /// Specifies that we can convert from a string
        /// </summary>
        /// <param mName="context"></param>
        /// <param mName="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return false;

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Specifies that we can convert to a string.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts to its string representation
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                GUIControl control = context.Instance as GUIControl;
                if (control != null)
                {
                    int textureID = (int)value;
                    TextureInfo info = control.Scene.GetTextureInfo(textureID);

                    if (info != null)
                        return info.Filename;
                }
            }

            return "None";
        }
    }
}
