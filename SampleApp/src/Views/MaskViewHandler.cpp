#include "MaskViewHandler.h"
#include "SampleUI.h"

/* Constructor
 */
MaskViewHandler::MaskViewHandler(SampleUI* pSampleUI, View* pView) : ViewHandler(pSampleUI, pView)
{
	mNextButton			= NULL;
	mPrevButton			= NULL;

	mView->mOnActivate.AddHandler(this, &MaskViewHandler::OnActivate);
	mView->mOnDeactivate.AddHandler(this, &MaskViewHandler::OnDeactivate);
	mView->mOnAnimationEnded.AddHandler(this, &MaskViewHandler::OnAnimationEnded);
}

/* Virtual Destructor
 */
MaskViewHandler::~MaskViewHandler(void)
{
	mView->mOnActivate.RemoveHandler(this, &MaskViewHandler::OnActivate);
	mView->mOnDeactivate.RemoveHandler(this, &MaskViewHandler::OnDeactivate);
	mView->mOnAnimationEnded.RemoveHandler(this, &MaskViewHandler::OnAnimationEnded);
}

/** Executed when the view has become active
 * 
 * We need to pull the various controls that we want to operate on in here.
 * In particular, we want to listen for events when the user hits the buttons.
 */
void MaskViewHandler::OnActivate(void* pSender, void* pContext)
{
	mNextButton = (Button*)mView->GetControl("NextButton");
	if(mNextButton)
		mNextButton->mOnClick.AddHandler(this, &MaskViewHandler::OnNextButtonClicked);

	mPrevButton = (Button*)mView->GetControl("PrevButton");
	if(mPrevButton)
		mPrevButton->mOnClick.AddHandler(this, &MaskViewHandler::OnPrevButtonClicked);
}

/** @brief Executed when the view has become deactive
 */
void MaskViewHandler::OnDeactivate(void* pSender, void* pContext)
{
	if(mNextButton)
	{
		mNextButton->mOnClick.RemoveHandler(this, &MaskViewHandler::OnNextButtonClicked);
		mNextButton = NULL;
	}

	if(mPrevButton)
	{
		mPrevButton->mOnClick.RemoveHandler(this, &MaskViewHandler::OnPrevButtonClicked);
		mPrevButton = NULL;
	}
}

/** @brief Executed when an animation has completed
 */
void MaskViewHandler::OnAnimationEnded(void* pSender, uint32 animID)
{

}

/* Called when the Next Button has been clicked
 */
void MaskViewHandler::OnNextButtonClicked(void* pSender, void* pData)
{
	mView->StopAnimation(ANIM_ONACTIVATE);
	mView->GetScene()->DeactivateView(mView);
	mView->GetScene()->ActivateView("BasicControlsView");
}

/* Called when the Prev Button has been clicked
 */
void MaskViewHandler::OnPrevButtonClicked(void* pSender, void* pData)
{
	mView->StopAnimation(ANIM_ONACTIVATE);
	mView->GetScene()->DeactivateView(mView);
	mView->GetScene()->ActivateView("TablesView");
}