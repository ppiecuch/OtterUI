#include "StdAfx.h"
#include "TextureManager.h"

struct Texture
{
	int				   mID;
	int				   mRefCount;

	std::string		   mPath;
	LPDIRECT3DTEXTURE9 mD3DTexture;
};

/* Constructor
 */
TextureManager::TextureManager(void)
{
	mNextID = 1; // IDs start at one.
}

/* Destructor
 */
TextureManager::~TextureManager()
{
	// See if we have a texture already loaded by the same path
	for(TextureMap::iterator it = mTextures.begin(); it != mTextures.end(); it++)
	{
		Texture* pTexture = it->second;

		if(pTexture)
		{
			pTexture->mD3DTexture->Release();
			delete pTexture;
		}
	}

	mTextures.clear();
}

/* Loads a texture from a specified path.
 * Returns a non-zero texture identifier if successful,
 * zero if otherwise.
 * 
 * If width or height is non-zero, the texture will be loaded to the specified
 * dimension.  Otherwise the texture's width/height will be used as specified in the
 * source image.
 */
int TextureManager::LoadTexture(LPDIRECT3DDEVICE9 pDevice, std::string path, int width, int height)
{
	// See if we have a texture already loaded by the same path
	for(TextureMap::iterator it = mTextures.begin(); it != mTextures.end(); it++)
	{
		Texture* pTexture = it->second;

		if(pTexture && pTexture->mPath == path)
		{
			pTexture->mRefCount++;
			return pTexture->mID;
		}
	}

	// No existing texture was found, load it now.
	Texture* pTexture = new Texture();

    size_t convertedChars = 0;
    wchar_t wpath[1024];
	mbstowcs_s(&convertedChars, wpath, path.length() + 1, path.c_str(), _TRUNCATE);

	HRESULT hr = D3DXCreateTextureFromFileEx(	pDevice,
												wpath,
												width,
												height,
												1, // Only need one mipmap for UI Textures
												0,
												D3DFMT_UNKNOWN,
												D3DPOOL_MANAGED,
												D3DX_FILTER_POINT,
												D3DX_FILTER_POINT,
												0,
												NULL,
												NULL,
												&pTexture->mD3DTexture
											);

	if(FAILED(hr))
	{
		delete pTexture;
		return 0;
	}

	pTexture->mID = mNextID++;
	pTexture->mRefCount = 1;
	pTexture->mPath = path;

	mTextures[pTexture->mID] = pTexture;

	return pTexture->mID;
}

/* Loads a texture and returns the ID
 */
int TextureManager::LoadTexture(LPDIRECT3DDEVICE9 pDevice, const unsigned char* buffer, int bufferLength, int width, int height, int bitdepth)
{
	D3DFORMAT format = D3DFMT_A8R8G8B8;

	if(bitdepth == 16)
		format = D3DFMT_A4R4G4B4;
	else if(bitdepth == 24)
		format = D3DFMT_R8G8B8;
	else if(bitdepth != 32)
		return 0;

	// No existing texture was found, load it now.
	Texture* pTexture = new Texture();

	HRESULT hr = D3DXCreateTexture(pDevice, width, height, 1, 0, format, D3DPOOL_MANAGED, &pTexture->mD3DTexture);
	if(FAILED(hr))
	{
		delete pTexture;
		return 0;
	}

	D3DLOCKED_RECT rect;
	hr = pTexture->mD3DTexture->LockRect(0, &rect, NULL, 0);

	if(FAILED(hr))
	{
		delete pTexture;
		return 0;
	}

	if(bitdepth == 16 || bitdepth == 32)
	{
		memcpy(rect.pBits, buffer, (bitdepth / 8) * width * height);
	}
	else
	{
		// 24-bit images are converted to 32.  We need to copy the bytes over line
		// by line.
		unsigned char* pBuffer = (unsigned char*)rect.pBits;
		int stride = 3 * width;
		for(int y = 0; y < height; y++)
		{
			for(int x = 0; x < width; x++)
			{
				const unsigned char* sourcePixel	= &buffer[y * stride + x * 3];
				unsigned char* destPixel			= &pBuffer[y * rect.Pitch + x * 4];

				destPixel[0] = sourcePixel[0]; // R
				destPixel[1] = sourcePixel[1]; // G
				destPixel[2] = sourcePixel[2]; // B
				destPixel[3] = 255; // A 
			}
		}
	}

	pTexture->mD3DTexture->UnlockRect(0);

	pTexture->mID = mNextID++;
	pTexture->mRefCount = 1;
	pTexture->mPath = "(memory texture)";

	mTextures[pTexture->mID] = pTexture;

	return pTexture->mID;
}

/* Unloads a texture by id
 */
bool TextureManager::UnloadTexture(int id)
{
	if(mTextures.find(id) == mTextures.end())
		return NULL;

	Texture* pTexture = mTextures[id];
	if(pTexture)
	{
		pTexture->mRefCount--;

		if(pTexture->mRefCount == 0)
		{
			pTexture->mD3DTexture->Release();
			delete pTexture;
			
			mTextures.erase(id);
		}

		return true;
	}

	return false;
}

/* Retrieves the number of loaded textures
 */
int TextureManager::NumTextures()
{
	return mTextures.size();
}

/* Retrieves the texture ID by index
 */
int TextureManager::GetTextureID(int index)
{
	TextureMap::iterator it = mTextures.begin();
	int i = 0;
	while((i != index) && it != mTextures.end())
	{
		i++;
		it++;
	}

	if(i == index && it != mTextures.end())
		return it->first;

	return -1;
}


/* Retrieves the texture information
 */
bool TextureManager::GetTextureInfo(int id, int& width, int& height)
{	
	D3DSURFACE_DESC desc;
	LPDIRECT3DTEXTURE9 texture = GetTexture(id);

	if(texture && SUCCEEDED(texture->GetLevelDesc(0, &desc)))
	{
		width = desc.Width;
		height = desc.Height;

		return true;
	}

	return false;
}

/* Retrieves the texture by id
 */
LPDIRECT3DTEXTURE9 TextureManager::GetTexture(int id)
{
	if(mTextures.find(id) == mTextures.end())
		return NULL;

	Texture* pTexture = mTextures[id];
	if(pTexture)
		return pTexture->mD3DTexture;

	return NULL;
}