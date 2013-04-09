#pragma once
#include "ViewHandler.h"

/* Handles the events from the tables view
 */
class TablesViewHandler : public ViewHandler
{
public:

	/* Constructor
	 */
	TablesViewHandler(SampleUI* pSampleUI, View* pView);

	/* Virtual Destructor
	 */
	virtual ~TablesViewHandler(void);

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

	/* Sets up the table with some sample rows
	 */
	void	SetupTable();

private:

	Button*						mNextButton;
	Button*						mPrevButton;

	Table*						mTable;
};
