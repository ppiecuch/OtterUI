#include <float.h>
#include <math.h>
#include <limits.h>
#include <stdlib.h>

#include "Math/VectorMath.h"

namespace VectorMath
{	
	const float Constants::EPSILON			= FLT_EPSILON;
	const float Constants::ZERO_TOLERANCE	= 1e-06f;
	const float Constants::MAX_REAL			= FLT_MAX;
	const float Constants::PI				= (float)(4.0f * atan(1.0f));
	const float Constants::TWO_PI			= 2.0f * Constants::PI;
	const float Constants::HALF_PI			= 0.5f * Constants::PI;
	const float Constants::INV_PI			= 1.0f / Constants::PI;
	const float Constants::INV_TWO_PI		= 1.0f / Constants::TWO_PI;
	const float Constants::DEG_TO_RAD		= Constants::PI / 180.0f;
	const float Constants::RAD_TO_DEG		= 180.0f / Constants::PI;

	/*
	 */
	float Functions::ACos (float fValue)
	{
		if ( -1.0f < fValue )
		{
			if ( fValue < 1.0f )
				return (float)acos((double)fValue);
			else
				return (float)0.0;
		}
		else
		{
			return Constants::PI;
		}
	}

	/*
	 */
	float Functions::ASin (float fValue)
	{
		if ( -1.0f < fValue )
		{
			if ( fValue < 1.0f )
				return (float)asin((double)fValue);
			else
				return Constants::HALF_PI;
		}
		else
		{
			return -Constants::HALF_PI;
		}
	}

	/*
	 */
	float Functions::ATan (float fValue)
	{
		return (float)atan((double)fValue);
	}

	/*
	 */
	float Functions::ATan2 (float fY, float fX)
	{
		return (float)atan2((double)fY,(double)fX);
	}

	/*
	 */
	float Functions::Ceil (float fValue)
	{
		return (float)ceil((double)fValue);
	}

	/*
	 */
	float Functions::Cos (float fValue)
	{
		return (float)cos((double)fValue);
	}

	/*
	 */
	float Functions::Exp (float fValue)
	{
		return (float)exp((double)fValue);
	}

	/*
	 */
	float Functions::FAbs (float fValue)
	{
		return (float)fabs((double)fValue);
	}

	/*
	 */
	float Functions::Floor (float fValue)
	{
		return (float)floor((double)fValue);
	}

	/*
	 */
	float Functions::FMod (float fX, float fY)
	{
		return (float)fmod((double)fX,(double)fY);
	}

	/*
	 */
	float Functions::Log (float fValue)
	{
		return (float)log((double)fValue);
	}

	/*
	 */
	float Functions::Pow (float fBase, float fExponent)
	{
		return (float)pow((double)fBase,(double)fExponent);
	}

	/*
	 */
	float Functions::Sin (float fValue)
	{
		return (float)sin((double)fValue);
	}

	/*
	 */
	float Functions::Sqr (float fValue)
	{
		return fValue*fValue;
	}

	/*
	 */
	float Functions::Sqrt (float fValue)
	{
		return (float)sqrt((double)fValue);
	}

	/*
	 */
	float Functions::Tan (float fValue)
	{
		return (float)tan((double)fValue);
	}
}