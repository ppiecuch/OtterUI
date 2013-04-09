#include <string.h>

#include "Table.h"
#include "View.h"

#include "Data/OtterData.h"
#include "Graphics/Graphics.h"
#include "Memory/Memory.h"

namespace Otter
{
	/* Constructor
	 */
	Table::Table(Otter::Scene* pScene, Otter::Control* pParent, const TableData* pTableData) : Otter::Control(pScene, pParent, pTableData)
	{
		mEnableScrolling = true;
	}

	/** 
	 * Constructor
	 */
	Table::Table() : Control(NULL, NULL, OTTER_NEW(TableData, ()))
	{
		mUserCreated = true;
	}
	 
	/* Destructor
	 */
	Table::~Table(void)
	{
		for(uint32 i = 0; i < mRows.size(); i++)
		{
			RemoveControl(mRows[i]);

			if(MemoryManager::isValid(mRows[i]))
				OTTER_DELETE(mRows[i]);
		}

		mRows.clear();
	}

	/* Creates a row and adds it to the end of the table
	 */
	Result Table::AddRow(Row* pRow)
	{
		if(!pRow)
			return kResult_InvalidParameter;

		const TableData* pTableData = static_cast<const TableData*>(GetData());
		if(!pTableData)
			return kResult_Error;

		uint32 numRows = mRows.size();
		float y = (numRows == 0) ? 0.0f : (mRows[numRows - 1]->GetPosition().y + mRows[numRows - 1]->GetSize().y);

		AddControl(pRow);
		mRows.push_back(pRow);

		const VectorMath::Vector2& size = GetSize();		
		pRow->SetSize(VectorMath::Vector2(size.x, pRow->GetSize().y));
		pRow->SetPosition(VectorMath::Vector2(0.0f, y + pTableData->mRowSpacing));

		return kResult_OK;
	}
	 
	/* Retrieves the total number of rows in the table
	 */
	uint32 Table::GetNumRows()
	{
		return mRows.size();
	}

	/* Retrieves a row at an index
	 */
	Row* Table::GetRow(uint32 atIndex)
	{
		if(atIndex >= mRows.size())
			return NULL;

		return mRows[atIndex];
	}

	/* Removes a row at a specific index
	 */
	Result Table::RemoveRow(uint32 atIndex)
	{
		if(atIndex >= mRows.size())
			return Otter::kResult_IndexOutOfBounds;

		Row* pRow = mRows[atIndex];
		VectorMath::Vector2 size = pRow->GetSize();

		RemoveControl(pRow);
		mRows.erase(atIndex);

		uint32 numRows = mRows.size();
		for(uint32 i = atIndex; i < numRows; i++)
		{
			VectorMath::Vector2 pos = mRows[i]->GetPosition();
			mRows[i]->SetPosition(VectorMath::Vector2(pos.x, pos.y - size.y));
		}

		return Otter::kResult_OK;
	}

	/* Enables / Disables scrolling of the table control
		*/
	Result Table::EnableScrolling(bool bEnable)
	{
		mEnableScrolling = bEnable;

		return Otter::kResult_OK;
	}

	/* Points (touches/mouse/etc) were pressed down
	 */
	bool Table::OnPointsDown(Point* points, sint32 numPoints)
	{
		if(!mTouchEnabled)
			return false;

		if(numPoints <= 0)
			return false;

		if(Control::OnPointsDown(points, numPoints))
			return false;

		mLastPoint = points[0];
		return true;
	}
		
	/* Points (touches/mouse/etc) were released
	 */
	bool Table::OnPointsUp(Point* points, sint32 numPoints)
	{
		if(!mTouchEnabled)
			return false;

		if(numPoints <= 0)
			return false;

		return Control::OnPointsUp(points, numPoints);
	}

	/* Points (touches/mouse/etc) were moved.
	 */
	bool Table::OnPointsMove(Point* points, sint32 numPoints)
	{
		if(!mTouchEnabled)
			return false;

		if(numPoints <= 0)
			return false;

		if(Control::OnPointsMove(points, numPoints))
			return true;

		const Point& point = points[0];

		if(mEnableScrolling)
		{
			float diff = point.y - mLastPoint.y;

			uint32 numRows = mRows.size();
			const VectorMath::Vector2& size = this->GetSize();

			if(numRows >= 1)
			{
				const VectorMath::Vector2& firstPos = mRows[0]->GetPosition();
				const VectorMath::Vector2& lastPos = mRows[numRows - 1]->GetPosition();
				const VectorMath::Vector2& lastDim = mRows[numRows - 1]->GetSize();

				if((firstPos.y + diff) > 0.0f)
				{
					diff -= (firstPos.y + diff);
				}
				else if((lastPos.y + lastDim.y + diff) < size.y)
				{
					diff -= (lastPos.y + lastDim.y + diff) - size.y;

					// The first pos again - make sure we won't move it
					if((firstPos.y + diff) > 0.0f)
						diff = 0.0f;
				}
			}

			for(uint32 i = 0; i < numRows; i++)
			{
				VectorMath::Vector2 pos = mRows[i]->GetPosition();
				pos.y += diff;

				mRows[i]->SetPosition(pos);
			}
		}

		mLastPoint = points[0];

		return true;
	}

	/**
	 * Clones the control and returns the new instance
	 */
	Control* Table::Clone()
	{
		Table* pTable = OTTER_NEW(Table, ());
		memcpy((uint8*)pTable->GetData(), (uint8*)GetData(), sizeof(TableData));
		((ControlData*)pTable->GetData())->mID = GetParentView()->GenerateNewID();
		mParent->AddControl(pTable);

		pTable->mEnableScrolling = this->mEnableScrolling;

		uint32 numRows = GetNumRows();
		for(uint32 i = 0; i < numRows; i++)
		{
			pTable->AddRow((Row*)GetRow(i)->Clone());
		}

		return pTable;
	}

	/* Draws the Table to screen
	 */
	void Table::Draw(Otter::Graphics* pGraphics)
	{
		if(!mEnabled)
			return;		
		
		const VectorMath::Vector2& size = GetSize();
		if(size.x == 0.0f || size.y == 0.0f)
			return;
		
		pGraphics->PushMatrix(GetTransform());
		// pGraphics->DrawRectangle(-1, 0.0f, 0.0f, size.x, size.y, 0x80808080);

		for(uint32 i = 0; i < mRows.size(); i++)
		{ 
			const VectorMath::Vector2& rPos = mRows[i]->GetPosition();			
			const VectorMath::Vector2& rDim = mRows[i]->GetSize();

			if(rPos.y < 0.0f || (rPos.y + rDim.y) > size.y)
				continue;
			
			mRows[i]->Draw(pGraphics);
		}

		pGraphics->PopMatrix();
	}

	/* Applies the interpolation of two keyframes to the control.
	 * pEndFrame can be NULL.
	 */
	void Table::ApplyKeyFrame(const Otter::KeyFrameData* pStartFrame, const Otter::KeyFrameData* pEndFrame, float factor)
	{
		Otter::Control::ApplyKeyFrame(pStartFrame, pEndFrame, factor);
	}

	///------------------------------------------------------------------------------------------------------------------------------
	/// ROW CONTROL
	///------------------------------------------------------------------------------------------------------------------------------
	struct RowData : ControlData
	{
	};

	/* Constructor
	 */
	Row::Row(float height) : Control(NULL, NULL, OTTER_NEW(RowData, ()))
	{
		if(height < 0.0f)
			height = 0.0f;

		SetSize(VectorMath::Vector2(0.0f, height));
	}

	/* Destructor
	 */
	Row::~Row()
	{		
		uint32 numControls = GetNumControls();
		for(uint32 i = 0; i < numControls; i++)
		{
			Control* pControl = GetControlAtIndex(i);

			if(MemoryManager::isValid(pControl))
			{
				// Deletes the control cloned in Row::Clone()
				OTTER_DELETE(pControl);
			}
		}

		const ControlData* pData = GetData();
		OTTER_DELETE(pData);
	}

	/**
	 * Clones the row and all of its internal controls
	 */
	Control* Row::Clone()
	{
		Row* pRow = OTTER_NEW(Row, (GetSize().y));
		memcpy((uint8*)pRow->GetData(), (uint8*)GetData(), sizeof(RowData));
		((ControlData*)pRow->GetData())->mID = GetParentView()->GenerateNewID();

		mParent->AddControl(pRow);

		uint32 numControls = GetNumControls();
		for(uint32 i = 0; i < numControls; i++)
		{
			// The cloned control will be deleted in Row::~Row()
			Control* pControl = GetControlAtIndex(i)->Clone();
			pRow->AddControl(pControl);
		}

		return pRow;
	}
		
	/* Retrieves a control by point.
	 */
	Control* Row::GetControl(Point& point, Point* localPoint, bool touchablesOnly)
	{
		if(!mEnabled || touchablesOnly && !TouchesEnabled())
			return NULL;
		
		for(sint32 i = (sint32)mControls.size() - 1; i >= 0; i--)
		{
			Control* pControl = mControls[i];
			pControl = pControl->GetControl(point, localPoint, touchablesOnly);

			if(pControl)
				return pControl;
		}

		return NULL;
	}
}