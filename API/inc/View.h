#pragma once

#include "Common/Array.h"
#include "Common/Types.h"
#include "Common/Event.h"
#include "Common/UTF8String.h"

#include "Control.h"

namespace Otter
{
	struct ViewData;
	struct ControlData;
	struct AnimationData;
	struct ActionData;
	class Scene;
	class Graphics;
	
	enum 
	{
		ANIM_ONACTIVATE = 0xF0000000,
		ANIM_ONDEACTIVATE = 0xF0000001
	};
	
	/**
	 * Active animation information
	 */
	struct ActiveAnimation
	{
		uint32	mID;				// ID is 0xF0000000 | (active_anim_index)
		float	mFrameTime;
		float	mStartFrameTime;
		float	mEndFrameTime;
		uint32	mAnimationIndex;
		const AnimationData* mAnimation;	// If not NULL, will be used instead of fetching via mAnimationIndex

		bool	mKeyOff;			// If set, will not repeat if a loop section has been defined.
		bool	mReverse;			// If set, plays the animation in reverse

		sint32*	mControlRemap;		// Channel -> ControlID Remap

		VectorMath::Matrix4 mTransform;

		ActiveAnimation()
		{
			mID = 0;
			mFrameTime = 0.0f;
			mAnimationIndex = 0;
			mAnimation = NULL;
			mStartFrameTime = 0.0f;
			mEndFrameTime = 0.0f;
			mKeyOff = false;
			mReverse = false;
			mTransform = VectorMath::Matrix4::IDENTITY; 
			mControlRemap = NULL;
		}
	};

	typedef Array<ActiveAnimation> ActiveAnimations;

	/* The GUI View maintains all GUI controls and functionality associated with a
	 * a single view.
	 */
	class View : public Control
	{
	private:
		/**
		 * Default Constructor
		 */
		View();

	public:
		/**
		 * Constructor - builds the view from the provided view data 
		 */
		View(Scene* pScene, const ViewData* pViewData);

		/**
		 * Virtual Destructor
		 */
		virtual ~View(void);

	public:

		/**
		 * Called whenever this view is activated
		 */
		virtual void OnActivate();

		/**
		 * Called whenever this view is deactivated
		 */
		virtual void OnDeactivate();

		/**
		 * Retrieves a control by name in an animation.  If animInstanceID is non-zero,
		 * this function will search only against controls within a specific active animation
		 */
		Control* GetControlInAnimation(uint32 animInstanceID, const char* szControlName);

		/**
		 * Plays an animation by name, and returns an ID to that animation.
		 * If endFrame is not 0xFFFFFFFF the animation will play until it reaches
		 * the endFrame, otherwise it will continue to its logical end.
		 *
		 * If startFrame is 0, it will start at the first frame if bReverse is false, otherwise it will
		 * start at the last frame.
		 *
		 * If endFrame is not 0, it must be larger than or equal to startFrame if bReverse if false, otherwise
		 * it must be less than or equal to startFrame.
		 */
		uint32 PlayAnimation(const char* szName, uint32 startFrame = 0, uint32 endFrame = 0, bool bReverse = false, VectorMath::Matrix4 transform = VectorMath::Matrix4::IDENTITY, bool bMakeInstance = false);

		/**
		 * Plays an animation by index.
		 */
		uint32 PlayAnimation(uint32 index, uint32 startFrame = 0, uint32 endFrame = 0, bool bReverse = false, VectorMath::Matrix4 transform = VectorMath::Matrix4::IDENTITY, bool bMakeInstance = false);

		/**
		 * Stops an active animation by name
		 */
		Result StopAnimation(uint32 animID);

		/**
		 * Flags an animation to run past the looping section.
		 */
		Result KeyOffAnimation(uint32 animID);

		/** 
		 * Retrieves an active animation's name by its unique animation ID
		 */
		const char* GetActiveAnimationName(uint32 animID);
		
		/**
		 * Retrieves the list of active/playing animations by name
		 */
		Array<uint32> GetActiveAnimations(const char* szName);

		/** 
		 * Retrieves the 1-based frame index of a named main channel frame of the
		 * specified animation
		 */
		uint32 GetFrameIndex(const char* szAnimationName, const char* szFrameName);

		/** 
		 * Retrieves the number of frames in a given Animation. Returns 0 if the animation is not found.
		 */

		uint32 GetAnimationLength(const char* szName);

		/** 
		 * Retrieves the current frame time of a given Animation. Returns -1 if the animation is not found.
		 */
		float GetAnimationFrameTime(uint32 animID);

		/** 
		* Retrieves the end frame time of a given Animation. Returns -1 if the animation is not found.
		*/
		float GetAnimationEndFrameTime(uint32 animID);

		/**
		 * Draws the view
		 */
		void Draw(Graphics* pGraphics);

		/**
		 * Updates the view
		 * "frameDelta" is the number of frames that have passed since last update
		 */
		void Update(float frameDelta);

		/**
		 * Brings the specified control the front, ie drawn on top of everything else
		 */
		void BringToFront(Control* pControl);

		/**
		 * Sends a control to the back, ie drawn behind everything else
		 */
		void SendToBack(Control* pControl);

		/**
		 * Processes an action
		 */
		void ProcessAction(Control* pSender, const ActionData* pAction);

		/*
		 * Returns a new (unused) control ID
		 */
		uint32 GenerateNewID();

	public:		
		
		/**
		 * Points (touches/mouse/etc) were pressed down
		 */
		bool OnPointsDown(Point* points, sint32 numPoints);
		
		/**
		 * Points (touches/mouse/etc) were released
		 */
		bool OnPointsUp(Point* points, sint32 numPoints);
		
		/**
		 * Points (touches/mouse/etc) were moved.
		 */
		bool OnPointsMove(Point* points, sint32 numPoints);

	private:
		/**
		 * Loads the view from the provided view data
		 */
		bool LoadFromData(const ViewData* pViewData);

		/**
		 * Unloads and frees the view's internal objects.
		 */
		void Unload();

		/**
		 * Creates a control from the provided control data
		 */
		Control* CreateControl(const ControlData* pControlData, Control* pParent = 0);

		/** 
		 * Destroys a control and all of its children
		 */
		void DestroyControl(Control* pControl);

		/** 
		 * Creates an animation instance
		 */
		void CreateAnimationInstance(ActiveAnimation& activeAnimation);

		/** 
		 * Destroys an animation instance
		 */
		void DestroyAnimationInstance(ActiveAnimation& activeAnimation);

		/**
		 * Retrieves an active animation by name  Returns NULL if not found
		 */
		ActiveAnimation* GetActiveAnimation(const char* szName);

		/**
		 * Retrieves an active animation by ID.  Returns NULL if not found
		 */
		ActiveAnimation* GetActiveAnimation(uint32 animID);
		
		/**
		 * Processes and fires the events between the start and end frame times
		 */
		void ProcessEvents(float startFrameTime, float endFrameTime, const ActiveAnimation* pActiveAnimation, const AnimationData* pAnimation);

		/**
		 * Animates the controls associated with an animation
		 */
		void AnimateControls(const AnimationData* pAnimation, ActiveAnimation& activeAnim);

	private:
		
		/**
		 * Counts how many animations have played during the lifetime of this view.  Necessary
		 * to create unique animation IDs.
		 */
		uint32					mAnimCounter;

		/**
		 * Array of active animations
		 */
		ActiveAnimations		mActiveAnimations;

		/**
		 * Maintains the next control ID when dynamically cloning controls
		 */
		sint32					mNextControlID;

		/**
		 * Array of touched controls.  Only touched controls will respond to 
		 * move or up touch inputs.
		 */
		ControlArray			mTouchedControls;

		/**
		 * Array to maintiain a list of finished animations in the current update loop.
		 */
		Array<uint32>			mFinishedAnims;

	public:		
		/**
		 * Raised whenever the view has been activated
		 * Event Parameter: Unused
		 */
		Event<void*>				mOnActivate;

		/**
		 * Raised whenever the view has been deactivated
		 * Event Parameter: Unused
		 */
		Event<void*>				mOnDeactivate;

		/**
		 * Raised whenever the view has started an animation
		 * Event Parameter: Animation ID
		 */
		Event<uint32>				mOnAnimationStarted;

		/**
		 * Raised whenever the view has ended an animation
		 * Event Parameter: Animation ID
		 */
		Event<uint32>				mOnAnimationEnded;

		/**
		 * Raised whenever a message has been sent from an active animation
		 * Event Parameter: Message Arguments
		 */
		Event<const MessageArgs&>	mOnMessage;
	};
}