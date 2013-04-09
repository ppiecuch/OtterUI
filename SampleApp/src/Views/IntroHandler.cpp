#include "IntroHandler.h"

/* Constructor
 */
IntroHandler::IntroHandler(SampleUI* pSampleUI, View* pView) : ViewHandler(pSampleUI, pView)
{
	mView->mOnAnimationEnded.AddHandler(this, &IntroHandler::OnAnimationEnded);
}

/* Virtual Destructor
 */
IntroHandler::~IntroHandler(void) 
{
	mView->mOnAnimationEnded.RemoveHandler(this, &IntroHandler::OnAnimationEnded);
}

/** @brief Executed when an animation has ended
 */  
void IntroHandler::OnAnimationEnded(void* pSender, uint32 animID)
{	 
	if(animID == ANIM_ONACTIVATE)
	{
		// Switch views
		mView->GetScene()->DeactivateView("Intro");
		mView->GetScene()->ActivateView("BasicControlsView");
	}
}