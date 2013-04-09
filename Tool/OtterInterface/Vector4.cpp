#include "Stdafx.h"
#include "Vector4.h"

namespace Otter
{
namespace Interface
{	
	Vector4^ Vector4::Transform(Vector4^ vector, Matrix^ matrix)
	{
		Vector4^ kProd = gcnew Vector4(0.0f, 0.0f, 0.0f, 0.0f);

		for (int iRow = 0; iRow < 4; iRow++)
		{
			for (int iCol = 0; iCol < 4; iCol++)
			{
				kProd->mEntry[iRow] += matrix->Entries[Matrix::I(iRow, iCol)] * vector->mEntry[iCol];
			}
	            
		}
		return kProd;
	}
}
}