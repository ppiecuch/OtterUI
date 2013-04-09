using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

using Otter.Project;

namespace Otter.UI.Resources
{
    /// <summary>
    /// Maintains info on a specific texture
    /// </summary>
    [Serializable]
    public class TextureInfo : Resource
    {
        #region Enums
        public enum AtlasBorder
        {
            Default,
            Clear,
            RepeatPixel
        }
        #endregion

        #region Data
        private string mTextureFile = "";
        private int mTextureID = -1;

        private Bitmap mThumbnail = null;

        private bool mNoAtlas = false;
        private int mAtlasBorderWidth = -1;
        private AtlasBorder mAtlasBorderType = AtlasBorder.RepeatPixel;

        private int mWidth = 0;
        private int mHeight = 0;

        private float mDisplayQuality = 1.0f;

        private FileSystemWatcher mWatcher = new FileSystemWatcher();
        private System.DateTime mTimestamp = System.DateTime.Now;

        private byte[] mImageData = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the fileName of the texture info
        /// </summary>
        [XmlAttribute]
        public string Filename
        {
            get { return mTextureFile; }
            set { mTextureFile = value; }
        }

        /// <summary>
        /// Retrieves the internal texture id
        /// </summary>
        [XmlIgnore]
        public int TextureID
        {
            get { return mTextureID; }
            set { mTextureID = value; }
        }

        /// <summary>
        /// Gets the texture thumbnail if loaded
        /// </summary>
        public Bitmap Thumbnail
        {
            get { return mThumbnail; }
        }

        /// <summary>
        /// If true, this texture will not be added to a texture atlas
        /// </summary>
        public bool NoAtlas
        {
            get { return mNoAtlas; }
            set { mNoAtlas = value; }
        }

        /// <summary>
        /// Gets / Sets the Atlas Border Width.  If < 0, will default to system
        /// settings
        /// </summary>
        public int AtlasPadding
        {
            get { return mAtlasBorderWidth; }
            set { mAtlasBorderWidth = value; }
        }

        /// <summary>
        /// Gets  sets the Atlas Border Type
        /// </summary>
        public AtlasBorder AtlasBorderType
        {
            get { return mAtlasBorderType; }
            set { mAtlasBorderType = value; }
        }

        /// <summary>
        /// Retrieves the texture's width
        /// </summary>
        public int Width
        {
            get { return mWidth; }
        }

        /// <summary>
        /// Retrieves the texture's height
        /// </summary>
        public int Height
        {
            get { return mHeight; }
        }

        /// <summary>
        /// 
        /// </summary>
        public float DisplayQuality
        {
            get { return mDisplayQuality; }
            set 
            { 
                float quality = (value > 1.0f) ? 1.0f : (value <= 0.0f ? 0.01f : value);
                if (mDisplayQuality != quality)
                {
                    mDisplayQuality = quality;
                    if(mTextureID > 0)
                        ReloadInternalTexture();
                }
            }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public TextureInfo()
        {
            mWatcher.NotifyFilter = NotifyFilters.FileName |
                                    NotifyFilters.CreationTime |
                                    NotifyFilters.LastWrite;

            mWatcher.Changed += new FileSystemEventHandler(mWatcher_Changed);
            mWatcher.Created += new FileSystemEventHandler(mWatcher_Changed);
        }

        /// <summary>
        /// Loads the GUI DeviceTexture
        /// </summary>
        /// <returns></returns>
        public override bool Load()
        {
            if (Otter.Interface.Graphics.Instance == null || mTextureID != -1)
                return false;

            string path = GUIProject.CurrentProject.ProjectDirectory + "/" + Filename;

            if (!System.IO.File.Exists(path))
                return false;

            int x = 0;
            int y = 0;
            int w = 0;
            int h = 0;

            int thumbWidth = 256;
            int thumbHeight = 256;

            Image image = null;
            mImageData = null;

            try
            {
                if (Path.GetExtension(path) == ".pcx")
                {
                    image = Imaging.PCXLoader.Load(path);
                    if (image != null)
                    {
                        int newW = (int)(image.Width * DisplayQuality);
                        int newH = (int)(image.Height * DisplayQuality);

                        Bitmap bitmap = new Bitmap(image, newW, newH);
                        BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

                        mImageData = new byte[data.Stride * data.Height];
                        Marshal.Copy(data.Scan0, mImageData, 0, mImageData.Length);

                        bitmap.UnlockBits(data);
                    }
                }
                else
                {
                    image = Image.FromFile(path);
                }
            }
            catch (OutOfMemoryException)
            {
                // Sometimes the Image class will throw an out-of-memory exception if it can't understand the fileformat.  No, really.
                System.Console.WriteLine("Image.Load OutOfMemory Exception.  Check to see if the file format is supported.");
            }
            catch (Exception)
            {

            }

            if (image == null)
                return false;

            if (image != null)
            {
                mWidth = w = image.Width;
                mHeight = h = image.Height;
            }

            if (w > thumbWidth)
            {
                h = (int)(h * (thumbWidth / (float)w));
                w = thumbWidth;
            }

            if (h > thumbHeight)
            {
                w = (int)(w * (thumbHeight / (float)h));
                h = thumbHeight;
            }

            // Center it
            x = (thumbWidth / 2) - w / 2;
            y = (thumbHeight / 2) - h / 2;

            mThumbnail = new Bitmap(thumbWidth, thumbHeight);
            Graphics graphics = Graphics.FromImage(mThumbnail);

            graphics.FillRectangle(Brushes.White, new Rectangle(0, 0, thumbWidth, thumbHeight));
            graphics.DrawImage(image, x, y, w, h);

            graphics.Dispose();
            image.Dispose();

            ReloadInternalTexture();

            if (mTextureID > 0)
            {
                mWatcher.Path = System.IO.Path.GetDirectoryName(path);
                mWatcher.Filter = System.IO.Path.GetFileName(path);

                mWatcher.EnableRaisingEvents = true;
            }

            return mTextureID > 0;
        }

        /// <summary>
        /// Unloads the GUI DeviceTexture
        /// </summary>
        /// <returns></returns>
        public override bool Unload()
        {
            if (mTextureID == -1)
                return false;

            Otter.Interface.Graphics.Instance.UnloadTexture(mTextureID);
            mTextureID = -1;
            mThumbnail.Dispose();
            mThumbnail = null;

            mWatcher.EnableRaisingEvents = false;
            return true;
        }

        /// <summary>
        /// Reloads the entire texture from disk
        /// </summary>
        public void ReloadTexture()
        {
            // Called when the target file has changed.  Reload it, if necessary
            if (mTextureID >= 0)
            {
                Unload();
                Load();
            }
        }

        /// <summary>
        /// Reloads the texture from disk, without changing the width/height
        /// </summary>
        public void ReloadInternalTexture()
        {
            if (mTextureID >= 0)
                Otter.Interface.Graphics.Instance.UnloadTexture(mTextureID);

            int w = DisplayQuality == 1.0f ? 0 : (int)(DisplayQuality * mWidth);
            int h = DisplayQuality == 1.0f ? 0 : (int)(DisplayQuality * mHeight);

            if (mImageData == null)
            {
                mTextureID = Otter.Interface.Graphics.Instance.LoadTexture(GUIProject.CurrentProject.ProjectDirectory + "/" + Filename, w, h);
            }
            else
            {
                mTextureID = Otter.Interface.Graphics.Instance.LoadTexture(mImageData, w, h, 32);
            }
        }

        /// <summary>
        /// Called when the texture has changed somehow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            // Attempt to throttle multiple change events
            if (DateTime.Now.Subtract(mTimestamp).TotalMilliseconds < 1000)
                return;

            mTimestamp = DateTime.Now;

            ReloadTexture();

            NotifyResourceUpdated();
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Filename;
        }
    }
}
