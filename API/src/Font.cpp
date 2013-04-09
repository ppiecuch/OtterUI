#include <string.h>

#include "Font.h"
#include "Graphics/Graphics.h"
#include "Data/OtterData.h"
#include "Memory/Memory.h"

namespace Otter
{
	/* Retrieves the character length of the first word
	 * encountered in the provided unicode text.
	 */
	int GetFirstWordLength(Array<const GlyphData*>& glyphs, int startIndex)
	{
		int numGlyphs = glyphs.size();
		if(numGlyphs == 0 || startIndex >= numGlyphs)
			return 0;

		int numChars = 0;
		for(int i = startIndex; i < numGlyphs; i++)
		{
			const GlyphData* glyph = glyphs[i];

			if(!glyph->mIsImageGlyph)
			{
				if(glyph->mCharCode == '\r'	||
				   glyph->mCharCode == '\n'	||
				   glyph->mCharCode == ' '	||
				   glyph->mCharCode == '\t')
				{
					// Only identify the character as a word if it started the word.
					if(i == startIndex)
					{
						numChars = 1;
					}

					break;
				}
			}

			numChars++;
		}

		return numChars;
	}

	/* Retrieves the pixel-width of the text
	 */
	int GetStringWidth(Font& font, Array<const GlyphData*>& glyphs, uint32 startIndex, uint32 length, sint32 tracking)
	{
		int strWidth = 0;
		const GlyphData* glyphData = NULL;

		if(startIndex + length > glyphs.size())
			return 0;

		for(uint32 c = startIndex; c < (startIndex + length); c++)
		{
			// Make sure to advance by the previous character
			if(glyphData)
				strWidth += (glyphData->mAdvance - glyphData->mWidth) + tracking;

			glyphData = glyphs[c];
			if(glyphData)
			{
				strWidth += glyphData->mLeftBearing + glyphData->mWidth;
			}
			else
			{
				strWidth += font.GetData().mFontSize + tracking;
			}
		}

		return strWidth;
	}

	/* Retrieves the glyph data for the string, applying all necessary formatting if necessary
	 */
	void GetGlyphData(Font& font, Array<const GlyphData*>& glyphData, const UTF8String& str)
	{
		Array<uint32> unicodeChars;
		str.GetUnicodeChars(unicodeChars);
		
		bool isEscapeChar = false;
		int numChars = unicodeChars.size();
		for(int i = 0; i < numChars; i++)
		{
			uint32 uchar = unicodeChars[i];
			if(uchar == 0)
				break;

			const GlyphData* glyph = NULL;

			if(!isEscapeChar && uchar == '\\')
			{
				isEscapeChar = true;
				continue;
			}

			if(isEscapeChar)
			{
				 // TODO - special handling
			}
			// Ignore carriage return
			else if(uchar == '\r')
			{
				continue;
			}
			else if(uchar == '\n')
			{
				static GlyphData lineFeed('\n', 0, 0, 0, 0, 0, 0, 0, 0);
				glyph = &lineFeed;
			}
			else if(uchar == '{')
			{
				// look for a fourcc identifier
				// Note - no need to explicitly check for four characters; If
				// more were specified, it will simply bit-overflow and yield nothing.
				uint32 fourcc = 0;
				sint32 shift = 24;
				while(true)
				{
					uchar = unicodeChars[++i];
					
					if(!uchar || uchar == '}')
						break;

					if(shift >= 0)
					{
						fourcc |= (uchar & 0x000000FF) << shift;
						shift -= 8;
					}
				}
				
				glyph = font.GetData().GetGlyphData(fourcc, true);

				// If the char is not found, default to '?'
				if(!glyph)
					uchar = '?';
			}
			
			if(!glyph)
				glyph = font.GetData().GetGlyphData(uchar);

			if(!glyph)
				glyph = font.GetData().GetGlyphData('?');

			if(glyph)
				glyphData.push_back(glyph);

			isEscapeChar = false;
		}
	}

	/* Retrieves the character lines that we'll output to screen. Returns the size of the widest line to draw.
	 */
	int Font::GetLines(const UTF8String& str, Font& font, Array<CharLine>& lines, float maxWidth, float maxHeight, float leading, sint32 tracking, uint32 textFit)
	{	
		lines.clear();
		CharLine line;
		line.mOffsetY = 0;

        int cur_x = 0;
        int cur_y = 0;
		int fontSize = font.GetData().mFontSize;
        int lineAdvance = fontSize + (int)(fontSize * leading);
		int max_x = 0;

		int wordLen = 0;

		Array<const GlyphData*> glyphs;
		glyphs.reserve(str.StrLen());
		GetGlyphData(font, glyphs, str);
		
		int i = 0;
		uint32 uchar = 0;
		int numGlyphs = glyphs.size();
		while(i < numGlyphs)
        {		
			int strWidth = 0;
			bool bWhiteSpaceChar = false;

			wordLen = GetFirstWordLength(glyphs, i);
			uchar = glyphs[i]->mCharCode;

			bool bImageGlyph = glyphs[i]->mIsImageGlyph ? true : false;

            if (!bImageGlyph && uchar == '\n')
            {
                cur_x = 0;
                cur_y += lineAdvance;

				// Add the current line.
				if(line.mCharacters.size() > 0)
					lines.push_back(line);

				// Start a new line
				line.mCharacters.clear();				
				line.mOffsetY = cur_y;

				if(cur_y + fontSize >= maxHeight)
					break;

				i += wordLen;
				continue;
            }

			if (!bImageGlyph && uchar == '\t')
            {
                strWidth = (fontSize * 2) - (cur_x % (fontSize * 2)) + tracking;
				bWhiteSpaceChar = true;
			}
			else if(!bImageGlyph && uchar == ' ')
			{
				strWidth = fontSize / 3 + tracking;
				bWhiteSpaceChar = true;
			}
			else
			{
				strWidth = GetStringWidth(font, glyphs, i, wordLen, tracking);
			}

			if(textFit == Wrap && (cur_x != 0) && (cur_x + strWidth) > maxWidth)
			{
				cur_x = 0;
				cur_y += lineAdvance;

				// Add the current line.
				if(line.mCharacters.size() > 0)
					lines.push_back(line);

				// Start a new line
				line.mCharacters.clear();			
				line.mOffsetY = cur_y;

				if(cur_y + fontSize >= maxHeight)
					break;

				// If this is a whitespace character, ignore it completely.
				if(bWhiteSpaceChar)
					strWidth = 0;
			}

			if(!bWhiteSpaceChar)
			{
				for(int c = 0; c < wordLen; c++)
				{
					// Retrieve the character information
					const GlyphData* glyphData = glyphs[i + c];

					CharQuad charQuad;
					charQuad.mGlyphData = glyphData;

					if(glyphData)
					{
						charQuad.mLeft		= cur_x + glyphData->mLeftBearing;
						charQuad.mTop		= cur_y + font.GetData().mMaxTop - (glyphData->mTop + 1);
						charQuad.mRight		= charQuad.mLeft + glyphData->mWidth;
						charQuad.mBottom	= charQuad.mTop + glyphData->mHeight;
					}
					else
					{
						charQuad.mLeft		= cur_x;
						charQuad.mTop		= cur_y;
						charQuad.mRight		= charQuad.mLeft + fontSize;
						charQuad.mBottom	= charQuad.mTop + fontSize;
					}
				
					line.mCharacters.push_back(charQuad);

					cur_x += glyphData->mAdvance + tracking;
					if (charQuad.mRight > max_x)
						max_x = charQuad.mRight;
				}
			} 
			else
			{
				cur_x += strWidth;
			}

			i += wordLen;
        }

		// Add the last line we were constructing if it has characters
		if(line.mCharacters.size() > 0)
		{
			lines.push_back(line);
		}

		return max_x;
	}

	/* Constructor
	 */
	Font::Font(const FontData* pFontData)
	{
		mFontData = pFontData;
		mTextures.clear();
	}

	/* Destructor
	 */
	Font::~Font(void)
	{
	}

	/* Prepares a UTF8String for renderering
	 */
	void Font::PrepareFontString(FontString& fontString, float w, float h, float scaleX, float scaleY)
	{
		if(mFontData == NULL)
			return;

		if(scaleX <= 0.0f)
			scaleX = 0.01f;

		if(scaleY <= 0.0f)
			scaleY = 0.01f;

        int max = 0;
		for(uint32 i = 0; i < fontString.mLines.size(); i++)
        {
			CharLine& line = fontString.mLines[i];

			if(max < line.mOffsetY)
				max = line.mOffsetY;
        }

		fontString.mWidth  = w;
		fontString.mHeight = h;
		fontString.mScaleX = scaleX;
		fontString.mScaleY = scaleY;
		fontString.mStringHeight = (max + GetData().mFontSize) * scaleY;
	}

	/* Prepares a UTF8String for renderering
		*/
	void Font::PrepareFontString(const UTF8String& str, FontString& fontString, float w, float h, float scaleX, float scaleY, float leading, sint32 tracking, uint32 textFit)
	{
		const uint8* szText = str.GetUTF8();

		if(szText[0] == 0 || mFontData == NULL)
			return;

		if(scaleX <= 0.0f)
			scaleX = 0.01f;

		if(scaleY <= 0.0f)
			scaleY = 0.01f;

		fontString.mLines.clear();
		fontString.mLines.reserve(50);

		float textWidth = scaleX * GetLines(szText, *this, fontString.mLines, w / scaleX, h / scaleY, leading, tracking, textFit);

		//check whether to apply the special scaling modes
		if (textFit == Scale_To_Fit || textFit == Scale_Down && textWidth > w)
		{
			//horizontally scale the entire line of text to fit the bounding box
			float scaleFactor = (float)w / textWidth;
			for (unsigned int line = 0; line < fontString.mLines.size(); ++line)
			{
				CharLine* thisLine = &fontString.mLines[line];
				if (thisLine->mCharacters.size() == 0)
					continue;

				int lineStart = thisLine->mCharacters[0].mLeft;
				for (unsigned int quad = 0; quad < thisLine->mCharacters.size(); ++quad)
				{
					CharQuad* thisQuad = &thisLine->mCharacters[quad];
					thisQuad->mLeft = (int)((thisQuad->mLeft - lineStart) * scaleFactor);
					thisQuad->mRight = (int)((thisQuad->mRight - lineStart) * scaleFactor);
				}
			}
		}
		else if (textFit == Truncate && textWidth > w)
		{
			if (fontString.mLines.size() != 1)
				return;

			CharLine* thisLine = &fontString.mLines[0];
			if (thisLine->mCharacters.size() == 0)
				return;

			//first, figure out the size of a "..." string
			Array<CharLine> ellipsisLines;
			ellipsisLines.clear();
			ellipsisLines.reserve(1);
			int ellipsisLength = GetLines(UTF8String("..."), *this, ellipsisLines, 9999.0f, 9999.0f, 0.0f, 0, No_Wrap);

			//now remove characters from the end of the string until enough space for the "..." is available
			int origLen = str.StrLen();
			int finalStringLength = origLen;
			int charIndex = str.StrLen() - 1;
			for (int quad = thisLine->mCharacters.size() - 1; quad >= 0; --quad)
			{
				while (str[charIndex] == ' ')
				{
					//skip any space characters, since they aren't actually quads
					--charIndex;
					--finalStringLength;
				}

				CharQuad* thisQuad = &thisLine->mCharacters[quad];
				if (thisQuad->mRight + ellipsisLength < w && (origLen - finalStringLength) >= 3)
				{
					//we've found a length that fits
					break;
				}

				--charIndex;
				--finalStringLength;
			}

			//end the visible string with "..."
			UTF8String finalString = UTF8String(szText);
			int i = finalStringLength;
			for (; i < finalStringLength + 3; ++i)
				finalString[i] = '.';
			finalString[i] = 0;

			//finally, pass the new truncated string into GetLines again
			GetLines(finalString, *this, fontString.mLines, w / scaleX, h / scaleY, leading, tracking, No_Wrap);
		}

		PrepareFontString(fontString, w, h, scaleX, scaleY);
	}

	/* Draws szText to the screen
	 */
	void Font::Draw(Graphics* pGraphics, const UTF8String& str, float x, float y, float w, float h, float scaleX, float scaleY, HoriAlignment halign, VertAlignment valign, uint32 color, const VectorMath::Vector2& center, float rotation, float leading, sint32 tracking, float skewAngle, uint32 textFit, int dropShadow, int maskID)
	{
		FontString fontString;
		PrepareFontString(str, fontString, w, h, scaleX, scaleY, leading, tracking, textFit);
		Draw(pGraphics, fontString, x, y, halign, valign, color, center, rotation, skewAngle, dropShadow, maskID);
	}
	
	/* Draws a previously prepared font string
	 */
	void Font::Draw(Graphics* pGraphics, const FontString& fontString, float x, float y, HoriAlignment halign, VertAlignment valign, uint32 color, const VectorMath::Vector2& center, float rotation, float skewAngle, int dropShadow, int maskID)
	{
		const float cos_a = VectorMath::Functions::Cos(rotation * VectorMath::Constants::DEG_TO_RAD);
		const float sin_a = VectorMath::Functions::Sin(rotation * VectorMath::Constants::DEG_TO_RAD);

		//multiply this by the height of a character to get the actual offset
		float skewMultiplier = VectorMath::Functions::Tan(skewAngle * VectorMath::Constants::DEG_TO_RAD);

		static GUIVertex verts[6]; // Two triangles

		VectorMath::Vector2 pos;
		VectorMath::Vector2 size;
		VectorMath::Vector2 uv1;
		VectorMath::Vector2 uv2;

		float fontWidth = (float)mFontData->mFontWidth;
		float fontHeight = (float)mFontData->mFontHeight;

		for(uint32 i = 0; i < fontString.mLines.size(); i++)
        {
			const CharLine& line = fontString.mLines[i];			

            float offsetX = x;
            float offsetY = y;
			float lineWidth = line.GetWidth() * fontString.mScaleX;

			if (halign == kHAlign_Center)
                offsetX += (fontString.mWidth - lineWidth) / 2;
			else if (halign == kHAlign_Right)
                offsetX += fontString.mWidth - lineWidth;

			if (valign == kVAlign_Center)
				offsetY += (fontString.mHeight - fontString.mStringHeight) / 2;
            else if (valign == kVAlign_Bottom)
                offsetY += fontString.mHeight - fontString.mStringHeight;

			for(uint32 j = 0; j < line.mCharacters.size(); j++)
            {
				const CharQuad& charQuad = line.mCharacters[j];			
				const GlyphData* glyphData = charQuad.mGlyphData;

				if(glyphData)
				{
					pos.x	= (float)(offsetX + charQuad.mLeft * fontString.mScaleX);
					pos.y	= (float)(offsetY + charQuad.mTop * fontString.mScaleY);		

					size.x	= (float)(charQuad.mRight - charQuad.mLeft) * fontString.mScaleX;
					size.y	= (float)(charQuad.mBottom - charQuad.mTop) * fontString.mScaleY;

					uv1.x	= (float)glyphData->mX / fontWidth;
					uv1.y	= (float)glyphData->mY / fontHeight;

					uv2.x	= uv1.x + ((float)glyphData->mWidth / fontWidth);
					uv2.y	= uv1.y + ((float)glyphData->mHeight / fontHeight);

					float skewOffset = skewMultiplier * glyphData->mTop;
					float bottomSkewOffset = skewMultiplier * ((int)glyphData->mTop - (int)glyphData->mHeight);

					//draw a drop shadow behind the text first, if specified
					if (dropShadow)
					{
						VectorMath::Vector2 dropShadowPos = pos;
						dropShadowPos.x += dropShadow;
						dropShadowPos.y += dropShadow;
						PopulateVerts(verts, dropShadowPos, x + dropShadow, y + dropShadow, size, center, uv1, uv2, 0xFF000000, skewOffset, bottomSkewOffset, sin_a, cos_a);

						pGraphics->DrawPrimitives(mTextures[glyphData->mAtlasIndex], Otter::kPrim_TriangleList, 2, verts, 0, false, maskID);
					}

					PopulateVerts(verts, pos, x, y, size, center, uv1, uv2, color, skewOffset, bottomSkewOffset, sin_a, cos_a);

					pGraphics->DrawPrimitives(mTextures[glyphData->mAtlasIndex], Otter::kPrim_TriangleList, 2, verts, 0, false, maskID);
				}
			}
		}
	}

	void Font::PopulateVerts(GUIVertex* verts, const VectorMath::Vector2& pos, float x, float y, const VectorMath::Vector2& size, const VectorMath::Vector2& center, const VectorMath::Vector2& uv1, const VectorMath::Vector2& uv2, uint32 color, float skewOffset, float bottomSkewOffset, float sin_a, float cos_a)
	{
		// Top Left
		verts[0].mPosition.x = pos.x + skewOffset;
		verts[0].mPosition.y = pos.y;
		verts[0].mPosition.z = 0.0f;
		verts[0].mTexCoord.x = uv1.x;
		verts[0].mTexCoord.y = uv1.y;
		verts[0].mColor = color;

		// Top Right
		verts[1].mPosition.x = pos.x + size.x + skewOffset;
		verts[1].mPosition.y = pos.y;
		verts[1].mPosition.z = 0.0f;
		verts[1].mTexCoord.x = uv2.x;
		verts[1].mTexCoord.y = uv1.y;
		verts[1].mColor = color;

		// Bottom Left
		verts[2].mPosition.x = pos.x + bottomSkewOffset;
		verts[2].mPosition.y = pos.y + size.y;
		verts[2].mPosition.z = 0.0f;
		verts[2].mTexCoord.x = uv1.x;
		verts[2].mTexCoord.y = uv2.y;
		verts[2].mColor = color;

		// Bottom Left
		verts[3].mPosition.x = pos.x + bottomSkewOffset;
		verts[3].mPosition.y = pos.y + size.y;
		verts[3].mPosition.z = 0.0f;
		verts[3].mTexCoord.x = uv1.x;
		verts[3].mTexCoord.y = uv2.y;
		verts[3].mColor = color;

		// Top Right
		verts[4].mPosition.x = pos.x + size.x + skewOffset;
		verts[4].mPosition.y = pos.y;
		verts[4].mPosition.z = 0.0f;
		verts[4].mTexCoord.x = uv2.x;
		verts[4].mTexCoord.y = uv1.y;
		verts[4].mColor = color;

		// Bottom Right
		verts[5].mPosition.x = pos.x + size.x + bottomSkewOffset;
		verts[5].mPosition.y = pos.y + size.y;
		verts[5].mPosition.z = 0.0f;
		verts[5].mTexCoord.x = uv2.x;
		verts[5].mTexCoord.y = uv2.y;
		verts[5].mColor = color;					

		for (int i = 0; i < 6; i++)
		{
			float a = verts[i].mPosition.x - (x + center.x);
			float b = verts[i].mPosition.y - (y + center.y);

			// Note, by NOT adding center back in, we're appropriately offsetting the verts.
			verts[i].mPosition.x = a * cos_a - b * sin_a + x;
			verts[i].mPosition.y = a * sin_a + b * cos_a + y;
		}
	}
}
