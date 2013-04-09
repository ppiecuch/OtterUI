#include "stdafx.h"
#include <atlstr.h>

#include <ft2build.h>
#include FT_FREETYPE_H

using namespace System::IO;
using namespace System::Runtime::InteropServices;
using namespace System::Collections;
using namespace System::Collections::Generic;
using namespace System::Drawing;
using namespace System::ComponentModel;

namespace FontBuilder
{
	public ref struct Glyph
	{
		System::UInt32 mCharCode;
		System::UInt32 mImageGlyph;

		int mW;
		int mH;

		int mTop;
		int mAdvance;
		int mLeftBearing;

		Bitmap^ mBitmap;

		Glyph()
		{
			mImageGlyph = 0;
			mCharCode = 0;
			mW = 0;
			mH = 0;
			mTop = 0;
			mAdvance = 0;
			mLeftBearing = 0;
			mBitmap = nullptr;
		}
	};

	public ref class ImageGlyph
	{
	private:

		System::String^ mID;
		System::String^ mImagePath;
		Image^ mImage;

	public:
		/// <summary>
        /// Gets / sets the glyph's FourCC ID
        /// </summary>
		property System::String^ ID
		{
			System::String^ get()
			{
				return mID;
			}
			void set(System::String^ value)
			{
				mID = value;
				
				if(mID->Length > 4)
					mID = mID->Substring(0, 4);
			}
		}

		/// <summary>
        /// Gets / sets the image glyph path
        /// </summary>
		[ReadOnly(true)]
		property System::String^ ImagePath
		{
			System::String^ get()
			{
				return mImagePath;
			}
			void set(System::String^ value)
			{
				mImagePath = value;
			}
		}

		/// <summary>
        /// Gets the image if loaded
        /// </summary>
		[Browsable(false)]
		property System::Drawing::Image^ Image
		{
			System::Drawing::Image^ get()
			{
				return mImage;
			}
		}		

		/// <summary>
        /// Constructor
        /// </summary>
		ImageGlyph()
		{
			mID = "";
		}

		/// <summary>
		/// Loads the image.  If basePath is specified, prepends it
		/// to the image path to load the image.
		/// </summary>
		void Load(System::String^ basePath)
		{
			System::String^ path = mImagePath;

			if(basePath)
				path = basePath + "/" + path;

			if(System::IO::File::Exists(path))
				mImage = System::Drawing::Image::FromFile(path);
			else
				mImage = nullptr;
		}

		/// <summary>
		/// Unloads the image
		/// <summary>
		void Unload()
		{
			mImage = nullptr;
		}
	};

	/* Maintains font data.
	 * Loads a font from disk using FreeType
	 * and converts it into a format that we can use.
	 */
	public ref class FontData
	{
	public:

		bool mResizeImageGlyphs;

		List<System::UInt32> ^ mCharacters;
		List<ImageGlyph^> ^ mImageGlyphs;

	public:
		FontData(List<System::Char> ^ characters, List<ImageGlyph^> ^ imageGlyphs)
		{
			mResizeImageGlyphs = true;

			if(imageGlyphs)
				mImageGlyphs = gcnew List<ImageGlyph^>(imageGlyphs);
			else
				mImageGlyphs = gcnew List<ImageGlyph^>();

			mCharacters = gcnew List<System::UInt32>();

			int count = characters->Count;
			for(int i = 0; i < count; i++)
				mCharacters->Add((System::UInt32)characters[i]);
		}

		~FontData()
		{
		}

	public:

		// Retrieves an array of glyphs that will comprise this font
		array<Glyph^> ^ GetGlyphs(System::String ^ fontFile, int fontSize)
		{
			const char* szFontFile = (const char*)(void*)Marshal::StringToHGlobalAnsi(fontFile);
			printf(szFontFile);

			FT_Library	library;
			FT_Face		face;

			// Initialize the FreeType library
			int error = FT_Init_FreeType(&library);
			if(error)
			{
				Marshal::FreeHGlobal((::System::IntPtr)(void*)szFontFile);
				return nullptr;
			}
			
			FT_New_Face(library, szFontFile, 0, &face);
			
			error = FT_Set_Pixel_Sizes(face, 0, fontSize);
			if(error)
			{
				Marshal::FreeHGlobal((::System::IntPtr)(void*)szFontFile);
				return nullptr;
			}

			array<Glyph^> ^ glyphs = gcnew array<Glyph^>(mCharacters->Count + mImageGlyphs->Count);

			// Iterate over each character, create a new texture, and draw it.
			FT_GlyphSlot	glyphSlot	= face->glyph;
			int numChars = mCharacters->Count;
			int maxGlyphHeight = 0;
			for ( int n = 0; n < numChars; n++ )
			{
				System::UInt32 charCode = (System::UInt32)mCharacters[n];
				error = FT_Load_Char( face, charCode, FT_LOAD_RENDER );
				if ( error )
					continue;  // ignore errors 
				
				glyphs[n] = gcnew Glyph();
				if(glyphSlot->bitmap.width != 0 && glyphSlot->bitmap.rows != 0)
				{
					glyphs[n]->mBitmap = gcnew System::Drawing::Bitmap(glyphSlot->bitmap.width, glyphSlot->bitmap.rows);
				}

				// Ok, we got the character.  Now we need to record it's position and stuff.
				glyphs[n]->mCharCode		= charCode;
				glyphs[n]->mImageGlyph		= 0;
				glyphs[n]->mW 				= glyphSlot->bitmap.width;
				glyphs[n]->mH 				= glyphSlot->bitmap.rows;
				glyphs[n]->mTop				= glyphSlot->bitmap_top;
				glyphs[n]->mAdvance			= (glyphSlot->advance.x >> 6);
				glyphs[n]->mLeftBearing		= glyphSlot->bitmap_left;

				if(glyphs[n]->mBitmap)
				{
					// now, draw to our target surface 
					DrawToTexture(&glyphSlot->bitmap, glyphs[n]->mBitmap);
				}

				if(maxGlyphHeight < glyphSlot->bitmap.rows)
					maxGlyphHeight = glyphSlot->bitmap.rows;
			}

			// Cleanup
			FT_Done_Face    ( face );
			FT_Done_FreeType( library );
			Marshal::FreeHGlobal((System::IntPtr)(void*)szFontFile);

			// Now add in the image glyphs
			int numGlyphs = mImageGlyphs->Count;
			for(int n = 0; n < numGlyphs; n++)
			{
				ImageGlyph^ imageGlyph = mImageGlyphs[n];
				Image^ image = imageGlyph->Image;

				if(!image)
					continue;

				Bitmap^ bmp = nullptr;

				if(mResizeImageGlyphs && image->Height > maxGlyphHeight)
				{
					bmp = gcnew System::Drawing::Bitmap(image, Size((int)(image->Width * (maxGlyphHeight / (float)image->Height)), maxGlyphHeight));
				}
				else
				{
					bmp = gcnew System::Drawing::Bitmap(image);
				}
				
				if(bmp->Height == 0 || bmp->Width == 0)
					continue;

				// Prepare the fourcc identifier for the glyph
				System::UInt32 fourCC = 0;
				int shift = 24;
				for(int i = 0; i < 4; i++)
				{
					if(i < imageGlyph->ID->Length)
						fourCC |= imageGlyph->ID[i] << shift;

					shift -= 8;
				}

				int index = mCharacters->Count + n;
				glyphs[index] = gcnew Glyph();
				
				glyphs[index]->mCharCode		= fourCC;
				glyphs[index]->mImageGlyph		= 1;
				glyphs[index]->mW 				= bmp->Width;
				glyphs[index]->mH 				= bmp->Height;
				glyphs[index]->mTop				= bmp->Height;
				glyphs[index]->mAdvance			= bmp->Width + 5;
				glyphs[index]->mLeftBearing		= 0;
				glyphs[index]->mBitmap			= bmp;
			}

			return glyphs;
		}

		void DrawToTexture(FT_Bitmap* ft_bitmap, Bitmap^ bmp)
		{
			if(ft_bitmap->width == 0 || ft_bitmap->rows == 0)
				return;

			// Copy the bitmap bits from the source bitmap
			// onto the target bitmap surface.  Essentially copying to the texture.
			for( int by = 0; by < ft_bitmap->rows; by++ )
			{
				for( int bx = 0; bx < ft_bitmap->width; bx++ )
				{
					UINT8 alpha = (UINT8)(ft_bitmap->buffer[ft_bitmap->width * by + bx] & 0xff);				
					bmp->SetPixel(bx, by, System::Drawing::Color::FromArgb(alpha, 255, 255, 255));
				}
			}
		}
	};
}