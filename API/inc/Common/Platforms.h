#pragma once

#ifndef PLATFORM_WIN32
#define sprintf_s(d, ds, fmt, ...) sprintf(d, fmt, __VA_ARGS__)
#define memcpy_s(d, ds, s, ss) memcpy(d, s, ss)
#define vsprintf_s(d, ds, fmt, args) vsprintf(d, fmt, args)
#define strncpy_s(d, ds, s, ss) strncpy(d, s, ss)
#endif