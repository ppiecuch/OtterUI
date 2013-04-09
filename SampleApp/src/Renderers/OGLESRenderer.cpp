#if defined(PLATFORM_IPHONE)
#include <OpenGLES/ES1/gl.h>
#include <OpenGLES/ES1/glext.h>
#elif defined(PLATFORM_ANDROID)
#include <GLES/gl.h>
#include <GLES/glext.h>
#include <android/log.h>
#endif

#include <map>
#include "OGLESRenderer.h"
#include "libpng/png.h"


#define NUM_VERTS		4000
#define NUM_INDICES		NUM_VERTS

uint8* LoadPNG(FILE* pFile, uint32* pWidth, uint32* pHeight, uint32* pBPP, bool* pHasAlpha);
bool FullFilePath(const char* szRelativePath, char* szFullPath, uint32 len);

/* Constructor
 */
OGLESRenderer::OGLESRenderer(int width, int height)
{	
	mVertexBuffer = 0;	
	mNumCommittedVertices = 0;
	
	mWidth = width;
	mHeight = height;
	
	mTextures.clear();
	
	CreateBuffers();
}

/* Virtual Destructor
 */
OGLESRenderer::~OGLESRenderer(void)
{
	std::map<int, uint32>::iterator it = mTextures.begin();
	
	for(; it != mTextures.end(); it++)
	{
		glDeleteTextures(1, &(it->second));	
	}
	
	mTextures.clear();
	
	if(mVertexBuffer)
	{
		glDeleteBuffers(1, &mVertexBuffer);
		mVertexBuffer = 0;
	}
}

/* Sets the screen width/height
 */
void OGLESRenderer::SetResolution(int width, int height)
{
	mWidth = width;
	mHeight = height;
}

/* Creates the necessary index and vertex buffers
 */
void OGLESRenderer::CreateBuffers()
{
	Otter::GUIVertex* verts = new Otter::GUIVertex[NUM_VERTS];
	glGenBuffers(1, &mVertexBuffer);
	glBindBuffer(GL_ARRAY_BUFFER, mVertexBuffer);
	glBufferData(GL_ARRAY_BUFFER, NUM_VERTS * sizeof(Otter::GUIVertex), verts, GL_DYNAMIC_DRAW);
	glBindBuffer(GL_ARRAY_BUFFER, 0);	
	delete[] verts;
}

/* Loads a texture with the specified id and path
 */
void OGLESRenderer::OnLoadTexture(int textureID, const char* szPath)
{
	if(mTextures.find(textureID) != mTextures.end())
		return;
	
	char fullPath[256];
	FullFilePath(szPath, fullPath, 256);
	
	FILE* fp = fopen(fullPath, "rb");
	if(!fp)
	{
		return;
	}
	
	uint32 w, h, bpp;
	bool alpha;
	
	uint8* data = LoadPNG(fp, &w, &h, &bpp, &alpha);
	if(!data)
	{
		fclose(fp);
		return;
	}
	
	GLuint texID = 0;
	
	glGenTextures(1, &texID);
	
	glBindTexture(GL_TEXTURE_2D, texID);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_REPEAT);
	glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_REPEAT);
	
	// Default is RGBA, otherwise RGB.
	uint32 type = GL_RGBA;
	if (bpp == 24)
	{
		type = GL_RGB;
	}
	
	glTexImage2D(GL_TEXTURE_2D,
				 0,
				 type,
				 w,
				 h,
				 0,
				 type,
				 GL_UNSIGNED_BYTE,
				 data);

	mTextures[textureID] = (uint32)texID;
	
	delete[] data;
	fclose(fp);
}

/* Unloads a texture with the specified id
 */
void OGLESRenderer::OnUnloadTexture(int textureID)
{
	std::map<int, uint32>::iterator it = mTextures.find(textureID);
	if( it == mTextures.end())
		return;
		
	glDeleteTextures(1, &(it->second));	
	mTextures.erase(it);
}

/* Called when a drawing pass has begun
 */
void OGLESRenderer::OnDrawBegin()
{
	mNumCommittedVertices = 0;
	
	glClear(GL_DEPTH_BUFFER_BIT);
	
	glMatrixMode(GL_MODELVIEW);
	glPushMatrix();
	glLoadIdentity();
	
	glMatrixMode(GL_PROJECTION);
	glPushMatrix();
	glLoadIdentity();
	glOrthof(0.0f, mWidth, mHeight, 0.0f, -1000.0f, 1000.0f);
	
	// Alpha Blend State
	glEnable (GL_BLEND);
	glBlendFunc (GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
	glEnable (GL_ALPHA_TEST);
	glAlphaFunc (GL_GREATER, 0.05f);
	
	glDepthMask(GL_FALSE);
	glDisable(GL_DEPTH_TEST);
	
	glDisable(GL_CULL_FACE);
	
	glEnableClientState(GL_VERTEX_ARRAY);
	glEnableClientState(GL_TEXTURE_COORD_ARRAY);	
	glEnableClientState(GL_COLOR_ARRAY);
	
	glBindBuffer(GL_ARRAY_BUFFER, mVertexBuffer);
	
	glVertexPointer(3, GL_FLOAT, sizeof(Otter::GUIVertex), (void*)0);
	glTexCoordPointer(2, GL_FLOAT, sizeof(Otter::GUIVertex), (void*)12);
	glColorPointer(4, GL_UNSIGNED_BYTE, sizeof(Otter::GUIVertex), (void*)20);
	
	// Flip the textures.
	glMatrixMode(GL_TEXTURE);
	glPushMatrix();
	glLoadIdentity();
	glScalef(1.0f, -1.0f, 1.0f);
}

/* Called when a drawing pass has ended
 */
void OGLESRenderer::OnDrawEnd()
{	
	glBindBuffer(GL_ARRAY_BUFFER, 0);
	
	glDisableClientState(GL_VERTEX_ARRAY);
	glDisableClientState(GL_TEXTURE_COORD_ARRAY);
	glDisableClientState(GL_COLOR_ARRAY);
	glDisable(GL_TEXTURE_2D);
	
	glEnable(GL_DEPTH_TEST);
	glDepthMask(GL_TRUE);
	
	glDisable(GL_STENCIL_TEST);
	glColorMask(1, 1, 1, 1);
	
	// Disable Alpha Blend State
	glDisable (GL_BLEND);
	glDisable (GL_ALPHA_TEST);	
	
	glMatrixMode(GL_TEXTURE);
	glPopMatrix();
	
	glMatrixMode(GL_PROJECTION);
	glPopMatrix();
	
	glMatrixMode(GL_MODELVIEW);
	glPopMatrix();
}

/* Commits a vertex buffer for rendering
 */
void OGLESRenderer::OnCommitVertexBuffer(const Otter::GUIVertex* pVertices, uint32 numVertices)
{
	if(numVertices > NUM_VERTS)
		numVertices = NUM_VERTS;
	
	glBufferSubData(GL_ARRAY_BUFFER, 0, numVertices * sizeof(Otter::GUIVertex), (char*)pVertices);
}

/* Draws primitives on screen
 */
void OGLESRenderer::OnDrawBatch(const Otter::DrawBatch& batch)
{
	GLuint texId = mTextures[batch.mTextureID];
	if(texId > 0)
	{
		glEnable(GL_TEXTURE_2D);
		glBindTexture(GL_TEXTURE_2D, texId);
	}
	else
	{		
		glDisable(GL_TEXTURE_2D);
		glBindTexture(GL_TEXTURE_2D, 0);
	}
	
	GLenum primType = GL_TRIANGLES;
	uint32 numVerts = batch.mPrimitiveCount * 3;
	switch(batch.mPrimitiveType)
	{
		case Otter::kPrim_TriangleFan:
		{
			primType = GL_TRIANGLE_FAN;
			numVerts -= 2;
			break;
		}
		case Otter::kPrim_TriangleStrip:
		{
			primType = GL_TRIANGLE_STRIP;
			numVerts -= 2;
			break;
		}
	}
	
	glMatrixMode(GL_MODELVIEW);
	glPushMatrix();
	
	glLoadMatrixf((float*)&batch.mTransform);	
	
	glDrawArrays(primType, batch.mVertexStartIndex, numVerts);	
	
	glPopMatrix();
}

/**
 * Sets the renderer to draw to the stencil buffer
 */
void OGLESRenderer::SetStencilState(StencilState state)
{		
	switch (state)
	{
		case DRAW_TO_STENCIL:
		{
			glColorMask(0, 0, 0, 0);	
			glClear(GL_STENCIL_BUFFER_BIT);
			
			glEnable(GL_STENCIL_TEST);
			
			glStencilFunc(GL_ALWAYS, 1, 1);
			glStencilOp(GL_REPLACE, GL_REPLACE, GL_REPLACE);		
			
			break;
		}
		case DRAW_USING_STENCIL:
		{
			glColorMask(1, 1, 1, 1);	
			glEnable(GL_STENCIL_TEST);
						
			glStencilFunc(GL_EQUAL, 1, 1);
			glStencilOp(GL_KEEP, GL_KEEP, GL_KEEP);
			break;
		}
		case DISABLE_STENCIL:
		{
			//disable stencil writing			
			glDisable(GL_STENCIL_TEST);
			glColorMask(1, 1, 1, 1);
			break;
		}
	}
}

/* Loads a PNG from file.
 */
uint8* LoadPNG(FILE* pFile, uint32* pWidth, uint32* pHeight, uint32* pBPP, bool* pHasAlpha)
{
	if(!pFile)
		return false;
	
	png_structp	png_ptr;			// internally used by libpng
	png_infop	info_ptr;			// user requested transforms
	uint8		signature[8];		// PNG signature array
	
	// Check for the 8-byte signature
	fread(signature, 8, 1, pFile);
	
	if (!png_check_sig((uint8*) signature, 8))
		return false;
	
	// Set up the PNG structs
	png_ptr = png_create_read_struct(PNG_LIBPNG_VER_STRING, NULL, NULL, NULL);
	if (!png_ptr)
		return false;
	
	info_ptr = png_create_info_struct(png_ptr);
	if (!info_ptr)
	{
		png_destroy_read_struct(&png_ptr, (png_infopp) NULL, (png_infopp) NULL);
		return false;
	}
	
	// Block to handle libpng errors,
	// then check whether the PNG file had a bKGD chunk
	if (setjmp(png_jmpbuf(png_ptr)))
	{
		png_destroy_read_struct(&png_ptr, &info_ptr, NULL);
		return false;
	}
	
	// takes our file stream pointer (infile) and
	// stores it in the png_ptr struct for later use.
	// png_ptr->io_ptr = (png_voidp)infile;
	// png_init_io(png_ptr, infile);
	png_init_io(png_ptr, pFile);
	
	// Lets libpng know that we already checked the 8
	// signature bytes, so it should not expect to find
	// them at the current file pointer location
	png_set_sig_bytes(png_ptr, 8);
	
	// Read the image
	int png_transforms = PNG_TRANSFORM_STRIP_16 | PNG_TRANSFORM_EXPAND;
	png_read_png(png_ptr, info_ptr, png_transforms, png_voidp_NULL);
	
	// Set our data
	*pWidth = info_ptr->width;
	*pHeight = info_ptr->height;
	*pBPP = 32;
	
	// Get the stride from the source texture.
	// Assume we have alpha present (four bytes).
	int stride = 4; // Assume alpha present
	switch(info_ptr->color_type)
	{
		case PNG_COLOR_TYPE_RGB:
		{
			stride = 3;
			break;
		}
		case PNG_COLOR_TYPE_RGB_ALPHA:
		{
			stride = 4;
			break;
		}
		default:
		{
			printf("Unsupported Color Format!  Result undefined!\n");
			break;
		}
	}
	
	// Now copy the data over, pixel by pixel.  Slow, but it'll do for now.
	// Also - note that we flip the image.
	uint8* rgba_data = new uint8[*pWidth * (*pHeight) * 4];
	for(int row = 0; row < *pHeight; row++)
	{
		uint8* pRow = info_ptr->row_pointers[row];
		
		for(int pixel = 0; pixel < *pWidth; pixel++)
		{
			uint8 r = pRow[pixel * stride + 0];
			uint8 g = pRow[pixel * stride + 1];
			uint8 b = pRow[pixel * stride + 2];
			uint8 a = (stride == 3 ? 255 : pRow[pixel * stride + 3]);
			
			rgba_data[(*pHeight - 1 - row) * (*pWidth) * 4 + pixel * 4 + 0] = r;
			rgba_data[(*pHeight - 1 - row) * (*pWidth) * 4 + pixel * 4 + 1] = g;
			rgba_data[(*pHeight - 1 - row) * (*pWidth) * 4 + pixel * 4 + 2] = b;
			rgba_data[(*pHeight - 1 - row) * (*pWidth) * 4 + pixel * 4 + 3] = a;
			
			// If we find a single pixel that is not full opaque,
			// mark the texture is having alpha
			if(a != 255)
				*pHasAlpha = true;
		}
	}
	
	png_destroy_read_struct(&png_ptr, &info_ptr, NULL);
	return rgba_data;
}
