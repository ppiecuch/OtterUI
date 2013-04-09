#include <stdlib.h>
#include <assert.h>
#include "Common/Types.h"

namespace MemoryManager
{
#define MAGIC_NUMBER	0xBEEFBEEF

#define ALLOC_MARKER_1	0xABCDDCBA		// Markers for allocations
#define ALLOC_MARKER_2	0xBEEFBEEF		//

// Power-of-two alignment
#define ALIGN(val, alignment)	((val + (alignment - 1)) & ~(alignment - 1))

// If OVERRUN_SIG_LEN is positive, we use that many uintptr_t's to detect
// buffer overruns
#define OVERRUN_SIG_LEN 4

	uint8*			MM_pool = NULL;

	/* Memory Manager's "global variables."
	 */
	struct MemoryInfo
	{
		uintptr_t				mMagicNumber;	// If set to MAGIC_NUMBER defined above, then memory manager has been initialized
		uintptr_t				mSize;
		uintptr_t				mFirstBlock;
	};

	/**
	 * Simple struct to lock/unlock a function
	 * through scope.
	 */
	struct FunctionLocker
	{
		FunctionLocker()
		{
			/*
			if(MM_pool)
			{
				MemoryInfo* pInfo = (MemoryInfo*)MM_pool;
				EnterCriticalSection(&pInfo->mCritSection);
			}
			*/
		}

		~FunctionLocker()
		{
			/*
			if(MM_pool)
			{
				MemoryInfo* pInfo = (MemoryInfo*)MM_pool;
				LeaveCriticalSection(&pInfo->mCritSection);
			}
			*/
		}
	};
	
#define LOCK_THIS_FUNCTION	// FunctionLocker locker

	/* Struct that contains the information pertaining to a particular memory block.
	 * This block represents two kinds of memory blocks: allocated and free.
	 * As an allocated block, Next and Prev is not used.
	 */
	struct Block
	{
		uintptr_t	mSize;		// [Free/Allocated] Entire size of the block, including this header.
		uintptr_t	mNext;		// [Free Only]      Location of the next free block.
		uintptr_t	mPrev;		// [Free Only]      Location of the previous free block.

		/* [Free Only]
		 * Returns how much is actually available for 
		 * allocation in this block.
		 */
		inline uintptr_t AvailableForAlloc() const
		{
			return (mSize - sizeof(Block));
		}
	};

	// Initialize set up any data needed to manage the memory pool
	void initializeMemoryManager(uint8* pBuffer, size_t size)
	{
		if(size <= sizeof(MemoryInfo))
			return;

		MM_pool = pBuffer;

		// Initialize the memory info, by creating a large
		// free block that spans the entire memory pool, minus the size
		// of the memory info header.
		
		// Let's set up the initial free block first, and then use its information
		// to initialize the memory info header.
		Block* pBlock = (Block*)(MM_pool + sizeof(MemoryInfo));
		pBlock->mSize = size - sizeof(MemoryInfo);
		pBlock->mNext = 0;
		pBlock->mPrev = 0;

		MemoryInfo* pInfo = (MemoryInfo*)MM_pool;
		pInfo->mMagicNumber = MAGIC_NUMBER;
		pInfo->mSize = (uintptr_t)size;
		pInfo->mFirstBlock = sizeof(MemoryInfo);
	}
	
	// Deinitializes the memory manager
	void deinitializeMemoryManager()
	{
		MM_pool = NULL;
	}

	/* Allocates memory from the specified block.
	 * Modifies the free block appropriately, depending on the result
	 * of the allocation.  If the entire block is allocated, simply removes
	 * itself from the free list.  Otherwise resizes the free block.
	 */
	void* allocateFromBlock(Block* pBlock, uintptr_t size)
	{
		MemoryInfo* pInfo = (MemoryInfo*)MM_pool;
		Block* pAlloc = ((Block*)0);

		// If this allocation will take up the entire free block, or create a hole 
		// of zero size, dedicate this entire block to the allocation.
		if((pBlock->mSize - (size + sizeof(Block))) <= sizeof(Block))
		{
			// Fix up the pointers, effectively removing this block from the list
			if(pBlock->mPrev != 0)
			{
				Block* pPrev = (Block*)(MM_pool + pBlock->mPrev);
				pPrev->mNext = pBlock->mNext;
			}
			else
			{
				// This was the first free hole, so fix up the first empty
				// hole member
				pInfo->mFirstBlock = pBlock->mNext;
			}

			if(pBlock->mNext != 0)
			{
				Block* pNext = (Block*)(MM_pool + pBlock->mNext);
				pNext->mPrev = pBlock->mPrev;
			}

			// Free block turns into allocated block.
			// Set up member(s).
			pAlloc = (Block*)pBlock;
			pAlloc->mSize = pBlock->mSize;
			pAlloc->mNext = ALLOC_MARKER_1;
			pAlloc->mPrev = ALLOC_MARKER_2;
		}
		// This allocation will simply shrink the block.
		else
		{
			// Get a pointer to the allocated block.  We'll set up
			// its data members after we've moved the free block around
			
			const uintptr_t blockNext = pBlock->mNext;
			const uintptr_t blockPrev = pBlock->mPrev;
			const uintptr_t blockSize = pBlock->mSize;

			// Move the block, keeping its members intact.  Since we're
			const uintptr_t totalAllocSize = size + sizeof(Block);
			Block* pMovedBlock = (Block*)(((uint8*)pBlock) + totalAllocSize);
			pMovedBlock->mNext = blockNext;
			pMovedBlock->mPrev = blockPrev;
			pMovedBlock->mSize = blockSize - totalAllocSize;

			// Now fix up the prev/next blocks to point to the moved block
			if(blockPrev  != 0)
			{
				Block* pPrev = (Block*)(MM_pool + blockPrev);
				pPrev->mNext = (uintptr_t)pMovedBlock - (uintptr_t)MM_pool;
			}
			else
			{
				// This was the first free hole, so fix up the first empty
				// hole member
				pInfo->mFirstBlock = (uintptr_t)pMovedBlock - (uintptr_t)MM_pool;
			}

			if(blockNext != 0)
			{
				Block* pNext = (Block*)(MM_pool + blockNext);
				pNext->mPrev = (uintptr_t)pMovedBlock - (uintptr_t)MM_pool;
			}

			// pBlock is now invalid.  Turn it into an allocation block
			// and set up its members.
			// Note: We use the "Prev" and "Next" members to mark this as
			// an allocation block.  We can store a simple flag it is likely
			// that random memory could keep it set for us.  So the only way
			// to ensure that this memory remains /known/ as an allocated block
			// is to insert known bogus values in mNext and mPrev.
			// When this block is deallocated, these members will be set to 
			// valid values.
			pAlloc = (Block*)pBlock;
			pAlloc->mSize = totalAllocSize;
			pAlloc->mNext = ALLOC_MARKER_1;
			pAlloc->mPrev = ALLOC_MARKER_2;
		}

#if OVERRUN_SIG_LEN
		for(int i = 0; i < OVERRUN_SIG_LEN; i++)
		{
			uintptr_t* pInt = (uintptr_t*)&((uint8*)pAlloc)[pAlloc->mSize + (i - OVERRUN_SIG_LEN) * sizeof(uintptr_t)];
			*pInt = 0xBAADC0DE;
		}
#endif

		// Return the address of the actual allocation.
		return ((uint8*)pAlloc) + sizeof(Block);
	}

	/* Merges a block with its right (next) neighbour
	 * if possible.
	 */
	void rightMerge(Block* pBlock)
	{
		uintptr_t offset = (uintptr_t)pBlock - (uintptr_t)MM_pool;
		if(pBlock->mNext != 0)
		{
			// Only merge empty blocks if the space between them is zero.
			if((pBlock->mNext - offset) == pBlock->mSize)
			{
				Block* pNodeToMerge = (Block*)(MM_pool + pBlock->mNext);
				pBlock->mSize += pNodeToMerge->mSize;
				pBlock->mNext = pNodeToMerge->mNext;

				// Fix up linked list pointers.
				if(pNodeToMerge->mNext != 0)
				{
					Block* pNext = (Block*)(MM_pool + pNodeToMerge->mNext);
					pNext->mPrev = pNodeToMerge->mPrev;
				}
			}
		}
	}

	/* Merges a free block in the linked list with its neighbours,
	 * if possible.
	 */
	void mergeBlock(Block* pBlock)
	{
		if(pBlock->mNext != 0)
		{
			rightMerge(pBlock);
		}

		if(pBlock->mPrev != 0)
		{
			Block* pPrev = (Block*)(MM_pool + pBlock->mPrev);
			rightMerge(pPrev);
		}
	}

	/* Inserts a free block into the free block list
	 */
	void insertBlock(Block* pBlock, uintptr_t before, uintptr_t after)
	{
		uintptr_t offset = (uintptr_t)pBlock - (uintptr_t)MM_pool;
		pBlock->mPrev = before;
		pBlock->mNext = after;

		// / Fix up the prev/next nodes to make sure the linked list
		// is set up properly.
		if(pBlock->mPrev != 0)
		{
			Block* pPrev = (Block*)(MM_pool + pBlock->mPrev);
			pPrev->mNext = offset;
		}
		else
		{
			// This was the first free hole, so fix up the first empty
			// hole member
			MemoryInfo* pInfo = (MemoryInfo*)MM_pool;
			pInfo->mFirstBlock = offset;
		}

		if(pBlock->mNext != 0)
		{
			Block* pNext = (Block*)(MM_pool + pBlock->mNext);
			pNext->mPrev = offset;
		}

		mergeBlock(pBlock);
	}

	/* Implements first-fit allocation algorithm.  Returns the very first
	 * empty block that can fit our required allocation size.
	 */
	void* firstFitAlloc(uintptr_t size)
	{
		const MemoryInfo* pInfo = (MemoryInfo*)MM_pool;

		if(pInfo->mFirstBlock == 0)
		{
			// No free blocks available thus out of memory.
			assert(false);
			return ((void*) 0);
		}

		// First-Fit
		Block* pBlock = (Block*)&MM_pool[pInfo->mFirstBlock];
		while(pBlock->AvailableForAlloc() < size)
		{
			if(pBlock->mNext == 0)
			{
				// Could not find a block to fit this allocation,
				// so bail out.
				assert(false);
				return ((void*) 0);
			}

			pBlock = (Block*)&MM_pool[pBlock->mNext];
		}

		return allocateFromBlock(pBlock, size);
	}

	// return a pointer inside the memory pool
	// If no chunk can accommodate aSize call onOutOfMemory()
	void* alloc(size_t aSize)
	{
		// Can't alloc memory of size 0.
		if(aSize == 0)
			return (void*)0;

		if(MM_pool == NULL)
			return malloc(aSize);

		const MemoryInfo* pInfo = (MemoryInfo*)MM_pool;
		if(pInfo->mMagicNumber != MAGIC_NUMBER)
			return (void*)0;

		LOCK_THIS_FUNCTION;

		// We want to alloc in multiples of 4 (32bits).
		// Add extra padding for the overrun signature.
		const uintptr_t sizeToAlloc = ALIGN(aSize, 4) + OVERRUN_SIG_LEN * sizeof(uintptr_t);
		return firstFitAlloc(sizeToAlloc);
	}

	// Free up a chunk previously allocated
	void dealloc(void* aPointer)
	{
		if(MM_pool == NULL)
		{
			free(aPointer);
			return;
		}

		const MemoryInfo* pInfo = (MemoryInfo*)MM_pool;
		if(!aPointer || pInfo->mMagicNumber != MAGIC_NUMBER)
			return;

		LOCK_THIS_FUNCTION;

		Block* pAlloc = (Block*)((uint8*)aPointer - sizeof(Block));
		uintptr_t allocOffset = (uintptr_t)pAlloc - (uintptr_t)MM_pool;

		if( pAlloc->mNext != ALLOC_MARKER_1 ||
			pAlloc->mPrev != ALLOC_MARKER_2)
		{
			// Not a valid alloc, bail
			return;
		}
		
#if OVERRUN_SIG_LEN
		// Check for buffer overrun
		for(int i = 0; i < OVERRUN_SIG_LEN; i++)
		{
			int checkOffset = allocOffset + pAlloc->mSize + (i - OVERRUN_SIG_LEN) * sizeof(uintptr_t);
			if(*((uintptr_t*)&MM_pool[checkOffset]) != 0xBAADC0DE)
			{
				break;
			}
		}
#endif
		
		// Go through our list of free nodes,
		// and determine which free nodes bracket this alloc
		// node.
		uintptr_t before = 0;
		uintptr_t after = 0;

		if(pInfo->mFirstBlock > allocOffset)
		{
			after = pInfo->mFirstBlock;
		}
		// We need to search for the bracketing nodes.
		else if(pInfo->mFirstBlock != 0)
		{
			Block* pBlock = (Block*)(MM_pool + pInfo->mFirstBlock);
			while(pBlock->mNext != 0 && pBlock->mNext < allocOffset)
			{
				pBlock = (Block*)(MM_pool + pBlock->mNext);
			}

			before = (uintptr_t)pBlock - (uintptr_t)MM_pool;
			after = pBlock->mNext;
		}

		// Now that we've found our bracketing nodes, insert it
		// into the linked list.
		Block* pBlock = (Block*)pAlloc;
		pBlock->mSize = pAlloc->mSize;
		pBlock->mNext = 0;
		pBlock->mPrev = 0;

		insertBlock(pBlock, before, after);
	}

	// Checks if this is a valid pointer in the memory manager
	bool isValid(const void* aPointer)
	{
		if (!MM_pool)
			return false;

		const MemoryInfo* pInfo = (MemoryInfo*)MM_pool;
		if(!aPointer || pInfo->mMagicNumber != MAGIC_NUMBER)
			return false;

		bool inRange = ((uintptr_t)aPointer >= (uintptr_t)MM_pool) && ((uintptr_t)aPointer <= (uintptr_t)MM_pool + (uintptr_t)pInfo->mSize);
		return inRange;
	}

	//---
	//--- support routines
	//--- 

	// Will scan the memory pool and return the total free space remaining
	int freeRemaining(void)
	{
		if (!MM_pool)
			return 0;

		LOCK_THIS_FUNCTION;

		const MemoryInfo* pInfo = (MemoryInfo*)MM_pool;
		if(pInfo->mFirstBlock == 0)
			return 0;
		
		const Block* pBlock = (const Block*)(MM_pool + pInfo->mFirstBlock);
		sint32 totalAvailable = pBlock->AvailableForAlloc();
		while(pBlock->mNext != 0)
		{
			pBlock = (const Block*)(MM_pool + pBlock->mNext);
			totalAvailable += pBlock->AvailableForAlloc();
		}

		return totalAvailable;
	}

	// Will scan the memory pool and return the largest free space remaining
	int largestFree(void)
	{
		LOCK_THIS_FUNCTION;

		const MemoryInfo* pInfo = (MemoryInfo*)MM_pool;
		if(pInfo->mFirstBlock == 0)
			return 0;
		
		const Block* pBlock = (const Block*)(MM_pool + pInfo->mFirstBlock);
		const Block* pLargest = pBlock;
		while(pBlock->mNext != 0)
		{
			const Block* pNext = (const Block*)(MM_pool + pBlock->mNext);
			if(pNext->mSize > pLargest->mSize)
				pLargest = pNext;

			pBlock = pNext;
		}

		return pLargest->AvailableForAlloc();
	}

	// Will scan the memory pool and return the smallest free space remaining
	int smallestFree(void)
	{
		LOCK_THIS_FUNCTION;

		MemoryInfo* pInfo = (MemoryInfo*)MM_pool;
		if(pInfo->mFirstBlock == 0)
			return 0;
		
		Block* pBlock = (Block*)(MM_pool + pInfo->mFirstBlock);
		Block* pSmallest = pBlock;
		while(pBlock->mNext != 0)
		{
			Block* pNext = (Block*)(MM_pool + pBlock->mNext);
			if(pNext->mSize < pSmallest->mSize)
				pSmallest = pNext;

			pBlock = pNext;
		}

		return pSmallest->AvailableForAlloc();
	}
}
