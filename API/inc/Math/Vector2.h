#pragma once

namespace VectorMath
{
	/* 2 Dimension Vector
	 */
	struct Vector2
	{
	public:

		/**
		 * Default Constructor
		 */
		Vector2()
		{
			x = 0.0f;
			y = 0.0f;
		}

		/**
		 * Constructs the vector with specific values.
		 */
		Vector2(float fX, float fY)
		{
			this->x = fX;
			this->y = fY;
		}

	public:
		static const Vector2 ZERO;
		static const Vector2 UNIT_X;
		static const Vector2 UNIT_Y;

	public:

		union
		{
			float mEntry[2];
			struct
			{
				float x;
				float y;
			};
		};
	};
};