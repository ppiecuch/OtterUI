#include "SampleUI.h"
#include "Views/IntroHandler.h"
#include "Views/BasicControlsViewHandler.h"
#include "Views/LabelsViewHandler.h"
#include "Views/TablesViewHandler.h"
#include "Views/MaskViewHandler.h"

#include "Plugins/SamplePlugin.h"

/* Constructor
 */
SampleUI::SampleUI(IRenderer* pRenderer, ISoundSystem* pSoundSystem, IFileSystem* pFileSystem)
{
	mSamplePlugin = new SamplePlugin();

	// Create the GUI System
	mSystem = new Otter::System(2 * 1024 * 1024);

	// Set the custom renderer and filesystem
	mSystem->SetRenderer(pRenderer);
	mSystem->SetFileSystem(pFileSystem);
	mSystem->SetSoundSystem(pSoundSystem);
	mSystem->AddPlugin(mSamplePlugin);

	// Attach event handlers
	mSystem->mOnSceneLoaded.AddHandler(this, &SampleUI::OnSceneLoaded);
	mSystem->mOnSceneUnloaded.AddHandler(this, &SampleUI::OnSceneUnloaded);
	
	mSystem->EnablePreTransformVerts(true);

	SwitchToScene("Sample Scene.gbs", "Intro");
}

/* Destructor
 */
SampleUI::~SampleUI()
{
	delete mSystem;
	delete mSamplePlugin;
}

/* Queues the current scene to be unloaded and the specified one to be loaded in its
 * place.
 */
void SampleUI::SwitchToScene(const char* szScene, const char* szInitialView)
{
	mNextScene = szScene;
	mInitialView = szInitialView;
}

/* Called when a scene has been unloaded
 */
void SampleUI::OnSceneLoaded(void* pSystem, Scene* pScene)
{
	int numViews = pScene->GetNumViews();
	for(int i = 0; i < numViews; i++)
	{
		View* pView = pScene->GetView(i);

		UTF8String name = pView->GetName();
		if(name == "Intro")
		{
			mViews.push_back(new IntroHandler(this, pView));
		}
		else if(name == "BasicControlsView")
		{
			mViews.push_back(new BasicControlsViewHandler(this, pView));
		}
		else if(name == "LabelsView")
		{
			mViews.push_back(new LabelsViewHandler(this, pView));
		}
		else if(name == "TablesView")
		{
			mViews.push_back(new TablesViewHandler(this, pView));
		}
		else if(name == "MaskView")
		{
			mViews.push_back(new MaskViewHandler(this, pView));
		}
	}
}

/* Called when a scene has been unloaded
 */
void SampleUI::OnSceneUnloaded(void* pSystem, Scene* pScene)
{
	int numViews = pScene->GetNumViews();
	for(int i = 0; i < numViews; i++)
	{
		View* pView = pScene->GetView(i);

		for(int j = 0; j < mViews.size(); j++)
		{
			if(mViews[j]->GetView() == pView)
			{
				delete mViews[j];
				mViews.erase(mViews.begin() + j);

				break;
			}
		}
	}
}

/* Updates the Sample UI
	*/
void SampleUI::Update(float delta)
{
	if(mNextScene.length() > 0)
	{
		mSystem->UnloadAllScenes();

		mSystem->LoadScene(mNextScene.c_str());
		Otter::Scene* pScene = mSystem->GetScene(0);

		if(pScene && mInitialView.length() > 0)
			pScene->ActivateView(mInitialView.c_str());

		mNextScene = "";
		mInitialView = "";
	}

	if(mSystem)
		mSystem->Update(1.0f);
}

/* Draws the Sample UI
	*/
void SampleUI::Draw()
{
	if(mSystem)
		mSystem->Draw();
}