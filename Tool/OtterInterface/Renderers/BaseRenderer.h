#pragma once

#include "Interfaces/IRenderer.h"

enum Property
{
	SHADER_SET,
	SHADER_TECHNIQUE,

	SHADER_PARAM_COUNT,
	
	SHADER_PARAM_TYPE = 10000,
	SHADER_PARAM_ID = 20000,
	SHADER_PARAM_DATA = 30000,
	SHADER_PARAM_DATA_LEN = 40000
};

/* Sample Renderer base class.
 */
class BaseRenderer : public Otter::IRenderer
{
public:

	/* Constructor
	*/
	BaseRenderer();

	/* Virtual Destructor
	*/
	virtual ~BaseRenderer(void);

public:

	/* Sets the bounds for the renderer.  Draws anything outside of the bounds
	 * modulated with the color
	 */
	void SetBounds(int width, int height, int color)
	{
		mBoundsWidth = width;
		mBoundsHeight = height;
		mBoundsColor = color;
	}

public:

	/* Creates a new drawing context of the specified width and height
		*/
	virtual long CreateContext(long window, int width, int height, long withID = 0) { return 0; }

	/* Destroys a context
		*/
	virtual void DestroyContext(long context) { }

	/* Sets the rendering context
	 */
	virtual void SetContext(long context) { mContext = context; }

public:

	/* Loads a shader
	 */
	virtual int LoadShader(const char* szName, const char* szShader) { return 0; }
	
	/* Sets the current shader
	 */
	virtual void SetShader(int shaderID, bool force) { }
	
	/* Retrieves the shader parameter's id
	 */
	virtual int GetShaderParameter(int shaderID, const char* szName) { return 0; }

public:

	/* Loads a texture and returns the ID
	 */
	virtual int LoadTexture(const char* szPath, int width = 0, int height = 0) = 0;

	/* Loads a texture and returns the ID
	 */
	virtual int LoadTexture(const unsigned char* buffer, int bufferLength, int width, int height, int bitdepth) = 0;

	/* Unloads a texture
	 */
	virtual void UnloadTexture(int textureID) = 0;

	/* Retrieves the number of loaded textures
	 */
	virtual int NumTextures() = 0;

	/* Retrieves the texture ID by index
	 */
	virtual int GetTextureID(int index) = 0;

	/* Retrieves the texture information
	 */
	virtual bool GetTextureInfo(int id, int& width, int& height) = 0;

	/* Sets an orthographic projection transform
	 */
	virtual void SetOrtho(float left, float right, float bottom, float top, float znear, float zfar) = 0;

	/* Sets the view to  a left-handed view transform
	 */
	virtual void SetLookAtLH(	float px, float py, float pz,
								float tx, float ty, float tz,
								float ux, float uy, float uz) = 0;
	
	/* Sets the viewport
	 */
	virtual void SetViewport(float x, float y, float w, float h) = 0;

	/* Unprojects the screen-space coordinate into object-space
	 */
	virtual void Unproject(float& x, float& y, float& z) = 0;

	/* Returns whether or not the device was lost in the last 
	 * frame
	 */
	virtual bool WasDeviceLost() = 0;
	
	/**
	 * Sets the renderer to draw to the stencil buffer
	 */
	virtual void SetStencilState(StencilState state) { }

protected:

	long mContext;

	int mBoundsWidth;
	int mBoundsHeight;
	int mBoundsColor;
};