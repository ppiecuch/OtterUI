using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

using System.Timers;

using Otter.Project;

using NAudio.Wave;

namespace Otter.UI.Resources
{
    /// <summary>
    /// Contains information on a particular sound
    /// </summary>
    public class SoundInfo : Resource
    {
        #region Events and Delegates
        public delegate void OnSoundEventHandler(SoundInfo info);

        public event OnSoundEventHandler OnStopped;
        #endregion

        #region Data
        private string mName = "";
        private int mSoundID = -1;

        private string mFilename = "";

        private IWavePlayer mWaveOut = null;
        private WaveStream mWaveStream = null;
        private Timer mTimer = null;

        #endregion

        #region Properties
        /// <summary>
        /// Gets / Sets the name of the sound
        /// </summary>
        [XmlAttribute]
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// Gets / Sets the associated sound filename if this sound has 
        /// an actual sound associated with it.
        /// </summary>
        [XmlAttribute]
        public string Filename
        {
            get { return mFilename; }
            set 
            {
                if (mFilename != value)
                {
                    mFilename = value;

                    if (mWaveOut != null && mWaveStream != null)
                    {
                        Unload();
                        Load();
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves the internal sound id
        /// </summary>
        [XmlIgnore]
        public int SoundID
        {
            get { return mSoundID; }
            set { mSoundID = value; }
        }
        #endregion

        /// <summary>
        /// Loads the sound
        /// </summary>
        /// <returns></returns>
        public override bool Load()
        {
            if (Filename == "")
                return true;

            if (mWaveOut != null && mWaveStream != null)
                return true;

            bool bLoaded = false;
            try
            {
                mWaveOut = new WaveOut(WaveCallbackInfo.FunctionCallback());
                mWaveStream = CreateInputStream(GUIProject.CurrentProject.ProjectDirectory + "/" + Filename);
                mWaveOut.Init(mWaveStream);
                bLoaded = true;
            }
            catch (Exception)
            {
                Unload();
            }

            return bLoaded;
        }

        /// <summary>
        /// Unloads the sound
        /// </summary>
        /// <returns></returns>
        public override bool Unload()
        {
            if (Filename == "")
                return true;

            if (mWaveOut != null)
                mWaveOut.Stop();

            if (mWaveStream != null)
            {
                mWaveStream.Close();
                mWaveStream = null;
            }

            if (mWaveOut != null)
            {
                mWaveOut.Dispose();
                mWaveOut = null;
            }

            return true;
        }

        /// <summary>
        /// Creates an input stream from the specified filename
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private WaveStream CreateInputStream(string fileName)
        {
            WaveChannel32 inputStream;
            if (fileName.EndsWith(".wav"))
            {
                WaveStream readerStream = new WaveFileReader(fileName);
                if (readerStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm)
                {
                    readerStream = WaveFormatConversionStream.CreatePcmStream(readerStream);
                    readerStream = new BlockAlignReductionStream(readerStream);
                }
                if (readerStream.WaveFormat.BitsPerSample != 16)
                {
                    var format = new WaveFormat(readerStream.WaveFormat.SampleRate,
                        16, readerStream.WaveFormat.Channels);
                    readerStream = new WaveFormatConversionStream(format, readerStream);
                }
                inputStream = new WaveChannel32(readerStream);
            }
            else if (fileName.EndsWith(".mp3"))
            {
                WaveStream mp3Reader = new Mp3FileReader(fileName);
                inputStream = new WaveChannel32(mp3Reader);
            }
            else
            {
                throw new InvalidOperationException("Unsupported extension");
            }

            return inputStream;
        }

        /// <summary>
        /// Returns whether or not this sound can be played back
        /// </summary>
        /// <returns></returns>
        public bool IsPlayable()
        {
            return (mWaveOut != null && mWaveStream != null);
        }

        /// <summary>
        /// Plays the sound
        /// </summary>
        /// <returns></returns>
        public bool Play(ISynchronizeInvoke syncObject)
        {
            if (mWaveOut == null || mWaveStream == null)
                return false;

            if (mWaveOut.PlaybackState != PlaybackState.Stopped)
                Stop();

            mTimer = new System.Timers.Timer();
            mTimer.Interval = 1;
            mTimer.Elapsed += new System.Timers.ElapsedEventHandler(mTimer_Elapsed);
            mTimer.SynchronizingObject = syncObject;
            mTimer.Start();

            mWaveStream.CurrentTime = TimeSpan.FromSeconds(0.0);
            mWaveOut.Play();
            return true;
        }

        /// <summary>
        /// Timer callback to check when the sound has finished playing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (mTimer != null && mWaveStream.Position >= mWaveStream.Length)
            {
                mWaveOut.Stop();

                if (OnStopped != null)
                    OnStopped(this);

                mTimer.Stop();
                mTimer.Dispose();
                mTimer = null;
            }
        }

        /// <summary>
        /// Returns whether or not the sound is currently playing back
        /// </summary>
        /// <returns></returns>
        public bool IsPlaying()
        {
            if (mWaveOut != null && mWaveOut.PlaybackState == PlaybackState.Playing)
                return true;

            return false;
        }

        /// <summary>
        /// Stops playback
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            if (mWaveOut.PlaybackState == PlaybackState.Stopped && mTimer == null)
                return false;

            mWaveOut.Stop();

            if (mTimer != null)
            {
                mTimer.Stop();
                mTimer.Dispose();
                mTimer = null;
            }

            if (OnStopped != null)
                OnStopped(this);

            return true;
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
