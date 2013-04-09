#pragma once
#include "Control.h"

#include "Common/UTF8String.h"
#include "Common/Event.h"

namespace Otter
{
	struct ButtonData;
	class View;
	class Label;

	/**
	 * The Button maintains several states, and fires an event
	 * whenever it's been clicked.
	 */
	class Button : public Control
	{
	private:
		enum State
		{
			Default,
			Down
		};

	public:
		/**
		 * Constructs a new button within the specified scene and with the specified parent control.
		 */
		Button(Scene* pScene, Control* pParent, const ButtonData* pButtonData);

		/**
		 * Constructs a new button control at runtime.  The button will not be active until it is added
		 * to an existing control or view as a child control.
		 */
		Button();

		/**
		 * Virtual Destructor
		 */
		virtual ~Button(void);

	public:

		/**
		 * Sets the font by name
		 */
		Result SetFont(const char* szFontName);
		
		/**
		 * Sets the label's text
		 */
		Result SetText(const UTF8String& text);

		/**
		 * Sets the label's text
		 */
		Result SetText(const char* szText);

		/**
		 * Sets the label's color
		 */
		Result SetTextColor(uint32 color);

		/**
		 * Retrieves the label's color
		 */
		uint32 GetTextColor();
		
		/**
		 *  Sets the horizontal and vertical text alignment
		 */
		Result SetTextAlignment(HoriAlignment halign, VertAlignment valign);

		/**
		 * Sets the label's font drawing scale
		 */
		Result SetTextScale(float scaleX, float scaleY);

		/**
		 * Sets the texture to be displayed during the default state
		 */
		Result SetDefaultTexture(const char* szTexture);

		/**
		 * Sets the texture to be displayed during the down state
		 */
		Result SetDownTexture(const char* szTexture);

		/**
		 * Sets the control's size
		 */
		virtual void SetSize(const VectorMath::Vector2& size);

		/**
		 * Sets the control's parent
		 */
		virtual Result SetParentControl(Control* pParent);

	public:
		
		/**
		 * Called whenever this control is activated
		 */
		virtual void OnActivate();

		/**
		 * Called whenever this control is deactivated
		 */
		virtual void OnDeactivate();

	public:
		
		/**
		 * Clones the control and returns the new instance
		 */
		virtual Control* Clone();

		/**
		 * Draws the button to screen
		 */
		virtual void Draw(Graphics* pGraphics);

	private:

		/**
		 * Points (touches/mouse/etc) were pressed down
		 */
		virtual bool OnPointsDown(Point* points, sint32 numPoints);
		
		/**
		 * Points (touches/mouse/etc) were released
		 */
		virtual bool OnPointsUp(Point* points, sint32 numPoints);
		
		/**
		 * Points (touches/mouse/etc) were moved.
		 */
		virtual bool OnPointsMove(Point* points, sint32 numPoints);

	private:

		/** @brief Updates the button's text label
		 */
		void UpdateLabel();
	
	public:	
		
		/**
		 * Raised whenever the button has been clicked.
		 * Event Parameter: Unused
		 */
		Event<void*>	mOnClick;

	private:

		/**
		 * Button's current state
		 */
		State			mButtonState;

		/**
		 * Label to be rendered on top of the button
		 */
		Label*			mLabel;
	};
}