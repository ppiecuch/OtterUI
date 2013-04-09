#include "Memory.h"

void* OTTER_ALLOC(uint32 size)
{
	return MemoryManager::alloc(size);
}

void OTTER_FREE(void* ptr)
{
	MemoryManager::dealloc(ptr);
}
