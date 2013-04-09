#include "Stdafx.h"
#include "Matrix.h"

using namespace System;

namespace Otter
{
namespace Interface
{	
	Matrix::Matrix()
	{
		mEntries = gcnew array<float>(16);

		M11 = 1.0f; M12 = 0.0f; M13 = 0.0f; M14 = 0.0f;
		M21 = 0.0f; M22 = 1.0f; M23 = 0.0f; M24 = 0.0f;
		M31 = 0.0f; M32 = 0.0f; M33 = 1.0f; M34 = 0.0f;
		M41 = 0.0f; M42 = 0.0f; M43 = 0.0f; M44 = 1.0f;
	}

	Matrix^ Matrix::operator*(Matrix^ lhs, Matrix^ rhs)
	{
		Matrix^ kProd = gcnew Matrix();

		// Row 0
		kProd->mEntries[I(0, 0)] 	=	rhs->mEntries[I(0, 0)] * lhs->mEntries[I(0, 0)] + 
										rhs->mEntries[I(0, 1)] * lhs->mEntries[I(1, 0)] + 
										rhs->mEntries[I(0, 2)] * lhs->mEntries[I(2, 0)] + 
										rhs->mEntries[I(0, 3)] * lhs->mEntries[I(3, 0)];
		kProd->mEntries[I(0, 1)] 	= 	rhs->mEntries[I(0, 0)] * lhs->mEntries[I(0, 1)] + 
										rhs->mEntries[I(0, 1)] * lhs->mEntries[I(1, 1)] + 
										rhs->mEntries[I(0, 2)] * lhs->mEntries[I(2, 1)] + 
										rhs->mEntries[I(0, 3)] * lhs->mEntries[I(3, 1)];
		kProd->mEntries[I(0, 2)] 	= 	rhs->mEntries[I(0, 0)] * lhs->mEntries[I(0, 2)] +
										rhs->mEntries[I(0, 1)] * lhs->mEntries[I(1, 2)] +
										rhs->mEntries[I(0, 2)] * lhs->mEntries[I(2, 2)] +
										rhs->mEntries[I(0, 3)] * lhs->mEntries[I(3, 2)];
		kProd->mEntries[I(0, 3)] 	= 	rhs->mEntries[I(0, 0)] * lhs->mEntries[I(0, 3)] +
										rhs->mEntries[I(0, 1)] * lhs->mEntries[I(1, 3)] +
										rhs->mEntries[I(0, 2)] * lhs->mEntries[I(2, 3)] +
										rhs->mEntries[I(0, 3)] * lhs->mEntries[I(3, 3)];

		// Row 1
		kProd->mEntries[I(1, 0)] 	=	rhs->mEntries[I(1, 0)] * lhs->mEntries[I(0, 0)] + 
										rhs->mEntries[I(1, 1)] * lhs->mEntries[I(1, 0)] + 
										rhs->mEntries[I(1, 2)] * lhs->mEntries[I(2, 0)] + 
										rhs->mEntries[I(1, 3)] * lhs->mEntries[I(3, 0)];
		kProd->mEntries[I(1, 1)] 	=	rhs->mEntries[I(1, 0)] * lhs->mEntries[I(0, 1)] + 
										rhs->mEntries[I(1, 1)] * lhs->mEntries[I(1, 1)] + 
										rhs->mEntries[I(1, 2)] * lhs->mEntries[I(2, 1)] + 
										rhs->mEntries[I(1, 3)] * lhs->mEntries[I(3, 1)];
		kProd->mEntries[I(1, 2)] 	=	rhs->mEntries[I(1, 0)] * lhs->mEntries[I(0, 2)] +
										rhs->mEntries[I(1, 1)] * lhs->mEntries[I(1, 2)] +
										rhs->mEntries[I(1, 2)] * lhs->mEntries[I(2, 2)] +
										rhs->mEntries[I(1, 3)] * lhs->mEntries[I(3, 2)];
		kProd->mEntries[I(1, 3)] 	=	rhs->mEntries[I(1, 0)] * lhs->mEntries[I(0, 3)] +
										rhs->mEntries[I(1, 1)] * lhs->mEntries[I(1, 3)] +
										rhs->mEntries[I(1, 2)] * lhs->mEntries[I(2, 3)] +
										rhs->mEntries[I(1, 3)] * lhs->mEntries[I(3, 3)];

		// Row 2
		kProd->mEntries[I(2, 0)] 	=	rhs->mEntries[I(2, 0)] * lhs->mEntries[I(0, 0)] + 
										rhs->mEntries[I(2, 1)] * lhs->mEntries[I(1, 0)] + 
										rhs->mEntries[I(2, 2)] * lhs->mEntries[I(2, 0)] + 
										rhs->mEntries[I(2, 3)] * lhs->mEntries[I(3, 0)];
		kProd->mEntries[I(2, 1)] 	=	rhs->mEntries[I(2, 0)] * lhs->mEntries[I(0, 1)] + 
										rhs->mEntries[I(2, 1)] * lhs->mEntries[I(1, 1)] + 
										rhs->mEntries[I(2, 2)] * lhs->mEntries[I(2, 1)] + 
										rhs->mEntries[I(2, 3)] * lhs->mEntries[I(3, 1)];
		kProd->mEntries[I(2, 2)] 	=	rhs->mEntries[I(2, 0)] * lhs->mEntries[I(0, 2)] +
										rhs->mEntries[I(2, 1)] * lhs->mEntries[I(1, 2)] +
										rhs->mEntries[I(2, 2)] * lhs->mEntries[I(2, 2)] +
										rhs->mEntries[I(2, 3)] * lhs->mEntries[I(3, 2)];
		kProd->mEntries[I(2, 3)] 	=	rhs->mEntries[I(2, 0)] * lhs->mEntries[I(0, 3)] +
										rhs->mEntries[I(2, 1)] * lhs->mEntries[I(1, 3)] +
										rhs->mEntries[I(2, 2)] * lhs->mEntries[I(2, 3)] +
										rhs->mEntries[I(2, 3)] * lhs->mEntries[I(3, 3)];

		// Row 3
		kProd->mEntries[I(3, 0)] 	=	rhs->mEntries[I(3, 0)] * lhs->mEntries[I(0, 0)] + 
										rhs->mEntries[I(3, 1)] * lhs->mEntries[I(1, 0)] + 
										rhs->mEntries[I(3, 2)] * lhs->mEntries[I(2, 0)] + 
										rhs->mEntries[I(3, 3)] * lhs->mEntries[I(3, 0)];
		kProd->mEntries[I(3, 1)] 	=	rhs->mEntries[I(3, 0)] * lhs->mEntries[I(0, 1)] + 
										rhs->mEntries[I(3, 1)] * lhs->mEntries[I(1, 1)] + 
										rhs->mEntries[I(3, 2)] * lhs->mEntries[I(2, 1)] + 
										rhs->mEntries[I(3, 3)] * lhs->mEntries[I(3, 1)];
		kProd->mEntries[I(3, 2)] 	=	rhs->mEntries[I(3, 0)] * lhs->mEntries[I(0, 2)] +
										rhs->mEntries[I(3, 1)] * lhs->mEntries[I(1, 2)] +
										rhs->mEntries[I(3, 2)] * lhs->mEntries[I(2, 2)] +
										rhs->mEntries[I(3, 3)] * lhs->mEntries[I(3, 2)];
		kProd->mEntries[I(3, 3)] 	=	rhs->mEntries[I(3, 0)] * lhs->mEntries[I(0, 3)] +
										rhs->mEntries[I(3, 1)] * lhs->mEntries[I(1, 3)] +
										rhs->mEntries[I(3, 2)] * lhs->mEntries[I(2, 3)] +
										rhs->mEntries[I(3, 3)] * lhs->mEntries[I(3, 3)];

		return kProd;
	}

	Matrix^ Matrix::RotationZ(float fAngle)
	{
		Matrix^ matrix = gcnew Matrix();

		float fCos, fSin;
		fCos = (float)Math::Cos(fAngle);
		fSin = (float)Math::Sin(fAngle);
		
		matrix->mEntries[I(0, 0)] = fCos;
		matrix->mEntries[I(0, 1)] = -fSin;
		matrix->mEntries[I(0, 2)] = 0.0f;
		matrix->mEntries[I(0, 3)] = 0.0f;
					  
		matrix->mEntries[I(1, 0)] = fSin;
		matrix->mEntries[I(1, 1)] = fCos;
		matrix->mEntries[I(1, 2)] = 0.0f;
		matrix->mEntries[I(1, 3)] = 0.0f;
					  
		matrix->mEntries[I(2, 0)] = 0.0f;
		matrix->mEntries[I(2, 1)] = 0.0f;
		matrix->mEntries[I(2, 2)] = 1.0f;
		matrix->mEntries[I(2, 3)] = 0.0f;
					  
		matrix->mEntries[I(3, 0)] = 0.0f;
		matrix->mEntries[I(3, 1)] = 0.0f;
		matrix->mEntries[I(3, 2)] = 0.0f;
		matrix->mEntries[I(3, 3)] = 1.0f;

		return matrix;
	}

	Matrix^ Matrix::Translation(float x, float y, float z)
	{
		Matrix^ mtx = gcnew Matrix();

		mtx->mEntries[I(0,3)] = x;
		mtx->mEntries[I(1,3)] = y;
		mtx->mEntries[I(2,3)] = z;
		mtx->mEntries[I(3,3)] = 1.0f;

		return mtx;
	}

	Matrix^ Matrix::Invert(Matrix^ matrix)
	{
		float fA0 = matrix->mEntries[I(0, 0)]	*	 matrix->mEntries[I(1, 1)] -  matrix->mEntries[I(0, 1)]	*	 matrix->mEntries[I(1, 0)];
		float fA1 = matrix->mEntries[I(0, 0)]	*	 matrix->mEntries[I(1, 2)] -  matrix->mEntries[I(0, 2)]	*	 matrix->mEntries[I(1, 0)];
		float fA2 = matrix->mEntries[I(0, 0)]	*	 matrix->mEntries[I(1, 3)] -  matrix->mEntries[I(0, 3)]	*	 matrix->mEntries[I(1, 0)];
		float fA3 = matrix->mEntries[I(0, 1)]	*	 matrix->mEntries[I(1, 2)] -  matrix->mEntries[I(0, 2)]	*	 matrix->mEntries[I(1, 1)];
		float fA4 = matrix->mEntries[I(0, 1)]	*	 matrix->mEntries[I(1, 3)] -  matrix->mEntries[I(0, 3)]	*	 matrix->mEntries[I(1, 1)];
		float fA5 = matrix->mEntries[I(0, 2)]	*	 matrix->mEntries[I(1, 3)] -  matrix->mEntries[I(0, 3)]	*	 matrix->mEntries[I(1, 2)];
		float fB0 = matrix->mEntries[I(2, 0)]	*	 matrix->mEntries[I(3, 1)] -  matrix->mEntries[I(2, 1)]	*	 matrix->mEntries[I(3, 0)];
		float fB1 = matrix->mEntries[I(2, 0)]	*	 matrix->mEntries[I(3, 2)] -  matrix->mEntries[I(2, 2)]	*	 matrix->mEntries[I(3, 0)];
		float fB2 = matrix->mEntries[I(2, 0)]	*	 matrix->mEntries[I(3, 3)] -  matrix->mEntries[I(2, 3)]	*	 matrix->mEntries[I(3, 0)];
		float fB3 = matrix->mEntries[I(2, 1)]	*	 matrix->mEntries[I(3, 2)] -  matrix->mEntries[I(2, 2)]	*	 matrix->mEntries[I(3, 1)];
		float fB4 = matrix->mEntries[I(2, 1)]	*	 matrix->mEntries[I(3, 3)] -  matrix->mEntries[I(2, 3)]	*	 matrix->mEntries[I(3, 1)];
		float fB5 = matrix->mEntries[I(2, 2)]	*	 matrix->mEntries[I(3, 3)] -  matrix->mEntries[I(2, 3)]	*	 matrix->mEntries[I(3, 2)];

		float fDet = fA0 * fB5 - fA1 * fB4 + fA2 * fB3 + fA3 * fB2 - fA4 * fB1 + fA5 * fB0;
		if (Math::Abs(fDet) <= (1e-06f))
		{
			return nullptr;
		}

		Matrix^ kInv = gcnew Matrix();
		kInv->mEntries[I(0, 0)] = +  matrix->mEntries[I(1, 1)]*fB5 -  matrix->mEntries[I(1, 2)]*fB4 +  matrix->mEntries[I(1, 3)]*fB3;
		kInv->mEntries[I(1, 0)] = -  matrix->mEntries[I(1, 0)]*fB5 +  matrix->mEntries[I(1, 2)]*fB2 -  matrix->mEntries[I(1, 3)]*fB1;
		kInv->mEntries[I(2, 0)] = +  matrix->mEntries[I(1, 0)]*fB4 -  matrix->mEntries[I(1, 1)]*fB2 +  matrix->mEntries[I(1, 3)]*fB0;
		kInv->mEntries[I(3, 0)] = -  matrix->mEntries[I(1, 0)]*fB3 +  matrix->mEntries[I(1, 1)]*fB1 -  matrix->mEntries[I(1, 2)]*fB0;
		kInv->mEntries[I(0, 1)] = -  matrix->mEntries[I(0, 1)]*fB5 +  matrix->mEntries[I(0, 2)]*fB4 -  matrix->mEntries[I(0, 3)]*fB3;
		kInv->mEntries[I(1, 1)] = +  matrix->mEntries[I(0, 0)]*fB5 -  matrix->mEntries[I(0, 2)]*fB2 +  matrix->mEntries[I(0, 3)]*fB1;
		kInv->mEntries[I(2, 1)] = -  matrix->mEntries[I(0, 0)]*fB4 +  matrix->mEntries[I(0, 1)]*fB2 -  matrix->mEntries[I(0, 3)]*fB0;
		kInv->mEntries[I(3, 1)] = +  matrix->mEntries[I(0, 0)]*fB3 -  matrix->mEntries[I(0, 1)]*fB1 +  matrix->mEntries[I(0, 2)]*fB0;
		kInv->mEntries[I(0, 2)] = +  matrix->mEntries[I(3, 1)]*fA5 -  matrix->mEntries[I(3, 2)]*fA4 +  matrix->mEntries[I(3, 3)]*fA3;
		kInv->mEntries[I(1, 2)] = -  matrix->mEntries[I(3, 0)]*fA5 +  matrix->mEntries[I(3, 2)]*fA2 -  matrix->mEntries[I(3, 3)]*fA1;
		kInv->mEntries[I(2, 2)] = +  matrix->mEntries[I(3, 0)]*fA4 -  matrix->mEntries[I(3, 1)]*fA2 +  matrix->mEntries[I(3, 3)]*fA0;
		kInv->mEntries[I(3, 2)] = -  matrix->mEntries[I(3, 0)]*fA3 +  matrix->mEntries[I(3, 1)]*fA1 -  matrix->mEntries[I(3, 2)]*fA0;
		kInv->mEntries[I(0, 3)] = -  matrix->mEntries[I(2, 1)]*fA5 +  matrix->mEntries[I(2, 2)]*fA4 -  matrix->mEntries[I(2, 3)]*fA3;
		kInv->mEntries[I(1, 3)] = +  matrix->mEntries[I(2, 0)]*fA5 -  matrix->mEntries[I(2, 2)]*fA2 +  matrix->mEntries[I(2, 3)]*fA1;
		kInv->mEntries[I(2, 3)] = -  matrix->mEntries[I(2, 0)]*fA4 +  matrix->mEntries[I(2, 1)]*fA2 -  matrix->mEntries[I(2, 3)]*fA0;
		kInv->mEntries[I(3, 3)] = +  matrix->mEntries[I(2, 0)]*fA3 -  matrix->mEntries[I(2, 1)]*fA1 +  matrix->mEntries[I(2, 2)]*fA0;

		float fInvDet = (1.0f)/fDet;
		for (int iRow = 0; iRow < 4; iRow++)
		{
			for (int iCol = 0; iCol < 4; iCol++)
			{
				kInv->mEntries[I(iRow, iCol)] *= fInvDet;
			}
		}

		return kInv;
	}
}
}