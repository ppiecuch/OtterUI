using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

using Otter.UI;

namespace Otter.Project
{
    /// <summary>
    /// A Scene Entry
    /// </summary>
    public class GUIProjectScene : GUIProjectEntry
    {
        #region Data
        private GUIScene mGUIScene = null;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the scene object encapsulated by this entry
        /// </summary>
        public GUIScene Scene
        {
            get { return mGUIScene; }
        }
        #endregion

        /// <summary>
        /// Parameterless Constructor - for xml (de)serialization
        /// </summary>
        public GUIProjectScene()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        public GUIProjectScene(string name)
        {
            mName = name;
        }

        /// <summary>
        /// Saves the scene to disk
        /// </summary>
        /// <returns></returns>
        public override bool Save()
        {
            GUIScene scene = mGUIScene;
            if (scene == null)
            {
                if (!Exists())
                {
                    scene = new GUIScene();
                }
                else
                {
                    return false;
                }
            }

            return GUIScene.Save(scene, GUIProject.XmlAttributeOverrides, FullPath);
        }

        /// <summary>
        /// Loads the scene, if not yet loaded.
        /// </summary>
        /// <returns></returns>
        public override bool Open()
        {
            if (mGUIScene != null)
                return true;

            mGUIScene = GUIScene.Load(FullPath, GUIProject.XmlAttributeOverrides);
            return (mGUIScene != null);
        }

        /// <summary>
        /// Returns whether or not the scene has been opened
        /// </summary>
        /// <returns></returns>
        public override bool IsOpen()
        {
            return mGUIScene != null;
        }

        /// <summary>
        /// Unloads the scene
        /// </summary>
        /// <returns></returns>
        public override bool Close()
        {
            if (mGUIScene == null)
                return false;

            mGUIScene = null;
            return true;
        }
    }
}
