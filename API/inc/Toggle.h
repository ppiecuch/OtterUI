#pragma once
#include "Control.h"
#include "Common/Event.h"

namespace Otter
{
	struct ToggleData;
	class View;

	/**
	 * Two-state control that switches between "On" and "Off" on touch input
	 */
	class Toggle : public Control
	{
	public:
		enum State
		{
			On,
			Off
		};

	public:
		/**
		 * Constructor
		 */
		Toggle(Scene* pScene, Control* pParent, const ToggleData* pToggleData);

		/**
		 * Constructor
		 */
		Toggle();

		/**
		 * Destructor
		 */
		virtual ~Toggle(void);

	public:

		/**
		 * Sets the Toggle's color
		 */
		void SetColor(uint32 color);

		/**
		 * Retrieves the Toggle's color
		 */
		uint32 GetColor();

		/**
		 * Sets the texture that is displayed when the toggle is "on"
		 */
		Result SetOnTexture(const char* szTexture);

		/**
		 * Sets the texture that is displayed when the toggle is "off"
		 */
		Result SetOffTexture(const char* szTexture);

		/**
		 * Sets the toggle's state
		 */
		Result SetState(State state);

		/**
		 * Gets the toggle's state
		 */
		State GetState();

	public:		

		/**
		 * Points (touches/mouse/etc) were pressed down
		 */
		virtual bool OnPointsDown(Point* points, sint32 numPoints);
		
		/**
		 * Points (touches/mouse/etc) were released
		 */
		virtual bool OnPointsUp(Point* points, sint32 numPoints);

	public:

		/**
		 * Draws the Toggle to screen
		 */
		virtual void Draw(Graphics* pGraphics);

		/**
		 * Applies the interpolation of two keyframes to the control.
		 * pEndFrame can be NULL.
		 */
		virtual void ApplyKeyFrame(const KeyFrameData* pStartFrame, const KeyFrameData* pEndFrame, float factor);

	protected:
		
		/**
		 * Clones the control and returns the new instance
		 */
		virtual Control* Clone();

		/**
		 * Draws the Toggle with the specified texture ID
		 */
		void Draw(Graphics* pGraphics, uint32 textureID);

	private:

		/**
		 * Toggle's current state
		 */
		State	mToggleState;

	public:		
		/**
		 * Raised whenever the toggle's state has changed
		 * Event Parameter: New state of the toggle
		 */
		Event<Toggle::State>	mOnToggleChanged;
	};
}