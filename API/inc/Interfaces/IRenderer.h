#pragma once 

#include "Common/Types.h"
#include "Otter.h"

namespace Otter
{
	/**
	 * Interface for user rendering of the UI
	 */
	class IRenderer
	{
	public:	
		enum StencilState
		{
			DISABLE_STENCIL,
			DRAW_TO_STENCIL,
			DRAW_USING_STENCIL
		};
		
	public:
		/**
		 * Constructor
		 */
		IRenderer(void) { };

		/**
		 * Virtual Destructor
		 */
		virtual ~IRenderer(void) { };

	public:

		/**
		 * Loads a texture with the specified id and path
		 */
		virtual void OnLoadTexture(sint32 /*textureID*/, const char* /*szPath*/) { }

		/**
		 * Unloads a texture with the specified id
		 */
		virtual void OnUnloadTexture(sint32 /*textureID*/) { }

		/**
		 * Called when a drawing pass has begun
		 */
		virtual void OnDrawBegin() { }

		/**
		 * Called when a drawing pass has ended
		 */
		virtual void OnDrawEnd() { }

		/**
		 * Commits a vertex buffer for rendering
		 */
		virtual void OnCommitVertexBuffer(const GUIVertex* /*pVertices*/, uint32 /*numVertices*/) = 0;

		/**
		 * Draws primitives on screen
		 */
		virtual void OnDrawBatch(const DrawBatch& /*batch*/) = 0;

		/**
		 * Sets the renderer to draw to the stencil buffer
		 */
		virtual void SetStencilState(StencilState state) { }
	};
}