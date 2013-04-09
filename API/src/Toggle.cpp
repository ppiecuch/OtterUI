#include <string.h>

#include "Toggle.h"
#include "View.h"

#include "Data/OtterData.h"
#include "Graphics/Graphics.h"
#include "Memory/Memory.h"

using namespace VectorMath;

namespace Otter
{
	/* Constructor
	 */
	Toggle::Toggle(Scene* pScene, Control* pParent, const ToggleData* pToggleData) : Control(pScene, pParent, pToggleData)
	{
		SetState(Toggle::Off);
	}

	/* Constructor
	 */
	Toggle::Toggle() : Control(NULL, NULL, OTTER_NEW(ToggleData, ()))
	{
		SetState(Toggle::Off);
		SetColor(0xFFFFFFFF);
		SetOnTexture(NULL);
		SetOffTexture(NULL);

		mUserCreated = true;
	}

	/* Destructor
	 */
	Toggle::~Toggle(void)
	{
		if(mUserCreated)
			OTTER_DELETE(GetData());
	}

	/* Sets the Toggle's color
	 */
	void Toggle::SetColor(uint32 color)
	{
		ToggleData* pToggleData = const_cast<ToggleData*>(static_cast<const ToggleData*>(mControlData));
		pToggleData->mColor = color;
	}

	/* Retrieves the Toggle's color
	 */
	uint32 Toggle::GetColor()
	{
		const ToggleData* pToggleData = static_cast<const ToggleData*>(mControlData);
		return pToggleData->mColor;
	}

	/* Sets the texture that is displayed when the toggle is "on"
	 */
	Result Toggle::SetOnTexture(const char* szTexture)
	{		
		Scene* pScene = GetScene();
		if(pScene == NULL)
			return kResult_Error;

		ToggleData* pToggleData = const_cast<ToggleData*>(static_cast<const ToggleData*>(GetData()));
		pToggleData->mOnTextureID = pScene->GetTextureID(szTexture);

		return kResult_OK;
	}

	/* Sets the texture that is displayed when the toggle is "off"
	 */
	Result Toggle::SetOffTexture(const char* szTexture)
	{		
		Scene* pScene = GetScene();
		if(pScene == NULL)
			return kResult_Error;

		ToggleData* pToggleData = const_cast<ToggleData*>(static_cast<const ToggleData*>(GetData()));
		pToggleData->mOffTextureID = pScene->GetTextureID(szTexture);

		return kResult_OK;
	}

	/* Sets the toggle's state
	 */
	Result Toggle::SetState(State state)
	{
		if(mToggleState != state)
		{
			mToggleState = state;
			mOnToggleChanged(this, mToggleState);
		}

		return kResult_OK;
	}

	/* Gets the toggle's state
	 */
	Toggle::State Toggle::GetState()
	{
		return mToggleState;
	}

	/* Points (touches/mouse/etc) were pressed down
	 */
	bool Toggle::OnPointsDown(Point* points, sint32 numPoints)
	{
		if(!mTouchEnabled)
			return false;

		if(!mEnabled)
			return false;

		return true;
	}
		
	/* Points (touches/mouse/etc) were released
	 */
	bool Toggle::OnPointsUp(Point* points, sint32 numPoints)
	{
		if(!mTouchEnabled)
			return false;

		if(!mEnabled)
			return false;

		const VectorMath::Vector2& size = GetSize();

		for(sint32 i = 0; i < numPoints; i++)
		{
			if( points[i].x >= 0 && points[i].y >= 0 &&
				points[i].x <= size.x && points[i].y <= size.y)
			{
				mToggleState = mToggleState == Toggle::On ? Toggle::Off : Toggle::On;
				mOnToggleChanged(this, mToggleState);
				break;
			}
		}

		return true;
	}

	/**
	 * Clones the control and returns the new instance
	 */
	Control* Toggle::Clone()
	{
		Toggle* pToggle = OTTER_NEW(Toggle, ());
		memcpy((uint8*)pToggle->GetData(), (uint8*)GetData(), sizeof(ToggleData));
		((ControlData*)pToggle->GetData())->mID = GetParentView()->GenerateNewID();
		mParent->AddControl(pToggle);

		pToggle->mToggleState = this->mToggleState;

		return pToggle;
	}
	
	/* Draws the Toggle to screen
	 */
	void Toggle::Draw(Graphics* pGraphics)
	{		
		const ToggleData* pToggleData = static_cast<const ToggleData*>(mControlData);
		Draw(pGraphics, mToggleState == Toggle::On ? pToggleData->mOnTextureID : pToggleData->mOffTextureID);
	}

	/* Draws the Toggle with the specified texture ID
	 */
	void Toggle::Draw(Graphics* pGraphics, uint32 textureID)
	{
		if(!mEnabled)
			return;		

		const Vector2& size = GetSize();
		uint32 color = GetColor();

		pGraphics->PushMatrix(GetTransform());
		const TextureData* pTextureData = GetScene()->GetTextureData(textureID);
		pGraphics->DrawRectangle(	pTextureData->mTextureRect.mTextureID, 
									0.0f, 0.0f, size.x, size.y,
									pTextureData->mTextureRect.uv[0], pTextureData->mTextureRect.uv[1], pTextureData->mTextureRect.uv[2], pTextureData->mTextureRect.uv[3],
									color, 0.0f);
		pGraphics->PopMatrix();
	}

	/* Applies the interpolation of two keyframes to the control.
	 * pEndFrame can be NULL.
	 */
	void Toggle::ApplyKeyFrame(const KeyFrameData *pStartFrame, const KeyFrameData *pEndFrame, float factor)
	{
		Control::ApplyKeyFrame(pStartFrame, pEndFrame, factor);

		if(pStartFrame == NULL && pEndFrame == NULL)
			return;

		// Ensure that they're both color key frames
		if(pStartFrame && pStartFrame->mFourCC != FOURCC_KFRM || pEndFrame && pEndFrame->mFourCC != FOURCC_KFRM)
			return;

		const ToggleLayout* pStartLayout	= (ToggleLayout*)pStartFrame->GetLayout();
		const ToggleLayout* pEndLayout		= (ToggleLayout*)(pEndFrame ? pEndFrame->GetLayout() : NULL);

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

		SetColor(f0 << 24 | f1 << 16 | f2 << 8 | f3);
	}
}