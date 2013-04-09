#pragma once

#include "Common/Types.h"
#include "Otter.h"

namespace Otter
{
	/**
	 * Responsible for simple drawing operations.  Organizes
	 * render calls into batches and prepares a vertex buffer.
	 */
	class Graphics
	{
	public:
		/**
		 * Constructor
		 */
		Graphics(void);

		/**
		 * Virtual Destructor
		 */
		virtual ~Graphics(void);

	public:

		/**
		 * Enables / disables (cpu) pre-transforming of vertices.
		 * Enabling vertex pretransformation may reduce draw calls at the cost
		 * of additional CPU load.
		 */
		void EnablePreTransformVerts(bool bEnable = true);

		/**
		 * Sets the user renderer interface
		 */
		void SetUserRenderer(IRenderer* pUserRenderer);

		/**
		 * Pushes a matrix onto the matrix stack
		 */
		void PushMatrix(const VectorMath::Matrix4& matrix);

		/**
		 * Pops a matrix from the matrix stack
		 */
		void PopMatrix();

		/**
		 * Gets the top matrix on the stack
		 */
		VectorMath::Matrix4* GetTopMatrix();

		/**
		 * Sets the matrix to use for stencil draws
		 */
		void SetStencilMatrix(const VectorMath::Matrix4& matrix);

		/**
		 * Begins drawing
		 */
		void Begin();

		/**
		 * Ends drawing 
		 */
		void End();

	public:

		/** 
		 * Sets a render property for the current batch.
		 */
		void SetProperty(uint32 propertyID, uintptr_t data);
		
		/** 
		 * Retrieves a property in the current batch
		 */
		bool GetProperty(uint32 propertyID, uintptr_t* data);

        /**
		 * Clears all properties in the current batch
		 */
		void ClearProperties();

	public:

		/**
		 * Loads a texture
		 */
		void LoadTexture(uint32 textureID, const char* szPath);

		/**
		 * Unloads a texture
		 */
		void UnloadTexture(uint32 textureID);

		/**
		 * Draws primitives
		 */
		void DrawPrimitives(uint32 textureID, PrimitiveType primType, int numPrims, GUIVertex* pVertices, uint32 renderFlags = 0, bool drawToStencil = false, int maskID = -1);

		/**
		 * Draws a rectangle
		 */
		void DrawRectangle(uint32 textureID, float x, float y, float w, float h, uint32 color);

		/**
		 * Draws a rectangle
		 */
		void DrawRectangle(uint32 textureID, float x, float y, float w, float h, float u1, float v1, float u2, float v2, uint32 color, float skewAngle, bool drawToStencil = false, int maskID = -1);

		/**
		 * Draws a line
		 */
		void DrawLine(float sx, float sy, float sz, float dx, float dy, float dz, uint32 color);

	private:
	
		/**
		 * Draws all present batches with current settings.
		 */
		void							DrawBatches();

	private:

		Array<DrawBatch>				mBatches;
		Array<VectorMath::Matrix4>		mMatrixStack;
		VectorMath::Matrix4				mStencilMatrix;
		bool							mStackUpdated;

		int								mBufferPosition;
		GUIVertex*						mVertices;

		IRenderer*						mUserRenderer;
		
		Array<Property>					mProperties;
		bool							mPropertiesUpdated;

		bool							mPreTransformVerts;

		int								mCurrentMask;
	};
}