#include "BasicControlsViewHandler.h"
#include "SampleUI.h"

/* Constructor
 */
BasicControlsViewHandler::BasicControlsViewHandler(SampleUI* pSampleUI, View* pView) : ViewHandler(pSampleUI, pView)
{
	mNextButton			= NULL;
	mPrevButton			= NULL;
	mHitMeButton		= NULL;

	mView->mOnActivate.AddHandler(this, &BasicControlsViewHandler::OnActivate);
	mView->mOnDeactivate.AddHandler(this, &BasicControlsViewHandler::OnDeactivate);
	mView->mOnAnimationEnded.AddHandler(this, &BasicControlsViewHandler::OnAnimationEnded);
	mView->mOnMessage.AddHandler(this, &BasicControlsViewHandler::OnMessage);
}

/* Virtual Destructor
 */
BasicControlsViewHandler::~BasicControlsViewHandler(void)
{
	mView->mOnActivate.RemoveHandler(this, &BasicControlsViewHandler::OnActivate);
	mView->mOnDeactivate.RemoveHandler(this, &BasicControlsViewHandler::OnDeactivate);
	mView->mOnAnimationEnded.RemoveHandler(this, &BasicControlsViewHandler::OnAnimationEnded);
}

/** Executed when the view has become active
 * 
 * We need to pull the various controls that we want to operate on in here.
 * In particular, we want to listen for events when the user hits the buttons.
 */
void BasicControlsViewHandler::OnActivate(void* pSender, void* pContext)
{
	mNextButton = (Button*)mView->GetControl("NextButton");
	if(mNextButton)
		mNextButton->mOnClick.AddHandler(this, &BasicControlsViewHandler::OnNextButtonClicked);

	mPrevButton = (Button*)mView->GetControl("PrevButton");
	if(mPrevButton)
		mPrevButton->mOnClick.AddHandler(this, &BasicControlsViewHandler::OnPrevButtonClicked);

	mHitMeButton = (Button*)mView->GetControl("HitMeButton");
	if(mHitMeButton)
		mHitMeButton->mOnClick.AddHandler(this, &BasicControlsViewHandler::OnHitMeButtonClicked);

	mAlphaSlider = (Slider*)mView->GetControl("AlphaSlider");
	if(mAlphaSlider)
		mAlphaSlider->mOnValueChanged.AddHandler(this, &BasicControlsViewHandler::OnAlphaSliderValueChanged);
}

/** @brief Executed when the view has become deactive
 */
void BasicControlsViewHandler::OnDeactivate(void* pSender, void* pContext)
{
	if(mNextButton)
	{
		mNextButton->mOnClick.RemoveHandler(this, &BasicControlsViewHandler::OnNextButtonClicked);
		mNextButton = NULL;
	}

	if(mPrevButton)
	{
		mPrevButton->mOnClick.RemoveHandler(this, &BasicControlsViewHandler::OnPrevButtonClicked);
		mPrevButton = NULL;
	}

	if(mHitMeButton)
	{
		mHitMeButton->mOnClick.RemoveHandler(this, &BasicControlsViewHandler::OnHitMeButtonClicked);
		mHitMeButton = NULL;
	}

	if(mAlphaSlider)
	{
		mAlphaSlider->mOnValueChanged.RemoveHandler(this, &BasicControlsViewHandler::OnAlphaSliderValueChanged);
		mAlphaSlider = NULL;
	}
}

/** @brief Executed when an animation has completed
 */
void BasicControlsViewHandler::OnAnimationEnded(void* pSender, uint32 animID)
{

}

/* Called when the Next Button has been clicked
 */
void BasicControlsViewHandler::OnNextButtonClicked(void* pSender, void* pData)
{
	mView->StopAnimation(ANIM_ONACTIVATE);
	mView->GetScene()->DeactivateView(mView);
	mView->GetScene()->ActivateView("LabelsView");
}

/* Called when the Prev Button has been clicked
 */
void BasicControlsViewHandler::OnPrevButtonClicked(void* pSender, void* pData)
{
	mView->StopAnimation(ANIM_ONACTIVATE);
	mView->GetScene()->DeactivateView(mView);
	mView->GetScene()->ActivateView("MaskView");
}

/* Called when the "Hit Me!" Button has been clicked
 */
void BasicControlsViewHandler::OnHitMeButtonClicked(void* pSender, void* pData)
{
	mView->PlayAnimation("HitMe", 0, 0, true);
}

/* Called when the Alpha Slider's value has changed
 */
void BasicControlsViewHandler::OnAlphaSliderValueChanged(void* pSender, sint32 value)
{
	Sprite* pSprite = (Sprite*)mView->GetControl("OtterSprite");
	if(!pSprite)  
		return;

	char alpha = (char)((1.0f - (value / 100.0f)) * 255);
	pSprite->SetColor((alpha << 24) | 0x00FFFFFF);
}

/* Called when the view receives a frame message action
 */
void BasicControlsViewHandler::OnMessage(void* pSender, const Otter::MessageArgs& args)
{
	char tmp[256];
	sprintf(tmp, "Message: %s\n", args.mText);
	OutputDebugStringA(tmp);
}