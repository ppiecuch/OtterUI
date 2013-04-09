using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Linq;
using System.Drawing.Imaging;

using System.IO;
using System.Xml.Serialization;
using System.Xml;

using Otter.Export;
using Otter.UI.Animation;
using Otter.Project;
using Otter.UI.Resources;
using Otter.Containers;

namespace Otter.UI
{
    /// <summary>
    /// TODO : Consider putting the global texture manager into
    /// the scene.
    /// </summary>
    public class GUIScene : IExportable
    {
        #region Events and Delegates
        public delegate void ViewEventHandler(object sender, GUIView view);
        public delegate void TextureEventHandler(object sender, TextureInfo textureInfo);

        public event ViewEventHandler OnViewAdded = null;
        public event ViewEventHandler OnViewRemoved = null;

        public event TextureEventHandler OnTextureUpdated = null;
        #endregion

        #region Data
        private uint mID = 0x0000BEEF;
        private string mVersion = Otter.Properties.Settings.Default.SceneVersion;

        private Resolution mResolution = new Resolution(1024, 768);
        private List<GUIView> mViews = new List<GUIView>();

        // TODO - We need to make textures, messages and sounds all under the same resource
        // list.
        private NotifyingList<TextureInfo> mTextures = new NotifyingList<TextureInfo>();
        private List<SoundInfo> mSounds = new List<SoundInfo>();
        private List<Otter.UI.Message> mMessages = new List<Message>();

        private List<TextureAtlas> mTextureAtlasses = new List<TextureAtlas>();

        private static int mAtlasPadding = 1;

        private List<string> mPluginsUsed = new List<string>();
        private List<string> mMissingPlugins = new List<string>();
        #endregion

        #region Event Handlers : Texture List
        /// <summary>
        /// Called when an item is added to the texture list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="item"></param>
        void mTextures_OnItemAdded(object sender, TextureInfo item)
        {
            item.OnResourceUpdated += new Resource.ResourceEventHandler(TextureInfo_OnResourceUpdated);
        }

        /// <summary>
        /// Called when an item is removed from the texture list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="item"></param>
        void mTextures_OnItemRemoved(object sender, TextureInfo item)
        {
            item.OnResourceUpdated -= new Resource.ResourceEventHandler(TextureInfo_OnResourceUpdated);
        }

        /// <summary>
        /// Called when a texture resource has been updated.
        /// </summary>
        /// <param name="sender"></param>
        void TextureInfo_OnResourceUpdated(object sender)
        {
            NotifyTextureUpdated(sender as TextureInfo);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Scene's ID
        /// </summary>
        [Browsable(false)]
        public uint ID
        {
            get { return mID; }
            set { mID = value; }
        }

        /// <summary>
        /// Gets / Sets the version of the scene
        /// </summary>
        [Browsable(false)]
        public string Version
        {
            get { return mVersion; }
            set { mVersion = value; }
        }

        /// <summary>
        /// Resolution of this scene, and as a result, all of its 
        /// views.
        /// </summary>
        [ReadOnly(true)]
        public Resolution Resolution
        {
            get { return mResolution; }
            set 
            { 
                mResolution = value; 

                foreach(GUIView view in Views)
                    view.Layout.Size = new SizeF(Resolution.Width, this.Resolution.Height);
            }
        }

        /// <summary>
        /// Retrieves the list of plugins used by the project.
        /// This list is updated whenever we save
        /// </summary>
        public List<string> CustomControlsUsed
        {
            get { return mPluginsUsed; }
        }

        /// <summary>
        /// Retrieves the list of plugins we're missing
        /// to ensure that this scene is valid
        /// </summary>
        [XmlIgnore]
        public List<string> MissingCustomControls
        {
            get { return mMissingPlugins; }
        }

        /// <summary>
        /// Gets / Sets the list of used textures in this scene
        /// </summary>
        public NotifyingList<TextureInfo> Textures
        {
            get { return mTextures; }
        }

        /// <summary>
        /// Gets / Sets the list of used sounds in this scene
        /// </summary>
        public List<SoundInfo> Sounds
        {
            get { return mSounds; }
        }

        /// <summary>
        /// Gets / Sets the list of messages used in this scene
        /// </summary>
        public List<Otter.UI.Message> Messages
        {
            get { return mMessages; }
            set { mMessages = value; }
        }

        /// <summary>
        /// Retrieves the texture atlasses.  Only updated if
		/// GenerateTextureAtlasses is called.
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public List<TextureAtlas> TextureAtlasses
        {
            get { return mTextureAtlasses; }
        }

        /// <summary>
        /// Gets a list of all the views within the scene
        /// </summary>
        public List<GUIView> Views
        {
            get
            {
                return mViews;
            }
            set
            {
                if (mViews != value)
                {
                    mViews = value;
                }
            }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public GUIScene()
        {
            mTextures.OnItemAdded += new NotifyingList<TextureInfo>.ListEventHandler(mTextures_OnItemAdded);
            mTextures.OnItemRemoved += new NotifyingList<TextureInfo>.ListEventHandler(mTextures_OnItemRemoved);
        }

        /// <summary>
        /// Adds a view to the scene
        /// </summary>
        /// <param name="view"></param>
        public void AddView(GUIView view)
        {
            AddView(view, -1);
        }

        /// <summary>
        /// Adds a view at the specified index
        /// </summary>
        /// <param name="view"></param>
        /// <param name="atIndex"></param>
        public void AddView(GUIView view, int atIndex)
        {
            if (Views.Contains(view))
                return;

            if (atIndex >= 0)
                Views.Insert(atIndex, view);
            else
                Views.Add(view);

            view.Scene = this;

            NotifyViewAdded(view);
        }

        /// <summary>
        /// Removes a view from the scene
        /// </summary>
        /// <param name="view"></param>
        public void RemoveView(GUIView view)
        {
            if (!Views.Contains(view))
                return;

            Views.Remove(view);
            view.Scene = null;

            NotifyViewRemoved(view);
        }

        /// <summary>
        /// Notifies that a view has been added
        /// </summary>
        /// <param name="view"></param>
        private void NotifyViewAdded(GUIView view)
        {
            if (OnViewAdded != null)
                OnViewAdded(this, view);
        }

        /// <summary>
        /// Notifies that a view has been removed
        /// </summary>
        /// <param name="view"></param>
        private void NotifyViewRemoved(GUIView view)
        {
            if (OnViewRemoved != null)
                OnViewRemoved(this, view);
        }

        /// <summary>
        /// Retrieves a view by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public GUIView GetView(string name)
        {
            foreach (GUIView view in mViews)
            {
                if (view.Name == name)
                    return view;
            }

            return null;
        }

        /// <summary>
        /// Notifies that a texture has been updated in some way
        /// </summary>
        /// <param name="textureInfo"></param>
        private void NotifyTextureUpdated(TextureInfo textureInfo)
        {
            if (OnTextureUpdated != null)
                OnTextureUpdated(this, textureInfo);
        }

        /// <summary>
        /// Retrieves an existing message by count
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public UI.Message GetMessage(int id)
        {
            foreach (UI.Message message in mMessages)
            {
                if (message.ID == id)
                    return message;
            }

            return null;
        }

        /// <summary>
        /// Removes a message from the global message list as well as from
        /// views.
        /// </summary>
        /// <param name="id"></param>
        public void RemoveMessage(int id)
        {
            UI.Message message = GetMessage(id);
            mMessages.Remove(message);

            foreach (GUIView view in Views)
            {
                foreach(GUIAnimation animation in view.Animations)
                {
                    foreach(MainChannelFrame mainChannelFrame in animation.MainChannelFrames)
                    {
                        mainChannelFrame.Actions.RemoveAll((a) => (a is Actions.MessageAction && ((Actions.MessageAction)a).Message == id));
                    }

                    animation.MainChannelFrames.RemoveAll((al) => (al.Actions.Count == 0));
                }
            }
        }

        private static List<Type> GetUsedCustomControls(GUIControlCollection controls)
        {
            List<Type> customControls = new List<Type>();

            foreach (GUIControl control in controls)
            {
                // Found a custom GUIControl.  Ensure that the "ControlAttribute" is present
                System.Attribute attribute = System.Attribute.GetCustomAttribute(control.GetType(), typeof(Plugins.ControlAttribute));
                if (attribute != null)
                {
                    Otter.Plugins.ControlAttribute controlAttribute = (Otter.Plugins.ControlAttribute)attribute;
                    Otter.Plugins.ControlDescriptor controlDescriptor = controlAttribute.GetDescriptor();

                    customControls.Add(control.GetType());
                }

                List<Type> list = GetUsedCustomControls(control.Controls);

                foreach (Type type in list)
                    if (!customControls.Contains(type))
                        customControls.Add(type);
            }

            return customControls;
        }

        /// <summary>
        /// Saves a GUIScene to disk.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool Save(GUIScene scene, XmlAttributeOverrides overrides, string filename)
        {
            FileStream fs = null;
            bool bSuccess = false;

            try
            {
                // First get the list of used control descriptors
                List<Type> controlDescriptors = new List<Type>();
                foreach (GUIView view in scene.Views)
                {
                    List<Type> list = GetUsedCustomControls(view.Controls);

                    foreach (Type type in list)
                        if (!controlDescriptors.Contains(type))
                            controlDescriptors.Add(type);
                }

                // Now store their names
                scene.CustomControlsUsed.Clear();
                scene.CustomControlsUsed.AddRange(controlDescriptors.Select((a) => (a.FullName)));

                fs = new FileStream(filename, FileMode.Create);
                XmlSerializer serializer = new XmlSerializer(typeof(GUIScene), overrides);

                // Prior to serialization, ensure the version is correct
                scene.Version = Otter.Properties.Settings.Default.SceneVersion;
                serializer.Serialize(fs, scene);

                bSuccess = true;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Exception : " + ex);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }

            return bSuccess;
        }

        /// <summary>
        /// Creates a new texture based on the fileName.
        /// If one already exists, returns the existing ID.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public int CreateTexture(string filename)
        {
            return CreateTexture(filename, 1.0f);
        }

        /// <summary>
        /// Creates a texture from file.  If it already exists,
        /// returns the existing texture ID.
        /// 
        /// Additionally, the internal texture will be loaded with the
        /// provided display quality, which defines how the texture will be
        /// rescaled internally.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="displayQuality"></param>
        /// <returns></returns>
        public int CreateTexture(string filename, float displayQuality)
        {
            if (Otter.Interface.Graphics.Instance == null)
                return -1;

            TextureInfo textureInfo = null;

            object lockObj = new System.Object();
            lock (lockObj)
            {
                string texturePath = filename;

                // Convert to a relative path if necessary
                if (System.IO.Path.IsPathRooted(filename))
                    texturePath = Utils.GetRelativePath(GUIProject.CurrentProject.ProjectDirectory, texturePath);

                // See if this texture has already been recorded
                int maxID = 0;
                foreach (TextureInfo info in mTextures)
                {
                    if (info.Filename == texturePath)
                    {
                        textureInfo = info;
                        break;
                    }

                    if (info.ID > maxID)
                        maxID = info.ID;
                }

                // If the texture has not been recorded, do so now.
                if (textureInfo == null)
                {
                    // Open succeeded, now record the info.
                    textureInfo = new TextureInfo();

                    textureInfo.ID = maxID + 1;
                    textureInfo.Filename = texturePath;
                    mTextures.Add(textureInfo);
                }
            }

            textureInfo.DisplayQuality = displayQuality;
            textureInfo.Load();

            return textureInfo.ID;
        }

        /// <summary>
        /// Destroys a texture from the scene.
        /// </summary>
        /// <param name="textureID"></param>
        public void DestroyTexture(int textureID)
        {
            foreach (TextureInfo info in mTextures)
            {
                if (info.ID == textureID)
                {
                    info.Unload();
                    mTextures.Remove(info);

                    return;
                }
            }
        }

        /// <summary>
        /// Creates a new sound based with a particular name.
        /// If one already exists, returns the existing ID.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public int CreateSound(string name)
        {
            // See if this texture has already been recorded
            SoundInfo soundInfo = null;
            int maxID = 0;
            foreach (SoundInfo info in mSounds)
            {
                if (info.Name == name)
                {
                    soundInfo = info;
                    break;
                }

                if (info.ID > maxID)
                    maxID = info.ID;
            }

            // If the texture has not been recorded, do so now.
            if (soundInfo == null)
            {
                // Open succeeded, now record the info.
                soundInfo = new SoundInfo();

                soundInfo.ID = maxID + 1;
                soundInfo.Name = name;

                if (!soundInfo.Load())
                    return -1;

                mSounds.Add(soundInfo);
            }

            return soundInfo.ID;
        }

        /// <summary>
        /// Destroys a sound from the scene.
        /// </summary>
        /// <param name="textureID"></param>
        public void DestroySound(int soundID)
        {
            foreach (SoundInfo info in mSounds)
            {
                if (info.ID == soundID)
                {
                    info.Unload();
                    mSounds.Remove(info);

                    break;
                }
            }
        }

        /// <summary>
        /// Retrieves texture info by ID
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public TextureInfo GetTextureInfo(int textureID)
        {
            foreach (TextureInfo info in mTextures)
            {
                if (info.ID == textureID)
                {
                    return info;
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves texture info by name
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public TextureInfo GetTextureInfo(string textureName)
        {
            foreach (TextureInfo info in mTextures)
            {
                if (info.Filename == textureName)
                {
                    return info;
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves sound info by ID
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public SoundInfo GetSoundInfo(int soundID)
        {
            foreach (SoundInfo info in mSounds)
            {
                if (info.ID == soundID)
                {
                    return info;
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves the final (exported) texture info, with the final texture
        /// id and rect
        /// </summary>
        /// <param name="textureID"></param>
        /// <returns></returns>
        public FinalTexture GetFinalTexture(int textureID)
        {
            TextureInfo info = GetTextureInfo(textureID);
            if (info == null)
                return null;

            FinalTexture finalTexture = new FinalTexture();

            finalTexture.mRectangle = new RectangleF(0.0f, 0.0f, 1.0f, 1.0f);
            finalTexture.mFinalTextureID = GetUniqueTextureID(textureID);
            finalTexture.mTextureInfo = info;

            foreach (TextureAtlas atlas in TextureAtlasses)
            {
                AtlasNode node = atlas.FindNode(info);
                if (node != null)
                {
                    int padding = (info.AtlasPadding < 0) ? mAtlasPadding : info.AtlasPadding;

                    float x = (node.mRectangle.X + padding) / (float)atlas.mRoot.mRectangle.Width;
                    float y = (node.mRectangle.Y + padding) / (float)atlas.mRoot.mRectangle.Height;
                    float w = (node.mRectangle.Width - (padding * 2)) / (float)atlas.mRoot.mRectangle.Width;
                    float h = (node.mRectangle.Height - (padding * 2)) / (float)atlas.mRoot.mRectangle.Height;

                    finalTexture.mRectangle = new RectangleF(x, y, w, h);

                    finalTexture.mFinalTextureID = atlas.mID;
                    finalTexture.mTextureAtlas = atlas;
                    return finalTexture;
                }
            }

            return finalTexture;
        }

        /// <summary>
        /// Converts the local Texture ID to a global unique Texture id, that will 
        /// (hopefully) be unique across the entire project
        /// </summary>
        /// <param name="TextureID"></param>
        /// <returns></returns>
        public uint GetUniqueTextureID(int textureID)
        {
            return (((uint)this.ID) << 24) | ((uint)1 << 16) | (((uint)textureID) & 0x0000FFFF);
        }

        /// <summary>
        /// Converts the local message ID to a global unique message id, that will 
        /// (hopefully) be unique across the entire project
        /// </summary>
        /// <param name="messageID"></param>
        /// <returns></returns>
        public uint GetUniqueMessageID(int messageID)
        {
            return (((uint)this.ID) << 24) | ((uint)3 << 16) | (((uint)messageID) & 0x0000FFFF);
        }

        /// <summary>
        /// Converts the local sound ID to a global unique sound id, that will 
        /// (hopefully) be unique across the entire project
        /// </summary>
        /// <param name="messageID"></param>
        /// <returns></returns>
        public uint GetUniqueSoundID(int soundID)
        {
            return (((uint)this.ID) << 24) | ((uint)4 << 16) | (((uint)soundID) & 0x0000FFFF);
        }

        /* Converts a string version identifier to an int, for easy
         * comparison
         */
        private static int VersionToInt(string version)
        {
            string[] tokens = version.Split(new char[] { '.' });
            if (tokens.Length != 3)
                return 0;

            int result = byte.Parse(tokens[0]) << 16 |
                         byte.Parse(tokens[1]) << 8 |
                         byte.Parse(tokens[2]);

            return result;
        }

        /// <summary>
        /// Loads the GUI Scene from disk
        /// </summary>
        /// <returns></returns>
        public static GUIScene Load(string filename, XmlAttributeOverrides overrides)
        {
            GUIScene scene = null;
            FileStream fs = null;
            try
            {
                fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                XmlSerializer serializer = new XmlSerializer(typeof(GUIScene), overrides);
                serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
                serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);
                serializer.UnknownElement += new XmlElementEventHandler(serializer_UnknownElement);
                scene = serializer.Deserialize(fs) as GUIScene;

                foreach (GUIView view in scene.Views)
                {
                    view.Scene = scene;
                    view.PostImport();

                    // Once all the references and whatnot have been fixed up, set the
                    // sceneView's current state to be the first frame of the first channel.
                    // (effectively frame 0 of "OnActivate")
                    view.CurrentAnimationIndex = 0;
                    if (view.CurrentAnimation != null)
                    {
                        view.CurrentAnimation.Frame = 0;
                        view.CurrentAnimation.UpdateAnimations();
                    }
                }

                // TODO - perform any specific fixups here.

                // Set the scene's version to current, as it is assumed that after
                // all fixups and stuff it is now "correct"
                scene.Version = Otter.Properties.Settings.Default.SceneVersion;

                GUIProjectScene sceneEntry = GUIProject.CurrentProject.GetSceneEntry(filename);
                if (sceneEntry != null)
                    scene.ID = sceneEntry.ID;

                List<string> availableCustomControls = new List<string>();

                // Now cycle through our list of control descriptors and see if we're missing any plugins
                XmlAttributes attributes = overrides[typeof(Otter.UI.GUIControl), "Controls"];
                foreach (XmlArrayItemAttribute xmlAttr in attributes.XmlArrayItems)
                {
                    Type type = xmlAttr.Type;

                    // Found a custom GUIControl.  Ensure that the "ControlAttribute" is present
                    System.Attribute attribute = System.Attribute.GetCustomAttribute(type, typeof(Plugins.ControlAttribute));
                    if (attribute != null)
                    {
                        Otter.Plugins.ControlAttribute controlAttribute = (Otter.Plugins.ControlAttribute)attribute;
                        Otter.Plugins.ControlDescriptor controlDescriptor = controlAttribute.GetDescriptor();

                        if(controlDescriptor != null)
                            availableCustomControls.Add(type.FullName);
                    }
                }

                // We have a list of available plugins, now store the plugins we're missing (if any)
                foreach (string customControlName in scene.CustomControlsUsed)
                {
                    if (!availableCustomControls.Contains(customControlName))
                        scene.MissingCustomControls.Add(customControlName);
                }
            }
            catch (Exception ex)
            {
                System.Console.Write(ex.Message);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }

            return scene;
        }

        static void serializer_UnknownElement(object sender, XmlElementEventArgs e)
        {
            // We changed from "ActionLists" to "MainChannelFrames"
            if (e.Element.Name == "ActionLists")
            {
                GUIAnimation guiAnimation = e.ObjectBeingDeserialized as GUIAnimation;
                guiAnimation.MainChannelFrames.Clear();

                foreach (XmlNode actionsListsNode in e.Element.ChildNodes)
                {
                    if (actionsListsNode.Name != "ActionList")
                        continue;

                    MainChannelFrame mcf = new MainChannelFrame();
                    mcf.Frame = uint.Parse(actionsListsNode.Attributes["Frame"].Value);
                    guiAnimation.MainChannelFrames.Add(mcf);

                    foreach (XmlNode actionListNode in actionsListsNode.ChildNodes)
                    {
                        if (actionListNode.Name != "Actions")
                            continue;

                        foreach (XmlNode actionsNode in actionListNode.ChildNodes)
                        {
                            Actions.Action action = null;

                            if (actionsNode.Name == "SoundAction")
                            {
                                Actions.SoundAction soundAction = new Actions.SoundAction();
                                soundAction.Sound = int.Parse(actionsNode.Attributes["Sound"].Value);
                                soundAction.Volume = uint.Parse(actionsNode["Volume"].InnerText);

                                action = soundAction;
                            }
                            else if (actionsNode.Name == "MessageAction")
                            {
                                Actions.MessageAction messageAction = new Actions.MessageAction();
                                messageAction.Message = int.Parse(actionsNode.Attributes["Message"].Value);

                                action = messageAction;
                            }

                            if (action != null)
                                mcf.Actions.Add(action);
                        }
                    }
                }
            }
        }

        static void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            System.Console.WriteLine("Unknown Attribute: " + e);
        }

        static void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            System.Console.WriteLine("Unknown Node: " + e);
        }

        /// <summary>
        /// Exports the GUI Scene to a file
        /// </summary>
        /// <param name="bw"></param>
        public void Export(PlatformBinaryWriter bw)
        {
            FourCCStack fourCCStack = new FourCCStack(bw);
            fourCCStack.Push("GGSC");
            {
                bw.Write(Otter.Properties.Settings.Default.DataVersion);
                bw.Write((uint)this.ID);

                bw.Write(GUIProject.CurrentProject.Fonts.Count);
                bw.Write(Textures.Count + TextureAtlasses.Count);
                bw.Write(Sounds.Count);
                bw.Write(Views.Count);
                bw.Write(Messages.Count);

                int fontOffsetPos = (int)bw.BaseStream.Position;
                bw.Write(0); // Placeholder - font data start

                int textureOffsetPos = (int)bw.BaseStream.Position;
                bw.Write(0); // Placeholder - texture data start

                int soundOffsetPos = (int)bw.BaseStream.Position;
                bw.Write(0); // Placeholder - sound data start

                int viewDataOffsetPos = (int)bw.BaseStream.Position;
                bw.Write(0); // Placeholder - sceneView data start

                int msgDataOffsetPos = (int)bw.BaseStream.Position;
                bw.Write(0); // Placeholder - message data start;

                int dataStartPos = (int)bw.BaseStream.Position;
                int pos = 0;

                // Fonts
                pos = (int)bw.BaseStream.Position;
                bw.Seek(fontOffsetPos, SeekOrigin.Begin);
                bw.Write(pos - dataStartPos);
                bw.Seek(0, SeekOrigin.End);
                foreach (GUIFont font in GUIProject.CurrentProject.Fonts)
                {
                    font.Export(bw);
                }

                // Textures
                pos = (int)bw.BaseStream.Position;
                bw.Seek(textureOffsetPos, SeekOrigin.Begin);
                bw.Write(pos - dataStartPos);
                bw.Seek(0, SeekOrigin.End);

                // Write out the texture atlasses first
                foreach (TextureAtlas textureAtlas in TextureAtlasses)
                {
                    char[] textureFile = new char[256];
                    textureAtlas.mFilename.CopyTo(0, textureFile, 0, Math.Min(255, textureAtlas.mFilename.Length));

                    bw.Write(textureAtlas.mID);
                    bw.Write(textureFile);
                    bw.Write(0);  // Placeholder : Reference Count

                    bw.Write((uint)0);
                    bw.Write(0.0f);
                    bw.Write(0.0f);
                    bw.Write(1.0f);
                    bw.Write(1.0f);
                }

                foreach (TextureInfo textureInfo in Textures)
                {
                    char[] textureFile = new char[256];
                    textureInfo.Filename.CopyTo(0, textureFile, 0, Math.Min(255, textureInfo.Filename.Length));

                    bw.Write(GetUniqueTextureID(textureInfo.ID));
                    bw.Write(textureFile);
                    bw.Write(0);  // Placeholder : Reference Count

                    FinalTexture finalTexture = GetFinalTexture(textureInfo.ID);

                    bw.Write(finalTexture.mFinalTextureID);
                    bw.Write(finalTexture.mRectangle.X);
                    bw.Write(finalTexture.mRectangle.Y);
                    bw.Write(finalTexture.mRectangle.X + finalTexture.mRectangle.Width);
                    bw.Write(finalTexture.mRectangle.Y + finalTexture.mRectangle.Height);
                }

                // Sounds
                pos = (int)bw.BaseStream.Position;
                bw.Seek(soundOffsetPos, SeekOrigin.Begin);
                bw.Write(pos - dataStartPos);
                bw.Seek(0, SeekOrigin.End);
                foreach (SoundInfo soundInfo in Sounds)
                {
                    char[] soundIdentifier = new char[256];

                    if(soundInfo.Filename != "")
                        soundInfo.Filename.CopyTo(0, soundIdentifier, 0, Math.Min(255, soundInfo.Filename.Length));
                    else
                        soundInfo.Name.CopyTo(0, soundIdentifier, 0, Math.Min(255, soundInfo.Name.Length));

                    bw.Write(GetUniqueSoundID(soundInfo.ID));
                    bw.Write(soundIdentifier);
                    bw.Write(0);  // Placeholder : Reference Count
                }

                // Views
                pos = (int)bw.BaseStream.Position;
                bw.Seek(viewDataOffsetPos, SeekOrigin.Begin);
                bw.Write(pos - dataStartPos);
                bw.Seek(0, SeekOrigin.End);
                foreach (GUIView view in Views)
                {
                    view.Export(bw);
                }

                // Messages
                pos = (int)bw.BaseStream.Position;
                bw.Seek(msgDataOffsetPos, SeekOrigin.Begin);
                bw.Write(pos - dataStartPos);
                bw.Seek(0, SeekOrigin.End);
                foreach (UI.Message msg in Messages)
                {
                    bw.Write(this.GetUniqueMessageID(msg.ID));

                    byte[] bytes = Utils.StringToBytes(msg.Text, 64);
                    bytes[63] = 0;
                    bw.Write(bytes, 0, 64);
                }

                // Write an end-marker for the scene file, to verify all data was loaded properly
                bw.Write((UInt32)0x12344321);
            }
            fourCCStack.Pop();
        }

        /// <summary>
        /// Generates the texture atlasses
        /// </summary>
        public void GenerateTextureAtlasses()
        {
            uint id = 1000;
            int cnt = 1;
            string baseName = "Textures/TA_" + ID + "_";

            mTextureAtlasses.Clear();
            TextureAtlas atlas = null;

            // Get a sorted list of views based on # of used textures
            List<GUIView> sortedViews = new List<GUIView>(Views);
            sortedViews.Sort((a, b) => ( b.TextureIDs.Count - a.TextureIDs.Count ));

            List<int> allTextureIDs = new List<int>();

            foreach (GUIView view in sortedViews)
                allTextureIDs.AddRange(view.TextureIDs);

            foreach (TextureInfo info in Textures)
                allTextureIDs.Add(info.ID);

            allTextureIDs = new List<int>(allTextureIDs.Distinct());

            foreach (int texId in allTextureIDs)
            {
                TextureInfo info = this.GetTextureInfo(texId);
                if (info == null)
                    continue;

                bool bExists = false;

                // This texture has already been added before.
                foreach (TextureAtlas tmpAtlas in mTextureAtlasses)
                {
                    if (tmpAtlas.FindNode(info) != null)
                    {
                        bExists = false;
                    }
                }

                if (bExists)
                    continue;

                if (info.NoAtlas)
                    continue;

                bool clear = (info.AtlasBorderType == TextureInfo.AtlasBorder.Clear);
                int padding = (info.AtlasPadding < 0) ? mAtlasPadding : info.AtlasPadding;

                Bitmap bitmap = Utils.ExpandImageBorder(Image.FromFile(GUIProject.CurrentProject.ProjectDirectory + "/" + info.Filename), padding, clear);
                if (bitmap == null)
                    continue;

                if (atlas == null || !atlas.AddTexture(bitmap, info))
                {
                    uint finalID = (((uint)this.ID) << 24) | ((uint)2 << 16) | (((uint)id++) & 0x0000FFFF);

                    atlas = new TextureAtlas(finalID, 2048, 2048, baseName + cnt + ".png");
                    if (atlas.AddTexture(bitmap, info))
                    {
                        mTextureAtlasses.Add(atlas);
                        cnt++;
                    }
                    else
                        atlas = null;
                }
            }
        }
    }

    /// <summary>
    /// Maps an existing texture to its final texture and 
    /// location.
    /// </summary>
    public class FinalTexture
    {
        public uint mFinalTextureID;
        public RectangleF mRectangle;

        public TextureInfo mTextureInfo;
        public TextureAtlas mTextureAtlas;
    };
}
