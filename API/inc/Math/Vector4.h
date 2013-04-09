#pragma once

namespace VectorMath
{
	/**
     * 4-Dimensional Vector
	 */
	struct Vector4
	{
	public:

		/**
		 * Default Constructor
		 */
		Vector4()
		{
			x = 0.0f;
			y = 0.0f;
			z = 0.0f;
			w = 0.0f;
		}

		/**
		 * Constructs the vector with specific values.
		 */
		Vector4(float fX, float fY, float fZ, float fW)
		{
			this->x = fX;
			this->y = fY;
			this->z = fZ;
			this->w = fW;
		}

	public:
		static const Vector4 ZERO;
		static const Vector4 UNIT_X;
		static const Vector4 UNIT_Y;
		static const Vector4 UNIT_Z;
		static const Vector4 UNIT_W;

	public:

		union
		{
			float mEntry[4];
			struct
			{
				float x;
				float y;
				float z;
				float w;
			};
		};
	};
};