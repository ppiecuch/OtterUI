#include <string.h>

#include "Sprite.h"
#include "View.h"
#include "Mask.h"

#include "Data/OtterData.h"
#include "Graphics/Graphics.h"
#include "Memory/Memory.h"
#include "Utilities/Utilities.h"

using namespace VectorMath;

namespace Otter
{
	/* Constructor
	 */
	Sprite::Sprite(Scene* pScene, Control* pParent, const SpriteData* pSpriteData) : Control(pScene, pParent, pSpriteData)
	{
		mTextureID = -1;

		tl_uv[0] = tl_uv[1] = 0.0f;
		br_uv[0] = br_uv[1] = 1.0f;

		SetTexture(pSpriteData->mTextureID);
	}

	/* Constructor
	 */
	Sprite::Sprite() : Control(NULL, NULL, OTTER_NEW(SpriteData, ()))
	{
		mTextureID = -1;

		tl_uv[0] = tl_uv[1] = 0.0f;
		br_uv[0] = br_uv[1] = 1.0f;

		SetColor(0xFFFFFFFF);
		SetMask(-1);
		SetSkew(0.0f);

		mUserCreated = true;
	}

	/* Destructor
	 */
	Sprite::~Sprite(void)
	{
		SetTexture(-1);

		if(mUserCreated)
			OTTER_DELETE(GetData());
	}

	/* Sets the sprite's color
	 */
	void Sprite::SetColor(uint32 color)
	{
		SpriteData* pSpriteData = const_cast<SpriteData*>(static_cast<const SpriteData*>(mControlData));
		pSpriteData->mColor = color;
	}

	/* Retrieves the sprite's color
	 */
	uint32 Sprite::GetColor()
	{
		const SpriteData* pSpriteData = static_cast<const SpriteData*>(mControlData);
		return pSpriteData->mColor;
	}

	/**
	* Gets the texture by ID
	*/
	uint32 Sprite::GetTexture()
	{
		return mTextureID;
	}

	/* Sets the texture to be displayed
		*/
	Result Sprite::SetTexture(const char* szTexture)
	{		
		Scene* pScene = GetScene();
		if(pScene == NULL)
			return kResult_Error;

		// Load the new texture
		uint32 texID = pScene->GetTextureID(szTexture);

		return SetTexture(texID);
	}	

	/**
	 * Sets the texture by ID
	 */
	Result Sprite::SetTexture(uint32 texID)
	{
		Scene* pScene = GetScene();
		if(pScene == NULL)
			return kResult_Error;

		// Make sure it's loaded
		pScene->LoadResource(texID);

		// Unload the previous one
		pScene->UnloadResource(mTextureID);

		// Assign the new one
		mTextureID = texID;

		return kResult_OK;
	}

	/**
	 * Clones the control and returns the new instance
	 */
	Control* Sprite::Clone()
	{
		Sprite* pSprite = OTTER_NEW(Sprite, ());
		memcpy((uint8*)pSprite->GetData(), (uint8*)GetData(), sizeof(SpriteData));

		pSprite->SetTexture(GetTexture());

		((ControlData*)pSprite->GetData())->mID = GetParentView()->GenerateNewID();
		mParent->AddControl(pSprite);

		return pSprite;
	}

	/** 
     * Sets the UVs for the sprite.  Useful to display a section
	 * of the sprite texture.  Clamped to [0,0] and [1,1]
	 */
	Result Sprite::SetUVs(float tl_u, float tl_v, 
						  float br_u, float br_v)
	{
		tl_uv[0] = Clamp(tl_u, 0.0f, 1.0f);
		tl_uv[1] = Clamp(tl_v, 0.0f, 1.0f);
						   	
		br_uv[0] = Clamp(br_u, 0.0f, 1.0f);
		br_uv[1] = Clamp(br_v, 0.0f, 1.0f);

		return kResult_OK;
	}

	/**
	 * Sets the sprite's skew
	 */
	void Sprite::SetSkew(float skew)
	{
		SpriteData* pSpriteData = const_cast<SpriteData*>(static_cast<const SpriteData*>(mControlData));
		pSpriteData->mSkew = skew;
	}

	/**
	 * Retrieves the sprite's skew
	 */
	float Sprite::GetSkew()
	{
		const SpriteData* pSpriteData = static_cast<const SpriteData*>(mControlData);
		return pSpriteData->mSkew;
	}
	
	/* Draws the sprite to screen
	 */
	void Sprite::Draw(Graphics* pGraphics)
	{		
		const SpriteData* pSpriteData = static_cast<const SpriteData*>(mControlData);
		Draw(pGraphics, pSpriteData->mTextureID);
	}

	/* Draws the sprite with the specified texture ID
	 */
	void Sprite::Draw(Graphics* pGraphics, uint32 textureID)
	{
		if(!mEnabled)
			return;		

		const Vector2& size = GetSize();
		uint32 color = GetColor();
		float skew = GetSkew();
		
		const SpriteData* pSpriteData = static_cast<const SpriteData*>(mControlData);
		const TextureData* pTextureData = GetScene()->GetTextureData(mTextureID);

		float u1 = pTextureData->mTextureRect.uv[0];
		float v1 = pTextureData->mTextureRect.uv[1];
		float u2 = pTextureData->mTextureRect.uv[2];
		float v2 = pTextureData->mTextureRect.uv[3];

		u1 = u1 + (u2 - u1) * tl_uv[0];
		v1 = v1 + (v2 - v1) * tl_uv[1];

		u2 = u1 + (u2 - u1) * br_uv[0];
		v2 = v1 + (v2 - v1) * br_uv[1];

		if(pSpriteData->mFlipType == 1)
		{
			Swap(u1, u2);
		}
		else if(pSpriteData->mFlipType == 2)
		{
			Swap(v1, v2);
		}

		pGraphics->PushMatrix(GetTransform());
		pGraphics->DrawRectangle(pTextureData->mTextureRect.mTextureID, 
									0.0f, 0.0f, size.x, size.y, 
									u1, v1, u2, v2, 
									color, skew, false, pSpriteData->mMaskID);
		pGraphics->PopMatrix();

		Control::Draw(pGraphics);
	}

	/* Applies the interpolation of two keyframes to the control.
	 * pEndFrame can be NULL.
	 */
	void Sprite::ApplyKeyFrame(const KeyFrameData *pStartFrame, const KeyFrameData *pEndFrame, float factor)
	{
		Control::ApplyKeyFrame(pStartFrame, pEndFrame, factor);

		if(pStartFrame == NULL && pEndFrame == NULL)
			return;

		// Ensure that they're both color key frames
		if(pStartFrame && pStartFrame->mFourCC != FOURCC_KFRM || pEndFrame && pEndFrame->mFourCC != FOURCC_KFRM)
			return;

		const SpriteLayout* pStartLayout	= (SpriteLayout*)pStartFrame->GetLayout();
		const SpriteLayout* pEndLayout		= (SpriteLayout*)(pEndFrame ? pEndFrame->GetLayout() : NULL);

		uint8 s0 = pStartLayout	? (pStartLayout->mColor >> 24)	& 0xFF : 0xFF;
		uint8 s1 = pStartLayout	? (pStartLayout->mColor >> 16)	& 0xFF : 0xFF;
		uint8 s2 = pStartLayout	? (pStartLayout->mColor >> 8)	& 0xFF : 0xFF;
		uint8 s3 = pStartLayout	? (pStartLayout->mColor)		& 0xFF : 0xFF;

		uint8 e0 = pEndLayout	? (pEndLayout->mColor >> 24)	& 0xFF : 0xFF;
		uint8 e1 = pEndLayout	? (pEndLayout->mColor >> 16)	& 0xFF : 0xFF;
		uint8 e2 = pEndLayout	? (pEndLayout->mColor >> 8)		& 0xFF : 0xFF;
		uint8 e3 = pEndLayout	? (pEndLayout->mColor)			& 0xFF : 0xFF;
		
		uint8 f0 = (uint8)(s0 + factor * (e0 - s0));
		uint8 f1 = (uint8)(s1 + factor * (e1 - s1));
		uint8 f2 = (uint8)(s2 + factor * (e2 - s2));
		uint8 f3 = (uint8)(s3 + factor * (e3 - s3));

		float skew0 = pStartLayout ? pStartLayout->mSkew : 0.0f;
		float skew1 = pEndLayout ? pEndLayout->mSkew : 0.0f;
		float skew = skew0 + factor * (skew1 - skew0);

		SetColor(f0 << 24 | f1 << 16 | f2 << 8 | f3);
		SetSkew(skew);
	}
}