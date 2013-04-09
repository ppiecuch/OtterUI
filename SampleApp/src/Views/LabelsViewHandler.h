#pragma once
#include "ViewHandler.h"

/* Handles the events from the main menu
 */
class LabelsViewHandler : public ViewHandler
{
public:

	/* Constructor
	 */
	LabelsViewHandler(SampleUI* pSampleUI, View* pView);

	/* Virtual Destructor
	 */
	virtual ~LabelsViewHandler(void);

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

private:

	Button*						mNextButton;
	Button*						mPrevButton;
};
