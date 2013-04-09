#pragma once

#include "Common/Types.h"
#include "Common/UTF8String.h"
#include "Math/VectorMath.h"

namespace Otter
{
	class Graphics;
	struct FontData;	
	struct GlyphData;
	
	// Defines a single character's bounds (on screen).
	// The bounds are relative.  It is up to the renderer
	// to offset the character accordingly.
	struct CharQuad
	{
		int mLeft;
		int mRight;
		int mTop;
		int mBottom;

		const GlyphData* mGlyphData;

		CharQuad() : mLeft(0), mRight(0), mTop(0), mBottom(0), mGlyphData(0)
		{
		}
	};

	/* Maintaints information pertaining to a single 'line' of characters to be drawn
	 * to screen.
	 */
	struct CharLine
	{
		Array<CharQuad> mCharacters;
		int mOffsetY;

		/* Constructor
		 */
		CharLine()
		{
			mCharacters.reserve(100);
			mOffsetY = 0;
		}

		~CharLine()
		{
			mCharacters.clear();
		}

		/* Retrieves the width of the line
		 */
		int GetWidth() const
		{
			int min = 0; 
			int max = 0;

			for(uint32 i = 0; i < mCharacters.size(); i++)
            {
				const CharQuad& charQuad = mCharacters[i];

				if (charQuad.mLeft < min)
                    min = charQuad.mLeft;

				if (charQuad.mRight > max)
                    max = charQuad.mRight;
            }
			
			return (max - min);
		}
	};

	struct FontString
	{
		Array<CharLine> mLines;

		float			mWidth;
		float			mHeight;
		float			mScaleX;
		float			mScaleY;

		float			mStringHeight;
	};

	/* The GUI Font is responsible for all font loading and rendering
	 */
	class Font
	{
	public:
		/* Constructor
		 */
		Font(const FontData* pFontData);

		/* Virtual Destructor
		 */
		virtual ~Font(void);

	public:

		/* Text fit mode enum corresponding to the GUILabel class's version
		 */
		enum TextFitMode
		{
			Wrap,
			No_Wrap,
			Scale_To_Fit,
			Scale_Down,
			Truncate
		};

		/* Returns the font's internal data
		 */
		const FontData& GetData() { return *mFontData; }

		/* Sets the font's internal data.
		 */
		void SetData(const FontData* pFontData) { mFontData = pFontData; }

		/* Sets the textures that this font will use to render itself.
		 */
		void SetTextures(uint32* textures, int count) 
		{ 
			mTextures.resize(count);
			for(int i = 0; i < count; i++)
				mTextures[i] = textures[i];
		}

		/* Retrieves the textures used by this font
		 */
		const Array<uint32>& GetTextures()
		{
			return mTextures;
		}

		/* Prepares a UTF8String for renderering
		 */
		void PrepareFontString(FontString& fontString, float w, float h, float scaleX, float scaleY);

		/* Prepares a UTF8String for renderering
		 */
		void PrepareFontString(const UTF8String& str, FontString& fontString, float w, float h, float scaleX, float scaleY, float leading, sint32 tracking, uint32 textFit);

		/* Draws text to the screen
		 */
		void Draw(	Graphics* pGraphics, 
					const UTF8String& str, 
					float x, 
					float y, 
					float w, 
					float h, 
					float scaleX, 
					float scaleY, 
					HoriAlignment halign = kHAlign_Left, 
					VertAlignment valign = kVAlign_Top, 
					uint32 color = 0xFFFFFFFF,
					const VectorMath::Vector2& center = VectorMath::Vector2::ZERO,
					float rotation = 0.0f,
					float leading = 0.15f,
					sint32 tracking = 0,
					float skewAngle = 0.0f,
					uint32 textFit = Wrap,
					int dropShadow = 0,
					int maskID = -1);

		/* Draws a previously prepared font string
		 */
		void Draw(	Graphics* pGraphics, 
					const FontString& fontString, 
					float x, 
					float y, 
					HoriAlignment halign, 
					VertAlignment valign, 
					uint32 color = 0xFFFFFFFF, 
					const VectorMath::Vector2& center = VectorMath::Vector2::ZERO, 
					float rotation = 0.0f,
					float skewAngle = 0.0f,
					int dropShadow = 0,
					int maskID = -1);

		static int GetLines(const UTF8String& str, Font& font, Array<CharLine>& lines, float maxWidth, float maxHeight, float leading, sint32 tracking, uint32 textFit);
		static void PopulateVerts(GUIVertex* verts, const VectorMath::Vector2& pos, float x, float y, const VectorMath::Vector2& size, const VectorMath::Vector2& center, const VectorMath::Vector2& uv1, const VectorMath::Vector2& uv2, uint32 color, float skewOffset, float bottomSkewOffset, float sin_a, float cos_a);

	protected:

		const FontData* mFontData;
		Array<uint32> mTextures;
	};
}
