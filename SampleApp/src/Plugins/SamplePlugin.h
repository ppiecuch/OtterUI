#pragma once

#include "Otter.h"

class SamplePlugin : public Otter::IPlugin
{
public:
	SamplePlugin(void);
	virtual ~SamplePlugin(void);

public:

	/* Creates a control from the provided control data
	 */
	virtual Otter::Control* CreateControl(Otter::Scene* pScene, const Otter::ControlData* pControlData, Otter::Control* pParent);

	/* Destroys a previously created control
	 */
	virtual bool DestroyControl(Otter::Control* pControl);
};

