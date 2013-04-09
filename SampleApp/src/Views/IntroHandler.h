#pragma once
#include "ViewHandler.h"

class IntroHandler : public ViewHandler
{
public:
	/* Constructor
	 */
	IntroHandler(SampleUI* pSampleUI, View* pView);

	/* Virtual Destructor
	 */
	virtual ~IntroHandler(void);

private:
	/** @brief Executed when an animation has ended
	 */
	void	OnAnimationEnded(void* pSender, uint32 animID);
};
