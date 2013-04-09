#pragma once
#include "Common/UTF8String.h"
#include "Control.h"

namespace Otter
{
	struct LabelData;
	struct FontString;
	class View;

	/**
	 * The Label control is responsible for drawing text
	 * to the screen, using the specified font, color, scale and alignment settings.
	 */
	class Label : public Control
	{
	public:
		/**
		 * Constructor
		 */
		Label();

		/**
		 * Constructor
		 */
		Label(Scene* pScene, Control* pParent, const LabelData* pLabelData);

		/**
		 * Virtual Destructor
		 */
		virtual ~Label(void);

	public:

		/**
		 * Sets the label's size.
		 */
		virtual void SetSize(const VectorMath::Vector2& size);

	public:

		/**
		 * Sets the font by name
		 */
		Result SetFont(const char* szFontName);

		/**
		 * Sets the font by id
		 */
		Result SetFont(uint32 fontID);

		/**
		 * Gets the font id
		 */
		uint32 GetFont();

		/**
		 * Sets the label's text
		 */
		Result SetText(const UTF8String& text);

		/**
		 * Sets the label's text
		 */
		Result SetText(const char* szText);

		/**
		 * Retrieves the label's text
		 */
		const UTF8String& GetText();
		
		/**
		 * Sets the horizontal and vertical text alignment
		 */
		Result SetTextAlignment(HoriAlignment halign, VertAlignment valign);

		/**
		 * Gets the horizontal text alignment
		 */
		HoriAlignment GetHorizontalTextAlignment();
		
		/**
		 * Gets the vertical text alignment
		 */
		VertAlignment GetVerticalTextAlignment();

		/**
		 * Sets the label's color
		 */
		Result SetColor(uint32 color);

		/**
		 * Retrieves the label's color
		 */
		uint32 GetColor();

		/**
		 * Sets the label's font drawing scale
		 */
		Result SetScale(float scaleX, float scaleY);

		/**
		 * Sets the label's skew
		 */
		void SetSkew(float skew);

		/**
		 * Retrieves the label's skew
		 */
		float GetSkew();

		/**
		 * Sets the label's drop shadow
		 */
		void SetDropShadow(int dropShadow);

		/**
		 * Retrieves the label's drop shadow
		 */
		int GetDropShadow();

	public:

		/**
		 * Clones the control and returns the new instance
		 */
		virtual Control* Clone();

		/**
		 * Draws the label to screen
		 */
		virtual void Draw(Graphics* pGraphics);

		/**
		 * Applies the interpolation of two keyframes to the control.
		 * 
		 * @param pStartFrame Start Keyframe.  Cannot be NULL.
		 * @param pEndFrame End keyframe.  Can be NULL.
		 * @param factor Interpolation factor between pStartFrame and pEndFrame.
		 */
		void ApplyKeyFrame(const KeyFrameData *pStartFrame, const KeyFrameData *pEndFrame, float factor);

	private:

		/**
		 * Updates the internal font string with the label's current layout
		 */
		void UpdateFontString(bool bUpdateText = true);

	private:
		/**
		 * UTF8 string to display
		 */
		UTF8String		mText;

		/**
		 * Pre-processed font string to render.
		 * Whenever the label's alignment, size or string changes,
		 * the font string will be updated accordingly.
		 */
		FontString*		mFontString;
	};
}