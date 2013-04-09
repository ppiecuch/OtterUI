#include <stdio.h>
#include <stdarg.h>

#include "Log.h"
#include "System.h"

namespace Otter
{
	static System* gSystem = NULL;

	void InitLog(System* pSystem)
	{
		if(gSystem)
			return;

		gSystem = pSystem;
	}

	void DeinitLog()
	{
		gSystem = NULL;
	}

	void LogInfo(const char* fmt, ...)
	{
		if(!gSystem)
			return;

		char buffer[512];

		va_list args;
		va_start(args, fmt);
		vsprintf_s(buffer, 512, fmt, args);
		va_end(args);

		gSystem->mOnLog(gSystem, buffer);
	}

	bool LogIsEnabled()
	{
		return gSystem != NULL;
	}
}