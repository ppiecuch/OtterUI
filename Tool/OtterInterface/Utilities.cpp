#include "StdAfx.h"
#include "Utilities.h"
#include "Common/Types.h"
#include "Common/UTF8String.h"

#include "Utilities/Utilities.h"

namespace Otter
{
namespace Interface
{
	/* Constructor
	 */
	Utilities::Utilities(void)
	{
	}

	/* C++/CLI Wrapper for Otter::ComputerAnchoredRectangle
	 */
	void Utilities::ComputeAnchoredRectangle(	RectangleF% controlRect, 
                                   				PointF% controlCenter,
                                   				RectangleF% parentRect, 
								   				unsigned int anchorFlags,
                                   				float leftValue,
                                   				float rightValue,
                                   				float topValue,
                                   				float bottomValue)
    {
		Otter::Rectangle crect(controlRect.X, controlRect.Y, controlRect.Width, controlRect.Height);
		Otter::Point ccenter(controlCenter.X, controlCenter.Y);
		Otter::Rectangle prect(parentRect.X, parentRect.Y, parentRect.Width, parentRect.Height);

		Otter::ComputeAnchoredRectangle(crect, ccenter, prect, (Otter::AnchorFlags)anchorFlags, leftValue, rightValue, topValue, bottomValue);

		controlRect.X = crect.x;
		controlRect.Y = crect.y;
		controlRect.Width = crect.w;
		controlRect.Height = crect.h;

		controlCenter.X = ccenter.x;
		controlCenter.Y = ccenter.y;
    }

	/* C++/CLI Wrapper for Otter::CalculateEase
	 */
	float Utilities::CalculateEase(float factor, int easeType, int easeAmount)
	{
		return Otter::CalculateEase(factor, easeType, easeAmount);
	}
}
}
