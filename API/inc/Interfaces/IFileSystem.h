#pragma once

#include "Common/Types.h"

namespace Otter
{
	enum AccessFlag
	{
		kBinary	 		=	(1 << 0),
		kRead	 		=	(1 << 1),
		kWrite	 		=	(1 << 2),
		kAppend  		=	(1 << 3),
		kTruncate		=	(1 << 4)
	};

	enum SeekFlag
	{
		kBegin			=	(1 << 0),
		kCurrent		=	(1 << 1),
		kEnd			=	(1 << 2)
	};

	/**
	 * Custom File System.  Used to implement own file i/o routines.
	 */
	class IFileSystem
	{
	public:
		/**
		 * Constructor
		 */
		IFileSystem(void) { }

		/**
		 * Virtual Destructor
		 */
		virtual ~IFileSystem(void) { }

	public:		

		/**
		 * Opens a file
		 */
		virtual void* Open(const char* szFilename, AccessFlag flags) = 0;

		/**
		 * Closes the file
		 */
		virtual void Close(void* pHandle) = 0;

		/**
		 * Reads data from the file.
		 */
		virtual uint32 Read(void* pHandle, uint8* data, uint32 count) = 0;

		/**
		 * Writes data to the file.
		 */
		virtual uint32 Write(void* pHandle, uint8* data, uint32 count) = 0;

		/**
		 * Seeks within the file.
		 */
		virtual void Seek(void* pHandle, uint32 offset, SeekFlag seekFlag) = 0;

		/**
		 * Returns the size of the file
		 */
		virtual uint32 Size(void* pHandle) = 0;
	};
}