using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

using Otter.UI.Resources;

namespace Otter.Containers
{
    /// <summary>
    /// Represents a binary node within the atlas.
    /// </summary>
    public class AtlasNode
    {
        public object mUserData;
        public Image mImage;
        public Rectangle mRectangle;

        public AtlasNode mLeft;
        public AtlasNode mRight;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="image"></param>
        /// <param name="rectangle"></param>
        public AtlasNode(object userData, Image image, Rectangle rectangle)
        {
            mUserData = userData;
            mImage = image;
            mRectangle = rectangle;

            mLeft = null;
            mRight = null;
        }

        /// <summary>
        /// Renders the node
        /// </summary>
        /// <param name="graphics"></param>
        public void Render(Graphics graphics)
        {
            if (mImage != null)
                graphics.DrawImage(mImage, mRectangle);

            if (mLeft != null)
                mLeft.Render(graphics);

            if (mRight != null)
                mRight.Render(graphics);
        }

        /// <summary>
        /// Searches for a particular texture info
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public AtlasNode FindNode(object userData)
        {
            if (mUserData == userData)
                return this;

            AtlasNode node = null;
            if (mLeft != null)
            {
                node = mLeft.FindNode(userData);
                if (node != null)
                    return node;
            }

            if (mRight != null)
            {
                node = mRight.FindNode(userData);
                if (node != null)
                    return node;
            }

            return null;
        }
    };

    /// <summary>
    /// Defines a texture atlas, where new textures can be added sequentially.
    /// </summary>
    public class TextureAtlas
    {
        public string mFilename;
        public AtlasNode mRoot;
        public uint mID;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="filename"></param>
        public TextureAtlas(uint id, int width, int height, string filename)
        {
            mID = id;
            mRoot = new AtlasNode(null, null, new Rectangle(0, 0, width, height));
            mFilename = filename;
        }

        /// <summary>
        /// Adds a new texture and image to the atlas
        /// </summary>
        /// <param name="info"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        public bool AddTexture(Image image, object userData)
        {
            if (!AddTexture(image, userData, mRoot))
            {
                // image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                // return AddTexture(info, image);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Adds a new texture and image to a particular node
        /// </summary>
        /// <param name="info"></param>
        /// <param name="image"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool AddTexture(Image image, object userData, AtlasNode node)
        {
            if (node == null)
                return false;

            if (node.mUserData != null)
                return false;

            if (node.mLeft != null && node.mRight != null)
            {
                if (AddTexture(image, userData, node.mLeft))
                    return true;

                if (AddTexture(image, userData, node.mRight))
                    return true;

                return false;
            }

            if (node.mRectangle.Width < image.Width ||
                node.mRectangle.Height < image.Height)
            {
                return false;
            }

            if (node.mRectangle.Width == image.Width &&
                node.mRectangle.Height == image.Height)
            {
                node.mUserData = userData;
                node.mImage = image;
                return true;
            }

            int dw = node.mRectangle.Width - image.Width;
            int dh = node.mRectangle.Height - image.Height;

            if (dw > dh)
            {
                node.mLeft = new AtlasNode(null, null, new Rectangle(node.mRectangle.Left,
                                                                     node.mRectangle.Top,
                                                                     image.Width,
                                                                     node.mRectangle.Height));

                node.mRight = new AtlasNode(null, null, new Rectangle(node.mRectangle.Left + image.Width,
                                                                      node.mRectangle.Top,
                                                                      node.mRectangle.Width - image.Width,
                                                                      node.mRectangle.Height));
            }
            else
            {
                node.mLeft = new AtlasNode(null, null, new Rectangle(node.mRectangle.Left,
                                                                     node.mRectangle.Top,
                                                                     node.mRectangle.Width,
                                                                     image.Height));

                node.mRight = new AtlasNode(null, null, new Rectangle(node.mRectangle.Left,
                                                                      node.mRectangle.Top + image.Height,
                                                                      node.mRectangle.Width,
                                                                      node.mRectangle.Height - image.Height));
            }

            return AddTexture(image, userData, node.mLeft);
        }

        /// <summary>
        /// Searches for a node by texture info
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public AtlasNode FindNode(object userData)
        {
            if (mRoot == null)
                return null;

            return mRoot.FindNode(userData);
        }

        /// <summary>
        /// Saves the atlas to a destination file
        /// </summary>
        /// <param name="destFile"></param>
        public void Save(string destFile)
        {
            Bitmap bitmap = GetBitmap();
            bitmap.Save(destFile, ImageFormat.Png);
            bitmap.Dispose();
        }

        public Bitmap GetBitmap()
        {
            Bitmap bitmap = new Bitmap(mRoot.mRectangle.Width, mRoot.mRectangle.Height);
            Graphics graphics = Graphics.FromImage(bitmap);

            graphics.Clear(Color.FromArgb(0, 0, 0, 0));
            mRoot.Render(graphics);
            graphics.Dispose();

            return bitmap;
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return mFilename;
        }
    }
}
