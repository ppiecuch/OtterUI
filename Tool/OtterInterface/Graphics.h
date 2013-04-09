#pragma once

#include "D3DX9.h"
#include "Renderers/BaseRenderer.h"
#include "Otter.h"

using namespace System;
using namespace System::Runtime::InteropServices;
using namespace System::Collections::Generic;

#define SCRATCH_SIZE	(10 * 1024)	// 10k

namespace Otter
{
	struct FontData;
	class Renderer;

namespace Interface
{
	/* Contains font information
	 */
	private ref class FontInfo
	{
	public:
		::System::String^	mName;
		int					mFontID;

		Font*				mFont;
		FontData*			mFontData;
	};

	/* Represents a single vertex with position, texture coords, and color.
	 */
	public ref class Vertex
	{
	public:
		array<float>^	mPosition;
		array<float>^	mUV;
		int				mColor;

	public:

		/* Constructor
		 */
		Vertex()
		{
			mPosition	= gcnew array<float>(3);
			mUV			= gcnew array<float>(2);
			mColor		= 0xFFFFFFFF;
		}

		/* Sets the position
		 */
		void SetPosition(float x, float y, float z)
		{
			mPosition[0] = x;
			mPosition[1] = y;
			mPosition[2] = z;
		}

		/* Sets the texture coordinates (UV)
		 */
		void SetUV(float u, float v)
		{
			mUV[0] = u;
			mUV[1] = v;
		}

		/* Sets the color
		 */
		void SetColor(int color)
		{
			mColor = color;
		}
	};

	/* Represents a single triangle (three vertices)
	 */
	public ref class Triangle
	{
	public:

		array<Vertex^>^ mVertices;

	public:

		/* Constructor
		 */
		Triangle()
		{
			mVertices = gcnew array<Vertex^>(3);
			mVertices[0] = gcnew Vertex();
			mVertices[1] = gcnew Vertex();
			mVertices[2] = gcnew Vertex();
		}

		/* Sets the information for a particular vertex
		 */
		void SetVertex(int index, float x, float y, float z, float u, float v, int color)
		{
			if(index < 0 || index > mVertices->Length)
				return;

			mVertices[index]->SetPosition(x, y, z);
			mVertices[index]->SetUV(u, v);
			mVertices[index]->SetColor(color);
		}
	};

	/* Represents a single triangle (three vertices)
	 */
	public ref class Projection
	{
	public:

		float	mLeft;
		float	mRight;
		float	mTop;
		float	mBottom;
		float	mNear;
		float	mFar;

	public:

		/* Constructor
		 */
		Projection(float l, float r, float t, float b, float n, float f)
			: mLeft(l), mRight(r), mTop(t), mBottom(b), mNear(n), mFar(f)
		{
		}
	};

	public ref class Graphics
	{	
	public:

		delegate void TextureEventHandler(int textureID);
		event TextureEventHandler^ OnTextureLoaded;
		event TextureEventHandler^ OnTextureUnloaded;

	public:
		/* Constructor
		 */
		Graphics(long context);

		/* Destructor
		 */
		~Graphics();

	public:

		/* Creates a new drawing context of the specified width and height
		 */
		long CreateContext(long window, int width, int height);

		/* Destroys a context
		 */
		void DestroyContext(long context);

		/* Begins drawing
		 */
		void Begin(long context);

		/* Ends drawing
		 */
		void End();	

		/* Sets an orthographic projection transform
		 */
		Projection^ SetOrtho(Projection^ projection);

		/* Sets the view to  a left-handed view transform
		 */
		void SetLookAtLH(	float px, float py, float pz,
							float tx, float ty, float tz,
							float ux, float uy, float uz);

		/* Sets the viewport
		 */
		void SetViewport(float x, float y, float w, float h);

		/* Sets the render bounds.  Anything drawn outside of the bounds will
		 * modulated by the provided color
		 */
		void SetBounds(int width, int height, int color);

		/* Loads a font and returns its ID.
		 * Returns 0 if failed.
		 */
		int LoadFont(::System::String^ name, array<::System::Byte>^ fontData, List<int>^ textures);

		/* Unloads a font by ID
		 */
		void UnloadFont(int fontID);

		/* Unprojects a vertex
		 */
		bool Unproject(float% x, float% y, float% z);

		/* Returns whether or not the device was lost in the last frame
		 */
		bool WasDeviceLost();
	
	// Shader Functions
	public:

		/* Loads a shader.  If successful, the shader's ID is returned.
		 */
		int LoadShader(String^ name, String^ shader);

		/* Retrieves a shader parameter ID by name
		 */
		int GetShaderParameter(int shaderID, String^ parameterName);
		
		/* Sets the active shader by ID
		 */
		void SetShader(int shaderID);

		/* Sets the current shader technique
		 */
		void SetShaderTechnique(String^ technique);

		/* Sets a shader texture parameter
		 */
		void SetShaderTexture(int paramID, int texID);

		/* Sets a shader float array parameter
		 */
		void SetShaderFloatArray(int paramID, array<::System::Single>^ values);

		/* Sets a shader int array parameter
		 */
		void SetShaderIntArray(int paramID, array<::System::Int32>^ values);

		/* Sets a shader int parameter
		 */
		void SetShaderInt(int paramID, int value);

	// Drawing Functions
	public:

		/* Draws a rectangle
		 */
		void DrawRectangle(int textureID, float x, float y, float width, float height, float u1, float v1, float u2, float v2, int color, float skewAngle, int maskID);

		/* Draws a rectangle
		 */
		void DrawRectangle(int textureID, float x, float y, float width, float height, int color);

		/* Draws a set of triangles
		 */
		void DrawTriangles(int textureID, array<Triangle^>^ triangles);

		/* Draws a string
		 */
		void DrawString(int fontID, ::System::String^ text, float x, float y, float width, float height, int color, float scaleX, float scaleY, int halign, int valign, float leading, int tracking, float skewAngle, uint32 textFit, int dropShadow, int maskID);

		/* Draws a line
		 */
		void DrawLine(float sx, float sy, float sz, float dx, float dy, float dz, int color);
		
		/* Draws a rectangle to the stencil buffer
		 */
		void DrawRectangleStencil(int textureID, float x, float y, float width, float height, float u1, float v1, float u2, float v2, int color, float skewAngle, int maskID);

	// Textures
	public:		

		/* Loads a texture at the specified path and returns its ID
		 */
		int LoadTexture(::System::String^ path, int width, int height);

		/* Loads a texture at the specified path and returns its ID
		 */
		int LoadTexture(array<::System::Byte>^ textureData, int width, int height, int bitdepth);

		/* Unloads a texture by id
		 */
		void UnloadTexture(int id);

		/* Gets the texture information by ID
		 */
		bool GetTextureInfo(int id, int% w, int% h);

		/* Retrieves an array of textures ids.
		 */
		array<int>^ GetTextures();

	public:

		/* Pushes a transformation matrix
		 */
		void PushMatrix(array<float>^ transform);

		/* Pops a transformation matrix
		 */
		void PopMatrix();

		/* Returns the top-most transformation matrix
		 */
		array<float>^ GetTopMatrix();
		
		/* Sets the matrix to use for stencil draws
		 */
		void SetStencilMatrix(array<float>^ transform);

	private:

		Otter::Graphics*	mOtterGraphics;
		BaseRenderer*		mUserRenderer;

		Projection^			mProjection;
		List<FontInfo^>		mFonts;

		uint8*				mScratchMemory;
		uint32				mScratchPos;

		static Graphics^	mInstance = nullptr;

	public:
		
		static property Graphics^ Instance
		{
			Graphics^ get()
			{
				return mInstance;
			}
			void set(Graphics^ value)
			{
				mInstance = value;
			}
		}
	};
}
}