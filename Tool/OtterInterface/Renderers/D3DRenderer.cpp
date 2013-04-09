#include "StdAfx.h"
#include "D3DRenderer.h"
#include "Common/Types.h"
#include "Shaders/Otter.h"

#define NUM_VERTS (6000)

/* Constructor
 */
D3DRenderer::D3DRenderer(HWND hWnd)
{	
	mD3D = NULL;
	mD3DDevice = NULL;

	mVertexBuffer = NULL;
	mVertexDeclaration = NULL;
	
	InitD3D(hWnd);
	CreateBuffers();
	CreateShaders();

	D3DXMatrixIdentity(&mModel);
	D3DXMatrixIdentity(&mView);
	D3DXMatrixIdentity(&mProjection);

	mNextSwapChainID = 1000;
	mNextShaderID = 1000;
	mCurrentShaderID = 0;

	mDeviceLost = false;
}

/* Virtual Destructor
 */
D3DRenderer::~D3DRenderer(void)
{
	ShaderMap::iterator it = mShaders.begin();
	for(; it != mShaders.end(); it++)
	{
		(*it).second.mEffect->Release();
	}

	mShaders.clear();

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

	HRESULT hr;
	
	D3DPRESENT_PARAMETERS d3dpp;
	GetPresentParams(d3dpp);    

	D3DCAPS9 caps;
	hr = mD3D->GetDeviceCaps(D3DADAPTER_DEFAULT, D3DDEVTYPE_HAL, &caps);

	DWORD vertexProcessing = D3DCREATE_HARDWARE_VERTEXPROCESSING;
	D3DDEVTYPE devType = D3DDEVTYPE_HAL;

	if(caps.VertexProcessingCaps == 0)
	{
		vertexProcessing = D3DCREATE_SOFTWARE_VERTEXPROCESSING;
	}

	hr = mD3D->CreateDevice(	D3DADAPTER_DEFAULT, 
								devType, 
								hWnd,
								vertexProcessing,
								&d3dpp, 
								&mD3DDevice);

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
	ZeroMemory( &d3dpp, sizeof(D3DPRESENT_PARAMETERS) );

	d3dpp.BackBufferWidth				= 1024;
	d3dpp.BackBufferHeight				= 768;
	d3dpp.BackBufferFormat				= D3DFMT_X8R8G8B8;
	d3dpp.BackBufferCount				= 1;

	d3dpp.Windowed						= true;
	d3dpp.MultiSampleType				= D3DMULTISAMPLE_NONE; 
	d3dpp.SwapEffect					= D3DSWAPEFFECT_DISCARD;
	d3dpp.EnableAutoDepthStencil		= TRUE;
	d3dpp.AutoDepthStencilFormat		= D3DFMT_D24S8;

	// Set the refresh rates and presentation intervals accordingly
	d3dpp.FullScreen_RefreshRateInHz	= d3dpp.Windowed ? 0 : D3DPRESENT_RATE_DEFAULT;	
	d3dpp.PresentationInterval			= D3DPRESENT_INTERVAL_IMMEDIATE;

	D3DDISPLAYMODE mode;
	if(SUCCEEDED(mD3D->GetAdapterDisplayMode(D3DADAPTER_DEFAULT , &mode))) 
	{
		d3dpp.BackBufferWidth	= mode.Width;
		d3dpp.BackBufferHeight	= mode.Height;
		d3dpp.BackBufferFormat	= mode.Format;
	}
}

/* Creates a new drawing context of the specified width and height
 */
long D3DRenderer::CreateContext(long window, int width, int height, long withContext)
{
	IDirect3DSwapChain9* pSwapChain = NULL;

	D3DPRESENT_PARAMETERS d3dpp;
	ZeroMemory( &d3dpp, sizeof(D3DPRESENT_PARAMETERS) );

	d3dpp.BackBufferWidth				= width;
	d3dpp.BackBufferHeight				= height;
	d3dpp.BackBufferFormat				= D3DFMT_X8R8G8B8;
	d3dpp.BackBufferCount				= 1;

	d3dpp.Windowed						= true;
	d3dpp.MultiSampleType				= D3DMULTISAMPLE_NONE; 
	d3dpp.SwapEffect					= D3DSWAPEFFECT_COPY;
	d3dpp.EnableAutoDepthStencil		= TRUE;
	d3dpp.AutoDepthStencilFormat		= D3DFMT_D24S8;
	d3dpp.hDeviceWindow					= (HWND)window;

	// Set the refresh rates and presentation intervals accordingly
	d3dpp.FullScreen_RefreshRateInHz	= d3dpp.Windowed ? 0 : D3DPRESENT_RATE_DEFAULT;	
	d3dpp.PresentationInterval			= D3DPRESENT_INTERVAL_IMMEDIATE;

	HRESULT hr = mD3DDevice->CreateAdditionalSwapChain(&d3dpp, &pSwapChain);
	if(SUCCEEDED(hr))
	{
		if(withContext == 0)
			withContext = mNextSwapChainID++;

		SwapChain swapChain;
		swapChain.mD3DSwapChain = pSwapChain;
		swapChain.mWidth = width;
		swapChain.mHeight = height;
		swapChain.mWindow = window;

		mSwapChains[withContext] = swapChain;
		return withContext;
	}

	return 0;
}

/* Destroys a context
	*/
void D3DRenderer::DestroyContext(long context)
{
	if(mSwapChains.find(context) == mSwapChains.end())
		return;

	IDirect3DSwapChain9* pSwapChain = mSwapChains[context].mD3DSwapChain;
	if(!pSwapChain)
		return;

	pSwapChain->Release();
	mSwapChains.erase(context);

	if(context == mContext)
		mContext = 0;
}

/* Sets the rendering context
	*/
void D3DRenderer::SetContext(long context)
{
	if(context != mContext)
	{
		if(mSwapChains.find(context) != mSwapChains.end())
		{
			IDirect3DSwapChain9* pSwapChain = mSwapChains[context].mD3DSwapChain;
			if(pSwapChain)
			{
				LPDIRECT3DSURFACE9 pBack = NULL;
				HRESULT hr;

				hr = pSwapChain->GetBackBuffer(0, D3DBACKBUFFER_TYPE_MONO, &pBack);
				if(SUCCEEDED(hr))
				{
					hr = mD3DDevice->SetRenderTarget(0, pBack);
					pBack->Release();
				}
			}
		}

		mContext = context;
	}
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
	LPD3DXEFFECT pEffect = NULL;

	hr = D3DXCreateEffect(	mD3DDevice,
							g_Otter,
							sizeof(g_Otter),
							NULL, 
							NULL,
							D3DXSHADER_OPTIMIZATION_LEVEL3,
							NULL,
							&pEffect,
							&pBufferErrors);

	if( FAILED(hr) )
	{
		const char* errorString = (const char*)pBufferErrors->GetBufferPointer();	

		// TODO: Error reporting

		pBufferErrors->Release();
		pBufferErrors = NULL;
	}

	Shader shader;
	shader.mName = "Default";
	shader.mEffect = pEffect;

	mShaders[0] = shader;
}

int D3DRenderer::LoadShader(const char* szName, const char* szShader)
{
	std::string name = szName;
	ShaderMap::iterator it = mShaders.begin();
	for(; it != mShaders.end(); it++)
	{
		if((*it).second.mName == name)
			return (*it).first;
	}

	LPD3DXBUFFER pBufferErrors = NULL;
	Shader shader;
	shader.mName = name;
	shader.mSource = szShader;

	HRESULT hr = D3DXCreateEffect(	mD3DDevice,
									szShader,
									strlen(szShader) + 1,
									NULL, 
									NULL,
									D3DXSHADER_OPTIMIZATION_LEVEL3,
									NULL,
									&shader.mEffect,
									&pBufferErrors);

	if( FAILED(hr) )
	{
		const char* errorString = (const char*)pBufferErrors->GetBufferPointer();	

		// TODO: Error reporting

		pBufferErrors->Release();
		pBufferErrors = NULL;

		return 0;
	}

	mShaders[mNextShaderID] = shader;
	return mNextShaderID++;
}

void D3DRenderer::SetShader(int shaderID, bool force)
{
	if(!force && mCurrentShaderID == shaderID)
		return;

	mCurrentShaderID = shaderID;
	
	float col[4] = {((mBoundsColor & 0x00FF0000) >> 16) / 255.0f, 
					((mBoundsColor & 0x0000FF00) >> 8) / 255.0f, 
					((mBoundsColor & 0x000000FF)     ) / 255.0f,
					((mBoundsColor & 0xFF000000) >> 24) / 255.0f}; 
	
	D3DXMATRIX vp = (mView * mProjection);
	D3DXMATRIX vpi;
	float det;
	D3DXMatrixInverse(&vpi, &det, &vp);

	Shader& shader = mShaders[mCurrentShaderID];
	shader.mEffect->SetMatrix("MATRIX_VP", &vp);
	shader.mEffect->SetMatrix("MATRIX_VP_INV", &vpi);
	shader.mEffect->SetInt("BOUNDS_WIDTH", mBoundsWidth);
	shader.mEffect->SetInt("BOUNDS_HEIGHT", mBoundsHeight);
	shader.mEffect->SetFloatArray("BOUNDS_COLOR", col, 4);
}

int D3DRenderer::GetShaderParameter(int shaderID, const char* szName)
{
	if(mShaders.find(shaderID) == mShaders.end())
		return -1;

	Shader& shader = mShaders[shaderID];

	if(shader.mEffect == NULL)
		return -1;

	return shader.GetParameterID(szName);
}

/* Loads a texture and returns the ID
 */
int D3DRenderer::LoadTexture(const char* szPath, int width, int height)
{
	return mTextureManager.LoadTexture(mD3DDevice, szPath, width, height);
}

/* Loads a texture and returns the ID
 */
int D3DRenderer::LoadTexture(const unsigned char* buffer, int bufferLength, int width, int height, int bitdepth)
{
	return mTextureManager.LoadTexture(mD3DDevice, buffer, bufferLength, width, height, bitdepth);
}

/* Unloads a texture with the specified id
 */
void D3DRenderer::UnloadTexture(int textureID)
{
	mTextureManager.UnloadTexture(textureID);
}

/* Retrieves the texture information
 */
bool D3DRenderer::GetTextureInfo(int id, int& width, int& height)
{
	return mTextureManager.GetTextureInfo(id, width, height);
}

/* Retrieves the number of loaded textures
 */
int D3DRenderer::NumTextures()
{
	return mTextureManager.NumTextures();
}

/* Retrieves the texture ID by index
 */
int D3DRenderer::GetTextureID(int index)
{
	return mTextureManager.GetTextureID(index);
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

/* Called when a drawing pass has begun
 */
void D3DRenderer::OnDrawBegin()
{
	mD3DDevice->Clear( 0, NULL, D3DCLEAR_ZBUFFER | D3DCLEAR_TARGET, D3DCOLOR_XRGB(100, 100, 100), 1.0f, 0 );
	mD3DDevice->BeginScene();

	mD3DDevice->SetVertexDeclaration(mVertexDeclaration);
	mD3DDevice->SetStreamSource(0, mVertexBuffer, 0, sizeof(Otter::GUIVertex));

	SetShader(0, true);
}

/* Called when a drawing pass has ended
 */
void D3DRenderer::OnDrawEnd()
{
	mDeviceLost = false;

	mD3DDevice->SetSamplerState(0, D3DSAMP_MIPMAPLODBIAS, 0);
	mD3DDevice->EndScene();

	HRESULT result = 0;

	result = mD3DDevice->TestCooperativeLevel();
	if(result == D3DERR_DEVICELOST)
		return;

	if(result == D3DERR_DEVICENOTRESET)
	{
		mDeviceLost = true;

		// Release the existing swap chains
		SwapChainMap::iterator swapChainIt = mSwapChains.begin();
		for(; swapChainIt != mSwapChains.end(); swapChainIt++)
		{
			IDirect3DSwapChain9* pSwapChain = swapChainIt->second.mD3DSwapChain;
			if(pSwapChain)
			{
				pSwapChain->Release();
			}
		}

		// Release the shaders
		ShaderMap::iterator shaderIt = mShaders.begin();
		for(; shaderIt != mShaders.end(); shaderIt++)
		{
			(*shaderIt).second.mEffect->Release();
		}

		// Release the vbuffer
		mVertexBuffer->Release();
		mVertexBuffer = NULL;

		// Release the vertex decl
		mVertexDeclaration->Release();
		mVertexDeclaration = NULL;

		// Reset the device
		D3DPRESENT_PARAMETERS d3dpp;
		GetPresentParams(d3dpp);   
		HRESULT hr = mD3DDevice->Reset(&d3dpp);
		if(FAILED(hr))
		{
			printf("FAILED\n");
		}

		// Recreate the swap chains
		swapChainIt = mSwapChains.begin();
		for(; swapChainIt != mSwapChains.end(); swapChainIt++)
		{
			SwapChain swapChain = swapChainIt->second;
			CreateContext(swapChain.mWindow, swapChain.mWidth, swapChain.mHeight, swapChainIt->first);
		}

		// Create the Vertex Buffer
		CreateBuffers();

		// Recreate the shaders
		ShaderMap tmp = mShaders;
		mShaders.clear();

		CreateShaders();

		shaderIt = tmp.begin();
		for(; shaderIt != tmp.end(); shaderIt++)
		{
			Shader& shader = shaderIt->second;
			if(shader.mSource != "")
				LoadShader(shader.mName.c_str(), shader.mSource.c_str());
		}

		// By setting mContext to zero, we force the back-buffer changes in the next frame
		long oldContext = mContext;
		mContext = 0;
		SetContext(oldContext);
	}

	SwapChainMap::iterator it = mSwapChains.find(mContext);
	if(it != mSwapChains.end())
	{
		IDirect3DSwapChain9* pSwapChain = it->second.mD3DSwapChain;
		if(pSwapChain)
			result = pSwapChain->Present(NULL, NULL, NULL, NULL, 0);
	}
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
	mModel = *(D3DXMATRIX*)&batch.mTransform;

	LPDIRECT3DTEXTURE9 texture = mTextureManager.GetTexture(batch.mTextureID); 

	const Otter::Property* pSetShader = batch.mProperties.GetProperty(SHADER_SET);
	if(pSetShader)
	{
		uint32 newShader = (uint32)pSetShader->mData;
		SetShader(newShader, false);
	}
				
	Shader& shader = mShaders[mCurrentShaderID];

	const Otter::Property* pShaderParamCount = batch.mProperties.GetProperty(SHADER_PARAM_COUNT);
	if(pShaderParamCount)
	{
		uint32 cnt = (uint32)pShaderParamCount->mData;
		for(uint32 i = 0; i < cnt; i++)
		{
			const Otter::Property* pParamType = batch.mProperties.GetProperty(SHADER_PARAM_TYPE + i);
			const Otter::Property* pParamID = batch.mProperties.GetProperty(SHADER_PARAM_ID + i);
			const Otter::Property* pParamData = batch.mProperties.GetProperty(SHADER_PARAM_DATA + i);
			const Otter::Property* pParamDataLen = batch.mProperties.GetProperty(SHADER_PARAM_DATA_LEN + i);

			if(pParamType == NULL || pParamID == NULL || pParamData == NULL)
				break;

			if(pParamType->mData == 0) // Texture
			{
				shader.mEffect->SetTexture(shader.GetParameterHandle((int)pParamID->mData), mTextureManager.GetTexture(pParamData->mData));
			}
			else if(pParamType->mData == 1 && pParamDataLen != NULL) // Float Array
			{
				shader.mEffect->SetFloatArray(shader.GetParameterHandle((int)pParamID->mData), (float*)pParamData->mData, (int)pParamDataLen->mData);
			}
			else if(pParamType->mData == 2 && pParamDataLen != NULL) // Int Array
			{
				shader.mEffect->SetIntArray(shader.GetParameterHandle((int)pParamID->mData), (int*)pParamData->mData, (int)pParamDataLen->mData);
			}
			else if(pParamType->mData == 3) // Int
			{
				shader.mEffect->SetInt(shader.GetParameterHandle((int)pParamID->mData), (int)pParamData->mData);
			}
		}
	}

	const Otter::Property* pTechnique = batch.mProperties.GetProperty(SHADER_TECHNIQUE);
	if(pTechnique)
		shader.mEffect->SetTechnique((const char*)pTechnique->mData);
	else
		shader.mEffect->SetTechnique(texture != NULL ? "WithTexture" : "WithoutTexture");

	shader.mEffect->SetTexture("DIFFUSE_TEXTURE", texture);
	shader.mEffect->SetMatrix("MATRIX_MODEL", &mModel);

	float lodBias = -1.0f;
	if(texture != NULL)
	{
		int numLevels = texture->GetLevelCount();
		mD3DDevice->SetSamplerState(0, D3DSAMP_MIPMAPLODBIAS, *(DWORD*)&lodBias);
	}

	if(batch.mRenderFlags & Otter::kRender_Wireframe)
	{
		mD3DDevice->SetRenderState(D3DRS_FILLMODE, D3DFILL_WIREFRAME);
	}
	else 
	{
		mD3DDevice->SetRenderState(D3DRS_FILLMODE, D3DFILL_SOLID); 
	}


	uint32 passes = 0;
	shader.mEffect->Begin(&passes, D3DXFX_DONOTSAVESAMPLERSTATE);
	{
		for(uint32 pass = 0; pass < passes; pass++)
		{
			shader.mEffect->BeginPass(pass);

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

			shader.mEffect->EndPass();
		}
	}
	shader.mEffect->End();
}

/* Sets an orthographic projection transform
 */
void D3DRenderer::SetOrtho(float left, float right, float bottom, float top, float znear, float zfar)
{
	D3DXMatrixOrthoOffCenterLH(&mProjection, left, right, bottom, top, znear, zfar); 
}

/* Sets the view to  a left-handed view transform
 */
void D3DRenderer::SetLookAtLH(	float px, float py, float pz,
								float tx, float ty, float tz,
								float ux, float uy, float uz)
{
	D3DXVECTOR3 eye(px, py, pz);
	D3DXVECTOR3 at(tx, ty, tz);
	D3DXVECTOR3 up(ux, uy, uz);

	D3DXMatrixLookAtLH(&mView, &eye, &at, &up);
}
	
/* Sets the viewport
 */
void D3DRenderer::SetViewport(float x, float y, float w, float h)
{
	D3DVIEWPORT9 oldVp;
	mD3DDevice->GetViewport(&oldVp);

	D3DVIEWPORT9 vp;
	vp.X = (DWORD)x;
	vp.Y = (DWORD)y;
	vp.Width = (DWORD)w;
	vp.Height = (DWORD)h;
	vp.MinZ = oldVp.MinZ;
	vp.MaxZ = oldVp.MaxZ;

	mD3DDevice->SetViewport(&vp);
}

/* Unprojects the screen-space coordinate into object-space
 */
void D3DRenderer::Unproject(float& x, float& y, float& z)
{
	D3DVIEWPORT9 viewport;
	mD3DDevice->GetViewport(&viewport);

	D3DXVECTOR3 in(x, y, z);
	D3DXVECTOR3 out;

	D3DXMATRIX world;
	D3DXMatrixIdentity(&world);

	D3DXVec3Unproject(&out, &in, &viewport, &mProjection, &mView, &world);

	x = out.x;
	y = out.y;
	z = out.z;
}

//------------------------------------------------------------------------------

int Shader::GetParameterID(const char* szName)
{
	std::string name = szName;
	for(int i = 0; i < (int)mParameters.size(); i++)
	{
		if(mParameters[i].mName == name)
			return i;
	}

	D3DXHANDLE pHandle = mEffect->GetParameterByName(NULL, szName);
	if(pHandle == NULL)
		return -1;

	Parameter param;
	param.mName = name;
	param.mHandle = pHandle;

	mParameters.push_back(param);

	return mParameters.size() - 1;
}

D3DXHANDLE Shader::GetParameterHandle(int id)
{
	if((int)mParameters.size() <= id)
		return NULL;

	return mParameters[id].mHandle;
}