#pragma once
#include <string>
#include <vector>
#include "Otter.h"

using namespace Otter;

class IntroHandler;
class ViewHandler;
class SamplePlugin;

/* Handles all GUI System Events
 */
class SampleUI
{
public:

	/* Constructor
	 */
	SampleUI(IRenderer* pRenderer, ISoundSystem* pSoundSystem, IFileSystem* pFileSystem);

	/* Destructor
	 */
	~SampleUI();

public:
	/* Queues the current scene to be unloaded and the specified one to be loaded in its
	 * place.
	 */
	void SwitchToScene(const char* szScene, const char* szInitialView);

	/* Called when a new scene has been loaded
	 */
	void OnSceneLoaded(void* pSystem, Scene* pScene);

	/* Called when a scene has been unloaded
	 */
	void OnSceneUnloaded(void* pSystem, Scene* pScene);

	/* Retrieves the Otter System object
	 */
	Otter::System* GetSystem() { return mSystem; }

public:

	/* Updates the Sample UI
	 */
	void Update(float delta);

	/* Draws the Sample UI
	 */
	void Draw();

private:

	Otter::System*			mSystem;

	std::string				mNextScene;
	std::string				mInitialView;

	std::vector<ViewHandler*> mViews;

	SamplePlugin*			mSamplePlugin;
};
