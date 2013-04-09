#include <string.h>

#include "Slider.h"
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
	Slider::Slider(Scene* pScene, Control* pParent, const SliderData* pSliderData) : Control(pScene, pParent, pSliderData)
	{
		SetValue(0);

		mDraggingThumb = false;
	}

	/* Constructor
	 */
	Slider::Slider() : Control(NULL, NULL, OTTER_NEW(SliderData, ()))
	{
		SetColor(0xFFFFFFFF);

		SetStartTexture(NULL);
		SetEndTexture(NULL);
		SetMiddleTexture(NULL);
		SetThumbTexture(NULL);
		SetRange(0,0);
		SetValue(0);

		mDraggingThumb = false;

		mUserCreated = true;
	}

	/* Destructor
	 */
	Slider::~Slider(void)
	{
		if(mUserCreated)
			OTTER_DELETE(GetData());
	}

	/* Sets the Slider's color
	 */
	void Slider::SetColor(uint32 color)
	{
		SliderData* pSliderData = const_cast<SliderData*>(static_cast<const SliderData*>(mControlData));
		pSliderData->mColor = color;
	}

	/* Retrieves the Slider's color
	 */
	uint32 Slider::GetColor()
	{
		const SliderData* pSliderData = static_cast<const SliderData*>(mControlData);
		return pSliderData->mColor;
	}

	/* Sets start cap texture
	 */
	Result Slider::SetStartTexture(const char* szTexture)
	{
		Scene* pScene = GetScene();
		if(pScene == NULL)
			return kResult_Error;

		SliderData* pSliderData = const_cast<SliderData*>(static_cast<const SliderData*>(GetData()));
		pSliderData->mStartTextureID = pScene->GetTextureID(szTexture);

		return kResult_OK;
	}

	/* Sets middle texture, that stretches between the start and end caps
	 */
	Result Slider::SetMiddleTexture(const char* szTexture)
	{
		Scene* pScene = GetScene();
		if(pScene == NULL)
			return kResult_Error;

		SliderData* pSliderData = const_cast<SliderData*>(static_cast<const SliderData*>(GetData()));
		pSliderData->mMiddleTextureID = pScene->GetTextureID(szTexture);

		return kResult_OK;
	}

	/* Sets end cap texture
	 */
	Result Slider::SetEndTexture(const char* szTexture)
	{
		Scene* pScene = GetScene();
		if(pScene == NULL)
			return kResult_Error;

		SliderData* pSliderData = const_cast<SliderData*>(static_cast<const SliderData*>(GetData()));
		pSliderData->mEndTextureID = pScene->GetTextureID(szTexture);

		return kResult_OK;
	}

	/* Sets thumb cap texture
	 */
	Result Slider::SetThumbTexture(const char* szTexture)
	{
		Scene* pScene = GetScene();
		if(pScene == NULL)
			return kResult_Error;

		SliderData* pSliderData = const_cast<SliderData*>(static_cast<const SliderData*>(GetData()));
		pSliderData->mThumbTextureID = pScene->GetTextureID(szTexture);

		return kResult_OK;
	}

	/* Sets the thumb size.  Changing the width and height of the 
	 * of the thumb also affects the start/end caps as well as the overall height 
	 * of the slider.
	 */
	Result Slider::SetThumbSize(uint32 width, uint32 height)
	{
		Scene* pScene = GetScene();
		if(pScene == NULL)
			return kResult_Error;

		SliderData* pSliderData = const_cast<SliderData*>(static_cast<const SliderData*>(GetData()));
		pSliderData->mThumbWidth = width;
		pSliderData->mThumbHeight = height;

		return kResult_OK;
	}

	/* Sets the slider's min/max range.  Clamps the current value if needed
	 */
	Result Slider::SetRange(sint32 min, sint32 max)
	{
		SliderData* pSliderData = const_cast<SliderData*>(static_cast<const SliderData*>(GetData()));
		if(!pSliderData)
			return kResult_Error;
		
		pSliderData->mMin = min;
		pSliderData->mMax = (max < min) ? min : max;

		// Set the value - will update the clamp the existing value if necessary
		SetValue(pSliderData->mValue);

		return kResult_OK;
	}

	/* Retrieves the slider's current range
	 */
	Result Slider::GetRange(sint32& min, sint32& max)
	{
		SliderData* pSliderData = const_cast<SliderData*>(static_cast<const SliderData*>(GetData()));
		if(!pSliderData)
			return kResult_Error;

		min = pSliderData->mMin;
		max = pSliderData->mMax;

		return kResult_OK;
	}

	/* Sets the slider's value.  Will be clamped in the current range
	 */
	Result Slider::SetValue(sint32 value)
	{
		SliderData* pSliderData = const_cast<SliderData*>(static_cast<const SliderData*>(GetData()));
		if(!pSliderData)
			return kResult_Error;
		
		sint32 oldValue = pSliderData->mValue;
		pSliderData->mValue = Clamp(value, pSliderData->mMin, pSliderData->mMax);

		if(pSliderData->mValue != oldValue)
			mOnValueChanged(this, pSliderData->mValue);

		return kResult_OK;
	}

	/* Retrieves the slider's current value
	 */
	sint32 Slider::GetValue()
	{
		SliderData* pSliderData = const_cast<SliderData*>(static_cast<const SliderData*>(GetData()));
		if(!pSliderData)
			return 0;

		return pSliderData->mValue;
	}

	/* Sets the step value
	 */
	Result Slider::SetStep(uint32 step)
	{
		SliderData* pSliderData = const_cast<SliderData*>(static_cast<const SliderData*>(GetData()));
		if(!pSliderData)
			return kResult_Error;

		pSliderData->mStep = (sint32)step;

		return kResult_OK;
	}

	/* Retrieves the step value
	 */
	uint32 Slider::GetStep()
	{
		SliderData* pSliderData = const_cast<SliderData*>(static_cast<const SliderData*>(GetData()));
		if(!pSliderData)
			return 0;

		return (uint32)pSliderData->mStep;
	}

	/* Points (touches/mouse/etc) were pressed down
	 */
	bool Slider::OnPointsDown(Point* points, sint32 numPoints)
	{
		if(!mTouchEnabled)
			return false;

		if(!mEnabled)
			return false;

		if(numPoints <= 0)
			return false;
		
		mLastPoint = points[0];
		const Vector2& size = GetSize();
		SliderData* pSliderData = const_cast<SliderData*>(static_cast<const SliderData*>(GetData()));
		
		float range = (float)(pSliderData->mMax - pSliderData->mMin);
		float perc = (range == 0.0f) ? 0.0f : pSliderData->mValue / range;

		float tx = (size.x - pSliderData->mThumbWidth) * perc;
		float ty = size.y / 2.0f - pSliderData->mThumbHeight / 2.0f;
		float tw = (float)pSliderData->mThumbWidth;
		float th = (float)pSliderData->mThumbHeight;

		if(mLastPoint.x >= tx && mLastPoint.y >= ty &&
		   mLastPoint.x <= (tx + tw) && mLastPoint.y <= (ty + th))
		{
			mDraggingThumb = true;
			return true;
		}

		return true;
	}
		
	/* Points (touches/mouse/etc) were released
	 */
	bool Slider::OnPointsUp(Point* points, sint32 numPoints)
	{
		mDraggingThumb = false;

		if(!mTouchEnabled)
			return false;

		if(!mEnabled)
			return false;

		return true;
	}
		
	/* Points (touches/mouse/etc) were moved.
	 */
	bool Slider::OnPointsMove(Point* points, sint32 numPoints)
	{
		if(!mTouchEnabled)
			return false;

		if(!mEnabled)
			return false;

		if(numPoints <= 0)
			return false;

		if(mDraggingThumb)
		{
			SliderData* pSliderData = const_cast<SliderData*>(static_cast<const SliderData*>(GetData()));
			sint32 range = (pSliderData->mMax - pSliderData->mMin); 
			const Vector2& size = GetSize();
			float w = size.x - pSliderData->mThumbWidth;

			if(range > 0 && w > 0)
			{
				float ratio = range / w;

				sint32 oldValue = pSliderData->mValue;
				sint32 step = (pSliderData->mStep > 0) ? pSliderData->mStep : 1;

				sint32 newValue = (sint32)((((points[0].x - pSliderData->mThumbWidth / 2.0f) * ratio) / (float)step) + 0.5f) * step;

				SetValue((sint32)(newValue));

				if(oldValue == pSliderData->mValue)
					return true;
			}
		}
		
		mLastPoint = points[0];
		return true;
	}

	/**
	 * Clones the control and returns the new instance
	 */
	Control* Slider::Clone()
	{
		Slider* pSlider = OTTER_NEW(Slider, ());
		memcpy((uint8*)pSlider->GetData(), (uint8*)GetData(), sizeof(SliderData));
		((ControlData*)pSlider->GetData())->mID = GetParentView()->GenerateNewID();
		mParent->AddControl(pSlider);

		return pSlider;
	}
	
	/* Draws the Slider to screen
	 */
	void Slider::Draw(Graphics* pGraphics)
	{		
		const SliderData* pSliderData = static_cast<const SliderData*>(mControlData);

		const Vector2& size = GetSize();
		uint32 color = GetColor();

		pGraphics->PushMatrix(GetTransform());
		{			
            float x = 0.0f;
            float y = size.y / 2.0f - pSliderData->mThumbHeight / 2.0f;
            float midWidth = Max(0.0f, size.x - pSliderData->mThumbWidth * 2.0f);
			
			const TextureData* pTextureData = NULL;

			pTextureData = GetScene()->GetTextureData(pSliderData->mStartTextureID);
			pGraphics->DrawRectangle(pTextureData->mTextureRect.mTextureID, 
									 x, y, (float)pSliderData->mThumbWidth, (float)pSliderData->mThumbHeight, 				
									 pTextureData->mTextureRect.uv[0], pTextureData->mTextureRect.uv[1], pTextureData->mTextureRect.uv[2], pTextureData->mTextureRect.uv[3],
									 color, 0.0f);

            x += pSliderData->mThumbWidth;
			pTextureData = GetScene()->GetTextureData(pSliderData->mMiddleTextureID);
            pGraphics->DrawRectangle(pTextureData->mTextureRect.mTextureID, 
									 x, y, midWidth, (float)pSliderData->mThumbHeight, 
									 pTextureData->mTextureRect.uv[0], pTextureData->mTextureRect.uv[1], pTextureData->mTextureRect.uv[2], pTextureData->mTextureRect.uv[3],
								 	 color, 0.0f);

            x += midWidth;
			pTextureData = GetScene()->GetTextureData(pSliderData->mEndTextureID);
            pGraphics->DrawRectangle(pTextureData->mTextureRect.mTextureID, 
									 x, y, (float)pSliderData->mThumbWidth, (float)pSliderData->mThumbHeight, 
									 pTextureData->mTextureRect.uv[0], pTextureData->mTextureRect.uv[1], pTextureData->mTextureRect.uv[2], pTextureData->mTextureRect.uv[3],
									 color, 0.0f);
			
			float range = (float)(pSliderData->mMax - pSliderData->mMin);
			float perc = (range == 0.0f) ? 0.0f : pSliderData->mValue / range;

			float tx = (size.x - pSliderData->mThumbWidth) * perc;
			float ty = size.y / 2.0f - pSliderData->mThumbHeight / 2.0f;
			
			pTextureData = GetScene()->GetTextureData(pSliderData->mThumbTextureID);
			pGraphics->DrawRectangle(pTextureData->mTextureRect.mTextureID, 
									 tx, ty, (float)pSliderData->mThumbWidth,(float) pSliderData->mThumbHeight, 
									 pTextureData->mTextureRect.uv[0], pTextureData->mTextureRect.uv[1], pTextureData->mTextureRect.uv[2], pTextureData->mTextureRect.uv[3],
									 color, 0.0f);
		}
		pGraphics->PopMatrix();
	}

	/* Applies the interpolation of two keyframes to the control.
	 * pEndFrame can be NULL.
	 */
	void Slider::ApplyKeyFrame(const KeyFrameData *pStartFrame, const KeyFrameData *pEndFrame, float factor)
	{
		Control::ApplyKeyFrame(pStartFrame, pEndFrame, factor);

		if(pStartFrame == NULL && pEndFrame == NULL)
			return;

		// Ensure that they're both color key frames
		if(pStartFrame && pStartFrame->mFourCC != FOURCC_KFRM || pEndFrame && pEndFrame->mFourCC != FOURCC_KFRM)
			return;

		const SliderLayout* pStartLayout	= (SliderLayout*)pStartFrame->GetLayout();
		const SliderLayout* pEndLayout		= (SliderLayout*)(pEndFrame ? pEndFrame->GetLayout() : NULL);

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