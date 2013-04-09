#pragma once
#include "Otter.h"

/* Sample IFileSystem implementation.  Simply opens, reads from, and closes raw files using
 * standard C++ methods.
 */
class SampleFileSystem : public Otter::IFileSystem
{
public:
	SampleFileSystem(void);
	~SampleFileSystem(void);

public:		

	/* @brief Opens a file
	 */
	virtual void* Open(const char* szFilename, Otter::AccessFlag flags);

	/* @brief Closes the file
	 */
	virtual void Close(void* pHandle);

	/* @brief Reads data from the file.
	 */
	virtual uint32 Read(void* pHandle, uint8* data, uint32 count);

	/* @brief Writes data to the file.
	 */
	virtual uint32 Write(void* pHandle, uint8* data, uint32 count);

	/* @brief Seeks within the file.
	 */
	virtual void Seek(void* pHandle, uint32 offset, Otter::SeekFlag seekFlag);

	/* @brief Returns the size of the file
	 */
	virtual uint32 Size(void* pHandle);
};
