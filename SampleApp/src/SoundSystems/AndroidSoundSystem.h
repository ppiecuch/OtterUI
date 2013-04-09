#pragma once
#include <map>
#include <jni.h>
#include "Otter.h"

typedef std::map<uint32, int> SoundMap;

/* Implements the Sound System for Android.  Makes JNI
 * calls back to the Java layer to load and play sounds.
 */
class AndroidSoundSystem : public Otter::ISoundSystem
{
public:
	/* Constructor
	 */
	AndroidSoundSystem(JNIEnv* pEnv);

	/*
	 */
	virtual ~AndroidSoundSystem(void);

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

	JNIEnv*	 mEnv;
	SoundMap mSounds;

	jclass	  mOtterRenderer;
	jmethodID mUnloadAllSoundsMethod;
	jmethodID mLoadSoundMethod;
	jmethodID mUnloadSoundMethod;
	jmethodID mPlaySoundMethod;
};

