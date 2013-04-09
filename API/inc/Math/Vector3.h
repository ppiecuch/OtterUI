#pragma once

namespace VectorMath
{
	/* 3-Dimension Vector
	 */
	struct Vector3
	{
	public:

		/**
		 * Default Constructor
		 */
		Vector3()
		{
			x = 0.0f;
			y = 0.0f;
			z = 0.0f;
		}

		/**
		 * Constructs the vector with specific values.
		 */
		Vector3(float fX, float fY, float fZ)
		{
			this->x = fX;
			this->y = fY;
			this->z = fZ;
		}

	public:
		static const Vector3 ZERO;
		static const Vector3 UNIT_X;
		static const Vector3 UNIT_Y;
		static const Vector3 UNIT_Z;

	public:

		union
		{
			float mEntry[3];
			struct
			{
				float x;
				float y;
				float z;
			};
		};
	};
};