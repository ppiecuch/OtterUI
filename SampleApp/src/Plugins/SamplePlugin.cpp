#include "SamplePlugin.h"
#include "Circle.h"

/* Constructor
 */ 
SamplePlugin::SamplePlugin(void)
{
}

/* Virtual Destructor
 */
SamplePlugin::~SamplePlugin(void)
{
}

/* Creates a control from the provided control data
 */
Otter::Control* SamplePlugin::CreateControl(Otter::Scene* pScene, const Otter::ControlData* pControlData, Otter::Control* pParent)
{
	if(pControlData == NULL)
		return NULL;

	
	if(pControlData->mFourCC == FOURCC_CIRC)
	{
		return new Circle(pScene, pParent, (CircleData*)pControlData);
	}

	return NULL;
}

/* Destroys a previously created control
 */
bool SamplePlugin::DestroyControl(Otter::Control* pControl)
{
	if(!pControl)
		return false;

	const Otter::ControlData* pControlData = pControl->GetData();
	if(pControlData->mFourCC != FOURCC_CIRC)
		return false;
	
	delete pControl;
	return true;
}