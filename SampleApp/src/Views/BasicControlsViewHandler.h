#pragma once
#include "ViewHandler.h"

/* Handles the events from the main menu
 */
class BasicControlsViewHandler : public ViewHandler
{
public:

	/* Constructor
	 */
	BasicControlsViewHandler(SampleUI* pSampleUI, View* pView);

	/* Virtual Destructor
	 */
	virtual ~BasicControlsViewHandler(void);

private:

	/** @brief Executed when the view has become active
	 */
	void	OnActivate(void* pSender, void* pContext);

	/** @brief Executed when the view has become deactive
	 */
	void	OnDeactivate(void* pSender, void* pContext);

	/** @brief Executed when an animation has completed
	 */
	void	OnAnimationEnded(void* pSender, uint32 animID);

	/* Called when the Next Button has been clicked
	 */
	void	OnNextButtonClicked(void* pSender, void* pData);

	/* Called when the Prev Button has been clicked
	 */
	void	OnPrevButtonClicked(void* pSender, void* pData);

	/* Called when the "Hit Me!" Button has been clicked
	 */
	void	OnHitMeButtonClicked(void* pSender, void* pData);

	/* Called when the Alpha Slider's value has changed
	 */
	void	OnAlphaSliderValueChanged(void* pSender, sint32 value);

	/* Called when the view receives a frame message action
	 */
	void	OnMessage(void* pSender, const Otter::MessageArgs& args);	

private:

	Button*						mNextButton;
	Button*						mPrevButton;
	Button*						mHitMeButton;

	Slider*						mAlphaSlider;
};
