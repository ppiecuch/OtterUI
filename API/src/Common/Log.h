#ifndef OTTER_UI_LOG_H
#define OTTER_UI_LOG_H

#include "Common/Types.h"
#include "Common/Platforms.h"

namespace Otter
{
	class System;

	void InitLog(System* pSystem);
	void DeinitLog();

	void LogInfo(const char* fmt, ...);
	
	bool LogIsEnabled();
}

#endif