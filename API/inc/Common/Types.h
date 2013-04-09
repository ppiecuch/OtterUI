#pragma once

#include "BaseTypes.h"
#include "Array.h"
#include "Math/VectorMath.h"

namespace Otter
{
	/**
	 * Result Codes
	 */
	enum Result
	{
		kResult_OK = 1,
		kResult_Error,
		kResult_NoFileSystem,
		kResult_NoRenderer,
		kResult_NoSoundSystem,
		kResult_FailedRead,
		kResult_InvalidFile,
		kResult_FileNotFound,
		kResult_InvalidSceneFile,
		kResult_InvalidSceneVersion,
		kResult_InvalidSceneID,
		kResult_InvalidViewID,
		kResult_ViewNotFound,
		kResult_AnimationNotFound,
		kResult_InvalidControlID,
		kResult_IndexOutOfBounds,
		kResult_InvalidParameter,
		kResult_ViewAlreadyActive
	};

	/**
	 * Anchor flags.
	 */
	enum AnchorFlags
	{
		Top = 1 << 0,
        Bottom = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,

        TopRelative = 1 << 28,
        BottomRelative = 1 << 29,
        LeftRelative = 1 << 30,
        RightRelative = 1 << 31
	};

	/**
	 * Horizontal Aligment
	 */
	enum HoriAlignment
	{
		kHAlign_Left,
		kHAlign_Center,
		kHAlign_Right
	};

	/**
	 * Vertical Alignment
	 */
	enum VertAlignment
	{
		kVAlign_Top,
		kVAlign_Center,
		kVAlign_Bottom
	};

	/**
	 * Base control layout data
	 */
	struct ControlLayout
	{
		uint32 mFourCC;
		uint32 mDataSize;

		float	mCenterX;
		float	mCenterY;

		float	mX;
		float	mY;

		float	mWidth;
		float	mHeight;

		float	mRotation;
	};

	/**
	 * Shared data for all controls
	 */
	struct ControlData
	{
		uint32 	mFourCC;
		uint32 	mDataSize;

		uint32 	mID;
		char	mName[64];
		float 	mPosition[2];
		float	mCenter[2];
		float 	mSize[2];
		float	mRotation;

		uint32	mAnchorFlags;

		int		mMaskID;
	};

	/**
	 * Contains keyframe data
	 */
	struct KeyFrameData
	{   
		uint32	mFourCC;
		uint32	mDataSize;

		uint32	mFrame;

		sint32	mEaseType;
		sint32	mEaseAmount;

		float	mLeftAbs;
		float	mLeftRel;

		float	mRightAbs;
		float	mRightRel;

		float	mTopAbs;
		float	mTopRel;

		float	mBottomAbs;
		float	mBottomRel;

		char	mBuffer[1];

		/** 
		 * Retrieves the control layout associated with this
		 * keyframe
		 */
		const ControlLayout* GetLayout() const
		{
			return (ControlLayout*)mBuffer;
		}
	};

	/**
	 * Represents a simple rectangle
	 */
	struct Rectangle
	{
		float	x;
		float	y;
		float	w;
		float	h;

		Rectangle()
		{
			x = 0.0f;
			y = 0.0f;
			w = 0.0f;
			h = 0.0f;
		}

		Rectangle(float fX, float fY, float fW, float fH)
		{
			this->x = fX;
			this->y = fY;
			this->w = fW;
			this->h = fH;
		}
	};

	/**
	 * Represents a two-dimensional point
	 */
	struct Point
	{
		float	x;
		float	y;

		Point()
		{
			x = 0.0f;
			y = 0.0f;
		}

		Point(float x, float y)
		{
			this->x = x;
			this->y = y;
		}
	};

	/**
	 * Defines the primitive types that we support
	 */
	enum PrimitiveType
	{
		kPrim_TriangleList,
		kPrim_TriangleStrip,
		kPrim_TriangleFan
	};

	enum RenderFlags
	{
		kRender_Wireframe = 1 << 0
	};

	/**
	 * Defines the vertex format that used across UI rendering
	 */
	struct GUIVertex
	{
		VectorMath::Vector3	mPosition;
		VectorMath::Vector2	mTexCoord;
		uint32				mColor;
	};	

	struct Property
	{
		uint32		mPropertyID;
		uintptr_t	mData;

		Property()
		{
			mPropertyID = 0;
			mData = 0;
		}
	};

	struct PropertyMap
	{
		Array<Property> mProperties;

		const Property* GetProperty(uint32 id) const
		{
			uint32 cnt = mProperties.size();
			for(uint32 i = 0; i < cnt; i++)
			{
				if(mProperties[i].mPropertyID == id)
					return &mProperties[i];
			}
			return NULL;
		}

		PropertyMap()
		{
		}

		PropertyMap(const Array<Property>& properties) :
			mProperties(properties)
		{
		}
	};

	/**
	 * Defines a batch of primitives to render, of a certain type and count.
	 */
	struct DrawBatch
	{
		sint32				mPrimitiveType;
		sint32				mPrimitiveCount;
		sint32				mVertexStartIndex;
		sint32				mVertexCount;
		sint32				mTextureID;
		VectorMath::Matrix4	mTransform;

		uint32				mRenderFlags;
		PropertyMap			mProperties;

		DrawBatch() :
			mPrimitiveType(kPrim_TriangleList),
			mPrimitiveCount(0),
			mVertexStartIndex(0),
			mVertexCount(0),
			mTextureID(-1),
			mTransform(VectorMath::Matrix4::IDENTITY),
			mRenderFlags(0)
		{
		}

		DrawBatch(sint32 primType, sint32 primCount, sint32 startIndex, sint32 vertexCount, uint32 textureID, const VectorMath::Matrix4& model, Array<Property> properties, uint32 renderFlags = 0) :
			mPrimitiveType(primType),
			mPrimitiveCount(primCount),
			mVertexStartIndex(startIndex),
			mVertexCount(vertexCount),
			mTextureID(textureID),
			mTransform(model),
			mRenderFlags(renderFlags),
			mProperties(properties)
		{
		}
	};

	/**
	 * Message arguments on a message event
	 */
	struct MessageArgs
	{
		uint32		mMessageID;
		char		mText[64];
	};
}
