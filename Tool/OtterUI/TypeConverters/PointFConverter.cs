using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Otter.TypeConverters
{
    public class PointFConverter : StringConverter
    {    
        /// <summary>
        /// Specifies that we can convert from a string to a Float3
        /// </summary>
        /// <param mName="context"></param>
        /// <param mName="sourceType"></param>
        /// <returns></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

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
        /// Converts from an object
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                string str = (string)value;
                string[] tokens = str.Split(new char[] { ',', ' ', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);

                if (tokens.Length == 2)
                {
                    return new System.Drawing.PointF(float.Parse(tokens[0]), float.Parse(tokens[1]));
                }
            }

            return base.ConvertFrom(context, culture, value);
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
                System.Drawing.PointF pt = (System.Drawing.PointF)value;
                return "" + pt.X + ", " + pt.Y;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
