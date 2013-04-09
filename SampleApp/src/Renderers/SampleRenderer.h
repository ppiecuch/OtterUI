#pragma once

#include "Otter.h"

/* Sample Renderer base class.
 */
class SampleRenderer : public Otter::IRenderer
{
public:

	/* Constructor
	*/
	SampleRenderer();

	/* Virtual Destructor
	*/
	virtual ~SampleRenderer(void);
	
protected:
	
	int		mWidth;
	int		mHeight;
};
