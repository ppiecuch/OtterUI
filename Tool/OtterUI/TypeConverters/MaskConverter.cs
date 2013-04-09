
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using Otter.UI;

namespace Otter.TypeConverters
{
    class MaskConverter : StringConverter
    {
        /// <summary>
        /// Returns true so the user can't type in his/her own mask.
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
        /// Converts the font id to its string representation
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

                int maskID = (int)value;
                GUIControl maskControl = control.ParentView.Controls.GetControl(maskID);

                if (maskControl != null)
                    return maskControl.ToString();
            }

            return "None";
        }
    }
}
