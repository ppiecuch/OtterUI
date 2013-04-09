#pragma once

using namespace System::Drawing;

namespace Otter
{
namespace Interface
{
	/* Shared utilities and functionality between the Otter Editor and the API.
	 */
	public ref class Utilities
	{
	public:
		Utilities(void);

	public:
		/* C++/CLI Wrapper for Otter::ComputerAnchoredRectangle
		 */
		static void ComputeAnchoredRectangle(	RectangleF% controlRect, 
                                   				PointF% controlCenter,
                                   				RectangleF% parentRect, 
								   				unsigned int anchorFlags,
                                   				float leftValue,
                                   				float rightValue,
                                   				float topValue,
                                   				float bottomValue);

		/* C++/CLI Wrapper for Otter::CalculateEase
		 */
		static float CalculateEase(float factor, int easeType, int easeAmount);
	};
}
}