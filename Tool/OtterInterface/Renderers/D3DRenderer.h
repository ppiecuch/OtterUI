#pragma once

#include <map>
#include <vector>
#include <deque>
#include <map>
#include <d3dx9.h>
#include <string>

#include "BaseRenderer.h"
#include "TextureManager.h"

struct Shader
{
	struct Parameter
	{
		std::string mName;
		D3DXHANDLE mHandle;
	};

	std::string				mName;
	std::string				mSource;
	LPD3DXEFFECT			mEffect;
	std::vector<Parameter>	mParameters;

	Shader()
	{
		mName = "";
		mSource = "";
		mEffect = NULL;
	}

	int GetParameterID(const char* szName);

	D3DXHANDLE GetParameterHandle(int id);
};

struct SwapChain
{
	IDirect3DSwapChain9*	mD3DSwapChain;
	int						mWidth;
	int						mHeight;

	long					mWindow;
};

typedef std::map<long, SwapChain> SwapChainMap;
typedef std::map<int, Shader> ShaderMap;

/* Direct3D Renderer implementation
 */
class D3DRenderer : public BaseRenderer
{
public:
	/* Constructor
	*/
	D3DRenderer(HWND hWnd);

	/* Virtual Destructor
	*/
	virtual ~D3DRenderer(void);

public:
	
	/* Creates a new drawing context of the specified width and height
		*/
	long CreateContext(long window, int width, int height, long withID = 0);

	/* Destroys a context
		*/
	void DestroyContext(long context);

	/* Sets the rendering context
	 */
	virtual void SetContext(long context);

public:

	/* Loads a shader
	 */
	virtual int LoadShader(const char* szName, const char* szShader);
	
	/* Sets the current shader
	 */
	void SetShader(int shaderID, bool force);
	
	/* Retrieves the shader parameter
	 */
	virtual int GetShaderParameter(int shaderID, const char* szName);

public:

	/* Called when a drawing pass has begun
	 */
	virtual void OnDrawBegin();

	/* Finalizes drawing.
	 */
	virtual void OnDrawEnd();
	
	/* Commits a vertex buffer for rendering
	 */
	void OnCommitVertexBuffer(const Otter::GUIVertex* pVertices, uint32 numVertices);

	/* Draws primitives on screen
	 */
	void OnDrawBatch(const Otter::DrawBatch& batch);

	/* Sets an orthographic projection transform
	 */
	virtual void SetOrtho(float left, float right, float bottom, float top, float znear, float zfar);

	/* Sets the view to  a left-handed view transform
	 */
	virtual void SetLookAtLH(	float px, float py, float pz,
								float tx, float ty, float tz,
								float ux, float uy, float uz);
	
	/* Sets the viewport
	 */
	virtual void SetViewport(float x, float y, float w, float h);

	/* Unprojects the screen-space coordinate into object-space
	 */
	virtual void Unproject(float& x, float& y, float& z);

public:

	/* Loads a texture and returns the ID
	 */
	virtual int LoadTexture(const char* szPath, int width = 0, int height = 0);

	/* Loads a texture and returns the ID
	 */
	virtual int LoadTexture(const unsigned char* buffer, int bufferLength, int width, int height, int bitdepth);

	/* Unloads a texture with the specified id
	 */
	virtual void UnloadTexture(int textureID);	

	/* Retrieves the texture information
	 */
	virtual bool GetTextureInfo(int id, int& width, int& height);

	/* Retrieves the number of loaded textures
	 */
	virtual int NumTextures();

	/* Retrieves the texture ID by index
	 */
	virtual int GetTextureID(int index);

	/* Returns whether or not the device was lost in the last 
	 * frame
	 */
	virtual bool WasDeviceLost() { return mDeviceLost; }
	
	/**
	 * Sets the renderer to draw to the stencil buffer
	 */
	virtual void SetStencilState(StencilState state);

private:
	
	/* Creates the necessary D3D objects
	 */
	void InitD3D(HWND hWnd);

	/* Retrieves the D3D Present Parameters
	 */
	void GetPresentParams(D3DPRESENT_PARAMETERS &d3dpp);

	/* Creates the index / vertex buffers
	 */
	void CreateBuffers();

	/* Creates the shaders
	 */
	void CreateShaders();

private:
	
	LPDIRECT3D9						mD3D;
	LPDIRECT3DDEVICE9				mD3DDevice;

	LPDIRECT3DVERTEXBUFFER9 		mVertexBuffer;
	LPDIRECT3DVERTEXDECLARATION9	mVertexDeclaration;

	D3DXMATRIX						mModel;
	D3DXMATRIX						mView;
	D3DXMATRIX						mProjection;

	TextureManager					mTextureManager;

	int								mNextSwapChainID;
	SwapChainMap					mSwapChains;

	int								mCurrentShaderID;
	int								mNextShaderID;
	ShaderMap						mShaders;

	bool							mDeviceLost;
};