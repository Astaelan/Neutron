#include <stddef.h>
#include <stdint.h>
#include <malloc.h>
#include <stdio.h>

extern "C" {
	void __cdecl GCAllocate(void** _this, size_t size);

	void __cdecl F_204310C5649413C5(void** _this, int16_t* value);    // System.Void System.String..ctor(System.Char*)

	struct SystemArray
	{
		void* Reserved;
		int32_t ArrayLength;
		int32_t StringLength;
		int16_t FirstChar;
	};
};

void __cdecl GCAllocate(void** _this, size_t size)
{
	*_this = malloc(size);
	printf("GCAllocate: %u bytes\n", size);
}

void __cdecl F_204310C5649413C5(void** _this, int16_t* value)    // System.Void System.String..ctor(System.Char*)
{
	int32_t stringLength = 0;
	while (value[stringLength]) ++stringLength;
	int32_t size = sizeof(SystemArray) + (stringLength << 1);
	GCAllocate(_this, size);
	SystemArray* object = (SystemArray*)(*_this);
	object->Reserved = NULL;
	object->ArrayLength = stringLength + 1;
	object->StringLength = stringLength;
	int16_t* chars = &object->FirstChar;
	for (int32_t index = 0; index <= stringLength; ++index)
		chars[index] = value[index];
}
