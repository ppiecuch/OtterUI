using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using System.Linq;

using Otter.UI;
using Otter.Export;

namespace Otter.Project
{
    /// <summary>
    /// The full GUI Project.  Maintains all the scenes, views, etc. data
    /// for a GUI project.
    /// </summary>
    public class GUIProject : IDisposable
    {
        #region Data
        private string mVersion = Otter.Properties.Settings.Default.ProjectVersion;
        private string mName = "";
        private string mProjectDirectory = "";
        private List<GUIProjectEntry> mEntries = new List<GUIProjectEntry>();

        private List<GUIFont> mFonts = new List<GUIFont>();
        private List<Platform> mPlatforms = new List<Platform>();
        private List<Resolution> mResolutions = new List<Resolution>();

        private static GUIProject mCurrentProject = null;
        private static XmlAttributeOverrides mXmlAttributeOverrides = new XmlAttributeOverrides();
        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the version of the scene
        /// </summary>
        [Browsable(false)]
        public string Version
        {
            get { return mVersion; }
            set 
            {
                if (mVersion != value)
                {
                    mVersion = value;
                }
            }
        }

        /// <summary>
        /// Gets / Sets the name of the project
        /// </summary>
        public string Name
        {
            get { return mName; }
            set 
            {
                if (mName != value)
                {
                    mName = value;
                }
            }
        }

        /// <summary>
        /// Gets / Sets the project directory
        /// </summary>
        [XmlIgnore]
        public string ProjectDirectory
        {
            get { return mProjectDirectory; }
            set 
            {
                if (mProjectDirectory != value)
                {
                    mProjectDirectory = value;
                }
            }
        }

        /// <summary>
        /// Retrieves the full path of the entry.
        /// </summary>
        public string FullPath
        {
            get
            {
                string projectDir = GUIProject.CurrentProject.ProjectDirectory;
                return mProjectDirectory + "/" + mName + ".ggp";
            }
        }

        /// <summary>
        /// Gets / Sets the list of entries in the project
        /// </summary>
        [XmlArrayItem("Scene", typeof(GUIProjectScene))]
        public List<GUIProjectEntry> Entries
        {
            get { return mEntries; }
            set { mEntries = value; }
        }

        /// <summary>
        /// Gets / Sets the list of fonts used by the project
        /// </summary>
        public List<GUIFont> Fonts
        {
            get { return mFonts; }
            set { mFonts = value; }
        }

        /// <summary>
        /// Gets / Sets the list of platforms for this project.
        /// </summary>
        public List<Platform> Platforms
        {
            get { return mPlatforms; }
            set { mPlatforms = value; }
        }

        /// <summary>
        /// Gets / sets the list of resolutions for this project.
        /// </summary>
        public List<Resolution> Resolutions
        {
            get { return mResolutions; }
            set { mResolutions = value; }
        }

        /// <summary>
        /// Gets / Sets the global project
        /// </summary>
        public static GUIProject CurrentProject
        {
            get { return mCurrentProject; }
            set { mCurrentProject = value; }
        }

        /// <summary>
        /// Retrieves the XML Attribute Overrides
        /// </summary>
        public static XmlAttributeOverrides XmlAttributeOverrides
        {
            get { return mXmlAttributeOverrides; }
        }
        #endregion

        /// <summary>
        /// Empty constructor - necessary for xml (de)serialization
        /// </summary>
        public GUIProject()
        {
        }

        /// <summary>
        /// Constructor - initializes the project with a name
        /// </summary>
        /// <param name="name"></param>
        public GUIProject(string name)
        {
            mName = name;
        }

        /// <summary>
        /// Retrieves a font by ID
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public GUIFont GetFont(int id)
        {
            foreach (GUIFont font in mFonts)
            {
                if (font.ID == id)
                    return font;
            }

            return null;
        }

        /// <summary>
        /// Retrieves a scene entry by filename
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public GUIProjectScene GetSceneEntry(string filename)
        {
            if (!System.IO.Path.IsPathRooted(filename))
                filename = this.FullPath + "/" + filename;

            foreach (GUIProjectScene sceneEntry in Entries.OfType<GUIProjectScene>())
            {
                if (sceneEntry.FullPath == filename)
                    return sceneEntry;
            }

            return null;
        }

        /// <summary>
        /// Disposes of the project
        /// </summary>
        public void Dispose()
        {
            foreach (GUIFont font in mFonts)
            {
                font.Dispose();
            }
        }
        
        /// <summary>
        /// Saves a GUIProject to disk.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool Save(GUIProject project, string filename)
        {
            FileStream fs = null;
            bool bSuccess = false;

            try
            {
                fs = new FileStream(filename, FileMode.Create);
                XmlSerializer serializer = new XmlSerializer(typeof(GUIProject));

                // Ensure the project version is set before serialization
                project.Version = Otter.Properties.Settings.Default.ProjectVersion;
                serializer.Serialize(fs, project);

                foreach (GUIProjectEntry entry in project.Entries)
                    entry.Save();

                bSuccess = true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }

            return bSuccess;
        }

        /// <summary>
        /// Loads the GUI Project from disk
        /// </summary>
        /// <returns></returns>
        public static GUIProject Load(string filename)
        {
            GUIProject project = null;
            FileStream fs = null;
            try
            {
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                XmlSerializer serializer = new XmlSerializer(typeof(GUIProject));
                project = serializer.Deserialize(fs) as GUIProject;

                project.ProjectDirectory = System.IO.Path.GetDirectoryName(filename);

                foreach (GUIFont font in project.Fonts)
                {
                    font.Project = project;
                    font.ReloadFont();
                }

                // TODO : Check project version, and perform any fixups necessary

                // Set the project version once all fixups have been performed.
                project.Version = Otter.Properties.Settings.Default.ProjectVersion;

                // Create a "unique" count for each entry upon load.
                uint id = 1000;
                foreach (GUIProjectEntry entry in project.Entries)
                    entry.ID = (id++); 
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Exception e:" + ex);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }

            return project;
        }
    }
}
