#pragma once
#include <map>
#include "Otter.h"

typedef std::map<uint32, uint32> SoundMap;

/* Implements the Sound System for iOS
 */
class iOSSoundSystem : public Otter::ISoundSystem
{
public:
	/* Constructor
	 */
	iOSSoundSystem();

	/*
	 */
	virtual ~iOSSoundSystem(void);

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
	
	SoundMap mSounds;
};

