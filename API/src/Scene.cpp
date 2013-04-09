#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <assert.h>

#include "Common/Platforms.h"
#include "Data/OtterData.h"
#include "Graphics/Graphics.h"

#include "Scene.h"
#include "View.h"
#include "Font.h"
#include "Memory/Memory.h"

namespace Otter
{
	/* Default Constructor
	 */
	Scene::Scene(System* pSystem, Graphics* pGraphics, ISoundSystem* pSoundSystem, const SceneData* pSceneData)
	{
		mFonts = NULL;
		mViews = NULL;
		mSceneData = pSceneData;
		mSystem = pSystem;
		mGraphics = pGraphics;
		mSoundSystem = pSoundSystem;

		LoadFromData(mSceneData);
	}

	/* Destructor
	 */
	Scene::~Scene(void)
	{
		Unload();
	}

	/* Retrieves the total number of views within this scene
	 */
	uint32 Scene::GetNumViews()
	{
		if(!mSceneData)
			return 0;

		return mSceneData->mNumViews;
	}

	/* Retrieves the scene id
	 */
	uint32 Scene::GetID()
	{
		if(!mSceneData)
			return 0;

		return mSceneData->mID;
	}

	/* Activates a view by name.  Prompts the renderer
	 * to load as many textures as it needs to.
	 *
	 * The life-cycle of a view goes something like this:
	 *
	 * PlayAnim("OnActivate")---->OnActivate()---> (stuff) --> PlayAnim("OnDeactivate")--->OnDeactivate()
	 * 
	 * We have to set up the listeners here, instead of "OnActivate", in order to get
	 * events from the view
	 */
	Result Scene::ActivateView(const char* szName)
	{
		View* pView = GetView(szName);
		if(!pView)
			return kResult_ViewNotFound;

		return ActivateView(pView);
	}

	/* Activates a view by index
	 */
	Result Scene::ActivateView(uint32 index)
	{
		if(index >= mSceneData->mNumViews)
			return kResult_ViewNotFound;
 
		return ActivateView(mViews[index]);
	}

	/* Activates a view by pointer
	 */
	Result Scene::ActivateView(View* pView)
	{
		for(uint32 i = 0; i < mActiveViews.size(); i++)
		{
			if(mActiveViews[i] == pView)
			{
				if (pView->GetActiveAnimationName(Otter::ANIM_ONDEACTIVATE) != NULL)
				{
					pView->StopAnimation(Otter::ANIM_ONDEACTIVATE);
					break;
				}
				else
				{                            
					return kResult_ViewAlreadyActive;
				}
			}
		}

		mActiveViews.push_back(pView);

		pView->mOnActivate.AddHandler(this, &Scene::OnViewActivate);
		pView->mOnDeactivate.AddHandler(this, &Scene::OnViewDeactivate);
		pView->mOnAnimationStarted.AddHandler(this, &Scene::OnViewAnimationStarted);
		pView->mOnAnimationEnded.AddHandler(this, &Scene::OnViewAnimationEnded);

		LoadViewResources((ViewData*)pView->GetData());	
		pView->OnActivate();	
		pView->PlayAnimation("OnActivate");

		return kResult_OK;
	}

	/* Deactivates a view by name
	 */
	Result Scene::DeactivateView(const char* szName)
	{
		View* pView = GetActiveView(szName);
		if(!pView)
			return kResult_ViewNotFound;

		return DeactivateView(pView);
	}

	/* Deactivates a view by index
	 */
	Result Scene::DeactivateView(uint32 index)
	{
		if(index >= mActiveViews.size())
			return kResult_ViewNotFound;

		return DeactivateView(mActiveViews[index]);
	}

	/* Deactivates a view by pointer
	 */
	Result Scene::DeactivateView(View* pView)
	{
		pView->PlayAnimation("OnDeactivate");
		pView->Update(0.0f); // Force a zero update to initialize positions.

		return kResult_OK;
	}

	/* Called when a view has activated
	 */
	void Scene::OnViewActivate(void* pSender, void* pContext)
	{		
	}

	/* Called when a view has deactivated.
	 * Remove the listeners here to ensure it's the very last thing the view does.
	 */
	void Scene::OnViewDeactivate(void* pSender, void* pContext)
	{
		View* pView = (View*)pSender;

		pView->mOnActivate.RemoveHandler(this, &Scene::OnViewActivate);
		pView->mOnDeactivate.RemoveHandler(this, &Scene::OnViewDeactivate);
		pView->mOnAnimationStarted.RemoveHandler(this, &Scene::OnViewAnimationStarted);
		pView->mOnAnimationEnded.RemoveHandler(this, &Scene::OnViewAnimationEnded);

		UnLoadViewResources((ViewData*)pView->GetData());
	}

	/* Called when a view's animation has started
	 */
	void Scene::OnViewAnimationStarted(void* pSender, uint32 animID)
	{
	}

	/* Called when a view's animation has ended
	 */
	void Scene::OnViewAnimationEnded(void* pSender, uint32 animID)
	{
		View* pView = (View*)pSender;

		if(animID == ANIM_ONDEACTIVATE)
		{
			for(uint32 i = 0; i < mActiveViews.size(); i++)
			{
				if(mActiveViews[i] == pView)
				{
					pView->OnDeactivate();
					mActiveViews.erase(i);
					return;
				}
			}
		}
	}

	/* Retrieves the total number of views within this scene
		*/
	uint32 Scene::GetNumActiveViews()
	{
		return mActiveViews.size();
	}

	/* Retrieves an active view by name
	 */
	View* Scene::GetActiveView(const char* szName)
	{
		if(mSceneData == NULL || mSceneData->mNumViews == 0)
			return NULL;

		for(uint32 i = 0; i < mActiveViews.size(); i++)
		{
			if(strcmp(mActiveViews[i]->GetData()->mName, szName) == 0)
				return mActiveViews[i];
		}

		return NULL;
	}	

	/* Retrieves an active view by index
		*/
	View* Scene::GetActiveView(uint32 index)
	{
		if(index >= mActiveViews.size())
			return NULL;

		return mActiveViews[index];
	}

	/* Loads all the resource (textures, sounds, etc) for a particular view
	 */
	void Scene::LoadViewResources(const char* szName)
	{
		View* pView = GetView(szName);
		if(!pView)
			return;

		LoadViewResources((ViewData*)pView->GetData());
	}

	/* Loads the specified view data's resources
	 */
	void Scene::LoadViewResources(const ViewData* pViewData)
	{
		if(!pViewData)
			return;
		
		// Load the textures
		const sint32* textureIDs = pViewData->GetTextureIDs();
		for(uint32 i = 0; i < pViewData->mNumTextures; i++)
		{
			LoadResource(textureIDs[i]);
		}
		
		// Load the sounds
		const sint32* soundIDs = pViewData->GetSoundIDs();
		for(uint32 i = 0; i < pViewData->mNumSounds; i++)
		{
			LoadResource(soundIDs[i]);
		}
	}

	/* Loads all the textures for a particular view
	 */
	void Scene::UnLoadViewResources(const char* szName)
	{
		View* pView = GetView(szName);
		if(!pView)
			return;

		UnLoadViewResources((ViewData*)pView->GetData());
	}

	/* Unloads the specified view data's textures
	 */
	void Scene::UnLoadViewResources(const ViewData* pViewData)
	{
		if(!pViewData)
			return;
		
		// Unload used textures
		const sint32* textureIDs = pViewData->GetTextureIDs();
		for(uint32 i = 0; i < pViewData->mNumTextures; i++)
		{
			UnloadResource(textureIDs[i]);
		}
		
		// Unload used sounds
		const sint32* soundIDs = pViewData->GetSoundIDs();
		for(uint32 i = 0; i < pViewData->mNumSounds; i++)
		{
			UnloadResource(soundIDs[i]);
		}
	}

	/**
	 * Loads a resource
	 */
	bool Scene::LoadResource(uint32 resource)
	{
		const TextureData* pTextureData = mSceneData->GetTexture(resource);
		if(pTextureData)
		{
			const TextureData* pActualTexture = mSceneData->GetTexture(pTextureData->mTextureRect.mTextureID);
			if(pActualTexture != NULL && pActualTexture->mRefCount == 0)
			{				
				mGraphics->LoadTexture(pActualTexture->mTextureID, pActualTexture->mTextureName);
			}

			const_cast<TextureData*>(pActualTexture)->mRefCount++;

			return true;
		}

		const SoundData* pSoundData = mSceneData->GetSound(resource);
		if(pSoundData)
		{
			if(pSoundData->mRefCount == 0 && mSoundSystem)
				mSoundSystem->OnLoadSound(pSoundData->mSoundID, pSoundData->mSoundName);
			
			const_cast<SoundData*>(pSoundData)->mRefCount++;

			return true;
		}

		return false;
	}

	/** 
	 * Unloads the specified resource
	 */
	bool Scene::UnloadResource(uint32 resource)
	{
		const TextureData* pTextureData = mSceneData->GetTexture(resource);
		if(pTextureData)
		{
			const TextureData* pActualTexture = mSceneData->GetTexture(pTextureData->mTextureRect.mTextureID);

			if(pActualTexture && pActualTexture->mRefCount > 0)
			{
				const_cast<TextureData*>(pActualTexture)->mRefCount--;
				
				if(pActualTexture->mRefCount <= 0)
					mGraphics->UnloadTexture(pActualTexture->mTextureID);
			}

			return true;
		}

		const SoundData* pSoundData = mSceneData->GetSound(resource);
		if(pSoundData && pSoundData->mRefCount > 0)
		{				
			const_cast<SoundData*>(pSoundData)->mRefCount--;

			if(pSoundData->mRefCount <= 0 && mSoundSystem)
				mSoundSystem->OnUnloadSound(pSoundData->mSoundID);

			return true;
		}

		return false;
	}

	/* Retrieves a font by ID
	 */
	Font* Scene::GetFont(uint32 fontID)
	{
		if(mSceneData == NULL || mSceneData->mNumFonts == 0 || mFonts == NULL)
			return NULL;

		for(uint32 i = 0; i < mSceneData->mNumFonts; i++)
		{
			if(mFonts[i]->GetData().mID == fontID)
				return mFonts[i];
		}

		return NULL;
	}
		  
	/* Retrieves a font by name
		*/
	Font* Scene::GetFont(const char* szName)
	{
		if(mSceneData == NULL || mSceneData->mNumFonts == 0 || mFonts == NULL)
			return NULL;

		UTF8String fontName = szName;
		for(uint32 i = 0; i < mSceneData->mNumFonts; i++)
		{
			UTF8String tmpName = mFonts[i]->GetData().mName;
			if(fontName == tmpName)
				return mFonts[i];
		}

		return NULL;
	}

	/* Retrieves a texture id by name
	 */
	uint32 Scene::GetTextureID(const char* szTextureName)
	{
		UTF8String nameA = szTextureName;
		UTF8String nameB = "";

		for(uint32 i = 0; i < mSceneData->mNumTextures; i++)
		{
			const TextureData* pTextureData = mSceneData->GetTextureByIndex(i);
			nameB = pTextureData->mTextureName;

			if(nameA == nameB)
				return pTextureData->mTextureID;
		}

		return 0xFFFFFFFF;
	}

	/* Retrieves the texture data by texture id
	 */
	const TextureData* Scene::GetTextureData(uint32 textureID)
	{
		for(uint32 i = 0; i < mSceneData->mNumTextures; i++)
		{
			const TextureData* pTextureData = mSceneData->GetTextureByIndex(i);

			if(pTextureData->mTextureID == textureID)
				return pTextureData;
		}

		static TextureData emptyData;
		return &emptyData;
	}

	/* Loads the scene from the provided scene data.
	 * Unloads all existing data first.
	 */
	bool Scene::LoadFromData(const SceneData* pSceneData)
	{
		Unload();

		if(pSceneData->mNumViews <= 0)
			return false;

		mSceneData = pSceneData;

		uint32 maxTexID = 0;
		for(uint32 i = 0; i < mSceneData->mNumTextures; i++)
		{
			const TextureData* pTextureData = mSceneData->GetTextureByIndex(i);
			if(pTextureData->mTextureID > maxTexID)
				maxTexID = pTextureData->mTextureID;
		}

		// Create our array of fonts if any
		if(mSceneData->mNumFonts > 0)
		{
			mFonts = (Font**)OTTER_ALLOC(sizeof(Font*) * mSceneData->mNumFonts);
			memset(mFonts, 0x00, sizeof(mFonts));

			for(uint32 i = 0; i < mSceneData->mNumFonts; i++)
			{
				const FontData* pFontData = mSceneData->GetFontData(i);
				mFonts[i] = OTTER_NEW(Font, (pFontData));

				uint32 textures[256];
				for(uint32 j = 0; j < pFontData->mNumTextures; j++)
				{
					// Now load up the texture for the font
					// We look for the font texture under the /Fonts directory
					char fontTexture[128];
					sprintf_s(fontTexture, 128, "Fonts\\%s_%d.png", pFontData->mName, j);

					maxTexID++;
					mGraphics->LoadTexture(maxTexID, fontTexture);
					textures[j] = maxTexID;
				}

				mFonts[i]->SetTextures(textures, pFontData->mNumTextures);
			}
		}

		// Create the array of views 
		mViews = (View**)OTTER_ALLOC(sizeof(View*) * mSceneData->mNumViews);
		memset(mViews, 0x00, sizeof(mViews));

		// And now load them individually
		for(uint32 i = 0; i < mSceneData->mNumViews; i++)
		{
			const ViewData* pViewData = mSceneData->GetViewData(i);
			if(pViewData->mFourCC == FOURCC_GGVW)
			{
				mViews[i] = OTTER_NEW(View, (this, pViewData));
			}
			else
			{
				assert(false);
			}
		}

		return true;
	}

	/* Unloads the scene's internal data
	 */
	void Scene::Unload()
	{
		if(mViews != NULL)
		{
			for(uint32 i = 0; i < mSceneData->mNumViews; i++)
			{
				OTTER_DELETE(mViews[i]);
			}

			OTTER_FREE(mViews);
		}

		mViews = NULL;

		if(mFonts != NULL)
		{
			for(uint32 i = 0; i < mSceneData->mNumFonts; i++)
			{
				const Array<uint32>& textures = mFonts[i]->GetTextures();
				for(uint32 j = 0; j < textures.size(); j++)
					mGraphics->UnloadTexture(textures[j]);

				OTTER_DELETE(mFonts[i]);
			}

			OTTER_FREE(mFonts);
		}

		mFonts = NULL;
	}

	/* Retrieves a view by name
	 */
	View* Scene::GetView(const char* szName)
	{
		if(mSceneData == NULL || mSceneData->mNumViews == 0)
			return NULL;

		for(uint32 i = 0; i < mSceneData->mNumViews; i++)
		{
			if(strcmp(mViews[i]->GetData()->mName, szName) == 0)
				return mViews[i];
		}

		return NULL;
	}

	/* Retrieves a view by index
	 */
	View* Scene::GetView(uint32 index)
	{
		if(mSceneData == NULL || (index >= mSceneData->mNumViews))
			return NULL;

		return mViews[index];
	}

	/* Sets the scene resolution
	 */
	void Scene::SetResolution(uint32 width, uint32 height)
	{
		int viewCount = GetNumViews();
		for(int i = 0; i < viewCount; i++)
			GetView(i)->SetSize(VectorMath::Vector2((float)width, (float)height));
	}
	
	/* Points (touches/mouse/etc) were pressed down
	 */
	void Scene::OnPointsDown(Point* points, sint32 numPoints)
	{
		Array<View*> temp(mActiveViews);
		for(uint32 i = 0; i < temp.size(); i++)
		{
			if(temp[i]->OnPointsDown(points, numPoints))
				return;
		}
	}
	
	/* Points (touches/mouse/etc) were released
	 */
	void Scene::OnPointsUp(Point* points, sint32 numPoints)
	{
		Array<View*> temp(mActiveViews);
		for(uint32 i = 0; i < temp.size(); i++)
		{
			if(temp[i]->OnPointsUp(points, numPoints))
				return;
		}
	}
	
	/* Points (touches/mouse/etc) were moved.
	 */
	void Scene::OnPointsMove(Point* points, sint32 numPoints)
	{
		Array<View*> temp(mActiveViews);
		for(uint32 i = 0; i < temp.size(); i++)
		{
			if(temp[i]->OnPointsMove(points, numPoints))
				return;
		}
	}
	
	/* Draws the scene.  Only the active views are
	 * drawn.
	 */
	void Scene::Draw()
	{
		for(uint32 i = 0; i < mActiveViews.size(); i++)
		{
			mActiveViews[i]->Draw(mGraphics);
		}
	}

	/* Updates the scene
	 * "frameDelta" is the number of frames that have passed since last update
	 */
	void Scene::Update(float frameDelta)
	{
		Array<View*> temp(mActiveViews);
		for(uint32 i = 0; i < temp.size(); i++)
		{
			temp[i]->Update(frameDelta);
		}
	}
};
