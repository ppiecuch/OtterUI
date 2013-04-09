#pragma once

#include "Common/Types.h"
#include "Otter.h"

namespace Otter
{
	/* Plugin Interface
	 */
	class IPlugin
	{
	public:
		/**
		 * Constructor
		 */
		IPlugin() { }

		/**
		 * Virtual Destructor
		 */
		virtual ~IPlugin() { }

	public:

		/**
		 * Creates a control from the provided control data
		 */
		virtual Otter::Control* CreateControl(Scene* pScene, const ControlData* pControlData, Control* pParent)
		{
			return NULL;
		}

		/**
		 * Destroys a previously created control
		 */
		virtual bool DestroyControl(Otter::Control* pControl)
		{
			return false;
		}
	};
}