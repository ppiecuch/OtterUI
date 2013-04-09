#include "D3DRenderer.h"

#include "Shaders/OtterShader.h"

#define NUM_VERTS		6000
#define NUM_INDICES		NUM_VERTS

/* Constructor
 */
D3DRenderer::D3DRenderer(HWND hWnd, int width, int height, bool fullscreen)
{	
	mD3D = NULL;
	mD3DDevice = NULL;

	mVertexBuffer = NULL;
	mEffect = NULL;
	mVertexDeclaration = NULL;

	mWidth = width;
	mHeight = height;
	mFullscreen = fullscreen;

	InitD3D(hWnd);
	CreateBuffers();
	CreateShaders();
}

/* Virtual Destructor
 */
D3DRenderer::~D3DRenderer(void)
{
	std::map<int, LPDIRECT3DTEXTURE9>::iterator it = mTextures.begin();
	for(; it != mTextures.end(); it++)
	{
		if(it->second)
			it->second->Release();
	}

	mTextures.clear();

	if(mEffect)
	{
		mEffect->Release();
		mEffect = NULL;
	}

	if(mVertexBuffer)
	{
		mVertexBuffer->Release();
		mVertexBuffer = NULL;
	}

	if( mD3DDevice != NULL) 
	{
        mD3DDevice->Release();
		mD3DDevice = NULL;
	}

    if( mD3D != NULL)
	{
        mD3D->Release();
		mD3D = NULL;
	}
}

/* Creates the necessary D3D objects
 */
void D3DRenderer::InitD3D(HWND hWnd)
{
	mD3D = Direct3DCreate9(D3D_SDK_VERSION);

    // Create the D3D object, which is needed to create the D3DDevice.
    if(mD3D == NULL)
	{
		// TODO : Error reporting
        return;
	}

	GetPresentParams(mD3DPresentParams);    

	HRESULT hr = mD3D->CreateDevice( 
										D3DADAPTER_DEFAULT, 
										D3DDEVTYPE_HAL, 
										hWnd,
										D3DCREATE_HARDWARE_VERTEXPROCESSING,
										&mD3DPresentParams, 
										&mD3DDevice 
									 );

    if(FAILED(hr))
    {
		// TODO : Error reporting
		return;
    }
}

/* Retrieves the D3D Present Parameters
 */
void D3DRenderer::GetPresentParams(D3DPRESENT_PARAMETERS &d3dpp)
{
	ZeroMemory( &mD3DPresentParams, sizeof(D3DPRESENT_PARAMETERS) );

	mD3DPresentParams.BackBufferWidth	= mWidth;
	mD3DPresentParams.BackBufferHeight	= mHeight;
	mD3DPresentParams.BackBufferFormat	= D3DFMT_X8R8G8B8;
	mD3DPresentParams.BackBufferCount	= 1;

    mD3DPresentParams.Windowed					= !mFullscreen;
    mD3DPresentParams.MultiSampleType			= D3DMULTISAMPLE_NONE; 
	mD3DPresentParams.SwapEffect				= D3DSWAPEFFECT_DISCARD;
	mD3DPresentParams.EnableAutoDepthStencil	= TRUE;
	mD3DPresentParams.AutoDepthStencilFormat	= D3DFMT_D24S8;

	// Set the refresh rates and presentation intervals accordingly
	mD3DPresentParams.FullScreen_RefreshRateInHz	= mD3DPresentParams.Windowed ? 0 : D3DPRESENT_RATE_DEFAULT;	
	mD3DPresentParams.PresentationInterval			= D3DPRESENT_INTERVAL_DEFAULT;
}

/* Creates the necessary index and vertex buffers
 */
void D3DRenderer::CreateBuffers()
{
	HRESULT result;

	result = mD3DDevice->CreateVertexBuffer	(
												NUM_VERTS * sizeof(Otter::GUIVertex), 
												D3DUSAGE_WRITEONLY | D3DUSAGE_DYNAMIC, 
												0, 
												D3DPOOL_DEFAULT, 
												&mVertexBuffer, 
												NULL
											);

	if(FAILED(result))
	{
		// TODO : Error handling
		return;
	}
}

/* Creates the shaders
 */
void D3DRenderer::CreateShaders()
{
	HRESULT hr;

	D3DVERTEXELEMENT9 dwDecl3[] =
    {
        { 0, 0,  D3DDECLTYPE_FLOAT3,   		D3DDECLMETHOD_DEFAULT, 		D3DDECLUSAGE_POSITION,	0 },
        { 0, 12, D3DDECLTYPE_FLOAT2,   		D3DDECLMETHOD_DEFAULT, 		D3DDECLUSAGE_TEXCOORD,	0 },
        { 0, 20, D3DDECLTYPE_D3DCOLOR,   	D3DDECLMETHOD_DEFAULT, 		D3DDECLUSAGE_COLOR,		0 },
        D3DDECL_END()
    };  

    hr = mD3DDevice->CreateVertexDeclaration(dwDecl3, &mVertexDeclaration);  

	if( FAILED(hr) )
	{
		// TODO : Error reporting
	}


	LPD3DXBUFFER pBufferErrors = NULL;

	hr = D3DXCreateEffect(	mD3DDevice,
							g_OtterShader,
							sizeof(g_OtterShader),
							NULL, 
							NULL,
							D3DXSHADER_DEBUG,
							NULL,
							&mEffect,
							&pBufferErrors);

	if( FAILED(hr) )
	{
		const char* errorString = (const char*)pBufferErrors->GetBufferPointer();	

		// TODO: Error reporting

		pBufferErrors->Release();
		pBufferErrors = NULL;
	}
}

/* Loads a texture with the specified id and path
 */
void D3DRenderer::OnLoadTexture(int textureID, const char* szPath)
{
	if(mTextures[textureID] != NULL)
		return;

	char szFullPath[1024];
	sprintf_s(szFullPath, 1024, "Data/Win32/%s", szPath);
		
    size_t convertedChars = 0;
    wchar_t wpath[1024];
	mbstowcs_s(&convertedChars, wpath, strlen(szFullPath) + 1, szFullPath, _TRUNCATE);

	LPDIRECT3DTEXTURE9 texture;
	HRESULT result = D3DXCreateTextureFromFileEx(
													mD3DDevice,
													wpath,
													D3DX_DEFAULT,
													D3DX_DEFAULT,
													D3DX_DEFAULT,
													0,
													D3DFMT_UNKNOWN,
													D3DPOOL_DEFAULT,
													D3DX_FILTER_TRIANGLE,
													D3DX_FILTER_TRIANGLE,
													0,
													NULL,
													NULL,
													&texture
												);

	if( SUCCEEDED(result) )
	{
		mTextures[textureID] = texture;
	}
}

/* Unloads a texture with the specified id
 */
void D3DRenderer::OnUnloadTexture(int textureID)
{
	LPDIRECT3DTEXTURE9 texture = mTextures[textureID];
	if(texture != NULL)
	{
		texture->Release();
		mTextures[textureID] = NULL;
	}
}

int numBatches = 0;

/* Called when a drawing pass has begun
 */
void D3DRenderer::OnDrawBegin()
{	
	D3DXMatrixOrthoOffCenterLH(&mProjection, 0.0f, (float)mWidth, (float)mHeight, 0.0f, 0.1f, 10000.0f); 

	D3DXVECTOR3 eye(0.0f, 0.0f, -1000.0f);
	D3DXVECTOR3 at(0.0f, 0.0f, 0.0f);
	D3DXVECTOR3 up(0.0f, 1.0f, 0.0f);
	D3DXMatrixLookAtLH(&mView, &eye, &at, &up);

	mD3DDevice->Clear( 0, NULL, D3DCLEAR_ZBUFFER | D3DCLEAR_TARGET, D3DCOLOR_XRGB(0, 0, 0), 1.0f, 0 );
	mD3DDevice->BeginScene();

	mD3DDevice->SetVertexDeclaration(mVertexDeclaration);
	mD3DDevice->SetStreamSource(0, mVertexBuffer, 0, sizeof(Otter::GUIVertex));

	mEffect->SetMatrix("MATRIX_VIEW", &mView);
	mEffect->SetMatrix("MATRIX_PROJECTION", &mProjection);	

	numBatches = 0;
}

/* Called when a drawing pass has ended
 */
void D3DRenderer::OnDrawEnd()
{
	mD3DDevice->EndScene();
	mD3DDevice->Present( NULL, NULL, NULL, NULL );
}

/* Commits a vertex buffer for rendering
 */
void D3DRenderer::OnCommitVertexBuffer(const Otter::GUIVertex* pVertices, uint32 numVertices)
{
	void* pData = NULL;
	mVertexBuffer->Lock(0, numVertices * sizeof(Otter::GUIVertex), &pData, D3DLOCK_DISCARD);
	memcpy(pData, pVertices, numVertices * sizeof(Otter::GUIVertex));
	mVertexBuffer->Unlock();
}

/* Draws primitives on screen
 */
void D3DRenderer::OnDrawBatch(const Otter::DrawBatch& batch)
{
	numBatches++;
	mModel = *(D3DXMATRIX*)&batch.mTransform;

	LPDIRECT3DTEXTURE9 texture = mTextures[batch.mTextureID];
	mEffect->SetTechnique(texture != NULL ? "WithTexture" : "WithoutTexture");
	mEffect->SetTexture("DIFFUSE_TEXTURE", texture);
	mEffect->SetMatrix("MATRIX_MODEL", &mModel);

	uint32 passes = 0;
	mEffect->Begin(&passes, D3DXFX_DONOTSAVESAMPLERSTATE);
	{
		for(uint32 pass = 0; pass < passes; pass++)
		{
			mEffect->BeginPass(pass);

			D3DPRIMITIVETYPE primType = D3DPT_TRIANGLELIST;
			switch(batch.mPrimitiveType)
			{
			case Otter::kPrim_TriangleFan:
				{
					primType = D3DPT_TRIANGLEFAN;
					break;
				}
			case Otter::kPrim_TriangleStrip:
				{
					primType = D3DPT_TRIANGLESTRIP;
					break;
				}
			}

			mD3DDevice->DrawPrimitive(primType, batch.mVertexStartIndex, batch.mPrimitiveCount);

			mEffect->EndPass();
		}
	}
	mEffect->End();
}

void D3DRenderer::SetStencilState(StencilState state)
{
	switch (state)
	{
	case DRAW_TO_STENCIL:
		//clear the stencil of any other masks that have been previously rendered
		mD3DDevice->Clear( 0, NULL, D3DCLEAR_STENCIL, 0, 0, 0 );

		//enable stencil writing, disable color writing
		mD3DDevice->SetRenderState(D3DRS_STENCILENABLE, TRUE);
		mD3DDevice->SetRenderState(D3DRS_COLORWRITEENABLE, 0);

		//set the stencil function to always write 1 to the stencil buffer
		mD3DDevice->SetRenderState(D3DRS_STENCILFUNC, D3DCMP_ALWAYS);
		mD3DDevice->SetRenderState(D3DRS_STENCILPASS, D3DSTENCILOP_REPLACE);
		mD3DDevice->SetRenderState(D3DRS_STENCILREF, 1);
		break;
	case DRAW_USING_STENCIL:
		//re-enable color writing
		mD3DDevice->SetRenderState(D3DRS_COLORWRITEENABLE, 0xFFFFFFFF);

		//set the stencil function to only draw to pixels with 1 in the stencil buffer
		mD3DDevice->SetRenderState(D3DRS_STENCILFUNC, D3DCMP_NOTEQUAL);
		mD3DDevice->SetRenderState(D3DRS_STENCILREF, 0);
		mD3DDevice->SetRenderState(D3DRS_STENCILPASS, D3DSTENCILOP_KEEP);
		break;
	case DISABLE_STENCIL:
		//disable stencil writing
		mD3DDevice->SetRenderState(D3DRS_STENCILENABLE, FALSE);
		mD3DDevice->SetRenderState(D3DRS_COLORWRITEENABLE, 0xFFFFFFFF);
		break;
	}
}