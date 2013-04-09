#pragma once

#include "Vector2.h"
#include "Vector3.h"
#include "Vector4.h"
#include "Matrix4.h"

namespace VectorMath
{
	struct Constants
	{
	public:
		static const float EPSILON;
		static const float ZERO_TOLERANCE;
		static const float MAX_REAL;
		static const float PI;
		static const float TWO_PI;
		static const float HALF_PI;
		static const float INV_PI;
		static const float INV_TWO_PI;
		static const float DEG_TO_RAD;
		static const float RAD_TO_DEG;
	};

	struct Functions
	{
	public:
		static float ACos(float fValue);
		static float ASin(float fValue);
		static float ATan(float fValue);
		static float ATan2(float fY, float fX);
		static float Ceil(float fValue);
		static float Cos(float fValue);
		static float Exp(float fValue);
		static float FAbs(float fValue);
		static float Floor(float fValue);
		static float FMod(float fX, float fY);
		static float InvSqr (float fValue);
		static float Log(float fValue);
		static float Pow(float fBase, float fExponent);
		static float Sin(float fValue);
		static float Sqr(float fValue);
		static float Sqrt(float fValue);
		static float Tan(float fValue);
	};
};