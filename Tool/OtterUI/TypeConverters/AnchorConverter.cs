using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

using Otter.UI;

namespace Otter.TypeConverters
{
    class AnchorConverter : StringConverter
    {
        /// <summary>
        /// Specifies that we cannot convert from a string
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
                    AnchorFlags flags = (AnchorFlags)value;
                    if (flags == AnchorFlags.None)
                        return "None";

                    List<string> components = new List<string>();

                    if ((flags & AnchorFlags.Top) == AnchorFlags.Top)
                        components.Add("Top");
                    if ((flags & AnchorFlags.TopRelative) == AnchorFlags.TopRelative)
                        components.Add("Top Relative");
                    if ((flags & AnchorFlags.Bottom) == AnchorFlags.Bottom)
                        components.Add("Bottom");
                    if ((flags & AnchorFlags.BottomRelative) == AnchorFlags.BottomRelative)
                        components.Add("Bottom Relative");
                    if ((flags & AnchorFlags.Left) == AnchorFlags.Left)
                        components.Add("Left");
                    if ((flags & AnchorFlags.LeftRelative) == AnchorFlags.LeftRelative)
                        components.Add("Left Relative");
                    if ((flags & AnchorFlags.Right) == AnchorFlags.Right)
                        components.Add("Right");
                    if ((flags & AnchorFlags.RightRelative) == AnchorFlags.RightRelative)
                        components.Add("Right Relative");

                    string retString = "";
                    for (int i = 0; i < components.Count; i++)
                    {
                        retString += components[i];
                        if (i < components.Count - 1)
                            retString += ", ";
                    }

                    return retString;
                }
            }

            return "None";
        }
    }
}
