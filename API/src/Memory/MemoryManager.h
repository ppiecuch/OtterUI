#pragma once

#ifndef __MEMORY_MANAGER_H__
#define __MEMORY_MANAGER_H__

#include "Common/Types.h"
#include <new>

namespace MemoryManager
{
	// Initialize any data needed to manage the memory pool
	void initializeMemoryManager(uint8* pBuffer, size_t size);

	// Deinitializes the memory manager
	void deinitializeMemoryManager();

	// return a pointer inside the memory pool
	// If no chunk can accommodate aSize call OnAllocFail()
	void* alloc(size_t aSize);

	// Free up a chunk previously allocated
	void dealloc(void* aPointer);

	// Checks if this is a valid pointer in the memory manager
	bool isValid(const void* aPointer);

	// Will scan the memory pool and return the total free space remaining
	int freeRemaining(void);

	// Will scan the memory pool and return the largest free space remaining
	int largestFree(void);

	// will scan the memory pool and return the smallest free space remaining
	int smallestFree(void);
};

#endif  // __MEMORY_MANAGER_H__
