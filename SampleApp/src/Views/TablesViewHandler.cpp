#include "TablesViewHandler.h"
#include "SampleUI.h"

#ifndef _T
#define _T(a) a
#endif 

/* Constructor
 */
TablesViewHandler::TablesViewHandler(SampleUI* pSampleUI, View* pView) : ViewHandler(pSampleUI, pView)
{
	mNextButton			= NULL;
	mPrevButton			= NULL;
	mTable				= NULL;

	mView->mOnActivate.AddHandler(this, &TablesViewHandler::OnActivate);
	mView->mOnDeactivate.AddHandler(this, &TablesViewHandler::OnDeactivate);
	mView->mOnAnimationEnded.AddHandler(this, &TablesViewHandler::OnAnimationEnded);
}

/* Virtual Destructor
 */
TablesViewHandler::~TablesViewHandler(void)
{
	mView->mOnActivate.RemoveHandler(this, &TablesViewHandler::OnActivate);
	mView->mOnDeactivate.RemoveHandler(this, &TablesViewHandler::OnDeactivate);
	mView->mOnAnimationEnded.RemoveHandler(this, &TablesViewHandler::OnAnimationEnded);
}

/** Executed when the view has become active
 * 
 * We need to pull the various controls that we want to operate on in here.
 * In particular, we want to listen for events when the user hits the buttons.
 */
void TablesViewHandler::OnActivate(void* pSender, void* pContext)
{
	mNextButton = (Button*)mView->GetControl("NextButton");
	if(mNextButton)
		mNextButton->mOnClick.AddHandler(this, &TablesViewHandler::OnNextButtonClicked);

	mPrevButton = (Button*)mView->GetControl("PrevButton");
	if(mPrevButton)
		mPrevButton->mOnClick.AddHandler(this, &TablesViewHandler::OnPrevButtonClicked);

	mTable = (Table*)mView->GetControl("Table1");
	SetupTable();
}

/** @brief Executed when the view has become deactive
 */
void TablesViewHandler::OnDeactivate(void* pSender, void* pContext)
{
	if(mNextButton)
	{
		mNextButton->mOnClick.RemoveHandler(this, &TablesViewHandler::OnNextButtonClicked);
		mNextButton = NULL;
	}

	if(mPrevButton)
	{
		mPrevButton->mOnClick.RemoveHandler(this, &TablesViewHandler::OnPrevButtonClicked);
		mPrevButton = NULL;
	}


	uint32 rows = mTable->GetNumRows();
	for(uint32 i = 0; i < rows; i++)
	{
		Otter::Row* pRow = mTable->GetRow(0);
		mTable->RemoveRow(0);

		uint32 numControls = pRow->GetNumControls();
		for(uint32 j = 0; j < numControls; j++)
		{
			Otter::Control* pControl = pRow->GetControlAtIndex(j);
			pRow->RemoveControl(pControl);

			delete pControl;
		}

		delete pRow;
	}
}

/** @brief Executed when an animation has completed
 */
void TablesViewHandler::OnAnimationEnded(void* pSender, uint32 animID)
{

}

/* Called when the Next Button has been clicked
 */
void TablesViewHandler::OnNextButtonClicked(void* pSender, void* pData)
{
	mView->StopAnimation(ANIM_ONACTIVATE);
	mView->GetScene()->DeactivateView(mView);
	mView->GetScene()->ActivateView("MaskView");
}

/* Called when the Prev Button has been clicked
 */
void TablesViewHandler::OnPrevButtonClicked(void* pSender, void* pData)
{
	mView->StopAnimation(ANIM_ONACTIVATE);
	mView->GetScene()->DeactivateView(mView);
	mView->GetScene()->ActivateView("LabelsView");
}

/* Sets up the table with some sample rows
 */
void TablesViewHandler::SetupTable()
{
	if(!mTable)
		return;

	Otter::Row* pRow = NULL;
	Otter::Label* pLabel = NULL;
	Otter::Button* pButton = NULL;
	Otter::Sprite* pSprite = NULL;
	Otter::Toggle* pToggle = NULL;
	Otter::Slider* pSlider = NULL;

	// Row 1 - 2
	pRow = new Otter::Row(50.0f);
	const VectorMath::Vector2& rowSize = pRow->GetSize();
	mTable->AddRow(pRow);

	pLabel = new Label();
	pRow->AddControl(pLabel);
	pLabel->SetPosition(VectorMath::Vector2(0.0f, 0.0f));
	pLabel->SetSize(pRow->GetSize());
	pLabel->SetFont("Cyberbit Med");
	pLabel->SetText("* Labels:");
	pLabel->SetTextAlignment(kHAlign_Left, kVAlign_Center);
	pLabel->EnableTouches(false);
	
	pRow = new Otter::Row(100.0f);
	mTable->AddRow(pRow);

	pLabel = new Label();
	pRow->AddControl(pLabel);
	pLabel->SetPosition(VectorMath::Vector2(0.0f, 0.0f));
	pLabel->SetSize(pRow->GetSize());
	pLabel->SetFont("Cyberbit Med");
	pLabel->SetText(_T("Hello! (こんにちは) {+}{-}"));
	pLabel->SetTextAlignment(kHAlign_Left, kVAlign_Top);
	pLabel->EnableTouches(false);

	// Row 3 - 4
	pRow = new Otter::Row(50.0f);
	mTable->AddRow(pRow);

	pLabel = new Label();
	pRow->AddControl(pLabel);
	pLabel->SetPosition(VectorMath::Vector2(0.0f, 0.0f));
	pLabel->SetSize(pRow->GetSize());
	pLabel->SetFont("Cyberbit Med");
	pLabel->SetText("* Buttons:");
	pLabel->SetTextAlignment(kHAlign_Left, kVAlign_Center);
	pLabel->EnableTouches(false);
	
	pRow = new Otter::Row(75.0f);
	mTable->AddRow(pRow);

	pButton = new Button();
	pRow->AddControl(pButton);
	pButton->SetPosition(VectorMath::Vector2(0.0f, 0.0f));
	pButton->SetSize(VectorMath::Vector2(256.0f, 64.0f));
	pButton->SetDefaultTexture("Textures/Button.png");
	pButton->SetDownTexture("Textures/ButtonDown.png");
	pButton->SetTextAlignment(kHAlign_Left, kVAlign_Center);

	// Row 4 - 5
	pRow = new Otter::Row(50.0f);
	mTable->AddRow(pRow);

	pLabel = new Label();
	pRow->AddControl(pLabel);
	pLabel->SetPosition(VectorMath::Vector2(0.0f, 0.0f));
	pLabel->SetSize(pRow->GetSize());
	pLabel->SetFont("Cyberbit Med");
	pLabel->SetText("* Sprites: ");
	pLabel->SetTextAlignment(kHAlign_Left, kVAlign_Center);
	pLabel->EnableTouches(false);
	
	pRow = new Otter::Row(75.0f);
	mTable->AddRow(pRow);

	pSprite = new Sprite();
	pRow->AddControl(pSprite);
	pSprite->SetPosition(VectorMath::Vector2(0.0f, 0.0f));
	pSprite->SetSize(VectorMath::Vector2(100.0f, 50.0f));
	pSprite->SetTexture("Textures/OtterHead.png");
	pSprite->EnableTouches(false); 

	// Row 6 - 7
	pRow = new Otter::Row(50.0f); 
	mTable->AddRow(pRow);

	pLabel = new Label();
	pRow->AddControl(pLabel);
	pLabel->SetPosition(VectorMath::Vector2(0.0f, 0.0f));
	pLabel->SetSize(pRow->GetSize());
	pLabel->SetFont("Cyberbit Med");
	pLabel->SetText("* Tables:");
	pLabel->SetTextAlignment(kHAlign_Left, kVAlign_Center);
	pLabel->EnableTouches(false);

	pRow = new Otter::Row(75.0f); 
	mTable->AddRow(pRow);

	pLabel = new Label();
	pRow->AddControl(pLabel);
	pLabel->SetPosition(VectorMath::Vector2(0.0f, 0.0f));
	pLabel->SetSize(VectorMath::Vector2(rowSize.x, 50.0f));
	pLabel->SetFont("Cyberbit Med");
	pLabel->SetText("(this!)");
	pLabel->SetTextAlignment(kHAlign_Left, kVAlign_Center);
	pLabel->EnableTouches(false);

	// Row 8 - 9
	pRow = new Otter::Row(50.0f); 
	mTable->AddRow(pRow);

	pLabel = new Label();
	pRow->AddControl(pLabel);
	pLabel->SetPosition(VectorMath::Vector2(0.0f, 0.0f));
	pLabel->SetSize(pRow->GetSize());
	pLabel->SetFont("Cyberbit Med");
	pLabel->SetText("* Toggles: ");
	pLabel->SetTextAlignment(kHAlign_Left, kVAlign_Center);
	pLabel->EnableTouches(false);
	
	pRow = new Otter::Row(75.0f); 
	mTable->AddRow(pRow);

	pToggle = new Toggle();
	pRow->AddControl(pToggle);
	pToggle->SetPosition(VectorMath::Vector2(0.0f, 0.0f));
	pToggle->SetSize(VectorMath::Vector2(32.0f, 32.0f));
	pToggle->SetOnTexture("Textures/Toggle_On.png");
	pToggle->SetOffTexture("Textures/Toggle_Off.png");

	pToggle = new Toggle();
	pRow->AddControl(pToggle);
	pToggle->SetPosition(VectorMath::Vector2(50.0f, 0.0f));
	pToggle->SetSize(VectorMath::Vector2(32.0f, 32.0f));
	pToggle->SetOnTexture("Textures/Toggle_On.png");
	pToggle->SetOffTexture("Textures/Toggle_Off.png");

	// Row 10 - 11
	pRow = new Otter::Row(50.0f); 
	mTable->AddRow(pRow);

	pLabel = new Label();
	pRow->AddControl(pLabel);
	pLabel->SetPosition(VectorMath::Vector2(0.0f, 0.0f));
	pLabel->SetSize(pRow->GetSize());
	pLabel->SetFont("Cyberbit Med");
	pLabel->SetText("* Sliders: ");
	pLabel->SetTextAlignment(kHAlign_Left, kVAlign_Center);
	pLabel->EnableTouches(false);
	
	pRow = new Otter::Row(75.0f); 
	mTable->AddRow(pRow);

	pSlider = new Slider();
	pRow->AddControl(pSlider);
	pSlider->SetPosition(VectorMath::Vector2(0.0f, 0.0f));
	pSlider->SetSize(VectorMath::Vector2(rowSize.x, 50.0f));
	pSlider->SetStartTexture("Textures/Slider_Start.png");
	pSlider->SetMiddleTexture("Textures/Slider_Middle.png");
	pSlider->SetEndTexture("Textures/Slider_End.png");
	pSlider->SetThumbTexture("Textures/Slider_Thumb.png");
	pSlider->SetThumbSize(25, 25);
	pSlider->SetRange(0, 100);
	pSlider->SetStep(5);
}