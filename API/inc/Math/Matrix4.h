#pragma once

#include "Vector4.h"

namespace VectorMath
{
	/**
	 * 4x4 Transformation Matrix.
	 *
	 * Matrix operations are right to left.  
	 * The matrix is stored as float[16] and organized in row-major order.  
	 * Entry of row 'r' and column 'c' is stored at: i = c + (4 * r)
	 */
	class Matrix4
	{
	public:
		/**
		 * Constructor
		 */
		Matrix4(void);
		
		/**
		 * Constructs the matrix with specific values.
		 */
		Matrix4 (	float fM00, float fM01, float fM02, float fM03,
             		float fM10, float fM11, float fM12, float fM13,
             		float fM20, float fM21, float fM22, float fM23,
             		float fM30, float fM31, float fM32, float fM33);

		/**
		 * Destructor
		 */
		~Matrix4(void);

	public:

		/**
		 * Multiplies this matrix with another.
		 * Operation is from left to right, ie (*this) * rkM is the
		 * transform of (*this) followed by rkM
		 */
		Matrix4 operator* (const Matrix4& rkM) const;		

		/**
		 * Returns the inverse of the matrix.
		 */
		Matrix4 Inverse() const;
		
		/**
		 * Returns the transpose of the matrix
		 */
		Matrix4 Transpose() const;

		/**
		 * Matrix times vector
		 */
		Vector4 operator*(const Vector4& rkV) const;

	public:
	
		static const Matrix4 ZERO;
		static const Matrix4 IDENTITY;

		/**
		 * Constructs a translation matrix
		 */
		static Matrix4 Translation(float x, float y, float z);

		/**
		 * Constructs a rotation matrix about X
		 */
		static Matrix4 RotationX(float fAngle);

		/**
		 * Constructs a rotation matrix about Y
		 */
		static Matrix4 RotationY(float fAngle);

		/**
		 * Constructs a rotation matrix about Z
		 */
		static Matrix4 RotationZ(float fAngle);

	public:

		float mEntry[16];
	};
}
