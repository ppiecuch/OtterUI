#pragma once

#include <map>
#include "Otter.h"
#include "SampleRenderer.h"

typedef std::map<int, uint32> TextureMap;

/* OpenGL ES Renderer implementation
 */
class OGLESRenderer : public SampleRenderer
{
public:
	/* Constructor
	 */
	OGLESRenderer(int width, int height);
	
	/* Virtual Destructor
	 */
	virtual ~OGLESRenderer(void);
	
public:
	
	/* Sets the screen resolution
	 */
	void SetResolution(int width, int height);
	
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
	
	/* Creates the index / vertex buffers
	 */
	void CreateBuffers();
	
private:
	
	
	uint32						mVertexBuffer;
	int							mNumCommittedVertices;

	TextureMap					mTextures;
};
