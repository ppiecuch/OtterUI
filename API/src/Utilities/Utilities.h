#pragma once

#include "Common/Types.h"

namespace Otter
{
	/* Computes an anchored rectangle based on its parents and anchor settings.
	 * NOTE:  We really need to find this function a better home.
	 */
	void ComputeAnchoredRectangle(Rectangle& target, Point& center, const Rectangle& parent, AnchorFlags flags, float leftValue, float rightValue, float topValue, float bottomValue);

	/* Calculates the ease in/out amount based on the initial factor and ease modified.
	 * easeType : 0 - None, 1 - Ease In, 2 - Ease Out, 3 - Ease In-Out
	 */
	float CalculateEase(float factor, int easeType, int easeAmount);
	
	template<class T>
	T Clamp(T x, T min, T max)
	{
		return (x < min ? min : (x > max ? max : x));
	}

	template<class T>
	T Max(T a, T b)
	{
		return (a > b) ? a : b;
	}

	template<class T>
	void Swap(T& a, T& b)
	{
		T t = a;
		a = b;
		b = t;
	}
}
