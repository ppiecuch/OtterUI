using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Otter.Interface;

namespace Otter
{
    /// <summary>
    /// Utility class.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Converts a string into a fixed char array, with a null char delimiter
        /// </summary>
        /// <param name="str"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static byte[] StringToBytes(string str, int alignment)
        {
            if (str == null)
                str = "";

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);

            // Check if alignment is a power of two
            if ((alignment & (alignment - 1)) == 0)
            {
                // +1 to account for null-character
                int alignedSize = ((bytes.Length + 1) + (alignment - 1)) & ~(alignment - 1);

                if (alignedSize > bytes.Length)
                {
                    byte[] newBytes = new byte[alignedSize];
                    bytes.CopyTo(newBytes, 0);

                    bytes = newBytes;
                }
            }

            return bytes;
        }
        
        /// <summary>
        /// Generates a relative path from an absolute path
        /// 6to an absolute path.
        /// </summary>
        /// <param mName="relativeFrom"></param>
        /// <param mName="absoluteTo"></param>
        /// <returns></returns>
        public static string GetRelativePath(string absoluteFrom, string absoluteTo)
        {
            // We have to made make sure these paths share the same root (ie, drive letter).
            // If they don't, just return the absolute to.
            if (Path.GetPathRoot(absoluteFrom) != Path.GetPathRoot(absoluteTo))
                return absoluteTo;

            // Remove the first 'common' folders.
            int firstMisMatch = 0;

            string[] splitFrom = absoluteFrom.Split(new char[] { '/', '\\'  });
            string[] splitTo = absoluteTo.Split(new char[] { '/', '\\' });
            
            for (int i = 0; i < splitFrom.Length || i < splitTo.Length; i++)
            {
                if (i >= splitFrom.Length ||
                    i >= splitTo.Length ||
                    splitFrom[i] != splitTo[i])
                {
                    firstMisMatch = i;
                    break;
                }
            };

            string relativePath = "";

            // if there were no mismatches, then the paths were the same, so don't return anything for relative path!
            if (firstMisMatch != 0)
            {
                for (int i = firstMisMatch; i < splitTo.Length; i++)
                    relativePath += (i != firstMisMatch ? "/" : "") + splitTo[i];

                for (int i = firstMisMatch; i < splitFrom.Length; i++)
                    relativePath = "../" + relativePath;
            }

            return relativePath;
        }
        
        /// <summary>
        /// Returns a clamped value between low and high (inclusive)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="low"></param>
        /// <param name="value"></param>
        /// <param name="high"></param>
        public static T LimitRange<T>(T low, T value, T high) where T : IComparable
        {
            if (value.CompareTo(low) < 0)
                return low;

            else if (value.CompareTo(high) > 0)
                return high;

            return value;
        }

        // Convert an object to a byte array
        public static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        // Convert a byte array to an Object
        public static Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);
            return obj;
        }
 
        

        /// <summary>
        /// Expands the image by giving it a border by repeating the outermost pixel edges
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Bitmap ExpandImageBorder(Image image, int atlasPadding, bool clear)
        {
            if (image == null)
                return null;

            Bitmap bitmap = new Bitmap(image.Width + (atlasPadding * 2), image.Height + (atlasPadding * 2));
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);

            // Entire image
            graphics.DrawImageUnscaledAndClipped(image, new Rectangle(atlasPadding, atlasPadding, image.Width, image.Height));
            graphics.Dispose();

            if (!clear)
            {
                Bitmap src = new Bitmap(image);
                for (int i = 0; i < atlasPadding; i++)
                {
                    // Top
                    for (int x = 0; x < src.Width; x++)
                        bitmap.SetPixel(x + atlasPadding, i, src.GetPixel(x, 0));

                    // Bottom
                    for (int x = 0; x < src.Width; x++)
                        bitmap.SetPixel(x + atlasPadding, bitmap.Height - 1 - i, src.GetPixel(x, src.Height - 1));

                    // Left
                    for (int y = 0; y < src.Height; y++)
                        bitmap.SetPixel(i, y + atlasPadding, src.GetPixel(0, y));

                    // Right
                    for (int y = 0; y < src.Height; y++)
                        bitmap.SetPixel(bitmap.Width - 1 - i, y + atlasPadding, src.GetPixel(src.Width - 1, y));

                    // Corners
                    for (int j = 0; j < atlasPadding; j++)
                    {
                        // TL
                        bitmap.SetPixel(i, j, src.GetPixel(0, 0));

                        // BL
                        bitmap.SetPixel(i, bitmap.Height - 1 - j, src.GetPixel(0, src.Height - 1));

                        // TR
                        bitmap.SetPixel(bitmap.Width - 1 - i, j, src.GetPixel(src.Width - 1, 0));

                        // BR
                        bitmap.SetPixel(bitmap.Width - 1 - i, bitmap.Height - 1 - j, src.GetPixel(src.Width - 1, src.Height - 1));
                    }
                }

                src.Dispose();
            }
            return bitmap;
        }
    }
}
