#pragma once

#include "Common/Array.h"
#include "Common/Types.h"

namespace Otter
{
	struct SceneData;
	struct ViewData;
	struct TextureData;

	class View;
	class Font;
	class Graphics;
	class System;
	class ISoundSystem;

	/**
	 * A scene maintains a collection of views, and is responsible for managing incoming, outgoing, and active views.
	 */
	class Scene
	{
	public:
		/**
		 * Constructor
		 */
		Scene(System* pSystem, Graphics* pGraphics, ISoundSystem* pSoundSystem, const SceneData* pSceneData);

		/**
		 * Destructor
		 */
		~Scene(void);

	public:

		/**
		 * Retrieves the internal scene data
		 */
		const SceneData* GetData() const { return mSceneData; }

		/**
		 * Gets the SceneID
		 */
		uint32 GetID();

		/**
		 * Retrieves the total number of views within this scene
		 */
		uint32 GetNumViews();

		/**
		 * Retrieves a view by name
		 */
		View* GetView(const char* szName);

		/**
		 * Retrieves a view by index
		 */
		View* GetView(uint32 index);

		/**
		 * Activates a view by name
		 */
		Result ActivateView(const char* szName);

		/**
		 * Activates a view by index
		 */
		Result ActivateView(uint32 index);

		/**
		 * Activates a view by pointer
		 */
		Result ActivateView(View* pView);

		/**
		 * Deactivates a view by name
		 */
		Result DeactivateView(const char* szName);

		/**
		 * Deactivates a view by index
		 */
		Result DeactivateView(uint32 index);

		/**
		 * Deactivates a view by pointer
		 */
		Result DeactivateView(View* pView);

		/**
		 * Retrieves the total number of views within this scene
		 */
		uint32 GetNumActiveViews();

		/**
		 * Retrieves an active view by name
		 */
		View* GetActiveView(const char* szName);

		/**
		 * Retrieves an active view by index
		 */
		View* GetActiveView(uint32 index);

		/**
		 * Loads all the resources for a particular view
		 */
		void LoadViewResources(const char* szName);

		/**
		 * Loads all the resources for a particular view
		 */
		void UnLoadViewResources(const char* szName);

		/**
		 * Loads a resource
		 */
		bool LoadResource(uint32 resource);

		/** 
		 * Unloads the specified resource
		 */
		bool UnloadResource(uint32 resource);
		  
		/**
		 * Retrieves a font by ID
		 */
		Font* GetFont(uint32 fontID);
		  
		/**
		 * Retrieves a font by name
		 */
		Font* GetFont(const char* szName);

		/**
		 * Retrieves a texture id by name
		 */
		uint32 GetTextureID(const char* szTextureName);

		/**
		 * Retrieves the texture data by texture id
		 */
		const TextureData* GetTextureData(uint32 textureID);

		/**
		 * Retrieves the OtterUI System object
		 */
		System* GetSystem() { return mSystem; }

		/**
		 * Retrieves the Sound System
		 */
		ISoundSystem* GetSoundSystem() { return mSoundSystem; }

		/**
		 * Sets the resolution of the scene
		 */
		void SetResolution(uint32 width, uint32 height);

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
		 * Draws the scene
		 */
		void Draw();

		/**
		 * Updates the scene.
		 *
		 * @param frameDelta Defines the number of frames that have passed since last update.
		 */
		void Update(float frameDelta);

	private:

		/**
		 * Loads the scene from the provided scene data
		 */
		bool LoadFromData(const SceneData* pSceneData);

		/**
		 * Unloads and frees the scene's internal objects.
		 */
		void Unload();		

		/**
		 * Loads the specified view data's resources
		 */
		void LoadViewResources(const ViewData* pViewData);

		/**
		 * Unloads the specified view data's resources
		 */
		void UnLoadViewResources(const ViewData* pViewData);

	private:

		/**
		 * Called when a view has activated
		 */
		void OnViewActivate(void* pSender, void* pContext);

		/**
		 * Called when a view has deactivated
		 */
		void OnViewDeactivate(void* pSender, void* pContext);

		/**
		 * Called when a view's animation has started
		 */
		void OnViewAnimationStarted(void* pSender, uint32 animID);

		/**
		 * Called when a view's animation has ended
		 */
		void OnViewAnimationEnded(void* pSender, uint32 animID);

	private:

		/**
		 * Internal scene data
		 */
		const SceneData*	mSceneData;

		/**
		 * OtterUI System object
		 */
		System*				mSystem;

		/**
		 * Internal graphics object
		 */
		Graphics*			mGraphics;
		
		/**
		 * User implementation of the sound system
		 */
		ISoundSystem*		mSoundSystem;
		
		/**
		 * Internal array of views
		 */
		View**				mViews;

		/**
		 * Internal array of fonts
		 */
		Font**				mFonts;

		/**
		 * Array of currently active views
		 */
		Array<View*>		mActiveViews; 
	};
}
