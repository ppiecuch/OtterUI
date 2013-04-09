#pragma once

#include "Common/Types.h"

#define FOURCC(a, b, c, d) (a << 24 | b << 16 | c << 8 | d << 0)

#define FOURCC_GGSC (FOURCC('G', 'G', 'S', 'C'))
#define FOURCC_GGVW (FOURCC('G', 'G', 'V', 'W'))
#define FOURCC_GSPR (FOURCC('G', 'S', 'P', 'R'))
#define FOURCC_GLBL (FOURCC('G', 'L', 'B', 'L'))
#define FOURCC_GBTT (FOURCC('G', 'B', 'T', 'T'))
#define FOURCC_GGRP (FOURCC('G', 'G', 'R', 'P'))
#define FOURCC_GMSK (FOURCC('G', 'M', 'S', 'K'))
#define FOURCC_KFRM (FOURCC('K', 'F', 'R', 'M'))
#define FOURCC_CLLT (FOURCC('C', 'L', 'L', 'T'))
#define FOURCC_BTLT (FOURCC('B', 'T', 'L', 'T'))
#define FOURCC_GPLT (FOURCC('G', 'P', 'L', 'T'))
#define FOURCC_LBLT (FOURCC('L', 'B', 'L', 'T'))
#define FOURCC_SPLT (FOURCC('S', 'P', 'L', 'T'))
#define FOURCC_TBLT (FOURCC('T', 'B', 'L', 'T'))
#define FOURCC_TGLT (FOURCC('T', 'G', 'L', 'T'))
#define FOURCC_SLLT (FOURCC('S', 'L', 'L', 'T'))
#define FOURCC_MKLT (FOURCC('M', 'K', 'L', 'T'))
#define FOURCC_MSGA (FOURCC('M', 'S', 'G', 'A'))
#define FOURCC_SNDA (FOURCC('S', 'N', 'D', 'A'))
#define FOURCC_GTBL (FOURCC('G', 'T', 'B', 'L'))
#define FOURCC_GTGL (FOURCC('G', 'T', 'G', 'L'))
#define FOURCC_GSLD (FOURCC('G', 'S', 'L', 'D'))

#define SCENE_DATA_VERSION	(0.950f)

namespace Otter
{
	/* Basic action
	 */
	struct ActionData
	{
		uint32 mFourCC;
		uint32 mDataSize;
	};

	/* FOURCC: MSGA
	 * Action that sends a text message
	 */
	struct MessageActionData : public ActionData
	{
		uint32		mMessageID;
	};

	/* FOURCC: SNDA
	 * Action to play a sound
	 */
	struct SoundActionData : public ActionData
	{
		uint32		mSoundID;
		float		mVolume;
	};

	/* Sprite Layout data
	 */
	struct SpriteLayout : public ControlLayout
	{
		uint32 mColor;
		float mSkew;
	};

	/* Contains all data relevant to a sprite
	 */
	struct SpriteData : public ControlData
	{
		sint32 mTextureID;
		uint32 mColor;
		float mSkew;
		uint32 mFlipType; // 0: None, 1: Horizontal, 2: Vertical

		uint32 mNumControls;
		char mBuffer[1];

		/* Retrieves control data by index
		 */
		const ControlData* GetControlData(uint32 index) const
		{
			if(index >= mNumControls)
				return (const ControlData*)0;

			const ControlData* pControlData = (const ControlData*)mBuffer;
			for(uint32 i = 0; i < index; i++)
			{
				// Step over the entire view's data block
				pControlData = (const ControlData*)(((uint8*)pControlData) + pControlData->mDataSize);
			}

			return pControlData;
		}
	};

	/* Label Layout data
	 */
	struct LabelLayout : public ControlLayout
	{
		uint32 mColor;
		float  mScaleX;
		float  mScaleY;
		float mSkew;
		int mDropShadow;
	};

	/* Contains all data relevant to a label
	 */
	struct LabelData : public ControlData
	{
		uint32 mFontID;
		uint32 mColor;
		float  mScaleX;
		float  mScaleY;
		float mSkew;
		int mDropShadow;
		uint32 mHAlign;
		uint32 mVAlign;
		float mLeading;
		sint32 mTracking;
		uint32 mTextFit;

		uint32 mTextBufferSize;
		char mBuffer[1];

		/* Retrieves the label's internal text
		 */
		const char* GetText() const
		{
			if(mTextBufferSize == 0)
				return "";

			return (const char*)mBuffer;
		}
	};

	/* Button Layout data
	 */
	struct ButtonLayout : public ControlLayout
	{
	};

	/* Button Data
	 */
	struct ButtonData : public ControlData
	{
		uint32 mDefaultTextureID;
		uint32 mDownTextureID;
				
		uint32 mFontID;
		uint32 mDefaultColor;
		uint32 mDownColor;
		float  mScaleX;
		float  mScaleY;
		uint32 mHAlign;
		uint32 mVAlign;

		uint32 mNumOnClickActions;

		uint32 mTextBufferSize;
		char mBuffer[1];

		/* Retrieves the button's internal text
		 */
		const char* GetText() const
		{
			if(mTextBufferSize == 0)
				return "";

			return (const char*)mBuffer;
		}

		/* Reterieves an action by index
		 */
		const ActionData* GetAction(uint32 index) const
		{
			if(index >= mNumOnClickActions)
				return (const ActionData*)0;

			const ActionData* pAction = (const ActionData*)(mBuffer + mTextBufferSize);
			while(index > 0)
			{
				pAction = (const ActionData*)(((uint8*)pAction) + pAction->mDataSize);
				index--;
			}

			return pAction;
		}
	};

	/* Group Layout data
	 */
	struct GroupLayout : public ControlLayout
	{
		uint32 mColor;
	};

	/* Group Data
	 */
	struct GroupData : public ControlData
	{
		uint32 mNumControls;

		char mBuffer[1];

		/* Retrieves control data by index
		 */
		const ControlData* GetControlData(uint32 index) const
		{
			if(index >= mNumControls)
				return (const ControlData*)0;

			const ControlData* pControlData = (const ControlData*)mBuffer;
			for(uint32 i = 0; i < index; i++)
			{
				// Step over the entire view's data block
				pControlData = (const ControlData*)(((uint8*)pControlData) + pControlData->mDataSize);
			}

			return pControlData;
		}
	};

	/* Table Layout data
	 */
	struct TableLayout : public ControlLayout
	{
	};

	/* Table Control Data
	 */
	struct TableData : public ControlData
	{
		uint32 mDefaultRowHeight;
		uint32 mRowSpacing;
	};

	/* Toggle Layout data
	 */
	struct ToggleLayout : public ControlLayout
	{
		uint32 mColor;
	};

	/* Toggle Control Data
	 */
	struct ToggleData : public ControlData
	{
		sint32 mOnTextureID;
		sint32 mOffTextureID;
		uint32 mColor;
	};

	/* Slider Layout data
	 */
	struct SliderLayout : public ControlLayout
	{
		uint32 mColor;
	};

	/* Slider Control Data
	 */
	struct SliderData : public ControlData
	{
		uint32  mThumbWidth;
		uint32  mThumbHeight;

		sint32 mStartTextureID;
		sint32 mMiddleTextureID;
		sint32 mEndTextureID;
		sint32 mThumbTextureID;

		sint32 mMin;
		sint32 mMax;
		sint32 mStep;
		sint32 mValue;

		uint32 mColor;
	};

	/* Mask Layout data
	 */
	struct MaskLayout : public ControlLayout
	{
		float mSkew;
	};

	/* Contains all data relevant to a mask
	 */
	struct MaskData : public ControlData
	{
		sint32 mTextureID;
		float mSkew;
		uint32 mFlipType; // 0: None, 1: Horizontal, 2: Vertical
	};

	/* FOURCC : GGAC
	 * Contains animation channel data
	 */
	struct AnimationChannelData
	{
		uint32 mFourCC;
		uint32 mDataSize;

		sint32 mControlID;
		uint32 mNumKeyFrames;

		uint8  mBuffer[1];

		/* Gets a keyframe by index
		 */
		const KeyFrameData* GetKeyFrameData(uint32 index) const
		{
			if(index >= mNumKeyFrames)
				return (const KeyFrameData*)0;


			const KeyFrameData* pKeyFrame = (const KeyFrameData*)mBuffer;
			for(uint32 i = 0; i < index; i++)
				pKeyFrame = (const KeyFrameData*)(((uint8*)pKeyFrame) + pKeyFrame->mDataSize);
		
			return pKeyFrame;
		}
	};

	/* FOURCC : MCFR
	 * Represents a frame on the main timeline channel.
	 */
	struct MainChannelFrameData
	{
		uint32 mFourCC;
		uint32 mDataSize;

		char   mName[64];
		uint32 mFrame;
		uint32 mNumActions;

		uint8  mBuffer[1];

		/* Reterieves an action by index
		 */
		const ActionData* GetAction(uint32 index) const
		{
			if(index >= mNumActions)
				return (const ActionData*)0;

			const ActionData* pAction = (const ActionData*)mBuffer;
			while(index > 0)
			{
				pAction = (const ActionData*)(((uint8*)pAction) + pAction->mDataSize);
				index--;
			}

			return pAction;
		}
	};

	/* FOURCC : GGAN
	 * Contains animation set information
	 */
	struct AnimationData
	{
		uint32 mFourCC;
		uint32 mDataSize;

		uint8  mName[64];

		uint32 mNumFrames;
		sint32 mRepeatStart;
		sint32 mRepeatEnd;

		uint32 mNumAnimationChannels;
		uint32 mNumMainChannelFrames;

		uint32 mMainChannelFramesOffset;

		uint8  mBuffer[1];

		/* Retrieves animation channel data by index
		 */
		const AnimationChannelData* GetAnimationChannel(uint32 index) const
		{
			if(index >= mNumAnimationChannels)
				return (const AnimationChannelData*)0;

			const AnimationChannelData* pChannel = (const AnimationChannelData*)mBuffer;
			for(uint32 i = 0; i < index; i++)
				pChannel = (const AnimationChannelData*)(((uint8*)pChannel) + pChannel->mDataSize);

			return pChannel;
		}

		/* Retrieves the main channel frame for a particular frame
		 */
		const MainChannelFrameData* GetMainChannelFrameData(uint32 frame) const
		{
			if(frame >= mNumFrames)
				return (const MainChannelFrameData*)0;

			const MainChannelFrameData* pMainChannelFrame = (const MainChannelFrameData*)&mBuffer[mMainChannelFramesOffset];
			for(uint32 i = 0; i < mNumMainChannelFrames; i++)
			{
				if(pMainChannelFrame->mFrame == frame)
					return pMainChannelFrame;

				// Step over the entire view's data block
				pMainChannelFrame = (const MainChannelFrameData*)(((uint8*)pMainChannelFrame) + pMainChannelFrame->mDataSize);
			}

			return (const MainChannelFrameData*)0;
		}
	};

	/* FOURCC : GGAL
	 * Contains animation list data for a view
	 */
	struct AnimationListData
	{
		uint32 mFourCC;
		uint32 mDataSize;

		uint32 mNumAnimations;

		uint8  mBuffer[1];

		/* Retrieves the animation set data by index
		 */
		const AnimationData* GetAnimation(uint32 index) const
		{
			if(index >= mNumAnimations)
				return (const AnimationData*)0;

			const AnimationData* pAnimation = (const AnimationData*)mBuffer;
			for(uint32 i = 0; i < index; i++)
				pAnimation = (const AnimationData*)(((uint8*)pAnimation) + pAnimation->mDataSize);

			return pAnimation;
		}
	};

	/* Contains all data relevant to a view
	 */
	struct ViewData : public ControlData
	{
		uint32 mNumTextures;
		uint32 mNumSounds;
		uint32 mNumControls;

		uint32 mTexturesOffset;
		uint32 mSoundsOffset;
		uint32 mControlsOffset;
		uint32 mAnimationListOffset;

		uint8 mBuffer[1];

		/* Returns the animation list data
		 */
		const AnimationListData* GetAnimationListData() const
		{
			return (const AnimationListData*)&mBuffer[mAnimationListOffset];
		}

		/* Returns the complete list of texture IDs used by this view
		 */
		const sint32* GetTextureIDs() const
		{
			if(mNumTextures <= 0)
				return (const sint32*)0;

			return (const sint32*)&mBuffer[mTexturesOffset];
		}

		/* Returns the complete list of sound IDs used by this view
		 */
		const sint32* GetSoundIDs() const
		{
			if(mNumSounds <= 0)
				return (const sint32*)0;

			return (const sint32*)&mBuffer[mSoundsOffset];
		}

		/* Retrieves control data by index
		 */
		const ControlData* GetControlData(uint32 index) const
		{
			if(index >= mNumControls)
				return (const ControlData*)0;

			const ControlData* pControlData = (const ControlData*)&mBuffer[mControlsOffset];
			for(uint32 i = 0; i < index; i++)
			{
				// Step over the entire view's data block
				pControlData = (const ControlData*)(((uint8*)pControlData) + pControlData->mDataSize);
			}

			return pControlData;
		}
	};

	/* Defines a rectangle on a specific texture,
	 * by two uv coordinates
	 */
	struct TextureRect
	{
		uint32 mTextureID;		
		float  uv[4]; // {L, T, R, B}

		TextureRect()
		{
			mTextureID = 0xFFFFFFFF;
			uv[0] = uv[1] = uv[2] = uv[3] = 0.0f;
		}
	};

	/* Contains texture info.
	 * Textures may reference other textures through the mTextureRect, allowing
	 * texture atlassing.
	 *
	 * Common case is for mTextureID == mTextureRect.mTextureID
	 */
	struct TextureData
	{
		uint32		mTextureID;
		char		mTextureName[256];

		sint32		mRefCount;

		TextureRect	mTextureRect;

		TextureData()
		{
			mTextureID = 0xFFFFFFFF;
			mTextureName[0] = 0;
			mRefCount = 0;
		}
	};

	/* Contains sound info
	 */
	struct SoundData
	{
		uint32 mSoundID;
		char mSoundName[256];

		sint32 mRefCount;
	};

	/* Contains font glyph data
	 */
	struct GlyphData
	{
		uint32 mCharCode;
		uint32 mIsImageGlyph;

		uint32 mX;
		uint32 mY;
		uint32 mWidth;
		uint32 mHeight;

		uint32 mTop;
		uint32 mAdvance;
		uint32 mLeftBearing;

		uint32 mAtlasIndex;

		GlyphData()
		{
			mCharCode = 0;
			mIsImageGlyph = 0;
		
			mX = 0;
			mY = 0;
			mWidth = 0;
			mHeight = 0;

			mTop = 0;
			mAdvance = 0;
			mLeftBearing = 0;

			mAtlasIndex = 0;
		}

		GlyphData(uint32 charCode, uint32 isImage, uint32 x, uint32 y, uint32 w, uint32 h, uint32 adv, uint32 leftBearing, uint32 atlasIndex)
		{
			mCharCode = charCode;
			mIsImageGlyph = isImage;
		
			mX = x;
			mY = y;
			mWidth = w;
			mHeight = h;

			mAdvance = adv;
			mLeftBearing = leftBearing;

			mAtlasIndex = atlasIndex;
		}
	};

	/* Contains font data.
	 */
	struct FontData
	{
		uint32 mFourCC;
		uint32 mDataSize;

		uint32 mID;
		char mName[64];
		uint32 mFontSize;
		uint32 mFontWidth;
		uint32 mFontHeight;

		uint32 mMaxTop;
		uint32 mNumTextures;
		uint32 mNumGlyphs;

		char mBuffer[1];

		/* Retrieves glyph data by unicode character
		 */
		const GlyphData* GetGlyphData(uint32 uc, bool imageGlyph = false) const
		{
			// Two loops - first for the actual uc, and the second
			// to return '?' if the character is not found.
			for(int j = 0; j < 2; j++)
			{
				for(uint32 i = 0; i < mNumGlyphs; i++)
				{
					const GlyphData* pData = &((const GlyphData*)mBuffer)[i];
					if(pData->mCharCode == uc && ((pData->mIsImageGlyph ? true : false) == imageGlyph))
						return pData;
				}

				uc = '?';
			}
			
			return (const GlyphData*)0;
		}
	};

	/* Contains message information
	 */
	struct MessageData
	{
		uint32	mID;
		char	mText[64];
	};

	/* Contains all data relevant to a scene
	 */
	struct SceneData
	{
		uint32	mFourCC;
		uint32	mDataSize;

		float	mVersion;
		uint32	mID;

		uint32  mNumFonts;
		uint32	mNumTextures;
		uint32	mNumSounds;
		uint32	mNumViews;
		uint32	mNumMessages;

		uint32  mFontsOffset;
		uint32	mTexturesOffset;
		uint32	mSoundsOffset;
		uint32  mViewsOffset;
		uint32	mMessagesOffset;

		uint8	mBuffer[1];

		/* Retrieves font data by font index
		 */
		const FontData* GetFontData(uint32 index) const
		{
			if(index > mNumFonts)
				return (const FontData*)0;

			const FontData* pFontData = (const FontData*)&mBuffer[mFontsOffset];
			for(uint32 i = 0; i < index; i++)
			{
				pFontData = (const FontData*)(((uint8*)pFontData) + pFontData->mDataSize);
			}

			return pFontData;
		}

		/* Retrieves texture data by index
		 */
		const TextureData* GetTextureByIndex(uint32 index) const
		{
			if(index > mNumTextures)
				return (const TextureData*)0;

			const TextureData* pTextureData = (const TextureData*)&mBuffer[mTexturesOffset];
			return (const TextureData*)&pTextureData[index];
		}


		/* Returns the texture name by ID
		 */
		const TextureData* GetTexture(uint32 textureID) const
		{
			const TextureData* pTextureData = (const TextureData*)&mBuffer[mTexturesOffset];
			for(uint32 i = 0; i < mNumTextures; i++)
			{
				if(pTextureData[i].mTextureID == textureID)
					return &pTextureData[i];
			}

			return (const TextureData*)0;
		}

		/* Retrieves sound data by index
		 */
		const SoundData* GetSoundByIndex(uint32 index) const
		{
			if(index > mNumSounds)
				return (const SoundData*)0;

			const SoundData* pSoundData = (const SoundData*)&mBuffer[mSoundsOffset];
			return (const SoundData*)&pSoundData[index];
		}


		/* Returns the sound name by ID
		 */
		const SoundData* GetSound(uint32 soundID) const
		{
			const SoundData* pSoundData = (const SoundData*)&mBuffer[mSoundsOffset];
			for(uint32 i = 0; i < mNumSounds; i++)
			{
				if(pSoundData[i].mSoundID == soundID)
					return &pSoundData[i];
			}

			return (const SoundData*)0;
		}

		/* Retrieves View Data by index
		 */
		const ViewData* GetViewData(uint32 index) const
		{
			if(index >= mNumViews)
				return (const ViewData*)0;

			const ViewData* pViewData = (const ViewData*)&mBuffer[mViewsOffset];
			for(uint32 i = 0; i < index; i++)
			{
				// Step over the entire view's data block
				pViewData = (const ViewData*)(((uint8*)pViewData) + pViewData->mDataSize);
			}

			return pViewData;
		}

		/* Retrieves Messages Data by id
		 */
		const MessageData* GetMessageData(uint32 messageId) const
		{
			const MessageData* pMessageData = (const MessageData*)&mBuffer[mMessagesOffset];
			for(uint32 i = 0; i < mNumMessages; i++)
			{
				if(pMessageData[i].mID == messageId)
					return &pMessageData[i];
			}

			return (const MessageData*)0;
		}
	};

	/* Loads scene data from file
	 */
	const SceneData* LoadSceneData(const char* szFilename);

	/* Unloads scene data
	 */
	void UnloadSceneData(const SceneData* pData);
};