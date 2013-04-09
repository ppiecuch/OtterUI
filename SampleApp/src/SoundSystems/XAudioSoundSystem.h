#pragma once
#include <map>
#include <xaudio2.h>
#include "Otter.h"

/* Implements the Sound System as DirectX XAudio 
 */
class XAudioSoundSystem : public Otter::ISoundSystem
{
private:

	struct Sound
	{
		IXAudio2SourceVoice*	mVoice;
		XAUDIO2_BUFFER			mAudioBuffer;
		BYTE*					mData;

		bool					mStarted;
	};

public:
	/* Constructor
	 */
	XAudioSoundSystem(void);

	/*
	 */
	virtual ~XAudioSoundSystem(void);

public:

	/* Loads a sound with the specified id and path
		*/
	virtual void OnLoadSound(uint32 soundID, const char* szSoundPath);

	/* Unloads a sound with the specified id
		*/
	virtual void OnUnloadSound(uint32 soundID);

	/* Plays a sound at the specified volume
		*/
	virtual void OnPlaySound(uint32 soundID, float volume);

private:

	IXAudio2*							mXAudio2;
	IXAudio2MasteringVoice*				mMasteringVoice;

	std::map<int, Sound*>				mSounds;
};

