#include "iOSSoundSystem.h"
#include "SoundEngine.h"

bool FullFilePath(const char* szRelativePath, char* szFullPath, uint32 len);

/* Constructor
 */
iOSSoundSystem::iOSSoundSystem()
{
}

/* Virtual Destructor
 */
iOSSoundSystem::~iOSSoundSystem(void)
{
#if !TARGET_IPHONE_SIMULATOR	
	SoundMap::iterator it = mSounds.begin();
	
	for(; it != mSounds.end(); it++)
		SoundEngine_UnloadEffect(it->second);
#endif
		
	mSounds.clear();
}

/* Loads a sound with the specified id and path
 */
void iOSSoundSystem::OnLoadSound(uint32 soundID, const char* szSoundPath)
{
#if !TARGET_IPHONE_SIMULATOR	
	if(mSounds.find(soundID) != mSounds.end())
		return;
	
	char fullPath[256];
	FullFilePath(szSoundPath, fullPath, 256);
	
	// Load the sounds
	uint32 effectID = 0;
	SoundEngine_LoadEffect(fullPath, (UInt32*)&effectID);
	
	mSounds[soundID] = effectID;
#endif
}	

/* Unloads a sound with the specified id
 */
void iOSSoundSystem::OnUnloadSound(uint32 soundID)
{
#if !TARGET_IPHONE_SIMULATOR	
	SoundMap::iterator it = mSounds.find(soundID);
	if(it == mSounds.end())
		return;
	
	SoundEngine_UnloadEffect(it->second);
	mSounds.erase(it);
#endif
}

/* Plays a sound at the specified volume
 */
void iOSSoundSystem::OnPlaySound(uint32 soundID, float volume)
{
#if !TARGET_IPHONE_SIMULATOR	
	SoundMap::iterator it = mSounds.find(soundID);
	if(it == mSounds.end())
		return;
	
	SoundEngine_SetEffectLevel(it->second, volume/100.0f);
	SoundEngine_StartEffect(it->second);
#endif
} 