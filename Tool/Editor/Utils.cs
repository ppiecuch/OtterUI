using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Otter.Interface;

namespace Otter.Editor
{
    /// <summary>
    /// Utility class.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Loads a bitmap from resource
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Bitmap LoadBitmapResource(string name)
        {
            Bitmap bitmap = null;
            Stream imageStream = null;
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                imageStream = assembly.GetManifestResourceStream(name);
                bitmap = new Bitmap(imageStream);
            }
            catch(Exception ex)
            {
                System.Console.WriteLine("Failed to load resource " + name + ".  Reason: " + ex.Message);
            }
            finally
            {
                if (imageStream != null)
                    imageStream.Close();
            }

            return bitmap;
        }
    }
}
