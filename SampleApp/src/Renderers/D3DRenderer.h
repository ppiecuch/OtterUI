#pragma once

#include <map>
#include <vector>
#include <d3dx9.h>
#include "SampleRenderer.h"

/* Direct3D Renderer implementation
 */
class D3DRenderer : public SampleRenderer
{
public:
	/* Constructor
	*/
	D3DRenderer(HWND hWnd, int width, int height, bool fullscreen);

	/* Virtual Destructor
	*/
	virtual ~D3DRenderer(void);

public:

	/* Loads a texture with the specified id and path
	 */
	virtual void OnLoadTexture(int textureID, const char* szPath);

	/* Unloads a texture with the specified id
	 */
	virtual void OnUnloadTexture(int textureID);

	/* Called when a drawing pass has begun
	 */
	virtual void OnDrawBegin();

	/* Called when a drawing pass has ended
	 */
	virtual void OnDrawEnd();

	/* Commits a vertex buffer for rendering
	 */
	virtual void OnCommitVertexBuffer(const Otter::GUIVertex* pVertices, uint32 numVertices);

	/* Draws primitives on screen
	 */
	virtual void OnDrawBatch(const Otter::DrawBatch& batch);
	
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
	
	LPDIRECT3D9							mD3D;
	LPDIRECT3DDEVICE9					mD3DDevice;
	D3DPRESENT_PARAMETERS				mD3DPresentParams;	

	LPD3DXEFFECT						mEffect;
	LPDIRECT3DVERTEXBUFFER9 			mVertexBuffer;
	LPDIRECT3DVERTEXDECLARATION9		mVertexDeclaration;

	bool								mFullscreen;

	std::map<int, LPDIRECT3DTEXTURE9>	mTextures;

	D3DXMATRIX							mModel;
	D3DXMATRIX							mView;
	D3DXMATRIX							mProjection;
};
