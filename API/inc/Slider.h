#pragma once
#include "Control.h"
#include "Common/Event.h"

namespace Otter
{
	struct SliderData;
	class View;

	/**
	 * Slides a thumb between two endpoints.
	 */
	class Slider : public Control
	{
	public:
		/**
		 * Constructor
		 */
		Slider(Scene* pScene, Control* pParent, const SliderData* pSliderData);

		/**
		 * Constructor
		 */
		Slider();

		/**
		 * Destructor
		 */
		virtual ~Slider(void);

	public:

		/**
		 * Sets the Slider's color
		 */
		void SetColor(uint32 color);

		/**
		 * Retrieves the Slider's color
		 */
		uint32 GetColor();

		/**
		 * Sets start cap texture
		 */
		Result SetStartTexture(const char* szTexture);

		/**
		 * Sets middle texture, that stretches between the start and end caps
		 */
		Result SetMiddleTexture(const char* szTexture);

		/**
		 * Sets end cap texture
		 */
		Result SetEndTexture(const char* szTexture);

		/**
		 * Sets thumb texture
		 */
		Result SetThumbTexture(const char* szTexture);

		/**
		 * Sets the thumb size.  Changing the width and height of the 
		 * of the thumb also affects the start/end caps as well as the overall height 
		 * of the slider.
		 */
		Result SetThumbSize(uint32 width, uint32 height);

		/**
		 * Sets the slider's min/max range.  Clamps the current value if needed
		 */
		Result SetRange(sint32 min, sint32 max);

		/**
		 * Retrieves the slider's current range
		 */
		Result GetRange(sint32& min, sint32& max);

		/**
		 * Sets the slider's value.  Will be clamped in the current range
		 */
		Result SetValue(sint32 value);

		/**
		 * Retrieves the slider's current value
		 */
		sint32 GetValue();

		/**
		 * Sets the step value
		 */
		Result SetStep(uint32 step);

		/**
		 * Retrieves the step value
		 */
		uint32 GetStep();

	public:		

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

	public:
		
		/**
		 * Clones the control and returns the new instance
		 */
		virtual Control* Clone();

		/**
		 * Draws the Slider to screen
		 */
		virtual void Draw(Graphics* pGraphics);

		/**
		 * Applies the interpolation of two keyframes to the control.
		 * pEndFrame can be NULL.
		 */
		virtual void ApplyKeyFrame(const KeyFrameData* pStartFrame, const KeyFrameData* pEndFrame, float factor);

	private:

		/** 
	     * True if the user is currently dragging the slider thumb
		 */
		bool			mDraggingThumb;

		/**
		 * Records the last know touch point
		 */
		Point			mLastPoint;

	public:		
		
		/**
		 * Raised whenever the slider's value has changed.
		 * Event Parameter: Current slider value
		 */
		Event<sint32>	mOnValueChanged;
	};
}