#include <stdio.h>
#include <string.h>

#include "SampleFileSystem.h"

bool FullFilePath(const char* szRelativePath, char* szFullPath, uint32 len);

/**
 * Opens a file according to the read/write flags.
 */
SampleFileSystem::SampleFileSystem(void)
{	
	
}

/**
 * Closes the file when the VirtualFile is destroyed.
 */
SampleFileSystem::~SampleFileSystem(void)
{
}

/* @brief Opens a file
 */
void* SampleFileSystem::Open(const char* szFilename, Otter::AccessFlag flags)
{
	char mode[8] = { 0 };
	
	if(flags & Otter::kWrite)
	{
		strcat(mode, "w");
	}
	
	if(flags & Otter::kRead)
	{
		strcat(mode, "r");
	}
	
	if(flags & Otter::kBinary)
	{
		strcat(mode, "b");
	}
	
	if(flags & Otter::kAppend)
	{
		strcat(mode, "+");
	}
	
	char fullPath[256];
	FullFilePath(szFilename, fullPath, 256);
	
	FILE* pFile = fopen(fullPath, mode);
	return pFile;
}

/* @brief Closes the file
 */
void SampleFileSystem::Close(void* pHandle)
{
	if(!pHandle)
		return;
	
	fclose((FILE*)pHandle);
}

/* @brief Reads data from the file.
 */
uint32 SampleFileSystem::Read(void* pHandle, uint8* data, uint32 count)
{
	if(!pHandle)
		return 0;
	
	return (uint32)fread((char*)data, 1, count, (FILE*)pHandle);
}

/* @brief Writes data to the file.
 */
uint32 SampleFileSystem::Write(void* pHandle, uint8* data, uint32 count)
{
	if(!pHandle)
		return 0;
	
	return (uint32)fwrite((char*)data, 1, count, (FILE*)pHandle);
}

/* @brief Seeks within the file.
 */
void SampleFileSystem::Seek(void* pHandle, uint32 offset, Otter::SeekFlag seekFlag)
{
	if(!pHandle)
		return;
	
	int flag = 0;
	if(seekFlag & Otter::kBegin)
	{
		flag = SEEK_SET;
	}
	
	if(seekFlag & Otter::kCurrent)
	{
		flag = SEEK_CUR;
	}
	
	if(seekFlag & Otter::kEnd)
	{
		flag = SEEK_END;
	}
	
	fseek((FILE*)pHandle, offset, flag);
}

/* @brief Returns the size of the file
 */
uint32 SampleFileSystem::Size(void* pHandle)
{
	if(!pHandle)
		return 0;
	
	uint32 pos = ftell((FILE*)pHandle);
	
	fseek((FILE*)pHandle, 0, SEEK_END); // move to end of file
	uint32 fileSize = ftell((FILE*)pHandle);
	fseek((FILE*)pHandle, pos, SEEK_SET);
	
	return fileSize;
}

bool FullFilePath(const char* szRelativePath, char* szFullPath, uint32 len)
{			
#if defined(PLATFORM_WIN32)
	sprintf(szFullPath, "Data/Win32/%s", szRelativePath);
#elif defined(PLATFORM_IPHONE)
	
	char tmp[256];
	strcpy(tmp, szRelativePath);	
	
	for(sint32 i = 0, n = strlen(tmp); i < n; i++)
	{
		if(tmp[i] == '\\')
			tmp[i] = '/';
	}
	
	CFBundleRef mainBundle = CFBundleGetMainBundle();
	
	CFStringRef		fileNameStr = CFStringCreateWithCString (NULL, tmp, kCFStringEncodingMacRoman);
	CFURLRef		fileURL = CFBundleCopyResourceURL(mainBundle, fileNameStr, CFSTR(""), NULL);
	
	if (fileURL == NULL)
	{
		strcpy(szFullPath, tmp);
	}
	else  
	{
		CFURLGetFileSystemRepresentation( fileURL, true, (UInt8*)szFullPath, len);	
		
		CFRelease( fileURL );
		fileURL = NULL;
	}		
	
	CFRelease( fileNameStr );
	fileNameStr = NULL;
#elif defined(PLATFORM_ANDROID)		
	char tmp[256];
	strcpy(tmp, szRelativePath);	
	
	for(sint32 i = 0, n = strlen(tmp); i < n; i++)
	{
		if(tmp[i] == '\\')
			tmp[i] = '/';
	}
	
	sprintf(szFullPath, "/sdcard/otter/%s", tmp);
#endif
	return true;
}
