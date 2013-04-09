using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Xml.Serialization;
using System.Drawing;

using Otter.Export;

namespace Otter.Export
{
    /// <summary>
    /// Contains platform-specific settings
    /// </summary>
    public class Platform
    {
        #region Data
        private string mName = "";
        private Endian mEndianness = Endian.Little;
        private ColorFormat mColorFormat = ColorFormat.ARGB;
        private bool mSquarifyTextures = false;
        private string mOutputDirectory = "";
        private float mSafeDisplayWidth = 1.0f;
        private float mSafeDisplayHeight = 1.0f;

        private string mPostExportScript = "";
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the name of the platform
        /// </summary>
        [Category("Platform Settings")]
        [XmlAttribute]
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// Gets / Sets the endianness for this platform
        /// </summary>
        [Category("Platform Settings")]
        [XmlAttribute]
        public Endian Endianness
        {
            get { return mEndianness; }
            set { mEndianness = value; }
        }

        /// <summary>
        /// Gets / Sets the platform's vertex color format.
        /// </summary>
        [Category("Platform Settings")]
        [DisplayName("Color Format")]
        public ColorFormat ColorFormat
        {
            get { return mColorFormat; }
            set { mColorFormat = value; }
        }

        /// <summary>
        /// Gets / Sets the output directory for this platform
        /// </summary>
        [Category("Export Settings")]
        [DisplayName("Output Directory")]
        [XmlAttribute]
        [EditorAttribute(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string OutputDirectory
        {
            get { return mOutputDirectory; }
            set 
            { 
                mOutputDirectory = value;
            }
        }

        [Category("Safe Display Bounds")]
        [DisplayName("Width")]
        [Description("Defines the safe display width as a percentage of the total screen width, placed at the center of the display")]
        [TypeConverter(typeof(Otter.TypeConverters.PercentageConverter))]
        public float SafeDisplayWidth
        {
            get { return mSafeDisplayWidth; }
            set { mSafeDisplayWidth = value; }
        }

        [Category("Safe Display Bounds")]
        [DisplayName("Height")]
        [Description("Defines the safe display height as a percentage of the total screen width, placed at the center of the display")]
        [TypeConverter(typeof(Otter.TypeConverters.PercentageConverter))]
        public float SafeDisplayHeight
        {
            get { return mSafeDisplayHeight; }
            set { mSafeDisplayHeight = value; }
        }

        [Category("Export Settings")]
        [DisplayName("Post Export Script")]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string PostExportScript
        {
            get { return mPostExportScript; }
            set { mPostExportScript = value; }
        }

        /// <summary>
        /// Gets / Sets whether or not we should automatically
        /// make textures square
        /// </summary>
        [Category("Export Settings")]
        [DisplayName("Squarify Textures")]
        [Description("If true, forces all non-atlassed textures to be power-of-two square textures")]
        public bool SquarifyTextures
        {
            get { return mSquarifyTextures; }
            set { mSquarifyTextures = value; }
        }
        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public Platform()
        {
        }

        /// <summary>
        /// Constructs the platform with the specified name
        /// </summary>
        /// <param name="name"></param>
        public Platform(string name)
        {
            mName = name;
        }
        
        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
