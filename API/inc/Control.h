#pragma once

#include "Common/Array.h"
#include "Common/Types.h"
#include "Math/VectorMath.h"

namespace Otter
{
	struct ControlData;
	struct KeyFrameData;
	class Graphics;
	class View;
	class Scene;
	class Control;
	class Mask;

	typedef Array<Control*> ControlArray;

	/**
	 * OtterUI base control.  All new controls are to be derived from
	 * this class.
	 *
	 * The OtterUI Control class forms the base of all user interface elements and controls.
	 * It manages child controls as well as the general layout of each control.
	 */
	class Control
	{
	public:
		/**
		 * Constructor.
		 * 
		 * @param pScene			Parent scene
		 * @param pParentControl	Parent control
		 * @param pControlData		Internal control data
		 */
		Control(Scene* pScene, Control* pParentControl, const ControlData* pControlData);

		/**
		 * Virtual Destructor.
		 */
		virtual ~Control(void);

	public:

		/**
		 * Enable / Disables the control.
		 *
		 * If the control is disabled, it will not respond to input nor draw itself.
		 * 
		 * @param bEnable True to enable this control, false if otherwise.
		 */
		void Enable(bool bEnable) { mEnabled = bEnable; }

		/**
		 * Returns whether or not the control is enabled.
		 *
		 * @return True if enabled, false if otherwise.
		 */
		bool IsEnabled() { return mEnabled; }

		/**
		 * Enables / Disables touch interaction
		 *
		 * If touch interaction is set to false, the control will still draw but it will not respond
		 * to any touch inputs.
		 *
		 * @param bEnable	True to enable touch interaction for this control, false if otherwise
		 *
		 */
		void EnableTouches(bool bEnable) { mTouchEnabled = bEnable; }

		/**
		 * Returns whether or not touches are enabled.
		 *
		 * @return True if touches are enable for this control, false if otherwise
		 */
		bool TouchesEnabled() { return mTouchEnabled; }

		/**
		 * Retrieves the control's name
		 */
		virtual const char* GetName();

		/**
		 * Retrieves the control's ID
		 */
		uint32 GetID();

		/*
		 * Retrieves the parent scene
		 */
		Scene* GetScene() { return mScene; }

		/**
		 * Retrieves the internal control data
		 */
		const ControlData* GetData() { return mControlData; }

		/**
		 * Retrieves the control's position
		 *
		 * The control's position is relative from its parent control or view.
		 */
		const VectorMath::Vector2& GetPosition();

		/**
		 * Retrieves the control's Absolute position
		 *
		 */
		const VectorMath::Vector2 GetAbsolutePosition();

		/**
		 * Sets the control's position
		 * 
		 * @param position Control's new position, relative from its parent control or view
		 */
		virtual void SetPosition(const VectorMath::Vector2& position);

		/**
		 * Retrieves the control's center
		 *
		 * The control center can also be thought of as its pivot point, and is relative off of the top-left
		 * corner of the control.  The control's center will always coincide with the control's actual position.
		 *
		 * @return X and Y center from the top-left corner of the control's bounds.
		 */
		const VectorMath::Vector2& GetCenter();

		/**
		 * Sets the control's center
		 *
		 * The control center can also be thought of as its pivot point, and is relative off of the top-left
		 * corner of the control.  The control's center will always coincide with the control's actual position.
		 *
		 * @param center X and Y center from the top-left corner of the control's bounds.
		 */
		virtual void SetCenter(const VectorMath::Vector2& center);

		/**
		 * Retrieves the control's size
		 */
		const VectorMath::Vector2& GetSize();

		/**
		 * Sets the control's size
		 */
		virtual void SetSize(const VectorMath::Vector2& size);

		/**
		 * Gets the control's rotation in degrees
		 */
		float GetRotation();

		/**
		 * Sets the control's rotation in degrees
		 */
		virtual void SetRotation(float rotation);

		/**
		 * Gets the control's mask ID
		 */
		int GetMask();

		/**
		 * Gets the control's actual mask control
		 */
		Mask* GetMaskControl();

		/**
		 * Sets the control's mask ID
		 */
		void SetMask(int maskID);

		/**
		 * Retrieves the local transform for this control.
		 */
		const VectorMath::Matrix4& GetTransform();

		/**
		 * Retrieves the Full transform for this control.
		 */
		const VectorMath::Matrix4 GetFullTransform();

		/**
		 * Sets the base transform for this control
		 */
		void SetBaseTransform(const VectorMath::Matrix4& baseTransform);

		/**
		 * Adds a child control
		 */
		Result AddControl(Control* pControl);

		/**
		 * Removes a child control
		 */
		Result RemoveControl(Control* pControl);

		/**
		 * Retrieves a control by name
		 */
		Control* GetControl(const char* szControlName);

		/**
		 * Retrieves a control by ID
		 */
		Control* GetControl(uint32 id);

		/**
		 * Gets a control by screen coordinate.  Returns the top-most control
		 * that contains the point.
		 */
		virtual Control* GetControl(Point& point, Point* localPoint, bool touchablesOnly = false);

		/**
		 * Retrieves a control by index
		 */
		Control* GetControlAtIndex(uint32 index);

		/**
		 * Retrieves the number of direct child controls
		 */
		uint32	GetNumControls();

		/**
		 * Converts the screen point to a local
		 * local point, relative to the control
		 */
		void ScreenToLocal(Point& point, Point& localPoint);

		/**
		 * Sets the control's parent
		 */
		virtual Result SetParentControl(Control* pParent);

		/**
		 * Returns the parent of this control
		 */
		Control* GetParentControl();

		/**
		 * Returns the containing view of this control
		 */
		View* GetParentView();

	public:

		/**
		 * Clones the control and returns the new instance
		 */
		virtual Control* Clone() { return NULL; }
		
		/**
		 * Called whenever this control is activated
		 */
		virtual void OnActivate() { }

		/**
		 * Called whenever this control is deactivated
		 */
		virtual void OnDeactivate() { }

		/**
		 * Points (touches/mouse/etc) were pressed down
		 * Returns a reference to the control that handled the points
		 */
		virtual bool OnPointsDown(Point* points, sint32 numPoints);
		
		/**
		 * Points (touches/mouse/etc) were released
		 * Returns a reference to the control that handled the points
		 */
		virtual bool OnPointsUp(Point* points, sint32 numPoints);
		
		/**
		 * Points (touches/mouse/etc) were moved.
		 * Returns a reference to the control that handled the points
		 */
		virtual bool OnPointsMove(Point* points, sint32 numPoints);

		/**
		 * Applies the interpolation of two keyframes to the control.
		 * pEndFrame can be NULL.
		 */
		virtual void ApplyKeyFrame(const KeyFrameData* pStartFrame, const KeyFrameData* pEndFrame, float factor);

		/**
		 * Draws the control.
		 */
		virtual void Draw(Graphics* pGraphics);

		/**
		 * Stores a pointer to this control's mask, and recurses on child controls
		 */
		void SetMaskPointers();

	protected:

		/**
		 * Internal control data. 
		 */
		const ControlData*		mControlData;			

		/**
		 * Parent control.
		 */
		Control*				mParent;

		/**
		 * Array of child controls. 
		 */
		ControlArray			mControls;	

		/**
		 * Parent scene. 
		 */
		Scene*					mScene;

		/**
		 * Enables / disables this control from input processing and rendering
		 */
		bool					mEnabled;

		/**
		 * Enables / disables touch input processing
		 */
		bool					mTouchEnabled;

		/**
		 * Indicates whether or not this control was created at runtime
		 */
		bool					mUserCreated;

		/**
		 * Pointer to the mask referred to by mControlData->mMaskID, for performance
		 */
		Mask*					mMaskControl;
	
	private:
		/**
		 * Control's base transform.  Used to manually pre-transform
		 * the control as needed.
		 */
		VectorMath::Matrix4		mBaseTransform;

		/**
		 * Control's transformation matrix
		 */
		VectorMath::Matrix4		mTransform;

		/**
		 * If true, the transformation matrix must be updated the next time it is
		 * retrieved
		 */
		bool					mUpdateTransform;
	};
};