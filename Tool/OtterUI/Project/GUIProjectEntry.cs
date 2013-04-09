using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Otter.Project
{
    /// <summary>
    /// Represents a single entry in the Project.
    /// </summary>
    public class GUIProjectEntry
    {
        #region Data
        private uint mID = 0;
        protected string mName = "";
        #endregion

        #region Properties
        /// <summary>
        /// Project entry's unique ID
        /// </summary>
        [Browsable(false)]
        public uint ID
        {
            get { return mID; }
            set { mID = value; }
        }

        /// <summary>
        /// Gets / Sets the name of the entry
        /// </summary>
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// Gets the fileName of the entry
        /// </summary>
        public string Filename
        {
            get { return mName + ".gsc"; }
        }

        /// <summary>
        /// Retrieves the full path of the entry.
        /// </summary>
        public string FullPath
        {
            get
            {
                string projectDir = GUIProject.CurrentProject.ProjectDirectory;
                return projectDir + "/" + Filename;
            }
        }
        #endregion

        /// <summary>
        /// Default (empty) constructor.  Necessary for XML (de)serialization
        /// </summary>
        public GUIProjectEntry()
        {
        }

        /// <summary>
        /// Checks whether or not the entry exists on disk
        /// </summary>
        /// <returns></returns>
        public virtual bool Exists()
        {
            return System.IO.File.Exists(FullPath);
        }

        /// <summary>
        /// Saves the project entry, if loaded.
        /// </summary>
        /// <returns></returns>
        public virtual bool Save()
        {
            return true;
        }

        /// <summary>
        /// Loads the project entry, if not yet loaded.
        /// </summary>
        /// <returns></returns>
        public virtual bool Open()
        {
            return false;
        }

        /// <summary>
        /// Returns whether or not the entry has been opened
        /// </summary>
        /// <returns></returns>
        public virtual bool IsOpen()
        {
            return false;
        }

        /// <summary>
        /// Unloads the project entry, if loaded.
        /// </summary>
        /// <returns></returns>
        public virtual bool Close()
        {
            return false;
        }

        /// <summary>
        /// Renames the file
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        public bool Rename(string newName)
        {
            if (!Exists())
            {
                Name = newName;
                return true;
            }

            string oldName = Name;
            string oldPath = FullPath;

            Name = newName;
            string newPath = FullPath;

            if (!System.IO.File.Exists(newPath))
            {
                System.IO.File.Move(oldPath, newPath);
            }
            else
            {
                Name = oldName;
            }

            return true;
        }

        /// <summary>
        /// Deletes the file that this entry points to
        /// </summary>
        public void Delete()
        {
            if (!Exists())
                return;

            try
            {
                System.IO.File.Delete(FullPath);
            }
            catch (Exception)
            {
            }
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
