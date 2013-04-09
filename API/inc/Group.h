#pragma once
#include "Control.h"

namespace Otter
{
	struct GroupData;

	/**
	 * Control that contains a set of child controls
	 */
	class Group : public Control
	{
	public:
		/**
		 *  Constructor
		 */
		Group(Scene* pScene, Control* pParent, const GroupData* pGroupData);

		/* Constructor
		 */
		Group();

		/**
		 * Virtual Destructor
		 */
		virtual ~Group(void);

	public:

		/**
		 * Clones the control and returns the new instance
		 */
		virtual Control* Clone();
	
		/**
		 * Draws the sprite to screen
		 */
		virtual void Draw(Graphics* pGraphics);
	};
}