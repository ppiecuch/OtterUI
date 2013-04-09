#ifndef _MEMORY_H
#define _MEMORY_H

#include "MemoryManager.h"

void* OTTER_ALLOC(uint32 size);
void OTTER_FREE(void* ptr);

#define OTTER_NEW(cls, params) new ((void*)OTTER_ALLOC(sizeof(cls))) cls params

template<class T>
inline void OTTER_DELETE(T* ptr)
{
	ptr->~T();
	OTTER_FREE((void*)ptr);
}

#endif