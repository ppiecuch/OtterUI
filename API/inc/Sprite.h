#pragma once
#include "Control.h"

namespace Otter
{
	struct SpriteData;
	class View;

	/**
	 * Simple 2D Sprite rendered on a quad
	 */
	class Sprite : public Control
	{
	public:
		/**
		 * Constructor
		 */
		Sprite(Scene* pScene, Control* pParent, const SpriteData* pSpriteData);

		/* Constructor
		 */
		Sprite();

		/* Destructor
		 */
		virtual ~Sprite(void);

	public:

		/**
		 * Sets the sprite's color
		 */
		void SetColor(uint32 color);

		/**
		 * Retrieves the sprite's color
		 */
		uint32 GetColor();

		/**
		 * Sets the texture to be displayed
		 */
		Result SetTexture(const char* szTexture);

		/**
		 * Sets the texture by ID
		 */
		Result SetTexture(uint32 texID);

		/**
		 * Gets the texture ID
		 */
		uint32 GetTexture();

		/** 
	     * Sets the UVs for the sprite.  Useful to display a section
		 * of the sprite texture.  Clamped to [0,0] and [1,1]
		 */
		Result SetUVs(	float u_tl, float v_tl, 
						float u_br, float v_br);

		/**
		 * Sets the sprite's skew
		 */
		void SetSkew(float skew);

		/**
		 * Retrieves the sprite's skew
		 */
		float GetSkew();

	public:
		
		/**
		 * Clones the control and returns the new instance
		 */
		virtual Control* Clone();

		/**
		 * Draws the sprite to screen
		 */
		virtual void Draw(Graphics* pGraphics);

		/**
		 * Applies the interpolation of two keyframes to the control.
		 * pEndFrame can be NULL.
		 */
		virtual void ApplyKeyFrame(const KeyFrameData* pStartFrame, const KeyFrameData* pEndFrame, float factor);

	protected:

		/**
		 * Draws the sprite with the specified texture ID
		 */
		void Draw(Graphics* pGraphics, uint32 textureID);

	private:

		float tl_uv[2];
		float br_uv[2];

		uint32 mTextureID;
	};
}