using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Xml.Serialization;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

using Otter.Export;
using Otter.TypeEditors;
using Otter.Project;
using Otter.Containers;
using Otter.Imaging;

namespace Otter.UI
{
    /// <summary>
    /// Horizontal Alignment
    /// </summary>
    public enum HoriAlignment
    {
        Left,
        Center,
        Right
    }

    /// <summary>
    /// Vertical Alignment
    /// </summary>
    public enum VertAlignment
    {
        Top,
        Center,
        Bottom
    }

    /// <summary>
    /// Manages a font that will be used by the GUI to render text.
    /// </summary>
    public class GUIFont : IExportable, IDisposable
    {
        #region Internal Datatypes
        
	    struct NewCharInfo
	    {
		    public UInt32 mCharCode;
            public UInt32 mImageGlyph;  // 0 : No, 1 : Yes

            public int mAtlasIndex;

            public int mX;
            public int mY;
            public int mW;
            public int mH;

            public int mTop;
            public int mAdvance;
            public int mLeftBearing;
	    }

        /// <summary>
        /// Contains the information for a single character to be rendered
        /// </summary>
        class CharQuad
        {
            public int mCharIndex = 0;                              // Character's index represented by this object
            public Rectangle mOutputRectangle = Rectangle.Empty;    // Where the character will be drawn.
        }

        /// <summary>
        /// List of character quads
        /// </summary>
        class CharQuadList
        {
            public List<CharQuad> mCharacters = new List<CharQuad>();

            public Rectangle Bounds
            {
                get
                {
                    int left = 0;
                    int right = 0;
                    int top = 0;
                    int bottom = 0;
                    foreach (CharQuad charQuad in mCharacters)
                    {
                        if (charQuad.mOutputRectangle.Left < left)
                            left = charQuad.mOutputRectangle.Left;
                        if (charQuad.mOutputRectangle.Right > right)
                            right = charQuad.mOutputRectangle.Right;
                        if (charQuad.mOutputRectangle.Top < top)
                            top = charQuad.mOutputRectangle.Top;
                        if (charQuad.mOutputRectangle.Bottom > bottom)
                            bottom = charQuad.mOutputRectangle.Bottom;
                    }

                    return new Rectangle(left, top, right - left, bottom - top);
                }            
            }
        }
        #endregion

        #region Data
        private int mID = -1;
        private string mName = "";
        private string mFontFile = "";
        private string mTextureFile = "";
        private int mTextureBaseline = 0;
        private int mFontSize = 18;
        private FontBuilder.FontData mFontData = null;
        private float mOutlineAmount = 0.0f;
        private float mOutlineSharpness = 0.9f;
        private Color mFillColor = Color.White;
        private Color mOutlineColor = Color.Transparent;
        private List<int> mTextures = new List<int>();
        private List<TextureAtlas> mTextureAtlasses = new List<TextureAtlas>();

        private int mFontID = -1;
        private List<char> mCharacters = new List<char>();
        private List<NewCharInfo> mCharInfoList = new List<NewCharInfo>();
        private List<FontBuilder.ImageGlyph> mImageGlyphs = new List<FontBuilder.ImageGlyph>();
        private Size mAtlasSize = new Size(512, 512);

        private int mMaxTop = 0;
        private Project.GUIProject mProject = null;

        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets this font's ID
        /// </summary>
        [Browsable(false)]
        [XmlAttribute]
        public int ID
        {
            get { return mID; }
            set { mID = value; }
        }

        /// <summary>
        /// Gets / Sets the name of the font
        /// </summary>
        [XmlAttribute]
        [Category("General")]
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// Gets / Sets the font file.
        /// </summary>
        [Editor(typeof(Otter.TypeEditors.UIFontFileEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [XmlAttribute]
        [Category("General")]
        public string Filename
        {
            get { return mFontFile; }
            set 
            {
                if (mFontFile != value)
                {
                    mFontFile = value;
                    if (System.IO.Path.IsPathRooted(mFontFile))
                    {
                        mFontFile = Utils.GetRelativePath(GUIProject.CurrentProject.ProjectDirectory, mFontFile);
                    }

                    ReloadFont();
                }
            }
        }

        [Category("Outline")]
        [DisplayName("Amount")]
        [Description("Amount of outline to apply to the font")]
        public float OutlineAmount
        {
            get { return mOutlineAmount; }
            set
            {
                if (mOutlineAmount != value)
                {
                    mOutlineAmount = value;
                    ReloadFont();
                }
            }
        }

        [Category("Outline")]
        [DisplayName("Sharpness")]
        [Description("Outline sharpness.")]
        public float OutlineSharpness
        {
            get { return mOutlineSharpness; }
            set
            {
                if (mOutlineSharpness != value)
                {
                    mOutlineSharpness = value;
                    ReloadFont();
                }
            }
        }

        [Category("Outline")]
        [DisplayName("Color")]
        [Description("Outline Color.")]
        [XmlIgnore]
        public Color OutlineColor
        {
            get { return mOutlineColor; }
            set
            {
                if (mOutlineColor != value)
                {
                    mOutlineColor = value;
                    ReloadFont();
                }
            }
        }

        [Browsable(false)]
        public string OutlineColorXML
        {
            get { return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(mOutlineColor); }
            set { mOutlineColor = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFrom(value); }
        }

        [Category("Fill")]
        [DisplayName("Color")]
        [Description("Fill Color.")]
        [XmlIgnore]
        public Color FillColor
        {
            get { return mFillColor; }
            set
            {
                if (mFillColor != value)
                {
                    mFillColor = value;
                    ReloadFont();
                }
            }
        }

        [Browsable(false)]
        public string FillColorXML
        {
            get { return TypeDescriptor.GetConverter(typeof(Color)).ConvertToString(mFillColor); }
            set { mFillColor = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFrom(value); }
        }

        /// <summary>
        /// The texture baseline defines where the glyph baseline will be aligned during sampling.
        /// </summary>
        [Category("Fill")]
        [DisplayName("Texture Baseline")]
        [Description("The texture baseline defines where the glyph baseline will be aligned during sampling")]
        public int TextureBaseline
        {
            get
            {
                return mTextureBaseline;
            }
            set
            {
                if (mTextureBaseline != value)
                {
                    mTextureBaseline = value;

                    if (System.IO.Path.IsPathRooted(mTextureFile))
                        mTextureFile = Utils.GetRelativePath(GUIProject.CurrentProject.ProjectDirectory, mTextureFile);

                    ReloadFont();
                }
            }
        }

        /// <summary>
        /// Gets / Sets the font file.
        /// </summary>
        [Editor(typeof(Otter.TypeEditors.UIImageFileEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [XmlAttribute]
        [Category("Fill")]
        public string Texture
        {
            get { return mTextureFile; }
            set
            {
                if (mTextureFile != value)
                {
                    mTextureFile = value;

                    if (System.IO.Path.IsPathRooted(mTextureFile))
                        mTextureFile = Utils.GetRelativePath(GUIProject.CurrentProject.ProjectDirectory, mTextureFile);

                    ReloadFont();
                }
            }
        }

        /// <summary>
        /// Gets / Sets the font size
        /// </summary>
        [Category("General")]
        [DisplayName("Size")]
        public int FontSize
        {
            get { return mFontSize; }
            set 
            { 
                mFontSize = value;
                ReloadFont();
            }
        }

        /// <summary>
        /// Retrieves the font data associated with this font.
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public FontBuilder.FontData FontData
        {
            get { return mFontData; }
        }

        /// <summary>
        /// Retrieves the texture atlasses
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public List<TextureAtlas> TextureAtlasses
        {
            get { return mTextureAtlasses; }
        }

        /// <summary>
        /// Retrieves the parent project for this font
        /// </summary>
        [XmlIgnore]
        [Browsable(false)]
        public Project.GUIProject Project
        {
            get { return mProject; }
            set { mProject = value; }
        }

        /// <summary>
        /// Gets / Sets the supported characters for this font.
        /// </summary>
        [Browsable(false)]
        [Editor(typeof(UIGlyphEditor), typeof(UITypeEditor))]
        public List<char> Characters
        {
            get { return mCharacters; }
            set 
            {
                List<char> chars = value;
                chars.Add('?');

                chars = new List<char>(chars.Distinct());
                chars.Sort();

                bool refresh = false;

                if (mCharacters.Count != chars.Count)
                {
                    refresh = true;
                }
                else
                {
                    int minLen = Math.Min(mCharacters.Count, chars.Count);
                    for (int i = 0; i < minLen; i++)
                    {
                        if (mCharacters[i] != chars[i])
                        {
                            refresh = true;
                            break;
                        }
                    }
                }

                if (refresh)
                {
                    mCharacters = chars;
                    ReloadFont();
                }
            }
        }
        
        /// <summary>
        /// Gets / sets the list of Image Glyphs available to this font
        /// </summary>
        [Browsable(false)]
        public List<FontBuilder.ImageGlyph> Images
        {
            get 
            { 
                return mImageGlyphs; 
            }
            set { mImageGlyphs = value; }
        }

        /// <summary>
        /// Gets / Sets the complete mixed list of glyphs
        /// for the font to use.  This list is expected to contain
        /// any number of 'char' and 'ImageGlyph' entries.
        /// 
        /// This property is a convenience property and will return the union
        /// of the Characters and Images properties.  Modifying this property will
        /// also affect those list accordingly.
        /// </summary>
        [XmlIgnore()]
        [Editor(typeof(UIGlyphEditor), typeof(UITypeEditor))]
        [Category("Glyph Sheet")]
        public ArrayList Glyphs
        {
            get
            {
                ArrayList list = new ArrayList(Characters);
                list.AddRange(Images);

                return list;
            }
            set
            {
                Characters = new List<char>(value.OfType<char>());
                Images = new List<FontBuilder.ImageGlyph>(value.OfType<FontBuilder.ImageGlyph>());

                ReloadFont();
            }
        }

        /// <summary>
        /// Gets / Sets the atlas texture size.  When the font is generated,
        /// it will generate texture atlasses of this size.  Larger
        /// values will generate larger textures but few atlasses.  Smaller values
        /// will generate smaller textures but more atlasses.
        /// </summary>
        [Category("Glyph Sheet")]
        public Size AtlasSize
        {
            get { return mAtlasSize; }
            set 
            { 
                mAtlasSize = value;
                ReloadFont();
            }
        }
        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public GUIFont()
        {
        }

        /// <summary>
        /// Adds standard ASCII character codes
        /// </summary>
        public void AddASCII()
        {
            List<char> chars = new List<char>('~' - ' ' + 1);
            for (char c = ' '; c <= '~'; c++)
                chars.Add(c);

            chars.AddRange(mCharacters);
            List<char> charList = new List<char>(chars.Distinct());
            charList.Sort();

            Characters = charList;
        }

        private Bitmap ApplyTexture(Bitmap sourceTexture, Bitmap targetTexture, int sourceBaseline, int targetBaseline)
        {
            if(targetTexture == null)
                return null;

            Bitmap finalBitmap = new Bitmap(targetTexture);
            if (sourceTexture != null)
            {
                Rectangle sourceRect = new Rectangle(0, 0, sourceTexture.Width, sourceTexture.Height);
                Rectangle targetRect = new Rectangle(0, 0, targetTexture.Width, targetTexture.Height);

                // Find the final source rect from where we will pull pixels
                Rectangle centeredTargetRect = new Rectangle(sourceRect.Width / 2 - targetRect.Width / 2,
                                                sourceBaseline - targetBaseline,
                                                targetRect.Width,
                                                targetRect.Height);
                Rectangle sourceLockRect = new Rectangle(sourceRect.X, sourceRect.Y, sourceRect.Width, sourceRect.Height);
                sourceLockRect.Intersect(centeredTargetRect);

                // Find the final target rect from where we will pull pixels
                Rectangle centeredSourceRect = new Rectangle(targetRect.Width / 2 - sourceRect.Width / 2,
                                                targetBaseline - sourceBaseline,
                                                sourceRect.Width,
                                                sourceRect.Height);
                Rectangle targetLockRect = new Rectangle(targetRect.X, targetRect.Y, targetRect.Width, targetRect.Height);
                targetLockRect.Intersect(centeredSourceRect);

                if (targetLockRect.Width == 0 || targetLockRect.Height == 0)
                    return finalBitmap;

                BitmapData sourceData = sourceTexture.LockBits(sourceLockRect, ImageLockMode.ReadOnly, sourceTexture.PixelFormat);
                BitmapData targetData = finalBitmap.LockBits(targetLockRect, ImageLockMode.ReadWrite, finalBitmap.PixelFormat);

                // Now iterate through the pixels
                for (int x = 0; x < targetLockRect.Width; x++)
                {
                    for (int y = 0; y < targetLockRect.Height; y++)
                    {
                        try
                        {
                            IntPtr sourcePixel = (IntPtr)((int)sourceData.Scan0 + sourceData.Stride * y + x * 4);
                            IntPtr targetPixel = (IntPtr)((int)targetData.Scan0 + targetData.Stride * y + x * 4);

                            UInt32 srcCol = (UInt32)Marshal.ReadInt32(sourcePixel);
                            UInt32 tgtCol = (UInt32)Marshal.ReadInt32(targetPixel);

                            byte _0 = (byte)((uint)((((srcCol & 0xFF000000) >> 24) / 255.0f) * ((tgtCol & 0xFF000000))) >> 24);
                            byte _1 = (byte)((uint)((((srcCol & 0x00FF0000) >> 16) / 255.0f) * ((tgtCol & 0x00FF0000))) >> 16);
                            byte _2 = (byte)((uint)((((srcCol & 0x0000FF00) >> 8 ) / 255.0f) * ((tgtCol & 0x0000FF00))) >> 8 );
                            byte _3 = (byte)((uint)((((srcCol & 0x000000FF)      ) / 255.0f) * ((tgtCol & 0x000000FF)))      );

                            tgtCol = (uint)(((uint)_0 << 24) | ((uint)_1 << 16) | ((uint)_2 << 8) | _3);
                            Marshal.WriteInt32(targetPixel, (int)tgtCol);
                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine("Exception : " + ex);
                        }
                    }
                }

                sourceTexture.UnlockBits(sourceData);
                finalBitmap.UnlockBits(targetData);

            }

            return finalBitmap;
        }

        private float CalcEase(float factor, int easeAmount)
        {
            // Ease Out          
            easeAmount = Math.Min(Math.Max(easeAmount, 0), 100);
            float easePower = 1.0f + 4.0f * Math.Abs(((float)easeAmount) / 100.0f);

            return (float)((1.0 - Math.Pow(1.0 - factor, easePower)));
        }

        private Bitmap BlurBitmap(Bitmap bitmap, float amount, float sharpness, Color color)
        {
            System.Drawing.Graphics g = null;

            // Guessing 3 pix * amount * 2 in increased dims.
            int inc = (int)(3 * amount * 2);
            Bitmap resizedBitmap = new Bitmap(bitmap.Width + inc, bitmap.Height + inc);

            g = System.Drawing.Graphics.FromImage(resizedBitmap);
            g.DrawImage(bitmap, inc / 2, inc / 2);
            g.Dispose();

            Bitmap blurBitmap = Otter.Imaging.GaussianBlur.FilterProcessImage(amount, resizedBitmap);
            resizedBitmap.Dispose();

            float s = 1.0f - sharpness;
            int cutoff = (int)(s * 255);    // Anything above this value will be clamped to 255.
            for (int x = 0; x < blurBitmap.Width; x++)
            {
                for (int y = 0; y < blurBitmap.Height; y++)
                {
                    Color col = blurBitmap.GetPixel(x, y);

                    if (col.R != 0)
                    {
                        int alpha = col.R;

                        if (alpha > cutoff || s == 0.0f)
                            alpha = 255;
                        else
                            alpha = (int)(alpha * (1.0f / s));

                        blurBitmap.SetPixel(x, y, Color.FromArgb(alpha, color));
                    }
                    else
                    {
                        blurBitmap.SetPixel(x, y, Color.FromArgb(0, color));
                    }

                }
            }

            return blurBitmap;
        }

        /// <summary>
        /// Reloads the font
        /// </summary>
        public void ReloadFont()
        {
            if (mProject == null)
                return;

            int borderWidth = 2;

            string fullPath = mProject.ProjectDirectory + "/" + mFontFile;
            if (System.IO.File.Exists(fullPath))
            {
                // Ensure that the character list ALWAYS contains at least the ? character.
                if (!Characters.Contains('?'))
                    Characters.Add('?');

                // Load the image glyph textures
                foreach (FontBuilder.ImageGlyph imageGlyph in Images)
                    imageGlyph.Load(mProject.ProjectDirectory);

                if (mFontData != null)
                    mFontData.Dispose();

                mFontData = new FontBuilder.FontData(Characters, Images);
                FontBuilder.Glyph[] glyphs = mFontData.GetGlyphs(fullPath, mFontSize);

                // Clear out our existing textures and texture atlasses
                foreach (TextureAtlas atlas in mTextureAtlasses)
                {
                    DisposeAtlasNode(atlas.mRoot);
                }
                mTextureAtlasses.Clear();

                mTextures.Clear();
                mCharInfoList.Clear();
                mMaxTop = -99999;

                Image texture = null;
                Bitmap textureBitmap = null;
                if (File.Exists(mProject.ProjectDirectory + "/" + mTextureFile))
                {
                    texture = Image.FromFile(mProject.ProjectDirectory + "/" + mTextureFile);
                    textureBitmap = new Bitmap(texture);
                }

                TextureAtlas curAtlas = null;
                foreach (FontBuilder.Glyph glyph in glyphs)
                {
                    if (glyph == null)
                        continue;

                    if (mMaxTop < glyph.mTop)
                        mMaxTop = glyph.mTop;

                    Bitmap finalBitmap = null;
                    if (glyph.mBitmap != null)
                    {
                        finalBitmap = (borderWidth == 0) ? new Bitmap(glyph.mBitmap) : Utils.ExpandImageBorder(glyph.mBitmap, borderWidth, true);
                        if (finalBitmap == null)
                            continue;

                        Bitmap outlinedBitmap = null;
                        Bitmap texturedBitmap = null;
                        
                        if (OutlineAmount > 0.0f && OutlineColor.A != 0)
                        {
                            float s = Math.Min(1.0f, Math.Max(OutlineSharpness, 0.0f));
                            outlinedBitmap = BlurBitmap(finalBitmap, OutlineAmount, OutlineSharpness, OutlineColor);
                            finalBitmap.Dispose();
                            finalBitmap = outlinedBitmap;
                        }

                        BitmapData texturedBitmapData = null;
                        if (textureBitmap != null && glyph.mImageGlyph == 0)
                        {
                            texturedBitmap = ApplyTexture(textureBitmap, glyph.mBitmap, mTextureBaseline, glyph.mTop);
                            texturedBitmapData = texturedBitmap.LockBits(new Rectangle(0, 0,
                                                                        texturedBitmap.Width,
                                                                        texturedBitmap.Height),
                                                                        ImageLockMode.ReadOnly,
                                                                        texturedBitmap.PixelFormat);
                        }
                        
                        // Now we need to use the original bitmap (from the glyph) and reapply it to the new
                        // bitmap (that may or may not have been blurred) with the fill color
                        BitmapData sourceBitmapData = glyph.mBitmap.LockBits( new Rectangle(0, 0, glyph.mBitmap.Width, glyph.mBitmap.Height),
                                                                              ImageLockMode.ReadOnly,
                                                                              glyph.mBitmap.PixelFormat);

                        BitmapData targetBitmapData = finalBitmap.LockBits(new Rectangle((finalBitmap.Width - glyph.mBitmap.Width) / 2,
                                                                      (finalBitmap.Height - glyph.mBitmap.Height) / 2,
                                                                      glyph.mBitmap.Width,
                                                                      glyph.mBitmap.Height),
                                                                      ImageLockMode.WriteOnly,
                                                                      finalBitmap.PixelFormat);

                        try
                        {
                            for (int x = 0; x < sourceBitmapData.Width; x++)
                            {
                                for (int y = 0; y < sourceBitmapData.Height; y++)
                                {
                                    IntPtr sourcePixel = (IntPtr)((int)sourceBitmapData.Scan0 + sourceBitmapData.Stride * y + x * 4);
                                    IntPtr targetPixel = (IntPtr)((int)targetBitmapData.Scan0 + targetBitmapData.Stride * y + x * 4);
                                    IntPtr texturePixel = IntPtr.Zero;

                                    if(texturedBitmapData != null)
                                        texturePixel = (IntPtr)((int)texturedBitmapData.Scan0 + texturedBitmapData.Stride * y + x * 4);

                                    Color srcColor = Color.FromArgb(Marshal.ReadInt32(sourcePixel));
                                    Color tgtColor = Color.FromArgb(Marshal.ReadInt32(targetPixel));
                                    Color texColor = (texturePixel != IntPtr.Zero) ? Color.FromArgb(Marshal.ReadInt32(texturePixel)) : Color.White;

                                    float alpha = srcColor.A / 255.0f;
                                    if (alpha == 0.0f)
                                        continue;

                                    byte r = (byte)(tgtColor.R * (1.0f - alpha) + FillColor.R * (srcColor.R / 255.0f) * (texColor.R / 255.0f) * alpha);
                                    byte g = (byte)(tgtColor.G * (1.0f - alpha) + FillColor.G * (srcColor.G / 255.0f) * (texColor.G / 255.0f) * alpha);
                                    byte b = (byte)(tgtColor.B * (1.0f - alpha) + FillColor.B * (srcColor.B / 255.0f) * (texColor.B / 255.0f) * alpha);
                                    byte a = (byte)(tgtColor.A * (1.0f - alpha) + FillColor.A * (srcColor.A / 255.0f) * (texColor.A / 255.0f) * alpha);

                                    Color finalColor = Color.FromArgb(a, r, g, b);

                                    Marshal.WriteInt32(targetPixel, finalColor.ToArgb());
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine("Exception : " + ex);
                        }

                        glyph.mBitmap.UnlockBits(sourceBitmapData);
                        finalBitmap.UnlockBits(targetBitmapData);

                        if (texturedBitmapData != null)
                            texturedBitmap.UnlockBits(texturedBitmapData);
                    }
                    else
                    {
                        finalBitmap = new Bitmap(4, 4);
                    }

                    if (curAtlas == null || !curAtlas.AddTexture(finalBitmap, glyph))
                    {
                        curAtlas = new TextureAtlas(0, AtlasSize.Width, AtlasSize.Height, "tmp");
                        mTextureAtlasses.Add(curAtlas);

                        curAtlas.AddTexture(finalBitmap, glyph);
                    }
                }

                if (textureBitmap != null)
                    textureBitmap.Dispose();

                if (texture != null)
                    texture.Dispose();
                
                // Unload the image glyph textures
                foreach (FontBuilder.ImageGlyph imageGlyph in Images)
                    imageGlyph.Unload();

                // Then iterate and add to the texture atlas(ses).
                if (Otter.Interface.Graphics.Instance != null)
                {
                    foreach(int textureID in mTextures)
                        Otter.Interface.Graphics.Instance.UnloadTexture(textureID);

                    foreach (TextureAtlas atlas in mTextureAtlasses)
                    {
                        Bitmap bitmap = atlas.GetBitmap();

                        // BitmapData 
                        BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);

                        // Get the address of the first line.
                        IntPtr ptr = bitmapData.Scan0;

                        // Declare an array to hold the bytes of the bitmap.
                        int size = Math.Abs(bitmapData.Stride) * bitmap.Height;
                        byte[] bytes = new byte[size];

                        System.Runtime.InteropServices.Marshal.Copy(ptr, bytes, 0, size);

                        int textureID = Otter.Interface.Graphics.Instance.LoadTexture(bytes, bitmap.Width, bitmap.Height, 32);

                        bitmap.UnlockBits(bitmapData);

                        mTextures.Add(textureID);

                        bitmap.Dispose();

                        // SLOW
                        foreach (FontBuilder.Glyph glyph in glyphs)
                        {
                            if (glyph == null)
                                continue;

                            AtlasNode node = atlas.FindNode(glyph);

                            if(node != null)
                            {
                                NewCharInfo info = new NewCharInfo();

                                int diffW = node.mRectangle.Width - borderWidth * 2 - glyph.mW;
                                int diffH = node.mRectangle.Height - borderWidth * 2 - glyph.mH;

                                info.mCharCode = glyph.mCharCode;
                                info.mImageGlyph = glyph.mImageGlyph;
                                info.mX = node.mRectangle.X + borderWidth;
                                info.mY = node.mRectangle.Y + borderWidth;
                                info.mW = glyph.mW + diffW;
                                info.mH = glyph.mH + diffH;

                                info.mTop = glyph.mTop + diffH / 2;
                                info.mAdvance = glyph.mAdvance;
                                info.mLeftBearing = glyph.mLeftBearing - diffW / 2;

                                info.mAtlasIndex = mTextureAtlasses.IndexOf(atlas);

                                mCharInfoList.Add(info);
                            }
                        }
                    }

                    MemoryStream stream = new MemoryStream();

                    Platform platform = new Platform();
                    platform.Endianness = Endian.Little;
                    platform.ColorFormat = ColorFormat.ARGB;

                    PlatformBinaryWriter bw = new PlatformBinaryWriter(stream, platform);
                    this.Export(bw);

                    mFontID = Otter.Interface.Graphics.Instance.LoadFont(this.Name, stream.GetBuffer(), mTextures);

                    bw.Close();
                    stream.Close();
                }
            }
        }

        public void Draw(string text, float x, float y, float width, float height, Color color, SizeF scale, HoriAlignment halign, VertAlignment valign)
        {
            Otter.Interface.Graphics.Instance.DrawString(mFontID, text, x, y, width, height, color.ToArgb(), scale.Width, scale.Height, (int)halign, (int)valign, 0.5f, 0, 0.0f, (int)GUILabel.TextFitMode.Wrap, 0, -1);
        }

        /// <summary>
        /// Draws a string to screen
        /// </summary>
        /// <param name="text"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="color"></param>
        /// <param name="halign"></param>
        /// <param name="valign"></param>
        public void Draw(string text, float x, float y, float width, float height, Color color, SizeF scale, HoriAlignment halign, VertAlignment valign, float leading, int tracking, float skewAngle, GUILabel.TextFitMode textFit, int dropShadow, int maskID)
        {
            Otter.Interface.Graphics.Instance.DrawString(mFontID, text, x, y, width, height, color.ToArgb(), scale.Width, scale.Height, (int)halign, (int)valign, leading, tracking, skewAngle, (UInt32)textFit, dropShadow, maskID);
        }

        /// <summary>
        /// Exports the GUI Font
        /// </summary>
        /// <param name="bw"></param>
        public void Export(PlatformBinaryWriter bw)
        {
            byte[] bytes = Utils.StringToBytes(mName, 64);
            bytes[63] = 0;

            FourCCStack fourCCStack = new FourCCStack(bw);
            fourCCStack.Push("GFNT");

            // Font information
            bw.Write(this.ID);
            bw.Write(bytes, 0, 64);
            bw.Write(this.FontSize);
            bw.Write(AtlasSize.Width);
            bw.Write(AtlasSize.Height);
            bw.Write(this.mMaxTop);
            bw.Write(this.mTextures.Count);

            // Now write the glyph data
            bw.Write(this.mCharInfoList.Count);

            foreach (NewCharInfo charInfo in this.mCharInfoList)
            {
                bw.Write(charInfo.mCharCode);
                bw.Write(charInfo.mImageGlyph);

                bw.Write(charInfo.mX);
                bw.Write(charInfo.mY);
                bw.Write(charInfo.mW);
                bw.Write(charInfo.mH);

                bw.Write(charInfo.mTop);
                bw.Write(charInfo.mAdvance);
                bw.Write(charInfo.mLeftBearing);

                bw.Write(charInfo.mAtlasIndex);
            }

            fourCCStack.Pop();
        }

        /// <summary>
        /// Disposes the font
        /// </summary>
        public void Dispose()
        {
            if (Otter.Interface.Graphics.Instance == null)
                return;

            Otter.Interface.Graphics.Instance.UnloadFont(mFontID);

            foreach(int textureID in mTextures)
                Otter.Interface.Graphics.Instance.UnloadTexture(textureID);

            foreach (TextureAtlas atlas in TextureAtlasses)
            {
                DisposeAtlasNode(atlas.mRoot);
            }
        }

        private void DisposeAtlasNode(AtlasNode node)
        {
            if (node == null)
                return;

            DisposeAtlasNode(node.mLeft);
            DisposeAtlasNode(node.mRight);

            FontBuilder.Glyph glyph = node.mUserData as FontBuilder.Glyph;
            if (glyph != null && glyph.mBitmap != null)
            {
                glyph.mBitmap.Dispose();

                if (node.mImage != glyph.mBitmap)
                {
                    node.mImage.Dispose();
                }

                node.mImage = null;
                glyph.mBitmap = null;
            }


        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return mName;
        }
    }
}
