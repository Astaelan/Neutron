#include <stddef.h>
#include <stdint.h>
#include <malloc.h>
#include <stdio.h>

extern "C" {
	struct SystemString
	{
		void* Reserved;
		int32_t ArrayLength;
		int32_t StringLength;
		int16_t FirstChar;
	};

	void __cdecl GCAllocate(void** _this, size_t size);

	void __cdecl F_204310C5649413C5(SystemString** _this, int16_t* value);    // System.Void System.String..ctor(System.Char*)
	void __cdecl F_164E479A86BE82E3(SystemString** _this, int16_t c, int32_t count);    // System.Void System.String..ctor(System.Char, System.Int32)
	int32_t __cdecl F_5DB28D7B95C5BF0F(SystemString* _this);    // System.Int32 System.String.Length.get()
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
	(*_this)->Reserved = NULL;
	(*_this)->ArrayLength = stringLength + 1;
	(*_this)->StringLength = stringLength;
	int16_t* chars = &(*_this)->FirstChar;
	for (int32_t index = 0; index < stringLength; ++index)
		chars[index] = value[index];
	chars[stringLength] = 0;
}

void __cdecl F_164E479A86BE82E3(SystemString** _this, int16_t c, int32_t count)    // System.Void System.String..ctor(System.Char, System.Int32)
{
	GCAllocate((void**)_this, sizeof(SystemString) + (count << 1));
	(*_this)->Reserved = NULL;
	(*_this)->ArrayLength = count + 1;
	(*_this)->StringLength = count;
	int16_t* chars = &(*_this)->FirstChar;
	for (int32_t index = 0; index < count; ++index)
		chars[index] = c;
	chars[count] = 0;
}

int32_t __cdecl F_5DB28D7B95C5BF0F(SystemString* _this)    // System.Int32 System.String.Length.get()
{
	return _this->StringLength;
}
