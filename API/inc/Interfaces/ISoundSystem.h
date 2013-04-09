#pragma once

#include "Common/Types.h"
#include "Otter.h"

namespace Otter
{
	/* Responsible for loading and playing sounds
	 */
	class ISoundSystem
	{
	public:
		/**
		 * Constructor
		 */
		ISoundSystem() { }

		/**
		 * Virtual Destructor
		 */
		virtual ~ISoundSystem() { }

	public:

		/**
		 * Loads a sound with the specified id and path
		 */
		virtual void OnLoadSound(uint32 soundID, const char* szSoundPath) { }

		/**
		 * Unloads a sound with the specified id
		 */
		virtual void OnUnloadSound(uint32 soundID) { }

		/**
		 * Plays a sound at the specified volume
		 */
		virtual void OnPlaySound(uint32 soundID, float volume) { }
	};
}