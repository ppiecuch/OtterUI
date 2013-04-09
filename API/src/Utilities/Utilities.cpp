#include <math.h>
#include "Utilities.h"
#include "Math/VectorMath.h"
#include "../Memory/Memory.h"

namespace Otter
{
	/* Computes an anchored rectangle based on its parents and anchor settings.
	 */
	void ComputeAnchoredRectangle(Rectangle& target, Point& center, const Rectangle& parent, AnchorFlags flags, float leftValue, float rightValue, float topValue, float bottomValue)
	{
		float left = target.x;
		float right = target.x + target.w;
		float top = target.y;
		float bottom = target.y + target.h;

		float centerRatioW = (target.w != 0) ? (center.x / target.w) : 0.0f;
		float centerRatioH = (target.h != 0) ? (center.y / target.h) : 0.0f;

		bool bUpdatedL = false;
		bool bUpdatedR = false;
		bool bUpdatedT = false;
		bool bUpdatedB = false;

		
		if ((flags & Otter::Left) != 0)
		{
			// Nothing to do for left
			bUpdatedL = true;
		}
		else if ((flags & Otter::LeftRelative) != 0)
		{
			left = leftValue * parent.w;
			bUpdatedL = true;
		}

		
		if ((flags & Otter::Right) != 0)
		{
			right = (parent.x + parent.w) - rightValue;
			bUpdatedR = true;
		}
		else if ((flags & Otter::RightRelative) != 0)
		{
			right = rightValue * parent.w;
			bUpdatedR = true;
		}

		if (bUpdatedL != bUpdatedR)
		{
			if (bUpdatedL)
				right = left + target.w;
			else
				left = right - target.w;
		}

		
		if ((flags & Otter::Top) != 0)
		{
			// Nothing to do for top
			bUpdatedT = true;
		}
		else if ((flags & Otter::TopRelative) != 0)
		{
			top = topValue * parent.h;
			bUpdatedT = true;
		}

		if ((flags & Otter::Bottom) != 0)
		{
			bottom = (parent.y + parent.h) - bottomValue;
			bUpdatedB = true;
		}
		else if((flags & Otter::BottomRelative) != 0)
		{
			bottom = bottomValue * parent.h;
			bUpdatedB = true;
		}

		if (bUpdatedT != bUpdatedB)
		{
			if (bUpdatedT)
				bottom = top + target.h;
			else
				top = bottom - target.h;
		}

		//if (bMaintainProportions)
		//{
		//    float neww = (bottom - top) * ratio;
		//    if ((Anchor & (AnchorFlags::x | AnchorFlags::Right)) != (AnchorFlags::x | AnchorFlags::Right))
		//    {
		//        if ((Anchor & AnchorFlags::Right) != 0)
		//        {
		//            left = right - neww;
		//        }
		//        else if ((Anchor & AnchorFlags::x) != 0)
		//        {
		//            right = left + neww;
		//        }
		//        else
		//        {
		//            left = left + ((right - left) / 2.0f) - neww / 2.0f;
		//            right = left + neww;
		//        }
		//    }
		//}
	    

		target.x = left;
		target.y = top;
		target.w = (right - left);
		target.h = (bottom - top);

		center.x = target.w * centerRatioW;
		center.y = target.h * centerRatioH;
	}

	/* Calculates the ease in amount
	 */
	float EaseIn(float factor, float power)
	{
		return VectorMath::Functions::Pow(factor, power);
	}

	/* Calculates the ease out amount
	 */
	float EaseOut(float factor, float power)
	{
		return 1.0f - VectorMath::Functions::Pow(1.0f - factor, power);
	}

	/* Calculates the ease in/out amount based on the initial factor and ease modified.
	 * easeType : 0 - None, 1 - Ease In, 2 - Ease Out, 3 - Ease In-Out
	 */
	float CalculateEase(float factor, int easeType, int easeAmount)
	{
		if(easeType == 0 || easeAmount == 0)
			return factor;

		easeAmount = Clamp(easeAmount, 0, 100);

		float easePower = 1.0f + 4.0f * (VectorMath::Functions::FAbs((float)easeAmount)/100.0f);

		if(easeType == 1)
			return EaseIn(factor, easePower);

		if(easeType == 2)
			return EaseOut(factor, easePower);

		if(factor < 0.5f)
			return EaseIn(factor * 2.0f, easePower) / 2.0f;

		return (EaseOut((factor - 0.5f) * 2.0f, easePower) / 2.0f) + 0.5f; 
	}
}