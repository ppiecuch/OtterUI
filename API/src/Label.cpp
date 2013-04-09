#include <string.h>

#include "Label.h"
#include "View.h"
#include "Scene.h"
#include "Font.h"

#include "Data/OtterData.h"
#include "Graphics/Graphics.h"
#include "Memory/Memory.h"

namespace Otter
{
	/* Constructor
	 */
	Label::Label(Scene* pScene, Control* pParent, const LabelData* pLabelData) : Control(pScene, pParent, pLabelData)
	{
		mFontString = OTTER_NEW(FontString, ());
		EnableTouches(false);

		SetText(pLabelData->GetText());
	}

	/* Constructor
	 */
	Label::Label() : Control(NULL, NULL, OTTER_NEW(LabelData, ()))
	{
		mFontString = OTTER_NEW(FontString, ());
		SetColor(0xFFFFFFFF);
		SetScale(1.0f, 1.0f);
		SetSkew(0.0f);
		SetDropShadow(0);
		SetMask(-1);
		EnableTouches(false);

		mUserCreated = true;
	}

	/* Virtual Destructor
	 */
	Label::~Label(void)
	{
		OTTER_DELETE(mFontString);
		mFontString = NULL;

		if(mUserCreated)
		{
			OTTER_DELETE(GetData());
		}
	}

	/* Sets the control's size.
		*/
	void Label::SetSize(const VectorMath::Vector2& size)
	{
		const VectorMath::Vector2& curSize = GetSize();
		if(size.x == curSize.x && size.y == curSize.y)
			return;

		Control::SetSize(size);
		UpdateFontString();
	}

	/* Sets the font by name
		*/
	Result Label::SetFont(const char* szFontName)
	{
		if(!mScene)
			return kResult_Error;

		if(!szFontName)
			Otter::kResult_InvalidParameter;

		LabelData* pLabelData = (LabelData*)mControlData;
		if(!pLabelData)
			return kResult_Error;
		
		Otter::Font* pFont = mScene->GetFont(szFontName);

		if(pFont)
			return SetFont(pFont->GetData().mID);

		return kResult_Error;
	}

	/* Sets the font by id
		*/
	Result Label::SetFont(uint32 fontID)
	{
		if(!mScene)
			return kResult_Error;

		LabelData* pLabelData = (LabelData*)mControlData;
		if(!pLabelData)
			return kResult_Error;

		if(pLabelData->mFontID == fontID)
			return kResult_OK;

		pLabelData->mFontID = fontID;

		UpdateFontString();
		return kResult_OK;
	}

	/**
	* Gets the font id
	*/
	uint32 Label::GetFont()
	{
		LabelData* pLabelData = (LabelData*)mControlData;

		return pLabelData->mFontID;
	}

	/* Sets the label's text
		*/
	Result Label::SetText(const UTF8String& text)
	{
		mText = text;
		UpdateFontString();		
		return kResult_OK;
	}

	/* Sets the label's text
		*/
	Result Label::SetText(const char* szText)
	{
 		mText = szText;	
		UpdateFontString();		
		return kResult_OK;
	}

	/**
	 * Retrieves the label's text
	 */
	const UTF8String& Label::GetText()
	{
		return mText;
	}
		
	/* Sets the horizontal and vertical text alignment
	 */
	Result Label::SetTextAlignment(HoriAlignment halign, VertAlignment valign)
	{		
		LabelData* pLabelData = (LabelData*)mControlData;
		pLabelData->mHAlign = halign;
		pLabelData->mVAlign = valign;

		return kResult_OK;
	}

	/**
	* Gets the horizontal text alignment
	*/
	HoriAlignment Label::GetHorizontalTextAlignment() 
	{ 
		LabelData* pLabelData = (LabelData*)mControlData;

		return (HoriAlignment)pLabelData->mHAlign; 
	}

	/**
	* Gets the vertical text alignment
	*/
	VertAlignment Label::GetVerticalTextAlignment() 
	{ 
		LabelData* pLabelData = (LabelData*)mControlData;

		return (VertAlignment)pLabelData->mVAlign; 
	}

	/* Updates the internal font string with the label's current layout
		*/
	void Label::UpdateFontString(bool bUpdateText)
	{
		if(!mScene)
			return;

		const LabelData* pLabelData = (const LabelData*)mControlData;
		if(!pLabelData)
			return;

		Font* pFont = mScene->GetFont(pLabelData->mFontID);
		if(!pFont)
			return;
			
		const VectorMath::Vector2& size = GetSize();

		if(bUpdateText)
			pFont->PrepareFontString(mText, *mFontString, size.x, size.y, pLabelData->mScaleX, pLabelData->mScaleY, pLabelData->mLeading, pLabelData->mTracking, pLabelData->mTextFit);
		else
			pFont->PrepareFontString(*mFontString, size.x, size.y, pLabelData->mScaleX, pLabelData->mScaleY);
	}

	/* Sets the label's color
	 */
	Result Label::SetColor(uint32 color)
	{
		LabelData* pLabelData = const_cast<LabelData*>(static_cast<const LabelData*>(mControlData));
		pLabelData->mColor = color;
		
		return kResult_OK;
	}

	/* Retrieves the label's color
	 */
	uint32 Label::GetColor()
	{
		LabelData* pLabelData = const_cast<LabelData*>(static_cast<const LabelData*>(mControlData));
		return pLabelData->mColor;
	}

	/* Sets the label's font drawing scale
	 */
	Result Label::SetScale(float scaleX, float scaleY)
	{
		LabelData* pLabelData = const_cast<LabelData*>(static_cast<const LabelData*>(mControlData));
		if(pLabelData->mScaleX == scaleX && pLabelData->mScaleY == scaleY)
			return kResult_OK;

		pLabelData->mScaleX = scaleX;
		pLabelData->mScaleY = scaleY;

		UpdateFontString();

		return kResult_OK;
	}

	/**
	 * Sets the label's skew
	 */
	void Label::SetSkew(float skew)
	{
		LabelData* pLabelData = const_cast<LabelData*>(static_cast<const LabelData*>(mControlData));
		pLabelData->mSkew = skew;
	}

	/**
	 * Retrieves the label's skew
	 */
	float Label::GetSkew()
	{
		const LabelData* pLabelData = static_cast<const LabelData*>(mControlData);
		return pLabelData->mSkew;
	}
	
	/**
	 * Sets the label's drop shadow
	 */
	void Label::SetDropShadow(int dropShadow)
	{
		LabelData* pLabelData = const_cast<LabelData*>(static_cast<const LabelData*>(mControlData));
		pLabelData->mDropShadow = dropShadow;
	}

	/**
	 * Retrieves the label's drop shadow
	 */
	int Label::GetDropShadow()
	{
		const LabelData* pLabelData = static_cast<const LabelData*>(mControlData);
		return pLabelData->mDropShadow;
	}

	/**
	 * Clones the control and returns the new instance
	 */
	Control* Label::Clone()
	{
		Label* pLabel = OTTER_NEW(Label, ());
		memcpy((uint8*)pLabel->GetData(), (uint8*)GetData(), sizeof(LabelData));
		((ControlData*)pLabel->GetData())->mID = GetParentView()->GenerateNewID();
		mParent->AddControl(pLabel);

		pLabel->SetText(mText);
		return pLabel;
	}

	/* Renders the label
	 */
	void Label::Draw(Graphics* pGraphics)
	{
		if(!mEnabled)
			return;

		const LabelData* pLabelData = (const LabelData*)mControlData;
		if(!pLabelData)
			return;

		Font* pFont = mScene->GetFont(pLabelData->mFontID);
		if(!pFont)
			return;
		
		pGraphics->PushMatrix(GetTransform());	

		pFont->Draw(pGraphics, 
					*mFontString, 
					0.0f, 0.0f,
					(HoriAlignment)pLabelData->mHAlign, (VertAlignment)pLabelData->mVAlign, 
					GetColor(),
					VectorMath::Vector2::ZERO, 0.0f,
					pLabelData->mSkew,
					pLabelData->mDropShadow,
					pLabelData->mMaskID);

		pGraphics->PopMatrix();
	}

	/* Applies the interpolation of two keyframes to the control.
	 * pEndFrame can be NULL.
	 */
	void Label::ApplyKeyFrame(const KeyFrameData *pStartFrame, const KeyFrameData *pEndFrame, float factor)
	{
		Control::ApplyKeyFrame(pStartFrame, pEndFrame, factor);

		if(pStartFrame == NULL && pEndFrame == NULL)
			return;

		// Ensure that they're both color key frames
		if(pStartFrame && pStartFrame->mFourCC != FOURCC_KFRM || pEndFrame && pEndFrame->mFourCC != FOURCC_KFRM)
			return;

		const LabelLayout* pStartLayout	= (LabelLayout*)pStartFrame->GetLayout();
		const LabelLayout* pEndLayout	= (LabelLayout*)(pEndFrame ? pEndFrame->GetLayout() : NULL);

		// ARGB
		uint8 sA = pStartLayout ? (pStartLayout->mColor >> 24) & 0xFF : 0xFF;
		uint8 sR = pStartLayout ? (pStartLayout->mColor >> 16) & 0xFF : 0xFF;
		uint8 sG = pStartLayout ? (pStartLayout->mColor >> 8) & 0xFF : 0xFF;
		uint8 sB = pStartLayout ? (pStartLayout->mColor >> 0) & 0xFF : 0xFF;

		// ARGB
		uint8 eA = pEndLayout ? (pEndLayout->mColor >> 24) & 0xFF : 0xFF;
		uint8 eR = pEndLayout ? (pEndLayout->mColor >> 16) & 0xFF : 0xFF;
		uint8 eG = pEndLayout ? (pEndLayout->mColor >> 8) & 0xFF : 0xFF;
		uint8 eB = pEndLayout ? (pEndLayout->mColor >> 0) & 0xFF : 0xFF;
		
		uint8 fA = (uint8)(sA + factor * (eA - sA));
		uint8 fR = (uint8)(sR + factor * (eR - sR));
		uint8 fG = (uint8)(sG + factor * (eG - sG));
		uint8 fB = (uint8)(sB + factor * (eB - sB));

		float sScaleX = pStartLayout ? pStartLayout->mScaleX : 1.0f;
		float sScaleY = pStartLayout ? pStartLayout->mScaleY : 1.0f;

		float eScaleX = pEndLayout ? pEndLayout->mScaleX : 1.0f;
		float eScaleY = pEndLayout ? pEndLayout->mScaleY : 1.0f;

		float fScaleX = sScaleX + factor * (eScaleX - sScaleX);
		float fScaleY = sScaleY + factor * (eScaleY - sScaleY);

		float skew0 = pStartLayout ? pStartLayout->mSkew : 0.0f;
		float skew1 = pEndLayout ? pEndLayout->mSkew : 0.0f;
		float skew = skew0 + factor * (skew1 - skew0);

		int dropShadow0 = pStartLayout ? pStartLayout->mDropShadow : 0;
		int dropShadow1 = pEndLayout ? pEndLayout->mDropShadow : 0;
		int dropShadow = (int)(dropShadow0 + factor * (dropShadow1 - dropShadow0));

		SetColor(fA << 24 | fR << 16 | fG << 8 | fB);
		SetScale(fScaleX, fScaleY);
		SetSkew(skew);
		SetDropShadow(dropShadow);
	}
}
