#include "LabelsViewHandler.h"
#include "SampleUI.h"

/* Constructor
 */
LabelsViewHandler::LabelsViewHandler(SampleUI* pSampleUI, View* pView) : ViewHandler(pSampleUI, pView)
{
	mNextButton			= NULL;
	mPrevButton			= NULL;

	mView->mOnActivate.AddHandler(this, &LabelsViewHandler::OnActivate);
	mView->mOnDeactivate.AddHandler(this, &LabelsViewHandler::OnDeactivate);
	mView->mOnAnimationEnded.AddHandler(this, &LabelsViewHandler::OnAnimationEnded);
}

/* Virtual Destructor
 */
LabelsViewHandler::~LabelsViewHandler(void)
{
	mView->mOnActivate.RemoveHandler(this, &LabelsViewHandler::OnActivate);
	mView->mOnDeactivate.RemoveHandler(this, &LabelsViewHandler::OnDeactivate);
	mView->mOnAnimationEnded.RemoveHandler(this, &LabelsViewHandler::OnAnimationEnded);
}

/** Executed when the view has become active
 * 
 * We need to pull the various controls that we want to operate on in here.
 * In particular, we want to listen for events when the user hits the buttons.
 */
void LabelsViewHandler::OnActivate(void* pSender, void* pContext)
{
	mNextButton = (Button*)mView->GetControl("NextButton");
	if(mNextButton)
		mNextButton->mOnClick.AddHandler(this, &LabelsViewHandler::OnNextButtonClicked);

	mPrevButton = (Button*)mView->GetControl("PrevButton");
	if(mPrevButton)
		mPrevButton->mOnClick.AddHandler(this, &LabelsViewHandler::OnPrevButtonClicked);
}

/** @brief Executed when the view has become deactive
 */
void LabelsViewHandler::OnDeactivate(void* pSender, void* pContext)
{
	if(mNextButton)
	{
		mNextButton->mOnClick.RemoveHandler(this, &LabelsViewHandler::OnNextButtonClicked);
		mNextButton = NULL;
	}

	if(mPrevButton)
	{
		mPrevButton->mOnClick.RemoveHandler(this, &LabelsViewHandler::OnPrevButtonClicked);
		mPrevButton = NULL;
	}
}

/** @brief Executed when an animation has completed
 */
void LabelsViewHandler::OnAnimationEnded(void* pSender, uint32 animID)
{

}

/* Called when the Next Button has been clicked
 */
void LabelsViewHandler::OnNextButtonClicked(void* pSender, void* pData)
{
	mView->StopAnimation(ANIM_ONACTIVATE);
	mView->GetScene()->DeactivateView(mView);
	mView->GetScene()->ActivateView("TablesView");
}

/* Called when the Prev Button has been clicked
 */
void LabelsViewHandler::OnPrevButtonClicked(void* pSender, void* pData)
{
	mView->StopAnimation(ANIM_ONACTIVATE);
	mView->GetScene()->DeactivateView(mView);
	mView->GetScene()->ActivateView("BasicControlsView");
}