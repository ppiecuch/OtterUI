using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using Otter.UI;
using Otter.Project;

namespace Otter.TypeConverters
{
    class FontConverter : StringConverter
    {
        /// <summary>
        /// Returns true so the user can't type in his/her own font names.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        /// Specifies that we can convert from a string to a Float3
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
        /// Specifies that we can convert from a Float3 to a string.
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
        /// Converts the font count to its string representation
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
                int fontID = (int)value;
                GUIFont font = GUIProject.CurrentProject.GetFont(fontID);

                if (font != null)
                    return font.Name;
            }

            return "None";
        }
    }
}
