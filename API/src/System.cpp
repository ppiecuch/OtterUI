#include <string.h>
#include <stdlib.h>

#include "System.h"
#include "Scene.h"

#include "Data/OtterData.h"
#include "Graphics/Graphics.h"
#include "Memory/Memory.h"
#include "Common/Log.h"

namespace Otter
{	
	/* Constructor.  Allocates an internal buffer from default allocators of size 'memorySize'
	 */
	System::System(int memorySize)
	{
		mScreenWidth = 0;
		mScreenHeight = 0;
		mMemoryBuffer = NULL;

		if(memorySize > 0)
		{
			mMemoryBuffer = (uint8*)malloc(memorySize);
			MemoryManager::initializeMemoryManager(mMemoryBuffer, memorySize);
		}

		mFileSystem = NULL;
		mSoundSystem = NULL;
		mGraphics = OTTER_NEW(Graphics, ());

		SetResolution(1024, 768);
	}

	/* Constructor.  Uses the provided buffer as its heap memory.
	 */
	System::System(uint8* pMemoryBuffer, int memorySize)
	{
		MemoryManager::initializeMemoryManager(pMemoryBuffer, memorySize);

		InitLog(this);

		mFileSystem = NULL;
		mGraphics = NULL;

		mScreenWidth = 0;
		mScreenHeight = 0;
		mMemoryBuffer = NULL;		

		SetResolution(1024, 768);
	}

	/* Destructor
	 */
	System::~System(void)
	{
		UnloadAllScenes();
		DeinitLog();

		mScenes.clear(true);
		mPlugins.clear(true);
		mOnSceneLoaded.Clear();
		mOnSceneUnloaded.Clear();
		mOnLog.Clear();

		OTTER_DELETE(mGraphics);
		mGraphics = NULL;

		MemoryManager::deinitializeMemoryManager();

		if(mMemoryBuffer)
		{
			free(mMemoryBuffer);
			mMemoryBuffer = NULL;
		}
	}

	/* Sets the file system.  This object will be called 
	 * when opening / closing / reading from files.
	 */
	Result System::SetFileSystem(IFileSystem* pFileSystem) 
	{ 
		mFileSystem = pFileSystem; 
		
		return kResult_OK;
	}

	/* Sets the object responsible for rendering the GUI
	 */
	Result System::SetRenderer(IRenderer* pRenderer) 
	{ 
		mGraphics->SetUserRenderer(pRenderer); 

		return kResult_OK;
	}

	/* Sets the sound system
	 */
	Result System::SetSoundSystem(ISoundSystem* pSoundSystem)
	{
		mSoundSystem = pSoundSystem;
		return kResult_OK;
	}

	/* Adds a plugin
		*/
	Result System::AddPlugin(IPlugin* pPlugin)
	{
		if(pPlugin == NULL)
			return kResult_Error;

		int cnt = mPlugins.size();
		for(int i = 0; i < cnt; i++)
		{
			if(mPlugins[i] == pPlugin)
				return kResult_OK;
		}

		mPlugins.push_back(pPlugin);
		return kResult_OK;
	}

	/**
	 * Enables / disables (cpu) pre-transforming of vertices.
	 * Enabling vertex pretransformation may reduce draw calls at the cost
	 * of additional CPU load.
	 */
	Result System::EnablePreTransformVerts(bool bEnable)
	{
		if(!mGraphics)
			return kResult_Error;

		mGraphics->EnablePreTransformVerts(bEnable);

		return kResult_OK;
	}

	/** 
	 * Enables / disables logging
	 */
	Result System::EnableLogging(bool bEnable)
	{
		if(bEnable)
		{
			InitLog(this);
		}
		else
		{
			DeinitLog();
		}

		return kResult_OK;
	}

	/* Sets the resolution of the UI.  Can be called at any time.
	 */
	Result System::SetResolution(uint32 width, uint32 height)
	{
		mScreenWidth = width;
		mScreenHeight = height;

		int cnt = GetSceneCount();
		for(int i = 0; i < cnt; i++)
			GetScene(i)->SetResolution(mScreenWidth, mScreenHeight);

		return kResult_OK;
	}

	/* Loads a scene from file
	 */
	Result System::LoadScene(const char* szPath, Scene** ppScene)
	{
		if(!mFileSystem)
			return kResult_NoFileSystem;

		void* pHandle = (void*)mFileSystem->Open(szPath, AccessFlag(kBinary | kRead));

		if(pHandle == NULL)			
			return kResult_FileNotFound;
		
		uint32 size = mFileSystem->Size(pHandle);
		if(size == 0)
			return kResult_InvalidFile;

		uint8* pBuffer = (uint8*)OTTER_ALLOC(size);
		uint32 readSize = mFileSystem->Read(pHandle, pBuffer, size);
		mFileSystem->Close(pHandle);

		// Check if we failed to read all the data, or if we
		// simply failed to load the scene.  Cleanup and bail if so.
		if(readSize != size)
		{
			OTTER_FREE(pBuffer);
			return kResult_FailedRead;
		}
		
		return LoadScene(pBuffer, size, false, ppScene);
	}

	/* Loads a scene from an existing in-memory buffer
	 */
	Result System::LoadScene(const uint8* pBuffer, uint32 bufferSize, bool bCopy, Scene** ppScene)
	{
		// Copy the data if necessary.
		const uint8* pSceneBuffer = pBuffer;
		if(bCopy)
		{
			pSceneBuffer = (uint8*)OTTER_ALLOC(bufferSize);
			memcpy((uint8*)pSceneBuffer, pBuffer, bufferSize);
		}

		// Cast the buffer to the scene data object and verify it's a 
		// valid scene data buffer and version.
		const SceneData* pSceneData = (const SceneData*)pSceneBuffer;
		if(pSceneData->mFourCC != FOURCC_GGSC)
		{
			return kResult_InvalidSceneFile;
		}

		if(pSceneData->mVersion != SCENE_DATA_VERSION)
		{
			return kResult_InvalidSceneVersion;
		}

		Scene* pScene = OTTER_NEW(Scene, (this, mGraphics, mSoundSystem, pSceneData));
		pScene->SetResolution(mScreenWidth, mScreenHeight);

		mScenes.push_back(pScene);

		mOnSceneLoaded(this, pScene);

		if(ppScene)
			*ppScene = pScene;

		return kResult_OK;
	}

	/**
	 * Unloads the specified scene
	 */
	Result System::UnloadScene(Scene* pScene)
	{
		if(!pScene)
			return kResult_Error;

		uint32 numActiveViews = pScene->GetNumActiveViews();
		for(uint32 i = 0; i <  numActiveViews; i++)
		{
			View* view = pScene->GetActiveView(i);
			view->OnDeactivate();
		}

		// Grab a pointer to the internal scene data in case we need
		// to free it ourselves later.
		const uint8* pData = (const uint8*)pScene->GetData();

		mOnSceneUnloaded(this, pScene);
		
		OTTER_DELETE(pScene);

		// The data may not have come from us.  If it's the memory manager's
		// memory pool, we allocated it and so we must release it.
		if(MemoryManager::isValid(pData))
		{
			OTTER_FREE((void*)pData);
		}

		for(uint32 i = 0; i < mScenes.size(); i++)
		{
			if(mScenes[i] == pScene)
			{
				mScenes.erase(i);
				break;
			}
		}

		return kResult_OK;
	}

	/**
	 * Unloads a scene by index
	 */
	Result System::UnloadScene(uint32 index)
	{
		return UnloadScene(GetScene(index));
	}

	/* Unloads the active scene
	 */
	Result System::UnloadAllScenes()
	{
		while(mScenes.size() > 0)
		{
			UnloadScene(mScenes[0]);
		}

		mScenes.clear();
		return kResult_OK;
	}

	/* Returns a loaded scene by index
		*/
	Scene* System::GetScene(uint32 index) 
	{ 
		if(index >= mScenes.size()) 
			return 0; 
			
		return mScenes[index]; 
	}

	/* Returns the number of scenes currently loaded
		*/
	uint32 System::GetSceneCount() 
	{ 
		return mScenes.size(); 
	}

	/* Draws the GUI
	 */
	Result System::Draw()
	{
		if(!mGraphics)
			return kResult_NoRenderer;

		uint32 numScenes = GetSceneCount();		
		if(numScenes)
		{
			mGraphics->Begin();

			for(uint32 i = 0; i < numScenes; i++)
				GetScene(i)->Draw();

			mGraphics->End();
		}

		return kResult_OK;
	}

	/* Updates the GUI
	 * "frameDelta" indicates the number of frames that have passed since last update.
	 */
	Result System::Update(float frameDelta)
	{
		uint32 numScenes = GetSceneCount();		
		if(numScenes)
		{
			for(uint32 i = 0; i < numScenes; i++)
				GetScene(i)->Update(frameDelta);
		}

		return kResult_OK;
	}
		
	/* Points (touches/mouse/etc) were pressed down
	 */
	void System::OnPointsDown(Point* points, sint32 numPoints)
	{
		uint32 numScenes = GetSceneCount();		
		if(numScenes)
		{
			for(uint32 i = 0; i < numScenes; i++)
				GetScene(i)->OnPointsDown(points, numPoints);
		}
	}
	
	/* Points (touches/mouse/etc) were released
	 */
	void System::OnPointsUp(Point* points, sint32 numPoints)
	{
		uint32 numScenes = GetSceneCount();		
		if(numScenes)
		{
			for(uint32 i = 0; i < numScenes; i++)
				GetScene(i)->OnPointsUp(points, numPoints);
		}
	}
	
	/* Points (touches/mouse/etc) were moved.
	 */
	void System::OnPointsMove(Point* points, sint32 numPoints)
	{
		uint32 numScenes = GetSceneCount();		
		if(numScenes)
		{
			for(uint32 i = 0; i < numScenes; i++)
				GetScene(i)->OnPointsMove(points, numPoints);
		}
	}
};
