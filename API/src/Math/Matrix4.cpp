#include "Math/Matrix4.h"
#include "Math/VectorMath.h"
#include <string.h>

#define I(r, c) (c * 4 + r)

namespace VectorMath
{
	const Matrix4 Matrix4::ZERO(
		0.0f, 0.0f, 0.0f, 0.0f,
		0.0f, 0.0f, 0.0f, 0.0f,
		0.0f, 0.0f, 0.0f, 0.0f,
		0.0f, 0.0f, 0.0f, 0.0f);

	const Matrix4 Matrix4::IDENTITY(
		1.0f, 0.0f, 0.0f, 0.0f,
		0.0f, 1.0f, 0.0f, 0.0f,
		0.0f, 0.0f, 1.0f, 0.0f,
		0.0f, 0.0f, 0.0f, 1.0f);

	/* Constructs a translation matrix
	 */
	Matrix4 Matrix4::Translation(float x, float y, float z)
	{
		Matrix4 mtx = Matrix4::IDENTITY;
		mtx.mEntry[I(0,3)] = x;
		mtx.mEntry[I(1,3)] = y;
		mtx.mEntry[I(2,3)] = z;
		mtx.mEntry[I(3,3)] = 1.0f;

		return mtx;
	}	

	/* Constructs a rotation matrix about X
	 */
	Matrix4 Matrix4::RotationX(float fAngle)
	{
		Matrix4 matrix;

		float fCos, fSin;
		fCos = Functions::Cos(fAngle);
		fSin = Functions::Sin(fAngle);
		
		matrix.mEntry[I(0, 0)] = 1.0f;
		matrix.mEntry[I(0, 1)] = 0.0f;
		matrix.mEntry[I(0, 2)] = 0.0f;
		matrix.mEntry[I(0, 3)] = 0.0f;
	    
		matrix.mEntry[I(1, 0)] = 0.0f;
		matrix.mEntry[I(1, 1)] = fCos;
		matrix.mEntry[I(1, 2)] = -fSin;
		matrix.mEntry[I(1, 3)] = 0.0f;
	    
		matrix.mEntry[I(2, 0)] = 0.0f;
		matrix.mEntry[I(2, 1)] = fSin;
		matrix.mEntry[I(2, 2)] = fCos;
		matrix.mEntry[I(2, 3)] = 0.0f;
	    
		matrix.mEntry[I(3, 0)] = 0.0f;
		matrix.mEntry[I(3, 1)] = 0.0f;
		matrix.mEntry[I(3, 2)] = 0.0f;
		matrix.mEntry[I(3, 3)] = 1.0f;

		return matrix;
	}

	/* Constructs a rotation matrix about Y
	 */
	Matrix4 Matrix4::RotationY(float fAngle)
	{
		Matrix4 matrix;

		float fCos, fSin;
		fCos = Functions::Cos(fAngle);
		fSin = Functions::Sin(fAngle);
		
		matrix.mEntry[I(0, 0)] = fCos;
		matrix.mEntry[I(0, 1)] = 0.0f;
		matrix.mEntry[I(0, 2)] = fSin;
		matrix.mEntry[I(0, 3)] = 0.0f;
					  
		matrix.mEntry[I(1, 0)] = 0.0f;
		matrix.mEntry[I(1, 1)] = 1.0f;
		matrix.mEntry[I(1, 2)] = 0.0f;
		matrix.mEntry[I(1, 3)] = 0.0f;
					  
		matrix.mEntry[I(2, 0)] = -fSin;
		matrix.mEntry[I(2, 1)] = 0.0f;
		matrix.mEntry[I(2, 2)] = fCos;
		matrix.mEntry[I(2, 3)] = 0.0f;
					  
		matrix.mEntry[I(3, 0)] = 0.0f;
		matrix.mEntry[I(3, 1)] = 0.0f;
		matrix.mEntry[I(3, 2)] = 0.0f;
		matrix.mEntry[I(3, 3)] = 1.0f;

		return matrix;
	}

	/* Constructs a rotation matrix about Z
	 */
	Matrix4 Matrix4::RotationZ(float fAngle)
	{
		Matrix4 matrix;

		float fCos, fSin;
		fCos = Functions::Cos(fAngle);
		fSin = Functions::Sin(fAngle);
		
		matrix.mEntry[I(0, 0)] = fCos;
		matrix.mEntry[I(0, 1)] = -fSin;
		matrix.mEntry[I(0, 2)] = 0.0f;
		matrix.mEntry[I(0, 3)] = 0.0f;
					  
		matrix.mEntry[I(1, 0)] = fSin;
		matrix.mEntry[I(1, 1)] = fCos;
		matrix.mEntry[I(1, 2)] = 0.0f;
		matrix.mEntry[I(1, 3)] = 0.0f;
					  
		matrix.mEntry[I(2, 0)] = 0.0f;
		matrix.mEntry[I(2, 1)] = 0.0f;
		matrix.mEntry[I(2, 2)] = 1.0f;
		matrix.mEntry[I(2, 3)] = 0.0f;
					  
		matrix.mEntry[I(3, 0)] = 0.0f;
		matrix.mEntry[I(3, 1)] = 0.0f;
		matrix.mEntry[I(3, 2)] = 0.0f;
		matrix.mEntry[I(3, 3)] = 1.0f;

		return matrix;
	}

	/* Constructor
	 */
	Matrix4::Matrix4(void)
	{
		memcpy(this, &Matrix4::IDENTITY, sizeof(Matrix4));
	}	
		
	/* Constructs the matrix with specific values.
	 */
	Matrix4::Matrix4 (	float fM00, float fM01, float fM02, float fM03,
         				float fM10, float fM11, float fM12, float fM13,
         				float fM20, float fM21, float fM22, float fM23,
         				float fM30, float fM31, float fM32, float fM33)
	{
		mEntry[I(0, 0)] = fM00;
		mEntry[I(0, 1)] = fM01;
		mEntry[I(0, 2)] = fM02;
		mEntry[I(0, 3)] = fM03;

		mEntry[I(1, 0)] = fM10;
		mEntry[I(1, 1)] = fM11;
		mEntry[I(1, 2)] = fM12;
		mEntry[I(1, 3)] = fM13;

		mEntry[I(2, 0)] = fM20;
		mEntry[I(2, 1)] = fM21;
		mEntry[I(2, 2)] = fM22;
		mEntry[I(2, 3)] = fM23;

		mEntry[I(3, 0)] = fM30;
		mEntry[I(3, 1)] = fM31;
		mEntry[I(3, 2)] = fM32;
		mEntry[I(3, 3)] = fM33;
	}

	/* Destructor
	 */
	Matrix4::~Matrix4(void)
	{
	}

	/* Multiplies this matrix with another.
	 * Operation is from right to left, ie (*this) * rkM is the
	 * transform of (*this) followed by rkM
	 */
	Matrix4 Matrix4::operator* (const Matrix4& rkM) const
	{
		Matrix4 kProd;

		// Row 0
		kProd.mEntry[I(0, 0)] 	=	mEntry[I(0, 0)] * rkM.mEntry[I(0, 0)] + 
									mEntry[I(0, 1)] * rkM.mEntry[I(1, 0)] + 
									mEntry[I(0, 2)] * rkM.mEntry[I(2, 0)] + 
									mEntry[I(0, 3)] * rkM.mEntry[I(3, 0)];
		kProd.mEntry[I(0, 1)] 	= 	mEntry[I(0, 0)] * rkM.mEntry[I(0, 1)] + 
									mEntry[I(0, 1)] * rkM.mEntry[I(1, 1)] + 
									mEntry[I(0, 2)] * rkM.mEntry[I(2, 1)] + 
									mEntry[I(0, 3)] * rkM.mEntry[I(3, 1)];
		kProd.mEntry[I(0, 2)] 	= 	mEntry[I(0, 0)] * rkM.mEntry[I(0, 2)] +
									mEntry[I(0, 1)] * rkM.mEntry[I(1, 2)] +
									mEntry[I(0, 2)] * rkM.mEntry[I(2, 2)] +
									mEntry[I(0, 3)] * rkM.mEntry[I(3, 2)];
		kProd.mEntry[I(0, 3)] 	= 	mEntry[I(0, 0)] * rkM.mEntry[I(0, 3)] +
									mEntry[I(0, 1)] * rkM.mEntry[I(1, 3)] +
									mEntry[I(0, 2)] * rkM.mEntry[I(2, 3)] +
									mEntry[I(0, 3)] * rkM.mEntry[I(3, 3)];

		// Row 1
		kProd.mEntry[I(1, 0)] 	=	mEntry[I(1, 0)] * rkM.mEntry[I(0, 0)] + 
									mEntry[I(1, 1)] * rkM.mEntry[I(1, 0)] + 
									mEntry[I(1, 2)] * rkM.mEntry[I(2, 0)] + 
									mEntry[I(1, 3)] * rkM.mEntry[I(3, 0)];
		kProd.mEntry[I(1, 1)] 	=	mEntry[I(1, 0)] * rkM.mEntry[I(0, 1)] + 
									mEntry[I(1, 1)] * rkM.mEntry[I(1, 1)] + 
									mEntry[I(1, 2)] * rkM.mEntry[I(2, 1)] + 
									mEntry[I(1, 3)] * rkM.mEntry[I(3, 1)];
		kProd.mEntry[I(1, 2)] 	=	mEntry[I(1, 0)] * rkM.mEntry[I(0, 2)] +
									mEntry[I(1, 1)] * rkM.mEntry[I(1, 2)] +
									mEntry[I(1, 2)] * rkM.mEntry[I(2, 2)] +
									mEntry[I(1, 3)] * rkM.mEntry[I(3, 2)];
		kProd.mEntry[I(1, 3)] 	=	mEntry[I(1, 0)] * rkM.mEntry[I(0, 3)] +
									mEntry[I(1, 1)] * rkM.mEntry[I(1, 3)] +
									mEntry[I(1, 2)] * rkM.mEntry[I(2, 3)] +
									mEntry[I(1, 3)] * rkM.mEntry[I(3, 3)];

		// Row 2
		kProd.mEntry[I(2, 0)] 	=	mEntry[I(2, 0)] * rkM.mEntry[I(0, 0)] + 
									mEntry[I(2, 1)] * rkM.mEntry[I(1, 0)] + 
									mEntry[I(2, 2)] * rkM.mEntry[I(2, 0)] + 
									mEntry[I(2, 3)] * rkM.mEntry[I(3, 0)];
		kProd.mEntry[I(2, 1)] 	=	mEntry[I(2, 0)] * rkM.mEntry[I(0, 1)] + 
									mEntry[I(2, 1)] * rkM.mEntry[I(1, 1)] + 
									mEntry[I(2, 2)] * rkM.mEntry[I(2, 1)] + 
									mEntry[I(2, 3)] * rkM.mEntry[I(3, 1)];
		kProd.mEntry[I(2, 2)] 	=	mEntry[I(2, 0)] * rkM.mEntry[I(0, 2)] +
									mEntry[I(2, 1)] * rkM.mEntry[I(1, 2)] +
									mEntry[I(2, 2)] * rkM.mEntry[I(2, 2)] +
									mEntry[I(2, 3)] * rkM.mEntry[I(3, 2)];
		kProd.mEntry[I(2, 3)] 	=	mEntry[I(2, 0)] * rkM.mEntry[I(0, 3)] +
									mEntry[I(2, 1)] * rkM.mEntry[I(1, 3)] +
									mEntry[I(2, 2)] * rkM.mEntry[I(2, 3)] +
									mEntry[I(2, 3)] * rkM.mEntry[I(3, 3)];

		// Row 3
		kProd.mEntry[I(3, 0)] 	=	mEntry[I(3, 0)] * rkM.mEntry[I(0, 0)] + 
									mEntry[I(3, 1)] * rkM.mEntry[I(1, 0)] + 
									mEntry[I(3, 2)] * rkM.mEntry[I(2, 0)] + 
									mEntry[I(3, 3)] * rkM.mEntry[I(3, 0)];
		kProd.mEntry[I(3, 1)] 	=	mEntry[I(3, 0)] * rkM.mEntry[I(0, 1)] + 
									mEntry[I(3, 1)] * rkM.mEntry[I(1, 1)] + 
									mEntry[I(3, 2)] * rkM.mEntry[I(2, 1)] + 
									mEntry[I(3, 3)] * rkM.mEntry[I(3, 1)];
		kProd.mEntry[I(3, 2)] 	=	mEntry[I(3, 0)] * rkM.mEntry[I(0, 2)] +
									mEntry[I(3, 1)] * rkM.mEntry[I(1, 2)] +
									mEntry[I(3, 2)] * rkM.mEntry[I(2, 2)] +
									mEntry[I(3, 3)] * rkM.mEntry[I(3, 2)];
		kProd.mEntry[I(3, 3)] 	=	mEntry[I(3, 0)] * rkM.mEntry[I(0, 3)] +
									mEntry[I(3, 1)] * rkM.mEntry[I(1, 3)] +
									mEntry[I(3, 2)] * rkM.mEntry[I(2, 3)] +
									mEntry[I(3, 3)] * rkM.mEntry[I(3, 3)];

		return kProd;
	}

	/* Returns the inverse of the matrix.
	 */
	Matrix4 Matrix4::Inverse() const
	{
		float fA0 = mEntry[I(0, 0)]	*	mEntry[I(1, 1)] - mEntry[I(0, 1)]	*	mEntry[I(1, 0)];
		float fA1 = mEntry[I(0, 0)]	*	mEntry[I(1, 2)] - mEntry[I(0, 2)]	*	mEntry[I(1, 0)];
		float fA2 = mEntry[I(0, 0)]	*	mEntry[I(1, 3)] - mEntry[I(0, 3)]	*	mEntry[I(1, 0)];
		float fA3 = mEntry[I(0, 1)]	*	mEntry[I(1, 2)] - mEntry[I(0, 2)]	*	mEntry[I(1, 1)];
		float fA4 = mEntry[I(0, 1)]	*	mEntry[I(1, 3)] - mEntry[I(0, 3)]	*	mEntry[I(1, 1)];
		float fA5 = mEntry[I(0, 2)]	*	mEntry[I(1, 3)] - mEntry[I(0, 3)]	*	mEntry[I(1, 2)];
		float fB0 = mEntry[I(2, 0)]	*	mEntry[I(3, 1)] - mEntry[I(2, 1)]	*	mEntry[I(3, 0)];
		float fB1 = mEntry[I(2, 0)]	*	mEntry[I(3, 2)] - mEntry[I(2, 2)]	*	mEntry[I(3, 0)];
		float fB2 = mEntry[I(2, 0)]	*	mEntry[I(3, 3)] - mEntry[I(2, 3)]	*	mEntry[I(3, 0)];
		float fB3 = mEntry[I(2, 1)]	*	mEntry[I(3, 2)] - mEntry[I(2, 2)]	*	mEntry[I(3, 1)];
		float fB4 = mEntry[I(2, 1)]	*	mEntry[I(3, 3)] - mEntry[I(2, 3)]	*	mEntry[I(3, 1)];
		float fB5 = mEntry[I(2, 2)]	*	mEntry[I(3, 3)] - mEntry[I(2, 3)]	*	mEntry[I(3, 2)];

		float fDet = fA0 * fB5 - fA1 * fB4 + fA2 * fB3 + fA3 * fB2 - fA4 * fB1 + fA5 * fB0;
		if (Functions::FAbs(fDet) <= Constants::ZERO_TOLERANCE)
		{
			return Matrix4::ZERO;
		}

		Matrix4 kInv;
		kInv.mEntry[I(0, 0)] = + mEntry[I(1, 1)]*fB5 - mEntry[I(1, 2)]*fB4 + mEntry[I(1, 3)]*fB3;
		kInv.mEntry[I(1, 0)] = - mEntry[I(1, 0)]*fB5 + mEntry[I(1, 2)]*fB2 - mEntry[I(1, 3)]*fB1;
		kInv.mEntry[I(2, 0)] = + mEntry[I(1, 0)]*fB4 - mEntry[I(1, 1)]*fB2 + mEntry[I(1, 3)]*fB0;
		kInv.mEntry[I(3, 0)] = - mEntry[I(1, 0)]*fB3 + mEntry[I(1, 1)]*fB1 - mEntry[I(1, 2)]*fB0;
		kInv.mEntry[I(0, 1)] = - mEntry[I(0, 1)]*fB5 + mEntry[I(0, 2)]*fB4 - mEntry[I(0, 3)]*fB3;
		kInv.mEntry[I(1, 1)] = + mEntry[I(0, 0)]*fB5 - mEntry[I(0, 2)]*fB2 + mEntry[I(0, 3)]*fB1;
		kInv.mEntry[I(2, 1)] = - mEntry[I(0, 0)]*fB4 + mEntry[I(0, 1)]*fB2 - mEntry[I(0, 3)]*fB0;
		kInv.mEntry[I(3, 1)] = + mEntry[I(0, 0)]*fB3 - mEntry[I(0, 1)]*fB1 + mEntry[I(0, 2)]*fB0;
		kInv.mEntry[I(0, 2)] = + mEntry[I(3, 1)]*fA5 - mEntry[I(3, 2)]*fA4 + mEntry[I(3, 3)]*fA3;
		kInv.mEntry[I(1, 2)] = - mEntry[I(3, 0)]*fA5 + mEntry[I(3, 2)]*fA2 - mEntry[I(3, 3)]*fA1;
		kInv.mEntry[I(2, 2)] = + mEntry[I(3, 0)]*fA4 - mEntry[I(3, 1)]*fA2 + mEntry[I(3, 3)]*fA0;
		kInv.mEntry[I(3, 2)] = - mEntry[I(3, 0)]*fA3 + mEntry[I(3, 1)]*fA1 - mEntry[I(3, 2)]*fA0;
		kInv.mEntry[I(0, 3)] = - mEntry[I(2, 1)]*fA5 + mEntry[I(2, 2)]*fA4 - mEntry[I(2, 3)]*fA3;
		kInv.mEntry[I(1, 3)] = + mEntry[I(2, 0)]*fA5 - mEntry[I(2, 2)]*fA2 + mEntry[I(2, 3)]*fA1;
		kInv.mEntry[I(2, 3)] = - mEntry[I(2, 0)]*fA4 + mEntry[I(2, 1)]*fA2 - mEntry[I(2, 3)]*fA0;
		kInv.mEntry[I(3, 3)] = + mEntry[I(2, 0)]*fA3 - mEntry[I(2, 1)]*fA1 + mEntry[I(2, 2)]*fA0;

		float fInvDet = (1.0f)/fDet;
		for (int iRow = 0; iRow < 4; iRow++)
		{
			for (int iCol = 0; iCol < 4; iCol++)
			{
				kInv.mEntry[I(iRow, iCol)] *= fInvDet;
			}
		}

		return kInv;
	}
	
	/* Returns the transpose of the matrix
	 */
	Matrix4 Matrix4::Transpose() const
	{
		Matrix4 kTranspose;
		for (int iRow = 0; iRow < 4; iRow++)
		{
			for (int iCol = 0; iCol < 4; iCol++)
			{
				kTranspose.mEntry[I(iRow,iCol)] = mEntry[I(iCol,iRow)];
			}
		}
		return kTranspose;
	}

	/* Matrix times vector
	 */
	Vector4 Matrix4::operator* (const Vector4& rkV) const
	{
		Vector4 kProd(0.0f, 0.0f, 0.0f, 0.0f);

		for (int iRow = 0; iRow < 4; iRow++)
		{
			for (int iCol = 0; iCol < 4; iCol++)
			{
				kProd.mEntry[iRow] += this->mEntry[I(iRow, iCol)] * rkV.mEntry[iCol];
			}
	            
		}
		return kProd;
	}
}
