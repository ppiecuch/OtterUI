#pragma once
#include "Control.h"

struct CircleData;

// Generates a four character code
#define FOURCC(a, b, c, d) (a << 24 | b << 16 | c << 8 | d << 0)

#define FOURCC_CIRC FOURCC('C', 'I', 'R', 'C')
#define FOURCC_CRLT FOURCC('C', 'R', 'L', 'T')

/**
 * 2D Circle to demonstrate plugin functionality
 */
class Circle : public Otter::Control
{
public:
	/**
	 * Constructor
	 */
	Circle(Otter::Scene* pScene, Otter::Control* pParent, const CircleData* pCircleData);

	/* Constructor
	 */
	Circle();

	/* Destructor
	 */
	virtual ~Circle(void);

public:

	/**
	 * Sets the circle's color
	 */
	void SetColor(uint32 color);

	/**
	 * Retrieves the circle's color
	 */
	uint32 GetColor();

	/**
	 * Sets the circle's width
	 */
	void SetWidth(uint32 width);

	/**
	 * Retrieves the circle's width
	 */
	uint32 GetWidth();

	/**
	 * Sets the circle's radius
	 */
	void SetRadius(uint32 radius);

	/**
	 * Retrieves the circle's radius
	 */
	uint32 GetRadius();

	/** Sets the number of segments on this
	 * circle.  Must be >= 3
	 */
	void SetNumSegments(uint32 numSegments);

public:

	/**
	 * Draws the Circle to screen
	 */
	virtual void Draw(Otter::Graphics* pGraphics);

	/**
 	 * Applies the interpolation of two keyframes to the control.
 	 * pEndFrame can be NULL.
	 */
	virtual void ApplyKeyFrame(const Otter::KeyFrameData* pStartFrame, const Otter::KeyFrameData* pEndFrame, float factor);

private:

	Otter::GUIVertex* mVertices;
};