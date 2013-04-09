#include "StdAfx.h"
#include "Graphics.h"
#include "Data/OtterData.h"
#include "Graphics/Graphics.h"
#include "Renderers/D3DRenderer.h"
#include "Font.h"

namespace Otter
{
namespace Interface
{	
	/* Constructor
	 */
	Graphics::Graphics(long context)
	{
		mOtterGraphics = new Otter::Graphics();
		mUserRenderer = new D3DRenderer((HWND)context);
		mOtterGraphics->SetUserRenderer(mUserRenderer);

		mProjection = nullptr;
		SetOrtho(gcnew Projection(0.0f, 1024.0f, 0.0f, 768.0f, 0.01f, 1000.0f));

		mScratchMemory = new uint8[SCRATCH_SIZE];
		mScratchPos = 0;
	}

	/* Destructor
	 */
	Graphics::~Graphics()
	{
		int len = mFonts.Count;
		for(int i = 0; i < len; i++)
		{
			FontInfo^ info = mFonts[i];
			
			delete info->mFont;
			delete[] (unsigned char*)info->mFontData;
		}

		mFonts.Clear();

		delete mUserRenderer;
		delete mOtterGraphics;

		delete[] mScratchMemory;
	}
	
	/* Creates a new drawing context of the specified width and height
	 */
	long Graphics::CreateContext(long window, int width, int height)
	{
		if(!mOtterGraphics)
			return 0;

		return mUserRenderer->CreateContext(window, width, height);
	}

	/* Destroys a context
		*/
	void Graphics::DestroyContext(long context)
	{
		if(!mOtterGraphics)
			return;

		mUserRenderer->DestroyContext(context);
	}

	/* Begins drawing
	 */
	void Graphics::Begin(long context)
	{
		if(!mOtterGraphics)
			return;
		
		mUserRenderer->SetContext(context);
		mOtterGraphics->Begin();
	}

	/* Ends drawing
	 */
	void Graphics::End()
	{
		if(!mOtterGraphics)
			return;

		mOtterGraphics->End();
	}

	/* Sets an orthographic projection transform
	 */
	Projection^ Graphics::SetOrtho(Projection^ projection)
	{
		if(!mOtterGraphics || !projection)
			return mProjection;

		Projection^ oldProjection = mProjection;
		mProjection = projection;
		mUserRenderer->SetOrtho(mProjection->mLeft, mProjection->mRight, mProjection->mBottom, mProjection->mTop, mProjection->mNear, mProjection->mFar);

		return oldProjection;
	}

	/* Sets the view to  a left-handed view transform
	 */
	void Graphics::SetLookAtLH(	float px, float py, float pz,
								float tx, float ty, float tz,
								float ux, float uy, float uz)
	{
		if(!mOtterGraphics)
			return;

		mUserRenderer->SetLookAtLH(	px, py, pz, 
								tx, ty, tz,
								ux, uy, uz);
	}

	/* Sets the viewport
	 */
	void Graphics::SetViewport(float x, float y, float w, float h)
	{
		if(!mOtterGraphics)
			return;

		mUserRenderer->SetViewport(x, y, w, h);
	}
	
	/* Loads a texture at the specified path and returns its ID
	 */
	int Graphics::LoadTexture(::System::String^ path, int width, int height)
	{
		if(!mOtterGraphics)
			return 0;

		IntPtr pathPtr = Marshal::StringToHGlobalAnsi(path);
		LPCSTR szPath = (LPCSTR)pathPtr.ToPointer();

		int id = mUserRenderer->LoadTexture(szPath, width, height);

		Marshal::FreeHGlobal (pathPtr);

		if(id != -1)
		{
			OnTextureLoaded(id);
		}
		
		return id;
	}	

	/* Loads a texture at the specified path and returns its ID
	 */
	int Graphics::LoadTexture(array<::System::Byte>^ textureData, int width, int height, int bitdepth)
	{
		if(!mOtterGraphics)
			return 0;

		unsigned char* buffer = new unsigned char[textureData->Length];
		for(int i = 0; i < textureData->Length; i++)
			buffer[i] = textureData[i];

		int id = mUserRenderer->LoadTexture(buffer, textureData->Length, width, height, bitdepth);

		delete[] buffer;

		if(id != -1)
		{
			OnTextureLoaded(id);
		}

		return id;
	}

	/* Unloads a texture by id
	 */
	void Graphics::UnloadTexture(int id)
	{
		if(!mOtterGraphics)
			return;

		mUserRenderer->UnloadTexture(id);		

		if(id != -1)
		{
			OnTextureUnloaded(id);
		}
	}	
	

	/* Gets the texture information by ID
	 */
	bool Graphics::GetTextureInfo(int id, int% w, int% h)
	{
		int width, height;
		if(mUserRenderer->GetTextureInfo(id, width, height))
		{
			w = width;
			h = height;

			return true;
		}

		return false;
	}

	/* Retrieves an array of textures ids.
	 */
	array<int>^ Graphics::GetTextures()
	{
		int num = mUserRenderer ? mUserRenderer->NumTextures() : 0;
		array<int>^ arr = gcnew array<int>(num);

		for(int i = 0; i < num; i++)
		{
			arr[i] = mUserRenderer ? mUserRenderer->GetTextureID(i) : -1;
		}

		return arr;
	}

	/* Sets the render bounds.  Anything drawn outside of the bounds will
	 * modulated by the provided color
	 */
	void Graphics::SetBounds(int width, int height, int color)
	{
		mUserRenderer->SetBounds(width, height, color);
	}

	/* Loads a font and returns its ID.
	 * Returns 0 if failed.
	 */
	int Graphics::LoadFont(::System::String^ name, array<::System::Byte>^ fontData, List<int>^ textures)
	{
		FontInfo^ info = nullptr;

		// Find an existing font and calculate the maximum ID at the same
		// time
		int len = mFonts.Count;
		int maxID = 0;
		for(int i = 0; i < len; i++)
		{
			if(!info && mFonts[i]->mName == name)
			{
				info = mFonts[i];
				delete[] (unsigned char*)info->mFontData;
				info->mFontData = NULL;
				info->mFont->SetData(NULL);
			}

			if(mFonts[i]->mFontID > maxID)
				maxID = mFonts[i]->mFontID;
		}

		if(!info)
		{
			info = gcnew FontInfo();
			info->mFontID = maxID + 1;
			info->mName = name;
			info->mFontData = NULL;
			info->mFont = new Font(NULL);

			mFonts.Add(info);
		}

		// Now that we have a valid font info, create the
		// FontData struct and set it accordingly.
		unsigned char* rawFontData = new unsigned char[fontData->Length];
		for(int i = 0; i < fontData->Length; i++)
			rawFontData[i] = fontData[i];

		uint32 textureIDs[256];
		for(int i = 0; i < textures->Count; i++)
			textureIDs[i] = textures[i];

		info->mFontData = (FontData*)rawFontData;
		info->mFont->SetData(info->mFontData);
		info->mFont->SetTextures(textureIDs, textures->Count);

		return info->mFontID;
	}

	/* Unloads a font by ID
	 */
	void Graphics::UnloadFont(int fontID)
	{
		int len = mFonts.Count;
		for(int i = 0; i < len; i++)
		{
			FontInfo^ info = mFonts[i];
			if(info->mFontID == fontID)
			{
				info->mFont->SetData(NULL);

				delete info->mFont;
				delete[] (unsigned char*)info->mFontData;

				mFonts.RemoveAt(i);
				break;
			}
		}
	}

	/* Pushes a transformation matrix
	 */
	void Graphics::PushMatrix(array<float>^ transform)
	{
		if(!mOtterGraphics)
			return;

		VectorMath::Matrix4 mtx(transform[0],	transform[4],	transform[8],	transform[12], 
								transform[1],	transform[5],	transform[9],	transform[13], 
								transform[2],	transform[6],	transform[10],	transform[14],
								transform[3],	transform[7],	transform[11],	transform[15]);

		mOtterGraphics->PushMatrix(mtx);
	}

	/* Pops a transformation matrix
	 */
	void Graphics::PopMatrix()
	{
		if(!mOtterGraphics)
			return;

		mOtterGraphics->PopMatrix();
	}

	/* Returns the top-most transformation matrix
	 */
	array<float>^ Graphics::GetTopMatrix()
	{
		const VectorMath::Matrix4* topMatrix = mOtterGraphics? mOtterGraphics->GetTopMatrix() : &VectorMath::Matrix4::IDENTITY;

		array<float>^ resultMatrix = gcnew array<float>(16);
		for (int i = 0; i < 16; ++i)
			resultMatrix[i] = topMatrix->mEntry[i];

		return resultMatrix;
	}

	/* Sets the matrix to use for stencil draws
	 */
	void Graphics::SetStencilMatrix(array<float>^ transform)
	{
		if(!mOtterGraphics)
			return;

		VectorMath::Matrix4 mtx(transform[0],	transform[4],	transform[8],	transform[12], 
			transform[1],	transform[5],	transform[9],	transform[13], 
			transform[2],	transform[6],	transform[10],	transform[14],
			transform[3],	transform[7],	transform[11],	transform[15]);

		mOtterGraphics->SetStencilMatrix(mtx);
	}

	/* Unprojects a vertex
	 */
	bool Graphics::Unproject(float% x, float% y, float% z)
	{
		if(!mOtterGraphics)
			return false;

		float pos[] = {x, y, z};

		mUserRenderer->Unproject(pos[0], pos[1], pos[2]);

		x = pos[0];
		y = pos[1];
		z = pos[2]; 
		return true;
	}

	/* Returns whether or not the device was lost in the last frame 
	 */
	bool Graphics::WasDeviceLost()
	{
		if(!mUserRenderer)
			return false;

		return mUserRenderer->WasDeviceLost();
	} 

	/* Loads a shader.  If successful, the shader's ID is returned.
	 */
	int Graphics::LoadShader(String^ name, String^ shader) 
	{
		const char* szName = (const char*)(void*)Marshal::StringToHGlobalAnsi(name);
		const char* szShader = (const char*)(void*)Marshal::StringToHGlobalAnsi(shader);
    
		int shaderID = mUserRenderer-> LoadShader(szName, szShader);

		Marshal::FreeHGlobal((::System::IntPtr)(void*)szShader);
		Marshal::FreeHGlobal((::System::IntPtr)(void*)szName);

		return shaderID;
	}

	/* Retrieves a shader parameter ID by name
	 */
	int Graphics::GetShaderParameter(int shaderID, String^ parameterName)
	{
		const char* szName = (const char*)(void*)Marshal::StringToHGlobalAnsi(parameterName);

		int parameterID = mUserRenderer->GetShaderParameter(shaderID, szName);

		Marshal::FreeHGlobal((::System::IntPtr)(void*)szName);

		return parameterID;
	}

	/* Sets the active shader by ID
	 */
	void Graphics::SetShader(int shaderID)
	{
		mOtterGraphics->ClearProperties();
		mOtterGraphics->SetProperty((uint32)SHADER_SET, shaderID);

		mScratchPos = 0;
	}

	/* Sets a shader texture parameter
	 */
	void Graphics::SetShaderTexture(int paramID, int texID)
	{
		uintptr_t numParams = 0;
		mOtterGraphics->GetProperty((uint32)SHADER_PARAM_COUNT, &numParams);

		mOtterGraphics->SetProperty((uint32)SHADER_PARAM_TYPE + numParams, 0);	// 0 : Texture
		mOtterGraphics->SetProperty((uint32)SHADER_PARAM_ID + numParams, paramID);
		mOtterGraphics->SetProperty((uint32)SHADER_PARAM_DATA + numParams, texID);

		mOtterGraphics->SetProperty((uint32)SHADER_PARAM_COUNT, numParams + 1);
	}

	/* Sets the current shader technique
	 */
	void Graphics::SetShaderTechnique(String^ technique)
	{
		const char* szName = (const char*)(void*)Marshal::StringToHGlobalAnsi(technique);

		int len = strlen(szName) + 1;
		if(len > 0 && ((mScratchPos + len) < SCRATCH_SIZE))
		{
			strcpy((char*)&mScratchMemory[mScratchPos], szName);
			mScratchMemory[mScratchPos + len] = 0;

			mOtterGraphics->SetProperty((uint32)SHADER_TECHNIQUE, (uintptr_t)&mScratchMemory[mScratchPos]);
			mScratchPos += len;
			mScratchPos = ((mScratchPos + 3) & 3); // 4-byte aligned
		}

		Marshal::FreeHGlobal((::System::IntPtr)(void*)szName);
	}

	/* Sets a shader float array parameter
	 */
	void Graphics::SetShaderFloatArray(int paramID, array<::System::Single>^ values)
	{
		if(values->Length == 0)
			return;

		if((mScratchPos + values->Length * 4) >= SCRATCH_SIZE)
			return;

		int pos = mScratchPos;

		for(int i = 0; i < values->Length; i++)
		{
			*((float*)&mScratchMemory[mScratchPos]) = values[i];
			mScratchPos += 4;
		}
		
		mScratchPos = ((mScratchPos + 3) & 3); // 4-byte aligned

		uintptr_t numParams = 0;
		mOtterGraphics->GetProperty((uint32)SHADER_PARAM_COUNT, &numParams);

		mOtterGraphics->SetProperty((uint32)SHADER_PARAM_TYPE + numParams, 1);	// 1 : FloatArray
		mOtterGraphics->SetProperty((uint32)SHADER_PARAM_ID + numParams, paramID);
		mOtterGraphics->SetProperty((uint32)SHADER_PARAM_DATA + numParams, (uintptr_t)&mScratchMemory[pos]);
		mOtterGraphics->SetProperty((uint32)SHADER_PARAM_DATA_LEN + numParams, values->Length);

		mOtterGraphics->SetProperty((uint32)SHADER_PARAM_COUNT, numParams + 1);
	}

	/* Sets a shader int array parameter
	 */
	void Graphics::SetShaderIntArray(int paramID, array<::System::Int32>^ values)
	{
		if(values->Length == 0)
			return;

		if((mScratchPos + values->Length * 4) >= SCRATCH_SIZE)
			return;

		int pos = mScratchPos;
		for(int i = 0; i < values->Length; i++)
		{
			*((int*)&mScratchMemory[mScratchPos]) = values[i];
			mScratchPos += 4;
		}
		
		mScratchPos = ((mScratchPos + 3) & 3); // 4-byte aligned

		uintptr_t numParams = 0;
		mOtterGraphics->GetProperty((uint32)SHADER_PARAM_COUNT, &numParams);

		mOtterGraphics->SetProperty((uint32)SHADER_PARAM_TYPE + numParams, 2);	// 2 : IntArray
		mOtterGraphics->SetProperty((uint32)SHADER_PARAM_ID + numParams, paramID);
		mOtterGraphics->SetProperty((uint32)SHADER_PARAM_DATA + numParams, (uintptr_t)&mScratchMemory[pos]);
		mOtterGraphics->SetProperty((uint32)SHADER_PARAM_DATA_LEN + numParams, values->Length);

		mOtterGraphics->SetProperty((uint32)SHADER_PARAM_COUNT, numParams + 1);
	}

	/* Sets a shader int parameter
	 */
	void Graphics::SetShaderInt(int paramID, int value)
	{
		uintptr_t numParams = 0;
		mOtterGraphics->GetProperty((uint32)SHADER_PARAM_COUNT, &numParams);

		mOtterGraphics->SetProperty((uint32)SHADER_PARAM_TYPE + numParams, 3);	// 3 : Int
		mOtterGraphics->SetProperty((uint32)SHADER_PARAM_ID + numParams, paramID);
		mOtterGraphics->SetProperty((uint32)SHADER_PARAM_DATA + numParams, value);

		mOtterGraphics->SetProperty((uint32)SHADER_PARAM_COUNT, numParams + 1);
	}

	/* Draws a rectangle
	 */
	void Graphics::DrawRectangle(int textureID, float x, float y, float width, float height, float u1, float v1, float u2, float v2, int color, float skewAngle, int maskID)
	{
		if(!mOtterGraphics)
			return;

		mOtterGraphics->DrawRectangle(textureID, x, y, width, height, u1, v1, u2, v2, color, skewAngle, false, maskID);
	}

	/* Draws a rectangle
	 */
	void Graphics::DrawRectangle(int textureID, float x, float y, float width, float height, int color)
	{
		if(!mOtterGraphics)
			return;

		mOtterGraphics->DrawRectangle(textureID, x, y, width, height, color);
	}

	/* Draws a set of triangles
	 */
	void Graphics::DrawTriangles(int textureID, array<Triangle^>^ triangles)
	{
		if(!mOtterGraphics)
			return;

		GUIVertex* verts = new GUIVertex[triangles->Length * 3];

		for(int i = 0; i < triangles->Length; i++)
		{
			for(int j = 0; j < 3; j++)
			{
				verts[i * 3 + j].mPosition.x = triangles[i]->mVertices[j]->mPosition[0];
				verts[i * 3 + j].mPosition.y = triangles[i]->mVertices[j]->mPosition[1];
				verts[i * 3 + j].mPosition.z = triangles[i]->mVertices[j]->mPosition[2];

				verts[i * 3 + j].mTexCoord.x = triangles[i]->mVertices[j]->mUV[0];
				verts[i * 3 + j].mTexCoord.y = triangles[i]->mVertices[j]->mUV[1];

				verts[i * 3 + j].mColor = triangles[i]->mVertices[j]->mColor;
			}
		}

		mOtterGraphics->DrawPrimitives(textureID, kPrim_TriangleList, triangles->Length, verts);

		delete[] verts;
	}

	/* Draws a string
	 */
	void Graphics::DrawString(int fontID, ::System::String^ text, float x, float y, float width, float height, int color, float scaleX, float scaleY, int halign, int valign, float leading, int tracking, float skewAngle, uint32 textFit, int dropShadow, int maskID)
	{
		if(!mOtterGraphics || text->Length == 0)
			return;

		FontInfo^ info = nullptr;
		int len = mFonts.Count;
		for(int i = 0; i < len; i++)
		{
			if(mFonts[i]->mFontID == fontID)
			{
				info = mFonts[i];
				break; 
			}
		}

		if(!info)
			return;

		const wchar_t* szText = (const wchar_t*)(void*)Marshal::StringToHGlobalUni (text);

		UTF8String string(szText);
		info->mFont->Draw(mOtterGraphics, UTF8String(szText), x, y, width, height, scaleX, scaleY, (Otter::HoriAlignment)halign, (Otter::VertAlignment)valign, color, VectorMath::Vector2::ZERO, 0.0f, leading, tracking, skewAngle, textFit, dropShadow, maskID);

		Marshal::FreeHGlobal((::System::IntPtr)(void*)szText);		
	}

	/* Draws a line
	 */
	void Graphics::DrawLine(float sx, float sy, float sz, float dx, float dy, float dz, int color)
	{
		if(!mOtterGraphics)
			return;

		mOtterGraphics->DrawLine(sx, sy, sz, dx, dy, dz, color);
	}

	void Graphics::DrawRectangleStencil(int textureID, float x, float y, float width, float height, float u1, float v1, float u2, float v2, int color, float skewAngle, int maskID)
	{
		if(!mOtterGraphics)
			return;

		mOtterGraphics->DrawRectangle(textureID, x, y, width, height, u1, v1, u2, v2, color, skewAngle, true, maskID);
	}
}
}