#include <stddef.h>
#include <stdint.h>
#include <malloc.h>
#include <stdio.h>

extern "C" {
#pragma pack(push)
#pragma pack(1)
    struct SystemObject
    {
		void* mReserved;
	};

	struct SystemString : public SystemObject
	{
		int32_t mArrayLength;
		int32_t mStringLength;
		int16_t mFirstChar;
	};
#pragma pack(pop)

	void __cdecl GCAllocate(void** _this, size_t size);

	void __cdecl F_204310C5649413C5(SystemString** _this, int16_t* value);    // System.Void System.String..ctor(System.Char*)
	void __cdecl F_164E479A86BE82E3(SystemString** _this, int16_t c, int32_t count);    // System.Void System.String..ctor(System.Char, System.Int32)
	int32_t __cdecl F_5DB28D7B95C5BF0F(SystemString* _this);    // System.Int32 System.String.Length.get()

	void __cdecl F_88DF95E6873B512C(SystemString* str);    // System.Void Neutron.Test.Program.ConsoleWrite(System.String str)
};

void __cdecl GCAllocate(void** _this, size_t size)
{
	*_this = malloc(size);
	printf("GCAllocate: %u bytes\n", size);
}

void __cdecl F_204310C5649413C5(SystemString** _this, int16_t* value)    // System.Void System.String..ctor(System.Char*)
{
	int32_t stringLength = 0;
	while (value[stringLength]) ++stringLength;
	GCAllocate((void**)_this, sizeof(SystemString) + (stringLength << 1));
	(*_this)->mReserved = NULL;
	(*_this)->mArrayLength = stringLength + 1;
	(*_this)->mStringLength = stringLength;
	int16_t* chars = &(*_this)->mFirstChar;
	for (int32_t index = 0; index < stringLength; ++index)
		chars[index] = value[index];
	chars[stringLength] = 0;
}

void __cdecl F_164E479A86BE82E3(SystemString** _this, int16_t c, int32_t count)    // System.Void System.String..ctor(System.Char, System.Int32)
{
	GCAllocate((void**)_this, sizeof(SystemString) + (count << 1));
	(*_this)->mReserved = NULL;
	(*_this)->mArrayLength = count + 1;
	(*_this)->mStringLength = count;
	int16_t* chars = &(*_this)->mFirstChar;
	for (int32_t index = 0; index < count; ++index)
		chars[index] = c;
	chars[count] = 0;
}

int32_t __cdecl F_5DB28D7B95C5BF0F(SystemString* _this)    // System.Int32 System.String.Length.get()
{
	return _this->mStringLength;
}



void __cdecl F_88DF95E6873B512C(SystemString* str)    // System.Void Neutron.Test.Program.ConsoleWrite(System.String str)
{
	wprintf((wchar_t*)&str->mFirstChar);
}