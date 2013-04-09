#include <string.h>
#include <assert.h>
#include "Graphics/Graphics.h"
#include "../Memory/Memory.h"

using namespace VectorMath;

#define NUM_VERTS (6000)

namespace Otter
{
	/* Constructor
	 */
	Graphics::Graphics(void)
	{
		mMatrixStack.push_front(Matrix4::IDENTITY);
		mStackUpdated = true;

		mVertices = (GUIVertex*)OTTER_ALLOC(sizeof(GUIVertex) * NUM_VERTS);
		mBufferPosition = 0;

		mUserRenderer = NULL;

		mPreTransformVerts = false;

		mCurrentMask = -1;
	}

	/* Virtual Destructor
	 */
	Graphics::~Graphics(void)
	{
		OTTER_FREE(mVertices);
	}	

	/**
	 * Enables / disables (cpu) pre-transforming of vertices.
	 * Enabling vertex pretransformation may reduce draw calls at the cost
	 * of additional CPU load.
	 */
	void Graphics::EnablePreTransformVerts(bool bEnable)
	{
		mPreTransformVerts = bEnable;
	}

	/* Sets the user renderer interface
		*/
	void Graphics::SetUserRenderer(IRenderer* pUserRenderer)
	{
		mUserRenderer = pUserRenderer;
	}

	/* Pushes a matrix onto the matrix stack.
	 */
	void Graphics::PushMatrix(const Matrix4& matrix)
	{
		if(mMatrixStack.size() == 0)
		{
			mMatrixStack.push_front(matrix);
		}
		else
		{
			const Matrix4& top = mMatrixStack[0];
			mMatrixStack.push_front(top * matrix); 
		}

		mStackUpdated = true;
	}

	/* Pops a matrix from the matrix stack
	 */
	void Graphics::PopMatrix()
	{
		if(mMatrixStack.size() > 0)
			mMatrixStack.pop_front();

		mStackUpdated = true;
	}

	/* Gets the top matrix on the stack
	 */
	VectorMath::Matrix4* Graphics::GetTopMatrix()
	{
		if(mMatrixStack.size() == 0)
			return NULL;

		return &mMatrixStack[0];
	}

	/* Sets the matrix to use for stencil draws
	*/
	void Graphics::SetStencilMatrix(const Matrix4& matrix)
	{
		mStencilMatrix = matrix;
	}


	/* Begins drawing
	 */
	void Graphics::Begin()
	{
		mCurrentMask = -1;
		
		mMatrixStack.clear();
		mMatrixStack.push_front(Matrix4::IDENTITY);

		mStencilMatrix = Matrix4::IDENTITY;

		mBatches.clear();

		mBufferPosition = 0;
		mStackUpdated = true;

		if(mUserRenderer)
			mUserRenderer->OnDrawBegin();
	}

	/* Ends drawing 
	 */
	void Graphics::End()
	{
		if(mUserRenderer)
		{
			DrawBatches();

			mUserRenderer->OnDrawEnd();
		}
	}

	void Graphics::SetProperty(uint32 propertyID, uintptr_t data)
	{
		uint32 propIndex = 0xFFFFFFFF;
		uint32 cnt = mProperties.size();
		for(uint32 i = 0; i < cnt; i++)
		{
			if(mProperties[i].mPropertyID == propertyID)
			{
				propIndex = i;
			}
		}

		if(propIndex == 0xFFFFFFFF)
		{
			mProperties.push_back(Property());
			propIndex = mProperties.size() - 1;
		}

		mProperties[propIndex].mPropertyID = propertyID;
		mProperties[propIndex].mData = data;

		mPropertiesUpdated = true;
	}
		
	bool Graphics::GetProperty(uint32 propertyID, uintptr_t* data)
	{
		if(data == NULL)
			return false;

		uint32 cnt = mProperties.size();
		for(uint32 i = 0; i < cnt; i++)
		{
			if(mProperties[i].mPropertyID == propertyID)
			{
				*data = mProperties[i].mData;
				return true;
			}
		}

		return false;
	}

	void Graphics::ClearProperties()
	{
		mProperties.clear();

		mPropertiesUpdated = true;
	}

	/* Loads a texture
	 */
	void Graphics::LoadTexture(uint32 textureID, const char* szPath)
	{
		if(mUserRenderer)
			mUserRenderer->OnLoadTexture(textureID, szPath);
	}

	/* Unloads a texture
	 */
	void Graphics::UnloadTexture(uint32 textureID)
	{
		if(mUserRenderer)
			mUserRenderer->OnUnloadTexture(textureID);
	}

	/* Draws primitives
	 */
	void Graphics::DrawPrimitives(uint32 textureID, PrimitiveType primType, int numPrims, GUIVertex* pVertices, uint32 renderFlags, bool drawToStencil, int maskID)
	{
		if (drawToStencil)
		{
			if (maskID == mCurrentMask)
				return; //this mask is already in the stencil buffer; we're good

			//we're drawing this to the stencil; first commit everything we've batched so far
			if (mBatches.size() != 0)
				DrawBatches();

			//and set the renderer to draw to the stencil buffer
			mUserRenderer->SetStencilState(IRenderer::DRAW_TO_STENCIL);
		}
		else if (maskID != mCurrentMask)
		{
			//we shouldn't ever be switching to a mask without first rendering it to the stencil buffer (via drawToStencil being true)
			assert(maskID == -1 && mCurrentMask != -1);

			//we just stopped rendering with our previous mask; commit everything we've batched so far
			DrawBatches();

			//and disable the stencil
			mUserRenderer->SetStencilState(IRenderer::DISABLE_STENCIL);

			mCurrentMask = -1;
		}

		// Determine the number of vertices we're working with
		int numVerts = 0;
		switch(primType)
		{
			case Otter::kPrim_TriangleList	: numVerts = numPrims * 3; break;
			case Otter::kPrim_TriangleStrip :
			case Otter::kPrim_TriangleFan	: numVerts = numPrims - 2; break;
		}

		// Can't write outside of the working buffer.
		if((mBufferPosition + numVerts) > NUM_VERTS)
			return;

		if(mPreTransformVerts && (mMatrixStack.size() > 0 || drawToStencil))
		{
			const VectorMath::Matrix4& tmpMtx = drawToStencil? mStencilMatrix : mMatrixStack[0];
			Vector4 v;

			for(int i = 0; i < numVerts; i++)
			{
				v.x = pVertices[i].mPosition.x;
				v.y = pVertices[i].mPosition.y;
				v.z = pVertices[i].mPosition.z;
				v.w = 1.0f;

				v = tmpMtx * v;
				pVertices[i].mPosition.x = v.x;
				pVertices[i].mPosition.y = v.y;
				pVertices[i].mPosition.z = v.z;
			}
		}

		// Determine if we're going to create a new batch
		bool newBatch = true;
		if(mBatches.size() != 0) 
		{
			DrawBatch& lastBatch = mBatches[mBatches.size() - 1];
			if(	primType == Otter::kPrim_TriangleList && 
				lastBatch.mPrimitiveType == primType && 
				lastBatch.mTextureID == textureID &&
				lastBatch.mRenderFlags == renderFlags &&
				!(!mPreTransformVerts && mStackUpdated) &&
				!mPropertiesUpdated)
			{
				newBatch = false;
			}
		}

		if(newBatch)
		{
			VectorMath::Matrix4 matrix = VectorMath::Matrix4::IDENTITY;
			if(!mPreTransformVerts && (mMatrixStack.size() > 0 || drawToStencil))
				matrix = drawToStencil? mStencilMatrix : mMatrixStack[0];

			mBatches.push_back(DrawBatch((int)primType, 0, mBufferPosition, 0, textureID, matrix, mProperties, renderFlags));
		}

		// Current batch is either a newly created or existing batch.
		DrawBatch& currentBatch = mBatches[mBatches.size() - 1];

		// Copy over the verts
		memcpy(&mVertices[mBufferPosition], pVertices, numVerts * sizeof(Otter::GUIVertex));

		currentBatch.mPrimitiveCount += numPrims;
		currentBatch.mVertexCount += numVerts;
		mBufferPosition += numVerts;

		mStackUpdated = false;

		if (drawToStencil)
		{
			//if we're drawing this to the stencil, commit what we just batched straight away
			DrawBatches();

			//now set the renderer to draw further primitives using what's in the stencil buffer
			mUserRenderer->SetStencilState(IRenderer::DRAW_USING_STENCIL);

			//and set this as the current mask
			mCurrentMask = maskID;
		}
	}

	/* Draws a rectangle on screen
	 */
	void Graphics::DrawRectangle(uint32 textureID, float x, float y, float w, float h, uint32 color)
	{
		DrawRectangle(textureID, x, y, w, h, 0.0f, 0.0f, 1.0f, 1.0f, color, 0.0f);
	}

	/* Draws a rectangle
	 */
	void Graphics::DrawRectangle(uint32 textureID, float x, float y, float w, float h, float u1, float v1, float u2, float v2, uint32 color, float skewAngle, bool drawToStencil, int maskID)
	{
		float skewOffset = h * VectorMath::Functions::Tan(skewAngle * VectorMath::Constants::DEG_TO_RAD);

		static GUIVertex verts[6]; // Two triangles	

		// Top Left
		verts[0].mPosition.x = x + skewOffset;
		verts[0].mPosition.y = y;
		verts[0].mPosition.z = 0.0f;
		verts[0].mTexCoord.x = u1;
		verts[0].mTexCoord.y = v1;
		verts[0].mColor = color;

		// Top Right
		verts[1].mPosition.x = x + w + skewOffset;
		verts[1].mPosition.y = y;
		verts[1].mPosition.z = 0.0f;
		verts[1].mTexCoord.x = u2;
		verts[1].mTexCoord.y = v1;
		verts[1].mColor = color;

		// Bottom Left
		verts[2].mPosition.x = x;
		verts[2].mPosition.y = y + h;
		verts[2].mPosition.z = 0.0f;
		verts[2].mTexCoord.x = u1;
		verts[2].mTexCoord.y = v2;
		verts[2].mColor = color;

		// Bottom Left
		verts[3].mPosition.x = x;
		verts[3].mPosition.y = y + h;
		verts[3].mPosition.z = 0.0f;
		verts[3].mTexCoord.x = u1;
		verts[3].mTexCoord.y = v2;
		verts[3].mColor = color;

		// Top Right
		verts[4].mPosition.x = x + w + skewOffset;
		verts[4].mPosition.y = y;
		verts[4].mPosition.z = 0.0f;
		verts[4].mTexCoord.x = u2;
		verts[4].mTexCoord.y = v1;
		verts[4].mColor = color;

		// Bottom Right
		verts[5].mPosition.x = x + w;
		verts[5].mPosition.y = y + h;
		verts[5].mPosition.z = 0.0f;
		verts[5].mTexCoord.x = u2;
		verts[5].mTexCoord.y = v2;
		verts[5].mColor = color;

		DrawPrimitives(textureID, kPrim_TriangleList, 2, verts, 0, drawToStencil, maskID);
	}

	void Graphics::DrawLine(float sx, float sy, float sz, float dx, float dy, float dz, uint32 color)
	{
		static GUIVertex verts[3]; // One triangle	

		// Top Left
		verts[0].mPosition.x = sx;
		verts[0].mPosition.y = sy;
		verts[0].mPosition.z = sz;
		verts[0].mTexCoord.x = 0.0f;
		verts[0].mTexCoord.y = 0.0f;
		verts[0].mColor = color;

		// Top Right
		verts[1].mPosition.x = sx;
		verts[1].mPosition.y = sy;
		verts[1].mPosition.z = sz;
		verts[1].mTexCoord.x = 0.0f; 
		verts[1].mTexCoord.y = 0.0f;
		verts[1].mColor = color;

		// Bottom Left
		verts[2].mPosition.x = dx;
		verts[2].mPosition.y = dy;
		verts[2].mPosition.z = dz;
		verts[2].mTexCoord.x = 0.0f;
		verts[2].mTexCoord.y = 0.0f;
		verts[2].mColor = color;

		DrawPrimitives(-1, kPrim_TriangleList, 1, verts, kRender_Wireframe);
	}

	void Graphics::DrawBatches()
	{
		mUserRenderer->OnCommitVertexBuffer(mVertices, mBufferPosition);

		for(uint32 i = 0; i < mBatches.size(); i++)
		{
			const DrawBatch& batch = mBatches[i];
			mUserRenderer->OnDrawBatch(batch);
		}

		mBatches.clear();
		mBufferPosition = 0;
	}
}
