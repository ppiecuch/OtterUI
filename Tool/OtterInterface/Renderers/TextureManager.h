#pragma once

#include <map>
#include <string>
#include <d3dx9.h>

struct Texture;

typedef std::map<int, Texture*> TextureMap;

/* Loads / Unloads textures
 */
class TextureManager
{
public:
	/* Constructor
	 */
	TextureManager(void);

	/* Destructor
	 */
	~TextureManager();

public:

	/* Loads a texture from a specified path.
	 * Returns a non-zero texture identifier if successful,
	 * zero if otherwise.
	 */
	int LoadTexture(LPDIRECT3DDEVICE9 pDevice, std::string path, int width = 0, int height = 0);

	/* Loads a texture and returns the ID
	 */
	int LoadTexture(LPDIRECT3DDEVICE9 pDevice, const unsigned char* buffer, int bufferLength, int width, int height, int bitdepth);

	/* Unloads a texture by id
	 */
	bool UnloadTexture(int id);

	/* Retrieves the number of loaded textures
	 */
	int NumTextures();

	/* Retrieves the texture ID by index
	 */
	int GetTextureID(int index);

	/* Retrieves the texture information
	 */
	bool GetTextureInfo(int id, int& width, int& height);

	/* Retrieves the texture by id
	 */
	LPDIRECT3DTEXTURE9 GetTexture(int id);

private:

	int			mNextID;
	TextureMap	mTextures;
};
