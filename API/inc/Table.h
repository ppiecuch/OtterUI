#pragma once
#include "Control.h"

namespace Otter
{
	struct TableData;
	class View;
	class Row;

	/**
	 * The Table Control maintains a scrollable list of rows, with each row containing a set of child controls.
	 */
	class Table : public Control
	{
	public:
		/**
		 * Constructor
		 */
		Table(Scene* pScene, Control* pParent, const TableData* pTableData);

		/** 
		 * Constructor
		 */
		Table();

		/**
		 * Destructor
		 */
		virtual ~Table(void);

	public:

		/**
		 * Creates a row and adds it to the end of the table
		 */
		Result	AddRow(Row* pRow);

		/**
		 * Retrieves the total number of rows in the table
		 */
		uint32	GetNumRows();

		/**
		 * Retrieves a row at an index
		 */
		Row*	GetRow(uint32 atIndex);

		/**
		 * Removes a row at a specific index
		 */
		Result	RemoveRow(uint32 atIndex);

		/**
		 * Enables / Disables scrolling of the table control
		 */
		Result	EnableScrolling(bool bEnable = true);

	public:
		
		/**
		 * Clones the control and returns the new instance
		 */
		virtual Control* Clone();

		/**
		 * Draws the table to screen
		 */
		virtual void Draw(Graphics* pGraphics);

		/**
		 * Applies the interpolation of two keyframes to the control.
		 * pEndFrame can be NULL.
		 */
		virtual void ApplyKeyFrame(const KeyFrameData* pStartFrame, const KeyFrameData* pEndFrame, float factor);
		
	public:

		/**
		 * Points (touches/mouse/etc) were pressed down
		 */
		virtual bool OnPointsDown(Point* points, sint32 numPoints);
		
		/**
		 * Points (touches/mouse/etc) were released
		 */
		virtual bool OnPointsUp(Point* points, sint32 numPoints);
		
		/**
		 * Points (touches/mouse/etc) were moved.
		 */
		virtual bool OnPointsMove(Point* points, sint32 numPoints);

	private:

		Array<Row*>	mRows;
		Point		mLastPoint;

		bool		mEnableScrolling;
	};

	/**
	 * Defines a single row in a table
	 */
	class Row : public Control
	{
	public:

		/**
		 * Constructor
		 */
		Row(float height);

		/**
		 * Destructor
		 */
		virtual ~Row();

	public:

		/**
		 * Clones the row and all of its internal controls
		 */
		virtual Control* Clone();
		
		/**
		 * Retrieves a control by point.
		 * Unlike the base Control::GetControl, this implementation will never return the row
		 * itself.
		 */
		virtual Control* GetControl(Point& point, Point* localPoint, bool touchablesOnly);
	};
}