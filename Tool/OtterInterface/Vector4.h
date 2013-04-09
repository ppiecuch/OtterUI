#pragma once

#include "Matrix.h"

namespace Otter
{
namespace Interface
{	
	public ref class Vector4
	{
	public:
		Vector4()
		{
			mEntry = gcnew array<float>(4);
			X = 0.0f;
			Y = 0.0f;
			Z = 0.0f;
			W = 0.0f;
		}

		Vector4(float x, float y, float z, float w)
		{
			mEntry = gcnew array<float>(4);
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

	public:

		static Vector4^ Transform(Vector4^ vector, Matrix^ matrix);
		
	public:
		array<float>^ mEntry;

		property float X 
		{
			float get()
			{
				return mEntry[0];
			}
			void set(float value)
			{
				mEntry[0] = value;
			}
		}

		property float Y
		{
			float get()
			{
				return mEntry[1];
			}
			void set(float value)
			{
				mEntry[1] = value;
			}
		}

		property float Z
		{
			float get()
			{
				return mEntry[2];
			}
			void set(float value)
			{
				mEntry[2] = value;
			}
		}

		property float W
		{
			float get()
			{
				return mEntry[3];
			}
			void set(float value)
			{
				mEntry[3] = value;
			}
		}

	};
}
}