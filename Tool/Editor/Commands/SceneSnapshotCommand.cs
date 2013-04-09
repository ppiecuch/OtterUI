using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

using Otter.UI;
using Otter.Commands;

namespace Otter.Editor.Commands
{
    /// <summary>
    /// Takes a snapshot of an entire scene, serializes it, and applies it during undo/redo.
    /// NOTE:  This is not currently used.  Here for reference, in case we want to implement snapshot
    /// undo/redo.
    /// </summary>
    public class SceneSnapshotCommand : Command
    {
        #region Data
        private MemoryStream mSnapshotBefore = null;
        private MemoryStream mSnapshotAfter = null;
        private GUIScene mScene = null;
        private string mDesc = "";
        #endregion

        #region Properties
        /// <summary>
        /// Gets the scene as serialized/deserialized by the command.
        /// Note: NOT the original scene provided in the constructor.  Every time the command
        /// is executed/undone this will return a new scene
        /// </summary>
        public GUIScene Scene
        {
            get { return mScene; }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="description"></param>
        public SceneSnapshotCommand(string description)
        {
            mDesc = description;
        }

        /// <summary>
        /// Saves the scene as a 'before' snapshot.
        /// </summary>
        /// <param name="scene"></param>
        public void SaveBefore(GUIScene scene)
        {
            Save(scene, ref mSnapshotBefore);
        }

        /// <summary>
        /// Saves the scene as an 'after' snapshot.
        /// </summary>
        /// <param name="scene"></param>
        public void SaveAfter(GUIScene scene)
        {
            Save(scene, ref mSnapshotAfter);
        }

        /// <summary>
        /// Sets the 
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="ms"></param>
        private void Save(GUIScene scene, ref MemoryStream ms)
        {
            try
            {
                ms = new MemoryStream();
                XmlSerializer serializer = new XmlSerializer(typeof(GUIScene));
                serializer.Serialize(ms, scene);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
            finally
            {
                if (ms != null)
                    ms.Close();
            }
        }

        /// <summary>
        /// Loads a scene from a memory stream
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        private GUIScene Load(MemoryStream ms)
        {
            GUIScene scene = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(GUIScene));
                scene = serializer.Deserialize(ms) as GUIScene;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

            return scene;
        }

        /// <summary>
        /// Checks if there is a difference between the before and after snapshots
        /// </summary>
        /// <returns></returns>
        public bool HasChanged()
        {
            if (mSnapshotBefore == null || mSnapshotAfter == null)
                return false;

            byte[] before = mSnapshotBefore.ToArray();
            byte[] after = mSnapshotAfter.ToArray();

            if (before.Length != after.Length)
                return true;

            for (int i = 0; i < before.Length; i++)
            {
                if (before[i] != after[i])
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Executes the command.  Deserializes the 'after' data and creates a new scene
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            mScene = Load(mSnapshotAfter);
            return mScene != null;
        }

        /// <summary>
        /// Executes the command.  Deserializes the 'before' data and creates a new scen
        /// </summary>
        /// <returns></returns>
        public override bool Undo()
        {
            mScene = Load(mSnapshotBefore);
            return mScene != null;
        }

        /// <summary>
        /// ToString override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return mDesc;
        }
    }
}
