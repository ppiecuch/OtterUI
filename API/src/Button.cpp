#include <string.h>

#include "Button.h"
#include "View.h"
#include "Scene.h"
#include "System.h"
#include "Font.h"
#include "Label.h"

#include "Data/OtterData.h"
#include "Graphics/Graphics.h"
#include "Memory/Memory.h"

using namespace VectorMath;

namespace Otter
{
	/* Constructor
	 */
	Button::Button(Scene* pScene, Control* pParent, const ButtonData* pButtonData) : Control(pScene, pParent, pButtonData)
	{
		mButtonState = Default;
		mLabel = OTTER_NEW(Label, ());
		mLabel->EnableTouches(false);
		AddControl(mLabel);

		SetText(pButtonData->GetText());
		SetTextColor(0xFFFFFFFF);
		SetTextAlignment((Otter::HoriAlignment)pButtonData->mHAlign, (Otter::VertAlignment)pButtonData->mVAlign);
		SetTextScale(pButtonData->mScaleX, pButtonData->mScaleY);
		mLabel->SetFont(pButtonData->mFontID);

		UpdateLabel();
	}

	/* Constructor
	 */
	Button::Button() : Control(NULL, NULL, OTTER_NEW(ButtonData, ()))
	{
		mButtonState = Default;
		mLabel = OTTER_NEW(Label, ());
		mLabel->EnableTouches(false);
		AddControl(mLabel);

		SetText("");
		SetTextColor(0xFFFFFFFF);

		mUserCreated = true;
		UpdateLabel();
	}

	/* Virtual Destructor
	 */
	Button::~Button(void)
	{
		// The label may have been removed for us
		if(RemoveControl(mLabel) != kResult_OK)
			OTTER_DELETE(mLabel);

		mLabel = NULL;

		if(mUserCreated)
		{
			OTTER_DELETE(GetData());
		}
	}

	/* Updates the label with the current settings
	 */
	void Button::UpdateLabel()
	{
		if(!mLabel)
			return;

		mLabel->SetSize(this->GetSize());
	}

	/* Sets the font by name
		*/
	Result Button::SetFont(const char* szFontName)
	{
		if(mLabel)
		{
			mLabel->SetFont(szFontName);
		}

		return kResult_OK;
	}
		
	/* Sets the label's text
	 */
	Result Button::SetText(const UTF8String& text)
	{
		if(mLabel)
		{
			mLabel->SetText(text);
		}

		return kResult_OK;
	}

	/* Sets the label's text
	 */
	Result Button::SetText(const char* szText)
	{
		if(mLabel)
		{
			mLabel->SetText(szText);
		}

		return kResult_OK;
	}
	
	/* Sets the button's color
	 */
	Result Button::SetTextColor(uint32 color)
	{
		if(mLabel)
		{
			mLabel->SetColor(color);
		}
		
		return kResult_OK;
	}

	/* Retrieves the button's color
	 */
	uint32 Button::GetTextColor()
	{
		if(mLabel)
		{
			return mLabel->GetColor();
		}

		return 0x00000000;
	}

	/* Sets the button's font drawing scale
	 */
	Result Button::SetTextScale(float scaleX, float scaleY)
	{
		if(mLabel)
		{
			return mLabel->SetScale(scaleX, scaleY);
		}
		
		return kResult_OK;
	}
		
	/* Sets the horizontal and vertical text alignment
	 */
	Result Button::SetTextAlignment(HoriAlignment halign, VertAlignment valign)
	{		
		if(mLabel)
		{
			mLabel->SetParentControl(this);
			return mLabel->SetTextAlignment(halign, valign);
		}
		
		return kResult_OK;
	}

	/* Sets the texture to be displayed during the default state
	 */
	Result Button::SetDefaultTexture(const char* szTexture)
	{
		Scene* pScene = GetScene();
		if(pScene == NULL)
			return kResult_Error;

		ButtonData* pButtonData = const_cast<ButtonData*>(static_cast<const ButtonData*>(GetData()));
		pButtonData->mDefaultTextureID = pScene->GetTextureID(szTexture);

		return kResult_OK;
	}

	/* Sets the texture to be displayed during the down state
	 */
	Result Button::SetDownTexture(const char* szTexture)
	{
		Scene* pScene = GetScene();
		if(pScene == NULL)
			return kResult_Error;

		ButtonData* pButtonData = const_cast<ButtonData*>(static_cast<const ButtonData*>(GetData()));
		pButtonData->mDownTextureID = pScene->GetTextureID(szTexture);

		return kResult_OK;
	}

	/**
	 * Sets the control's size
	 */
	void Button::SetSize(const VectorMath::Vector2& size)
	{
		Control::SetSize(size);
		UpdateLabel();
	}

	/**
	 * Sets the control's parent
	 */
	Result Button::SetParentControl(Control* pParent)
	{
		Control::SetParentControl(pParent);
		mLabel->SetParentControl(this);

		return kResult_OK;
	}
		
	/* Called whenever this control is activated
	 */
	void Button::OnActivate()
	{
	}

	/* Called whenever this control is deactivated
	 */
	void Button::OnDeactivate()
	{
	}

	/**
	 * Clones the control and returns the new instance
	 */
	Control* Button::Clone()
	{
		Button* pButton = OTTER_NEW(Button, ());
		memcpy((uint8*)pButton->GetData(), (uint8*)GetData(), sizeof(ButtonData));
		((ControlData*)pButton->GetData())->mID = GetParentView()->GenerateNewID();
		mParent->AddControl(pButton);

		pButton->mButtonState = this->mButtonState;
		memcpy((uint8*)pButton->mLabel->GetData(), (uint8*)mLabel->GetData(), sizeof(LabelData));

		pButton->mLabel->SetText(mLabel->GetText());

		return pButton;
	}

	/* Draws the button to screen
	 */
	void Button::Draw(Graphics* pGraphics)
	{
		if(!mEnabled)
			return;
		
		const VectorMath::Vector2& size = GetSize();

		const ButtonData* pButtonData = static_cast<const ButtonData*>(mControlData);
		uint32 textureID = mButtonState == Default ? pButtonData->mDefaultTextureID : pButtonData->mDownTextureID;	
		uint32 buttonCol = mButtonState == Default ? pButtonData->mDefaultColor : pButtonData->mDownColor;

		const TextureData* pTextureData = GetScene()->GetTextureData(textureID);

		if(mLabel)
			mLabel->SetSize(GetSize());

		pGraphics->PushMatrix(GetTransform());
		pGraphics->DrawRectangle(	pTextureData->mTextureRect.mTextureID, 
									0.0f, 0.0f, size.x, size.y, 
									pTextureData->mTextureRect.uv[0], pTextureData->mTextureRect.uv[1], pTextureData->mTextureRect.uv[2], pTextureData->mTextureRect.uv[3],
									buttonCol, 0.0f);

		pGraphics->PopMatrix();
		
		Control::Draw(pGraphics);
	}
	
	/* Points (touches/mouse/etc) were moved.
	 */
	bool Button::OnPointsMove(Point* points, sint32 numPoints)
	{
		if(!mTouchEnabled)
			return false;

		if(!mEnabled)
			return false;

		if(mButtonState == Down)
		{		
			const VectorMath::Vector2& size = GetSize();

			for(sint32 i = 0; i < numPoints; i++)
			{
				if(points[i].x < 0 || points[i].y < 0 ||
				   points[i].x > size.x || points[i].y > size.y)
				{
					mButtonState = Default;
					break;
				}
			}
		}
		 
		return true;
	}

	/* Points (touches/mouse/etc) were released
	 */
	bool Button::OnPointsUp(Point* points, sint32 numPoints)
	{
		if(!mTouchEnabled)
			return false;

		if(!mEnabled)
			return false;
		  
		if(mButtonState == Down)
		{
			mOnClick(this, NULL);
			
			const ButtonData* pButtonData = static_cast<const ButtonData*>(mControlData);
			uint32 numActions = pButtonData->mNumOnClickActions;

			View* pView = GetParentView();
			if(pView)
			{
				for(uint32 i = 0; i < numActions; i++)
				{
					const ActionData* pAction = pButtonData->GetAction(i);
					pView->ProcessAction(this, pAction);
				}
			}
		}

		// Always release the button state whenever the mouse
		// has been released
		mButtonState = Default;
		return true;
	}

	/* Points (touches/mouse/etc) were pressed down
	 */
	bool Button::OnPointsDown(Point* points, sint32 numPoints)
	{
		if(!mTouchEnabled)
		 	return false;

		if(!mEnabled)
			return false;

		mButtonState = Down;
		return true;
	}
}
