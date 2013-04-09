#pragma once
#include "Control.h"

namespace Otter
{
	struct MaskData;
	class View;

	/**
	 * Mask used to mask out parts of other controls
	 */
	class Mask : public Control
	{
	public:
		/**
		 * Constructor
		 */
		Mask(Scene* pScene, Control* pParent, const MaskData* pMaskData);

		/* Constructor
		 */
		Mask();

		/* Destructor
		 */
		virtual ~Mask(void);

	public:

		/**
		 * Sets the texture to be displayed
		 */
		Result SetTexture(const char* szTexture);

		/** 
	     * Sets the UVs for the mask.  Useful to display a section
		 * of the mask texture.  Clamped to [0,0] and [1,1]
		 */
		Result SetUVs(	float u_tl, float v_tl, 
						float u_br, float v_br);

		/**
		 * Sets the mask's skew
		 */
		void SetSkew(float skew);

		/**
		 * Retrieves the mask's skew
		 */
		float GetSkew();

	public:
		
		/**
		 * Clones the control and returns the new instance
		 */
		virtual Control* Clone();

		virtual void Draw(Graphics* pGraphics) {}

		/**
		 * Draws the mask to the stencil buffer
		 */
		virtual void DrawMask(Graphics* pGraphics);

		/**
		 * Applies the interpolation of two keyframes to the control.
		 * pEndFrame can be NULL.
		 */
		virtual void ApplyKeyFrame(const KeyFrameData* pStartFrame, const KeyFrameData* pEndFrame, float factor);

	private:

		float tl_uv[2];
		float br_uv[2];
	};
}