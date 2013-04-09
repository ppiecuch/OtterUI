#pragma once

namespace Otter
{
namespace Interface
{	
	public ref class Matrix
	{
	public:
		Matrix();

	public:

		static Matrix^ operator*(Matrix^ lhs, Matrix^ rhs);

	public:

		static Matrix^ RotationZ(float angle);
		static Matrix^ Translation(float x, float y, float z);
		static Matrix^ Invert(Matrix^ matrix);
		static int I(int r, int c) { return c * 4 + r; } 

	public:
		property static Matrix^ Identity
		{
			Matrix^ get()
			{
				return gcnew Matrix();
			}
		}

		property float M11
		{
			float get()
			{
				return mEntries[I(0, 0)];
			}
			void set(float value)
			{
				mEntries[I(0, 0)] = value;
			}
		}

		property float M12
		{
			float get()
			{
				return mEntries[I(0, 1)];
			}
			void set(float value)
			{
				mEntries[I(0, 1)] = value;
			}
		}

		property float M13
		{
			float get()
			{
				return mEntries[I(0, 2)];
			}
			void set(float value)
			{
				mEntries[I(0, 2)] = value;
			}
		}

		property float M14
		{
			float get()
			{
				return mEntries[I(0, 3)];
			}
			void set(float value)
			{
				mEntries[I(0, 3)] = value;
			}
		}

		property float M21
		{
			float get()
			{
				return mEntries[I(1, 0)];
			}
			void set(float value)
			{
				mEntries[I(1, 0)] = value;
			}
		}

		property float M22
		{
			float get()
			{
				return mEntries[I(1, 1)];
			}
			void set(float value)
			{
				mEntries[I(1, 1)] = value;
			}
		}

		property float M23
		{
			float get()
			{
				return mEntries[I(1, 2)];
			}
			void set(float value)
			{
				mEntries[I(1, 2)] = value;
			}
		}

		property float M24
		{
			float get()
			{
				return mEntries[I(1, 3)];
			}
			void set(float value)
			{
				mEntries[I(1, 3)] = value;
			}
		}

		property float M31
		{
			float get()
			{
				return mEntries[I(2, 0)];
			}
			void set(float value)
			{
				mEntries[I(2, 0)] = value;
			}
		}

		property float M32
		{
			float get()
			{
				return mEntries[I(2, 1)];
			}
			void set(float value)
			{
				mEntries[I(2, 1)] = value;
			}
		}

		property float M33
		{
			float get()
			{
				return mEntries[I(2, 2)];
			}
			void set(float value)
			{
				mEntries[I(2, 2)] = value;
			}
		}

		property float M34
		{
			float get()
			{
				return mEntries[I(2, 3)];
			}
			void set(float value)
			{
				mEntries[I(2, 3)] = value;
			}
		}

		property float M41
		{
			float get()
			{
				return mEntries[I(3, 0)];
			}
			void set(float value)
			{
				mEntries[I(3, 0)] = value;
			}
		}

		property float M42
		{
			float get()
			{
				return mEntries[I(3, 1)];
			}
			void set(float value)
			{
				mEntries[I(3, 1)] = value;
			}
		}

		property float M43
		{
			float get()
			{
				return mEntries[I(3, 2)];
			}
			void set(float value)
			{
				mEntries[I(3, 2)] = value;
			}
		}

		property float M44
		{
			float get()
			{
				return mEntries[I(3, 3)];
			}
			void set(float value)
			{
				mEntries[I(3, 3)] = value;
			}
		}

		property array<float>^ Entries
		{
			array<float>^ get()
			{
				return mEntries;
			}
		}

	private:
		
		array<float>^ mEntries;
	};
}
}