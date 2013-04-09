#include <string.h>
#include "Circle.h"
#include "Graphics/Graphics.h"

using namespace VectorMath;

/**
 * The circle layout matches the format as it was
 * exported by the sample plugin
 */
struct CircleLayout : Otter::ControlLayout
{
	uint32 mRadius;
	uint32 mWidth;
	uint32 mColor;
};

/**
 * The circle data contains all of our initial settings
 * for the circle.  During animation we will override Radius,
 * Width and Color.
 */
struct CircleData : Otter::ControlData
{
	uint32 mSegments;
	uint32 mRadius;
	uint32 mWidth;
	uint32 mColor;
};

/**
 * Constructor
 */
Circle::Circle(Otter::Scene* pScene, Otter::Control* pParent, const CircleData* pCircleData) : Otter::Control(pScene, pParent, pCircleData)
{
	mVertices = NULL;

	SetNumSegments(pCircleData->mSegments);
}

/**
 * Constructor
 */
Circle::Circle() : Control(NULL, NULL, new CircleData())
{
	mUserCreated = true;
	mVertices = NULL;

	SetNumSegments(10);
}

/**
 * Destructor
 */
Circle::~Circle(void)
{
	// Only delete the circle data if we created the control at runtime
	if(mUserCreated)
		delete GetData();

	delete[] mVertices;
}

/**
 * Sets the circle's color
 */
void Circle::SetColor(uint32 color)
{
	CircleData* pCircleData = const_cast<CircleData*>(static_cast<const CircleData*>(mControlData));
	pCircleData->mColor = color;
}

/**
 * Retrieves the circle's color
 */
uint32 Circle::GetColor()
{
	const CircleData* pCircleData = static_cast<const CircleData*>(mControlData);
	return pCircleData->mColor;
}

/**
 * Sets the circle's width
 */
void Circle::SetWidth(uint32 width)
{
	CircleData* pCircleData = const_cast<CircleData*>(static_cast<const CircleData*>(mControlData));
	pCircleData->mWidth = width;
}

/**
 * Retrieves the circle's width
 */
uint32 Circle::GetWidth()
{
	const CircleData* pCircleData = static_cast<const CircleData*>(mControlData);
	return pCircleData->mWidth;
}

/**
 * Sets the circle's radius
 */
void Circle::SetRadius(uint32 radius)
{
	CircleData* pCircleData = const_cast<CircleData*>(static_cast<const CircleData*>(mControlData));
	pCircleData->mRadius = radius;
}

/**
 * Retrieves the circle's radius
 */
uint32 Circle::GetRadius()
{
	const CircleData* pCircleData = static_cast<const CircleData*>(mControlData);
	return pCircleData->mRadius;
}

/**
 * Sets the number of line segments in the control
 */
void Circle::SetNumSegments(uint32 numSegments)
{
	if(numSegments <= 2)
		numSegments = 3;

	CircleData* pCircleData = const_cast<CircleData*>(static_cast<const CircleData*>(mControlData));
	pCircleData->mSegments = numSegments;

	// Two triangle per segment, three verts per triangle
	delete[] mVertices;
	mVertices = new Otter::GUIVertex[numSegments * 2 * 3];

	memset(mVertices, 0x00, sizeof(Otter::GUIVertex) * 2 * 3);
}
	
/**
 * Draws the Circle to screen
 */
void Circle::Draw(Otter::Graphics* pGraphics)
{		
	const CircleData* pCircleData = static_cast<const CircleData*>(mControlData);

	if(!IsEnabled() || pCircleData->mSegments <= 2 || pCircleData->mWidth == 0 || mVertices == NULL)
		return;

	pGraphics->PushMatrix(GetTransform());
	{
		float x = 1.0f;
		float y = 0.0f;

		float radius = pCircleData->mRadius;
		uint32 halfWidth = pCircleData->mWidth / 2;
		uint32 color = pCircleData->mColor; 

		const VectorMath::Vector2& size = GetSize();
		VectorMath::Vector2 center(size.x / 2.0f, size.y / 2.0f);

		for (int i = 0; i < pCircleData->mSegments; i++)
		{
			double angle1 = (i / (float)pCircleData->mSegments) * VectorMath::Constants::PI * 2;
			double angle2 = ((i + 1) / (float)pCircleData->mSegments) *VectorMath::Constants::PI * 2;

			float rx1 = (float)(x * VectorMath::Functions::Cos(angle1) - y * VectorMath::Functions::Sin(angle1));
			float ry1 = (float)(x * VectorMath::Functions::Sin(angle1) + y * VectorMath::Functions::Cos(angle1));

			float rx2 = (float)(x * VectorMath::Functions::Cos(angle2) - y * VectorMath::Functions::Sin(angle2));
			float ry2 = (float)(x * VectorMath::Functions::Sin(angle2) + y * VectorMath::Functions::Cos(angle2));

			mVertices[i * 6 + 0].mPosition = VectorMath::Vector3(center.x + rx1 * (radius + halfWidth), center.y + ry1 * (radius + halfWidth), 0.0f);
			mVertices[i * 6 + 0].mColor = pCircleData->mColor;
			mVertices[i * 6 + 1].mPosition = VectorMath::Vector3(center.x + rx2 * (radius + halfWidth), center.y + ry2 * (radius + halfWidth), 0.0f);
			mVertices[i * 6 + 1].mColor = pCircleData->mColor;
			mVertices[i * 6 + 2].mPosition = VectorMath::Vector3(center.x + rx1 * (radius - halfWidth), center.y + ry1 * (radius - halfWidth), 0.0f);
			mVertices[i * 6 + 2].mColor = pCircleData->mColor;

			mVertices[i * 6 + 3].mPosition = VectorMath::Vector3(center.x + rx1 * (radius - halfWidth), center.y + ry1 * (radius - halfWidth), 0.0f);
			mVertices[i * 6 + 3].mColor = pCircleData->mColor;
			mVertices[i * 6 + 4].mPosition = VectorMath::Vector3(center.x + rx2 * (radius + halfWidth), center.y + ry2 * (radius + halfWidth), 0.0f);
			mVertices[i * 6 + 4].mColor = pCircleData->mColor;
			mVertices[i * 6 + 5].mPosition = VectorMath::Vector3(center.x + rx2 * (radius - halfWidth), center.y + ry2 * (radius - halfWidth), 0.0f);
			mVertices[i * 6 + 5].mColor = pCircleData->mColor;
		}

		pGraphics->DrawPrimitives(-1, Otter::kPrim_TriangleList, pCircleData->mSegments * 2, mVertices);
	}
	pGraphics->PopMatrix();
}

/**
 * Applies the interpolation of two keyframes to the control.
 * pEndFrame can be NULL.
 */
void Circle::ApplyKeyFrame(const Otter::KeyFrameData *pStartFrame, const Otter::KeyFrameData *pEndFrame, float factor)
{
	Control::ApplyKeyFrame(pStartFrame, pEndFrame, factor);

	if(pStartFrame == NULL && pEndFrame == NULL)
		return;

	const CircleLayout* pStartLayout	= (CircleLayout*)pStartFrame->GetLayout();
	const CircleLayout* pEndLayout		= (CircleLayout*)(pEndFrame ? pEndFrame->GetLayout() : NULL);

	uint8 s0	= pStartLayout		? (pStartLayout->mColor >> 24)	& 0xFF : 0xFF;
	uint8 s1	= pStartLayout		? (pStartLayout->mColor >> 16)	& 0xFF : 0xFF;
	uint8 s2	= pStartLayout		? (pStartLayout->mColor >> 8)	& 0xFF : 0xFF;
	uint8 s3	= pStartLayout		? (pStartLayout->mColor)		& 0xFF : 0xFF;
	uint32 sr	= pStartLayout		? (pStartLayout->mRadius) : 0;
	uint32 sw	= pStartLayout		? (pStartLayout->mWidth) : 0;

	uint8 e0	= pEndLayout		? (pEndLayout->mColor >> 24)	& 0xFF : 0xFF;
	uint8 e1	= pEndLayout		? (pEndLayout->mColor >> 16)	& 0xFF : 0xFF;
	uint8 e2	= pEndLayout		? (pEndLayout->mColor >> 8)		& 0xFF : 0xFF;
	uint8 e3	= pEndLayout		? (pEndLayout->mColor)			& 0xFF : 0xFF;
	uint32 er	= pEndLayout		? (pEndLayout->mRadius) : 0;
	uint32 ew	= pEndLayout		? (pEndLayout->mWidth) : 0;
		
	uint8 f0 = (uint8)(s0 + factor * (e0 - s0));
	uint8 f1 = (uint8)(s1 + factor * (e1 - s1));
	uint8 f2 = (uint8)(s2 + factor * (e2 - s2));
	uint8 f3 = (uint8)(s3 + factor * (e3 - s3));

	uint32 fr = (uint32)(sr * (1.0f - factor) + er * factor);
	uint32 fw = (uint32)(sw * (1.0f - factor) + ew * factor);

	SetColor(f0 << 24 | f1 << 16 | f2 << 8 | f3);
	SetWidth(fw);
	SetRadius(fr);
}