/*! \mainpage OtterUI API Reference Documentation
 *
 * \section intro_sec Introduction
 *
 * Welcome to the OtterUI API Reference Documentation.  The following pages details
 * all of the public interfaces within the OtterUI API to help you use the API more
 * effectively.
 */	

#pragma once
#include "Common/Array.h"
#include "Common/Types.h"
#include "Common/Event.h"

namespace Otter
{
	class FileIO;
	class Graphics;
	class Scene;
	class IFileSystem;
	class IRenderer;
	class ISoundSystem;
	class IPlugin;

	/**
	 * Main object that manages and maintains all things game-gui related.
	 */
	class System
	{
	public:
		/**
		 * Constructs the OtterUI system object by allocating an internal buffer.
		 *
		 * @param memorySize	Size of internally allocated memory buffer
		 */
		System(int memorySize);

		/**
		 * Constructs the OtterUI system object by using a provided memory buffer.
		 * 
		 * @param pMemoryBuffer User-provided memory buffer
		 * @param memorySize Size of the user-provided memory buffer (bytes)
		 */
		System(uint8* pMemoryBuffer, int memorySize);

		/**
		 * Destructor.
		 */
		~System(void);

	public:

		/**
		 * Sets the file system.  This object will be called 
		 * when opening / closing / reading from files.
		 */
		Result SetFileSystem(IFileSystem* pFileSystem);

		/**
		 * Sets the object responsible for rendering the GUI
		 */
		Result SetRenderer(IRenderer* pRenderer);

		/**
		 * Sets the sound system
		 */
		Result SetSoundSystem(ISoundSystem* pSoundSystem);

		/**
		 * Adds a plugin
		 */
		Result AddPlugin(IPlugin* pPlugin);

		/**
		 * Retrieves the plugin array
		 */
		const Array<IPlugin*>& GetPlugins() const { return mPlugins; }

		/**
		 * Enables / disables (cpu) pre-transforming of vertices.
		 * Enabling vertex pretransformation may reduce draw calls at the cost
		 * of additional CPU load.
		 */
		Result EnablePreTransformVerts(bool bEnable = true);

		/** 
		 * Enables / disables logging
		 */
		Result EnableLogging(bool bEnable = true);

	public:
		
		/**
		 * Points (touches/mouse/etc) were pressed down
		 */
		void OnPointsDown(Point* points, sint32 numPoints);
		
		/**
		 * Points (touches/mouse/etc) were released
		 */
		void OnPointsUp(Point* points, sint32 numPoints);

		/**
		 * Points (touches/mouse/etc) were moved.
		 */
		void OnPointsMove(Point* points, sint32 numPoints);

	public:

		/**
		 * Sets the resolution of the UI.  Can be called at any time.
		 */
		Result SetResolution(uint32 width, uint32 height);

		/**
		 * Loads a scene from file
		 */
		Result LoadScene(const char* szPath, Scene** ppScene = NULL);

		/**
		 * Loads a scene from an existing in-memory buffer
		 */
		Result LoadScene(const uint8* pBuffer, uint32 bufferSize, bool bCopy, Scene** ppScene = NULL);

		/**
		 * Unloads the specified scene
		 */
		Result UnloadScene(Scene* pScene);

		/**
		 * Unloads a scene by index
		 */
		Result UnloadScene(uint32 index);

		/**
		 * Unloads all loaded scenes
		 */
		Result UnloadAllScenes();

		/**
		 * Returns a loaded scene by index
		 */
		Scene* GetScene(uint32 index);

		/**
		 * Returns the number of scenes currently loaded
		 */
		uint32 GetSceneCount();

	public:

		/**
		 * Draws the GUI
		 */
		Result Draw();

		/**
		 * Updates the GUI
		 *
		 * @param frameDelta Number of frames that have passed since last update.
		 */
		Result Update(float frameDelta);

	private:
		/**
		 * Internal graphics object
		 */
		Graphics*				mGraphics;

		/**
		 * User implementation of the file system
		 */
		IFileSystem*			mFileSystem;

		/**
		 * User implementation of the sound system
		 */
		ISoundSystem*			mSoundSystem;

		/**
		 * Array of loaded scenes
		 */
		Array<Scene*>			mScenes;

		/**
		 * Array of user plugins
		 */
		Array<IPlugin*>			mPlugins;

		/**
		 * Current screen width
		 */
		int						mScreenWidth;

		/**
		 * Current screen height
		 */
		int						mScreenHeight;

		/**
		 * Memory buffer from OtterUI will allocate all of its memory
		 */
		uint8*					mMemoryBuffer;

	public:
		/**
		 * Raised whenever a scene has been loaded
		 * Event Parameter: Pointer to the scene object that has been loaded
		 */
		Event<Scene*>			mOnSceneLoaded;

		/**
		 * Raised whenever a scene has been unloaded
		 * Event Parameter: Pointer to the scene object that has been unloaded
		 */
		Event<Scene*>			mOnSceneUnloaded;

		/**
		 * Raised whenever OtterUI output debug/runtime logging information
		 * Event Parameter: NULL-terminated C-String with the log entry
		 */
		Event<const char*>		mOnLog;
	};
}