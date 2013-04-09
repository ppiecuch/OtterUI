using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;

using System.IO;

using Otter.UI;
using Otter.Project;
using Otter.UI.Resources;
using Otter.Containers;

namespace Otter.Export
{
    /// <summary>
    /// General utility class that helps with importing / exporting of data
    /// </summary>
    public class ImporterExporter
    {
        /// <summary>
        /// Returns the full list of necessary items to export.
        /// </summary>
        /// <param name="projectScene"></param>
        /// <returns></returns>
        public static ArrayList GetItemsToExport(List<GUIProjectEntry> entries)
        {
            ArrayList list = new ArrayList();

            foreach (GUIProjectEntry entry in entries)
            {
                ArrayList itemList = null;
                if (entry is GUIProjectScene)
                {
                    itemList = GetItemsToExport(entry as GUIProjectScene);
                }

                if (itemList != null)
                {
                    foreach (object item in itemList)
                    {
                        if (!list.Contains(item))
                            list.Add(item);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Returns the full list of necessary files to export.
        /// </summary>
        /// <param name="projectScene"></param>
        /// <returns></returns>
        public static ArrayList GetItemsToExport(GUIProjectScene projectScene)
        {
            ArrayList list = new ArrayList();

            // Collect the fonts we'll need to export
            foreach (GUIFont font in GUIProject.CurrentProject.Fonts)
            {
                if(font.Filename != "" && font.FontData != null)
                    list.Add(font);
            }

            if (projectScene.Scene == null)
                projectScene.Open();

            if (projectScene.Scene != null)
            {
                projectScene.Scene.GenerateTextureAtlasses();
                list.AddRange(projectScene.Scene.TextureAtlasses);

                // Collect the textures we'll need to export
                List<TextureInfo> tmpList = new List<TextureInfo>();
                foreach (TextureInfo info in projectScene.Scene.Textures)
                {
                    FinalTexture finalTexture = projectScene.Scene.GetFinalTexture(info.ID);

                    if (finalTexture.mTextureAtlas == null && !list.Contains(info))
                        list.Add(info);
                }

                // Collect the sounds we'll need to export
                foreach (SoundInfo info in projectScene.Scene.Sounds)
                {
                    if(info.Filename != "" && !list.Contains(info.Filename))
                        list.Add(info.Filename);
                }

                // Now add the scene
                list.Add(projectScene);
            }

            return list;
        }

        /// <summary>
        /// Exports a generic item.  Item can be a string (fileName) or GUIFont
        /// </summary>
        /// <param name="item"></param>
        /// <param name="platforms"></param>
        /// <returns></returns>
        public bool Export(object item, List<Platform> platforms)
        {
            // Try exporting as a fileName
            string filename = item as string;
            if (filename != null)
            {
                return Export(filename, platforms);
            }

            // Try exporting as a font object
            UI.GUIFont font = item as UI.GUIFont;
            if (font != null)
            {
                return Export(font, platforms);
            }

            TextureInfo info = item as TextureInfo;
            if (info != null)
            {
                return Export(info, platforms);
            }

            TextureAtlas atlas = item as TextureAtlas;
            if (atlas != null)
            {
                return Export(atlas, platforms);
            }

            GUIProjectScene projectScene = item as GUIProjectScene;
            if (projectScene != null)
            {
                return Export(projectScene, platforms);
            }

            return false;
        }

        /// <summary>
        /// Exports a project scene
        /// </summary>
        /// <param name="projectScene"></param>
        /// <param name="platforms"></param>
        /// <returns></returns>
        public bool Export(GUIProjectScene projectScene, List<Platform> platforms)
        {
            string filename = projectScene.Filename;

            IExportable exportableObject = null;

            if (projectScene.Scene != null)
            {
                exportableObject = projectScene.Scene;
            }
            else
            {
                exportableObject = UI.GUIScene.Load(GUIProject.CurrentProject.ProjectDirectory + "/" + filename, GUIProject.XmlAttributeOverrides);
            }

            if (exportableObject == null)
                return false;

            ((GUIScene)exportableObject).GenerateTextureAtlasses();
            filename = System.IO.Path.ChangeExtension(filename, ".gbs");

            foreach (Platform platform in platforms)
            {
                // Determine the destination file
                string destFile = platform.OutputDirectory + "/" + filename;
                if (!System.IO.Path.IsPathRooted(destFile))
                    destFile = GUIProject.CurrentProject.ProjectDirectory + "/" + destFile;

                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(destFile));

                // Export a general exportable object
                if (exportableObject != null)
                {
                    Export<IExportable>(exportableObject, destFile, platform);
                    continue;
                }
            }

            return true;
        }

        /// <summary>
        /// Exports a file.  Determines the file type and takes appropriate action.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="platforms"></param>
        /// <returns></returns>
        public bool Export(string filename, List<Platform> platforms)
        {
            IExportable exportableObject = null;

            string ext = System.IO.Path.GetExtension(filename).ToLower();

            // GUI Scene
            if (ext == ".gsc")
            {
                exportableObject = UI.GUIScene.Load(GUIProject.CurrentProject.ProjectDirectory + "/" + filename, GUIProject.XmlAttributeOverrides);
                if (exportableObject != null)
                {
                    filename = System.IO.Path.ChangeExtension(filename, ".gbs");
                }
            }

            foreach (Platform platform in platforms)
            {
                // Determine the destination file
                string destFile = platform.OutputDirectory + "/" + filename;
                if (!System.IO.Path.IsPathRooted(destFile))
                    destFile = GUIProject.CurrentProject.ProjectDirectory + "/" + destFile;
                
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(destFile));

                // Export a general exportable object
                if (exportableObject != null)
                {
                    Export<IExportable>(exportableObject, destFile, platform);
                    continue;
                }

                // Simply copy the source to the destination if no explicity export step is
                // defined.
                string sourceFile = GUIProject.CurrentProject.ProjectDirectory + "/" + filename;
                if (System.IO.File.Exists(sourceFile))
                {
                    System.IO.File.Copy(sourceFile, destFile, true);

                    FileAttributes attr = System.IO.File.GetAttributes(destFile);

                    // Remove the ReadOnly flag
                    attr &= ~FileAttributes.ReadOnly;
                    System.IO.File.SetAttributes(destFile, attr);
                }
            }

            return true;
        }

        /// <summary>
        /// Exports a texture atlas
        /// </summary>
        /// <param name="atlas"></param>
        /// <param name="platforms"></param>
        /// <returns></returns>
        public bool Export(TextureAtlas atlas, List<Platform> platforms)
        {
            if (atlas == null)
                return false;
            
            foreach (Platform platform in platforms)
            {
                // Determine the destination file
                string destFile = platform.OutputDirectory + "/" + atlas.mFilename;
                if (!System.IO.Path.IsPathRooted(destFile))
                    destFile = GUIProject.CurrentProject.ProjectDirectory + "/" + destFile;

                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(destFile));

                atlas.Save(destFile);
            }

            return true;

        }

        /// <summary>
        /// Exports a texture
        /// </summary>
        /// <param name="info"></param>
        /// <param name="platforms"></param>
        /// <returns></returns>
        public bool Export(TextureInfo info, List<Platform> platforms)
        {
            if (info == null)
                return false;
            
            string filename = info.Filename;
            string ext = System.IO.Path.GetExtension(filename).ToLower();

            foreach (Platform platform in platforms)
            {
                // Determine the destination file
                string destFile = platform.OutputDirectory + "/" + filename;
                if (!System.IO.Path.IsPathRooted(destFile))
                    destFile = GUIProject.CurrentProject.ProjectDirectory + "/" + destFile;

                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(destFile));

                string sourceFile = GUIProject.CurrentProject.ProjectDirectory + "/" + filename;
                if (System.IO.File.Exists(sourceFile))
                {
                    bool doRawCopy = true;

                    if (platform.SquarifyTextures)
                    {
                        Bitmap bitmap = new Bitmap(sourceFile);
                        if (bitmap.Width != 0 && bitmap.Height != 0)
                        {
                            int w = bitmap.Width;
                            int h = bitmap.Height;
                            if (w != h || (w & (w - 1)) != 0 || (h & (h - 1)) != 0)
                            {
                                int larger = w > h ? w : h;
                                if ((larger & (larger - 1)) != 0)
                                {
                                    larger--;
                                    larger |= larger >> 1;
                                    larger |= larger >> 2;
                                    larger |= larger >> 4;
                                    larger |= larger >> 8;
                                    larger |= larger >> 16;
                                    larger++;
                                }

                                Bitmap newBitmap = new Bitmap(bitmap, new Size(larger, larger));
                                newBitmap.Save(destFile, bitmap.RawFormat);

                                newBitmap.Dispose();
                                bitmap.Dispose();

                                doRawCopy = false;
                            }
                        }
                    }

                    if (doRawCopy)
                        System.IO.File.Copy(sourceFile, destFile, true);

                    FileAttributes attr = System.IO.File.GetAttributes(destFile);

                    // Remove the ReadOnly flag
                    attr &= ~FileAttributes.ReadOnly;
                    System.IO.File.SetAttributes(destFile, attr);
                }
            }

            return true;
        }

        /// <summary>
        /// Exports a font
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="platforms"></param>
        /// <returns></returns>
        public bool Export(GUIFont font, List<Platform> platforms)
        {
            if (font == null)
                return false;
            
            foreach (Platform platform in platforms)
            {
                // Determine the destination file

                int index = 0;
                foreach (TextureAtlas atlas in font.TextureAtlasses)
                {
                    string filename = font.Name + "_" + index + ".png";

                    if (font.Filename != "")
                        filename = font.Filename.Replace(System.IO.Path.GetFileName(font.Filename), filename);

                    string destFile = platform.OutputDirectory + "/" + filename;
                    if (!System.IO.Path.IsPathRooted(destFile))
                        destFile = GUIProject.CurrentProject.ProjectDirectory + "/" + destFile;

                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(destFile));
                    atlas.Save(destFile);

                    index++;
                }
            }

            return true;
        }

        /// <summary>
        /// Exports an exportable object to file
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="fileName"></param>
        public bool Export<T>(T obj, string filename, Platform platform) where T : IExportable
        {
            FileStream fs = null;
            PlatformBinaryWriter bw = null;
            bool bSuccess = false;

            try
            {
                fs = new FileStream(filename, FileMode.Create);
                bw = new PlatformBinaryWriter(fs, platform);

                obj.Export(bw);

                bw.Close();
                fs.Close();

                bSuccess = true;
            }
            catch (Exception ex)
            {
                System.Console.Write("Error exporting object: " + ex.Message);
            }
            finally
            {
                if (bw != null)
                    bw.Close();

                if (fs != null)
                    fs.Close();
            }

            return bSuccess;
        }
    }
}
