#ifndef VECTOR_H
#define VECTOR_H

#include <new>
#include <stdlib.h>
#include "Common/BaseTypes.h"

void* OTTER_ALLOC(uint32);
void OTTER_FREE(void*);

namespace Otter
{	

	/**
	 * A very simple dynamic array class, that manages a set of objects.
	 */
	template <class T>
	class Array
	{
	public:
		/**
		 * Constructs the array with the specified initial capacity
		 *
		 * @param capacity Initial array capacity
		 */
		explicit Array() : 
			mObjects(0),
			mCapacity(0), 
			mSize(0)
		{ 
		}

		/**
		 * Constructs the array with the specified initial capacity
		 *
		 * @param capacity Initial array capacity
		 */
		explicit Array(uint32 capacity) : 
			mObjects(0),
			mCapacity(capacity), 
			mSize(capacity)
		{ 
			if(mCapacity != 0)
				mObjects = (T*)alloc_buffer(mCapacity);
		}

		/**
		 * Copy constructor
		 */
		Array(const Array<T>& rhs) : 
			mObjects(0),
			mCapacity(0), 
			mSize(0)
		{ 
			operator=(rhs); 
		}

		/**
		 * Destructor
		 */
		~Array()
		{ 
			clear(true);
		}

		/**
		 * Reserves a specified capacity within the array
		 * With copy/truncate the existing elements in the array
		 * accordingly to the new capacity.
		 */
		void reserve(uint32 newCapacity)
		{			
			if(newCapacity == mCapacity)
				return;

			T *oldArray = mObjects;
			uint32 numToCopy = (newCapacity < mCapacity) ? newCapacity : mCapacity;

			mObjects = (T*)alloc_buffer(newCapacity);

			for(uint32 i = 0; i < numToCopy; i++)
			{
				::new (&mObjects[i]) T(oldArray[i]);
				oldArray[i].~T();
			}

			free_buffer(oldArray);

			mSize = (newCapacity < mCapacity) ? newCapacity : mSize;
			mCapacity = newCapacity;
		}

		/**
		 * Resizes the array, copying or truncating elements as necessary.
		 */
		void resize(uint32 newSize)
		{
			reserve(newSize);
			mSize = newSize;
		}
	
		/**
		 * Clears the entire array
		 */
		void clear(bool releaseMem = false)
		{
			// Call the destructors of all the objects up to this point
			for(uint32 i = 0; i < mSize; i++)
				mObjects[i].~T();

			mSize = 0;

			if(releaseMem)
			{
				if(mObjects)
					free_buffer(mObjects);

				mObjects = 0;
				mCapacity = 0;
			}
		}

		/** 
		 * Retrieves the number of elements in the array.
		 */
		uint32 size() const
		{ 
			return mSize; 
		}

		/**
		 * Erases an element as the provided index.
		 */
		uint32 erase(uint32 index)
		{
			if(index >= mSize)
				return 0;

			// Call its destructor

			for(uint32 i = index; i < mSize - 1; i++)
			{
				mObjects[i].~T();
				::new (&mObjects[i]) T(mObjects[i + 1]);
			}

			// Destroy the last one
			mObjects[mSize - 1].~T();

			mSize--;

			return index;
		}

		/**
		 * Retrieves an element at the specified index
		 */
		T& operator[](uint32 index)
		{
			return mObjects[index];
		}

		/**
		 * Retrieves an element at the specified index
		 */
		const T& operator[](uint32 index) const
		{
			return mObjects[index];
		}

		/**
		 * Copies an array
		 */
		const Array<T>& operator=(const Array<T>& rhs)
		{
			if(this != &rhs)
			{
				free_buffer(mObjects);
				mObjects = 0;

				mCapacity = rhs.mCapacity;
				mSize = rhs.mSize;

				if(mCapacity > 0)
				{
					mObjects = (T*)alloc_buffer(mCapacity);

					for(uint32 i = 0; i < mCapacity; i++ )
					{
						::new (&mObjects[i]) T(rhs.mObjects[i]);
					}
				}
			}

			return *this;
		}

		/**
		 * Pushes an element on the back of the array
		 */
		void push_back(const T& value)
		{
			if(mSize + 1 > mCapacity)
			{
				reserve((uint32)((mSize + 1) * 1.5));
			}

			::new (&mObjects[mSize++]) T(value);
		}

		/**
		 * Pops an element from the back of the array
		 */
		void pop_back()
		{
			erase(mSize - 1);
		}

		/**
		 * Pushes an element on the front of the array
		 */
		void push_front(const T& value)
		{
			if(mSize + 1 > mCapacity)
				reserve((uint32)((mSize + 1) * 1.5));
			
			mSize++;

			for(uint32 i = (mSize - 1); i >= 1; --i)
			{
				mObjects[i].~T();
				::new (&mObjects[i]) T(mObjects[i - 1]);
			}

			mObjects[0].~T();
			new (&mObjects[0]) T(value);
		}

		/**
		 * Pops an element from the front of the array
		 */
		void pop_front()
		{
			erase(0);
		}

	private:
		static void* alloc_buffer(uint32 count)
		{
			if(count == 0)
				return (void*)0;

			int obj_size = sizeof(T);
			if(obj_size > 4)
				obj_size = ((sizeof(T) | 0x3) & ~0x3);

			return OTTER_ALLOC(count * obj_size);
		}

		static void free_buffer(void* buffer)
		{
			OTTER_FREE(buffer);
		}

	private:

		/**
		 * Array elements
		 */
		T* mObjects;

		/**
		 * Array capacity (size of mObjects)
		 */
		uint32 mCapacity;

		/**
		 * Actual number of elements in the array (subset of mObjects)
		 */
		uint32 mSize;
	};	
}

#endif


