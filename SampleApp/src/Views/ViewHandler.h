#pragma once

#include "Otter.h"

using namespace Otter;

class SampleUI;

/**
 * Simple base view handler class
 */
class ViewHandler
{
public:
	/**
	 * Constructor
	 */
	ViewHandler(SampleUI* pSampleUI, View* pView)
	{
		mSampleUI = pSampleUI;
		mView = pView;
	}

	/** Virtual Destructor
	 */
	virtual ~ViewHandler(void)
	{
	}

public:
	/** @brief Retrieves the view associated with the main menu
	 */
	View*	GetView() { return mView; }

protected:

	SampleUI*					mSampleUI;
	View*						mView;
};

