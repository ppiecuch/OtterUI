#include <android/log.h>
#include "AndroidSoundSystem.h"

/* Constructor
 */
AndroidSoundSystem::AndroidSoundSystem(JNIEnv* pEnv)
{
	mEnv = pEnv;
	mLoadSoundMethod = NULL;
	mUnloadSoundMethod = NULL;
	mPlaySoundMethod = NULL;

	if(!mEnv)
		return;

	mOtterRenderer = mEnv->FindClass("com/aonyx/ottersample/OtterRenderer");
	
	if(!mOtterRenderer)
	{
		__android_log_print(ANDROID_LOG_INFO, "OtterSample", "Failed to find OtterSample class");
		return;
	}
	else
	{
		__android_log_print(ANDROID_LOG_INFO, "OtterSample", "mOtterRenderer: 0x%08X", mOtterRenderer);
	}

	mUnloadAllSoundsMethod = mEnv->GetStaticMethodID(mOtterRenderer, "UnloadAllSounds", "()V");	
	if(!mUnloadAllSoundsMethod)
	{
		__android_log_print(ANDROID_LOG_INFO, "OtterSample", "Failed to find UnloadAllSounds method");
	}
	else
	{
		__android_log_print(ANDROID_LOG_INFO, "OtterSample", "mUnloadAllSoundsMethod: 0x%08X", mUnloadAllSoundsMethod);
	}

	mLoadSoundMethod = mEnv->GetStaticMethodID(mOtterRenderer, "LoadSound", "(Ljava/lang/String;)I");	
	if(!mLoadSoundMethod)
	{
		__android_log_print(ANDROID_LOG_INFO, "OtterSample", "Failed to find LoadSound method");
	}
	else
	{
		__android_log_print(ANDROID_LOG_INFO, "OtterSample", "mLoadSoundMethod: 0x%08X", mLoadSoundMethod);
	}

	mUnloadSoundMethod = mEnv->GetStaticMethodID(mOtterRenderer, "UnloadSound", "(I)V");	
	if(!mUnloadSoundMethod)
	{
		__android_log_print(ANDROID_LOG_INFO, "OtterSample", "Failed to find UnloadSound method");
	}
	else
	{
		__android_log_print(ANDROID_LOG_INFO, "OtterSample", "mUnloadSoundMethod: 0x%08X", mUnloadSoundMethod);
	}

	mPlaySoundMethod = mEnv->GetStaticMethodID(mOtterRenderer, "PlaySound", "(IF)V");	
	if(!mPlaySoundMethod)
	{
		__android_log_print(ANDROID_LOG_INFO, "OtterSample", "Failed to find PlaySound method");
	}
	else
	{
		__android_log_print(ANDROID_LOG_INFO, "OtterSample", "mPlaySoundMethod: 0x%08X", mPlaySoundMethod);
	}
}

/* Virtual Destructor
 */
AndroidSoundSystem::~AndroidSoundSystem(void)
{
	if(!mEnv)
		return;

	if(mUnloadAllSoundsMethod)		
		mEnv->CallStaticIntMethod(mOtterRenderer, mUnloadAllSoundsMethod);

	mSounds.clear();
}

/* Loads a sound with the specified id and path
 */
void AndroidSoundSystem::OnLoadSound(uint32 soundID, const char* szSoundPath)
{
	if(!mLoadSoundMethod || mSounds.find(soundID) != mSounds.end())
		return;
	
	jstring	jstr = mEnv->NewStringUTF(szSoundPath);
    if (jstr == 0)
	{
		__android_log_print(ANDROID_LOG_INFO, "OtterSample", "Failed to create new string from UTF");
		return;
	}

	int sid = mEnv->CallStaticIntMethod(mOtterRenderer, mLoadSoundMethod, jstr);
	if(sid > 0)
	{
		mSounds[soundID] = sid;
	}
	else
	{
		__android_log_print(ANDROID_LOG_INFO, "OtterSample", "Failed sound[%d] : %d", soundID, sid);
	}
}	

/* Unloads a sound with the specified id
 */
void AndroidSoundSystem::OnUnloadSound(uint32 soundID)
{
	if(!mUnloadSoundMethod)
		return;
		
	SoundMap::iterator it = mSounds.find(soundID);
	if(it == mSounds.end())
		return;

	mEnv->CallStaticVoidMethod(mOtterRenderer, mUnloadSoundMethod, it->second);
	mSounds.erase(it);
}

/* Plays a sound at the specified volume
 */
void AndroidSoundSystem::OnPlaySound(uint32 soundID, float volume)
{
	if(!mPlaySoundMethod)
		return;
		
	SoundMap::iterator it = mSounds.find(soundID);
	if(it == mSounds.end())
		return;

	mEnv->CallStaticVoidMethod(mOtterRenderer, mPlaySoundMethod, it->second, volume);
} 