using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Otter.Export
{
    /// <summary>
    /// Endian enumerations
    /// </summary>
    public enum Endian
    {
        Little,
        Big
    };

    /// <summary>
    /// Color Format
    /// </summary>
    public enum ColorFormat
    {
        RGBA,
        BGRA,
        ABGR,
        ARGB,
    };

    /// <summary>
    /// Binary writer that will write data in the target endianness
    /// </summary>
    public class PlatformBinaryWriter : BinaryWriter
    {
        /// <summary>
        /// Gets / Sets the little-endian flag for this writer.  If false, write data in Big Endian, otherwise Little.
        /// </summary>
        public Platform TargetPlatform { get; set; }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="targetEndian"></param>
        public PlatformBinaryWriter(Stream stream, Platform targetPlatform)
            : base(stream)
        {
            TargetPlatform = targetPlatform;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        public void Write(System.Drawing.Color color)
        {
            int outColor = 0;

            switch (TargetPlatform.ColorFormat)
            {
                case ColorFormat.ARGB:
                    {
                        outColor = color.A << 24 | color.R << 16 | color.G << 8 | color.B;
                        break;
                    }
                case ColorFormat.ABGR:
                    {
                        outColor = color.A << 24 | color.B << 16 | color.G << 8 | color.R;
                        break;
                    }
                case ColorFormat.RGBA:
                    {
                        outColor = color.R << 24 | color.G << 16 | color.B << 8 | color.A;
                        break;
                    }
                case ColorFormat.BGRA:
                    {
                        outColor = color.B << 24 | color.G << 16 | color.R << 8 | color.A;
                        break;
                    }
            }

            Write(outColor);
        }

        /// <summary>
        /// Writes an int value
        /// </summary>
        /// <param name="value"></param>
        public override void Write(int value)
        {
            if (TargetPlatform.Endianness == Endian.Big)
            {
                // Need to write this int into big-endian format.
                byte[] bytes = System.BitConverter.GetBytes(value);

                // From little-endian to big-endian
                Write(bytes[3]);
                Write(bytes[2]);
                Write(bytes[1]);
                Write(bytes[0]);

                return;
            }

            base.Write(value);
        }

        /// <summary>
        /// Writes out a uint value
        /// </summary>
        /// <param name="value"></param>
        public override void Write(uint value)
        {
            if (TargetPlatform.Endianness == Endian.Big)
            {
                // Need to write this int into big-endian format.
                byte[] bytes = System.BitConverter.GetBytes(value);

                // From little-endian to big-endian
                Write(bytes[3]);
                Write(bytes[2]);
                Write(bytes[1]);
                Write(bytes[0]);

                return;
            }

            base.Write(value);
        }

        /// <summary>
        /// Writes a float value
        /// </summary>
        /// <param name="value"></param>
        public override void Write(float value)
        {
            if (TargetPlatform.Endianness == Endian.Big)
            {
                // Need to write this int into big-endian format.
                byte[] bytes = System.BitConverter.GetBytes(value);

                // From little-endian to big-endian
                Write(bytes[3]);
                Write(bytes[2]);
                Write(bytes[1]);
                Write(bytes[0]);

                return;
            }

            base.Write(value);
        }
    }
}
