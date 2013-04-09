#pragma once

// stdint.h does not ship with Visual C++ versions prior to 2010
#if !defined(_MSC_VER) || _MSC_VER >= 1600 
	#include <stdint.h>
#else
	#ifndef _W64
	#  if !defined(__midl) && (defined(_X86_) || defined(_M_IX86)) && _MSC_VER >= 1300
	#     define _W64 __w64
	#  else
	#     define _W64
	#  endif
	#endif

	#ifdef _WIN64 // [
	   typedef signed __int64    intptr_t;
	   typedef unsigned __int64  uintptr_t;
	#else // _WIN64 ][
	   typedef _W64 signed int   intptr_t;
	   typedef _W64 unsigned int uintptr_t;
	#endif // _WIN64 ]
#endif

#ifndef _UINT8
#define _UINT8
typedef unsigned char		uint8;
#endif

#ifndef _SINT8
#define _SINT8
typedef char				sint8;
#endif

#ifndef _UINT16
#define _UINT16
typedef unsigned short		uint16;
#endif

#ifndef _SINT16
#define _SINT16
typedef short				sint16;
#endif

#ifndef _UINT32
#define _UINT32
typedef unsigned int		uint32;
#endif

#ifndef _SINT32
#define _SINT32
typedef int					sint32;
#endif

#ifndef _UINT64
#define _UINT64
typedef unsigned long long	uint64;
#endif

#ifndef _SINT64
#define _SINT64
typedef long long			sint64;
#endif

#ifndef NULL
#define NULL 0
#endif