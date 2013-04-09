#ifndef MY_STRING_H_
#define MY_STRING_H_

#include "Array.h"
#include "Types.h"

namespace Otter
{
	class UTF8String
	{
	public:
		/**
		 * Constructor
		 */
		UTF8String();

		/**
		 * Constructor - Initializes the string with a NULL-terminated UTF-8 string
		 */
		UTF8String(const uint8 *szStr);

		/**
		 * Constructor - Initializes the string with a NULL-terminated ASCII string
		 */
		UTF8String(const char *szStr);

		/**
		 * Constructor - Initializes the string with a NULL-terminated wide-char unicode string
		 */
		UTF8String(const wchar_t* szStr);

		/**
		 * Copy constructor
		 */
		UTF8String(const UTF8String & str );  

		/**
		 * Destructor
		 */
		~UTF8String()
		{ 
			mBuffer.clear();
		}

		/**
		 * Returns the internal NULL-terminated UTF-8 string
		 */
		const uint8* GetUTF8() const
		{
			return &mBuffer[0];
		}

		/**
		 * Returns an array of NULL-terminated unicode chars that represents the
		 * string.
		 */
		bool GetUnicodeChars(Array<uint32>& unicodeChars) const;

		/**
		 * Returns the number of characters in the string, not including the null
		 * terminator
		 */
		uint32 StrLen() const;

		/**
		 * Returns the size of the internal buffer
		 */
		uint32 Size() const
		{
			return mBuffer.size();
		}

		/**
		 * Returns the unicode character at index k
		 */
		uint8  operator[](sint32 k) const;

		/**
		 * Returns the unicode character at index k
		 */
		uint8& operator[](sint32 k);

		const UTF8String& operator=(const UTF8String& rhs );
		const UTF8String& operator=(const uint8* szStr );
		const UTF8String& operator=(const wchar_t* szStr );
		const UTF8String& operator+=(const UTF8String& rhs );

	private:
		Array<uint8>	mBuffer;
		uint32			mNumChars;
	};

	bool operator==( const UTF8String & lhs, const UTF8String & rhs );    // Compare ==
	bool operator!=( const UTF8String & lhs, const UTF8String & rhs );    // Compare !=
	bool operator< ( const UTF8String & lhs, const UTF8String & rhs );    // Compare <
	bool operator<=( const UTF8String & lhs, const UTF8String & rhs );    // Compare <=
	bool operator> ( const UTF8String & lhs, const UTF8String & rhs );    // Compare >
	bool operator>=( const UTF8String & lhs, const UTF8String & rhs );    // Compare >=
}

#endif
