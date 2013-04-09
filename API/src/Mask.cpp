#include <string.h>

#include "Mask.h"
#include "View.h"

#include "Data/OtterData.h"
#include "Graphics/Graphics.h"
#include "Memory/Memory.h"
#include "Utilities/Utilities.h"

using namespace VectorMath;

namespace Otter
{
	/* Constructor
	 */
	Mask::Mask(Scene* pScene, Control* pParent, const MaskData* pMaskData) : Control(pScene, pParent, pMaskData)
	{
		tl_uv[0] = tl_uv[1] = 0.0f;
		br_uv[0] = br_uv[1] = 1.0f;
	}

	/* Constructor
	 */
	Mask::Mask() : Control(NULL, NULL, OTTER_NEW(MaskData, ()))
	{
		tl_uv[0] = tl_uv[1] = 0.0f;
		br_uv[0] = br_uv[1] = 1.0f;

		mUserCreated = true;
	}

	/* Destructor
	 */
	Mask::~Mask(void)
	{
		if(mUserCreated)
			OTTER_DELETE(GetData());
	}

	/* Sets the texture to be displayed
		*/
	Result Mask::SetTexture(const char* szTexture)
	{		
		Scene* pScene = GetScene();
		if(pScene == NULL)
			return kResult_Error;

		MaskData* pMaskData = const_cast<MaskData*>(static_cast<const MaskData*>(GetData()));

		// Load the new texture
		uint32 texID = pScene->GetTextureID(szTexture);
		pScene->LoadResource(texID);

		// Unload the previous one
		pScene->UnloadResource(pMaskData->mTextureID);
		pMaskData->mTextureID = texID;

		return kResult_OK;
	}

	/**
	 * Clones the control and returns the new instance
	 */
	Control* Mask::Clone()
	{
		Mask* pMask = OTTER_NEW(Mask, ());
		memcpy((uint8*)pMask->GetData(), (uint8*)GetData(), sizeof(MaskData));
		((ControlData*)pMask->GetData())->mID = GetParentView()->GenerateNewID();
		mParent->AddControl(pMask);

		return pMask;
	}

	/** 
     * Sets the UVs for the mask.  Useful to display a section
	 * of the mask texture.  Clamped to [0,0] and [1,1]
	 */
	Result Mask::SetUVs(float tl_u, float tl_v, 
						  float br_u, float br_v)
	{
		tl_uv[0] = Clamp(tl_u, 0.0f, 1.0f);
		tl_uv[1] = Clamp(tl_v, 0.0f, 1.0f);
						   	
		br_uv[0] = Clamp(br_u, 0.0f, 1.0f);
		br_uv[1] = Clamp(br_v, 0.0f, 1.0f);

		return kResult_OK;
	}

	/**
	 * Sets the mask's skew
	 */
	void Mask::SetSkew(float skew)
	{
		MaskData* pMaskData = const_cast<MaskData*>(static_cast<const MaskData*>(mControlData));
		pMaskData->mSkew = skew;
	}

	/**
	 * Retrieves the mask's skew
	 */
	float Mask::GetSkew()
	{
		const MaskData* pMaskData = static_cast<const MaskData*>(mControlData);
		return pMaskData->mSkew;
	}

	/* Draws the mask to the stencil buffer
	 */
	void Mask::DrawMask(Graphics* pGraphics)
	{
		const Vector2& size = GetSize();

		const MaskData* pMaskData = static_cast<const MaskData*>(mControlData);
		const TextureData* pTextureData = GetScene()->GetTextureData(pMaskData->mTextureID);

		float u1 = pTextureData->mTextureRect.uv[0];
		float v1 = pTextureData->mTextureRect.uv[1];
		float u2 = pTextureData->mTextureRect.uv[2];
		float v2 = pTextureData->mTextureRect.uv[3];

		u1 = u1 + (u2 - u1) * tl_uv[0];
		v1 = v1 + (v2 - v1) * tl_uv[1];

		u2 = u1 + (u2 - u1) * br_uv[0];
		v2 = v1 + (v2 - v1) * br_uv[1];

		if(pMaskData->mFlipType == 1)
		{
			Swap(u1, u2);
		}
		else if(pMaskData->mFlipType == 2)
		{
			Swap(v1, v2);
		}

		pGraphics->SetStencilMatrix(GetTransform());
		pGraphics->DrawRectangle(pTextureData->mTextureRect.mTextureID, 
			0.0f, 0.0f, size.x, size.y, 
			u1, v1, u2, v2, 
			0xFFFFFFFF, pMaskData->mSkew, true, GetID());
	}

	/* Applies the interpolation of two keyframes to the control.
	 * pEndFrame can be NULL.
	 */
	void Mask::ApplyKeyFrame(const KeyFrameData *pStartFrame, const KeyFrameData *pEndFrame, float factor)
	{
		Control::ApplyKeyFrame(pStartFrame, pEndFrame, factor);

		if(pStartFrame == NULL && pEndFrame == NULL)
			return;

		// Ensure that they're both color key frames
		if(pStartFrame && pStartFrame->mFourCC != FOURCC_KFRM || pEndFrame && pEndFrame->mFourCC != FOURCC_KFRM)
			return;

		const MaskLayout* pStartLayout	= (MaskLayout*)pStartFrame->GetLayout();
		const MaskLayout* pEndLayout		= (MaskLayout*)(pEndFrame ? pEndFrame->GetLayout() : NULL);

		float skew0 = pStartLayout ? pStartLayout->mSkew : 0.0f;
		float skew1 = pEndLayout ? pEndLayout->mSkew : 0.0f;
		float skew = skew0 + factor * (skew1 - skew0);

		SetSkew(skew);
	}
}