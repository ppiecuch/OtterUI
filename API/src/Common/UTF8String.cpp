#include <string.h>
#include "Common/UTF8String.h"
#include "utf8proc/utf8proc.h"

namespace Otter
{
	/* Helper - returns the number of characters in a UTF8 string
	 */
	uint32 UTF8StrLen(const uint8* chars)
	{			
		ssize_t pos = 0;
		uint32 uc = 0;
		uint32 count = 0;

		while (1) 
		{
			pos += utf8proc_iterate(chars + pos, -1, (int32_t*)&uc);
			count++;

			// checking of return value is not neccessary,
			// as 'uc' is < 0 in case of error 
			if (uc < 0) 
				return 0;

			if (pos < 0)   
				return 0;

			if (uc == 0) 
				break;
		}

		return count;
	}

	/* Constructor
		*/
	UTF8String::UTF8String()
	{
		mNumChars = 0;
		operator=("");
	}

	UTF8String::UTF8String(const uint8* szStr)
	{
		mNumChars = 0;
		operator=(szStr);
	}

	UTF8String::UTF8String(const char *szStr)
	{
		mNumChars = 0;
		operator=((const uint8*)szStr);
	}

	UTF8String::UTF8String(const wchar_t* szStr)
	{
		mNumChars = 0;
		operator=(szStr);
	}

	UTF8String::UTF8String(const UTF8String& str)
	{
		mNumChars = 0;
		operator=(str);
	}

	/* Returns the number of characters in the string
	 * TODO - we can store this, no need to recalculate every time.
	 */
	uint32 UTF8String::StrLen() const
	{
		return mNumChars;
	}

	/* Returns an array of NULL-terminated unicode chars that represents the
	 * string.
	 */
	bool UTF8String::GetUnicodeChars(Array<uint32>& unicodeChars) const
	{
		ssize_t pos = 0;
		unicodeChars.reserve(StrLen() + 1);
		uint32 uc = 0;

		while (1) 
		{
			pos += utf8proc_iterate(&mBuffer[0] + pos, -1, (int32_t*)&uc);
			unicodeChars.push_back(uc);

			// checking of return value is not neccessary,
			// as 'uc' is < 0 in case of error 
			if (uc < 0) 
				return false;

			if (pos < 0) 
				return false;

			if (uc == 0) 
				break;
		}

		return true;
	}

	const UTF8String & UTF8String::operator=(const UTF8String& rhs)
	{
		if(this == &rhs)
			return *this;

		mBuffer = rhs.mBuffer;

		mNumChars = UTF8StrLen(GetUTF8());			
		return *this;
	}

	const UTF8String& UTF8String::operator=(const uint8* szStr)
	{
		if(szStr == 0)
			szStr = (const uint8*)"";

		int count = 0;
		for(; szStr[count] != 0; count++);

		mBuffer.resize(count + 1);
		memcpy(&mBuffer[0], szStr, count + 1);

		mNumChars = UTF8StrLen(GetUTF8());
		return *this;
	}

	const UTF8String& UTF8String::operator=(const wchar_t* szStr)
	{
		if(szStr == 0)
			szStr = L"";

		// Determine the length of the final UTF-8 string
		uint32 len = 0;
		uint32 newSize = 0;
		for(; szStr[len] != 0; len++)
			newSize += utf8proc_encode_char(szStr[len], NULL);

		mBuffer.resize(newSize + 1);

		uint32 count = 0;
		uint32 i = 0;
		while((count = utf8proc_encode_char(*szStr, &mBuffer[i])) > 0)
		{
			if(mBuffer[i] == 0)
				break;

			i += count;
			szStr += 1;
		}
			
		mNumChars = UTF8StrLen(GetUTF8());
		return *this;
	}

	const UTF8String & UTF8String::operator+=(const UTF8String& rhs)
	{
		if(this == &rhs)
		{
			UTF8String copy(rhs);
			return (*this += copy);
		}

		uint32 myLen = Size();
		uint32 rhLen = rhs.Size();

		myLen -= (myLen > 0) ? 1 : 0;
		rhLen -= (rhLen > 0) ? 1 : 0;

		uint32 newLength = myLen + rhLen + 1;
		if(newLength >= mBuffer.size())
		{
			mBuffer.reserve(newLength);
		}

		memcpy(&mBuffer[myLen], &rhs.mBuffer[0], rhLen + 1);

		mNumChars = UTF8StrLen(GetUTF8());
		return *this;
	}

	uint8& UTF8String::operator[](int k)
	{
		return mBuffer[k];
	}

	uint8 UTF8String::operator[](int k) const
	{
		return mBuffer[k];
	}

	bool operator==(const UTF8String & lhs, const UTF8String & rhs)
	{
		uint32 lhLen = lhs.Size();
		uint32 rhLen = rhs.Size();

		return memcmp(lhs.GetUTF8(), rhs.GetUTF8(),  (lhLen < rhLen) ? lhLen : rhLen) == 0;
	}

	bool operator!=(const UTF8String & lhs, const UTF8String & rhs)
	{
		return !(lhs == rhs);
	}

	bool operator<(const UTF8String & lhs, const UTF8String & rhs)
	{
		uint32 lhLen = lhs.Size();
		uint32 rhLen = rhs.Size();

		return memcmp(lhs.GetUTF8(), rhs.GetUTF8(),  (lhLen < rhLen) ? lhLen : rhLen) < 0;
	}

	bool operator<=(const UTF8String & lhs, const UTF8String & rhs)
	{
		uint32 lhLen = lhs.Size();
		uint32 rhLen = rhs.Size();

		return memcmp(lhs.GetUTF8(), rhs.GetUTF8(),  (lhLen < rhLen) ? lhLen : rhLen) <= 0;
	}

	bool operator>(const UTF8String & lhs, const UTF8String & rhs)
	{
		return !(lhs <= rhs);
	}

	bool operator>=(const UTF8String & lhs, const UTF8String & rhs)
	{
		return !(lhs < rhs);
	}
}