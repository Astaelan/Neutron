#include <stddef.h>
#include <stdint.h>
#include <malloc.h>
#include <stdio.h>

extern "C" {
#pragma pack(push)
#pragma pack(1)
    struct SystemObject
    {
		void* mRuntimeTypeHandle;
	};

	struct SystemString : public SystemObject
	{
		int32_t mArrayLength;
		int32_t mStringLength;
		int16_t mFirstChar;
	};

	struct SystemType : public SystemObject
	{
	};

	struct SystemRuntimeTypeData
	{
		int32_t Handle;
		int32_t Flags;
		int32_t Size;
		int32_t Name;
		int32_t Namespace;
		int32_t VTable;
	};

	struct SystemRuntimeType : public SystemType
	{
		void* mHandle;
		SystemRuntimeTypeData* mData;
	};
#pragma pack(pop)

	extern int32_t RuntimeTypeHandle_SystemString;

	extern SystemRuntimeTypeData RuntimeTypeDataTable[];
	extern int32_t RuntimeTypeDataTableCount;

	extern int8_t RuntimeTypeDataStringTable[];

	extern void* RuntimeTypeDataVirtualTable[];
	extern int32_t RuntimeTypeDataVirtualTableCount;

	void __cdecl GCAllocate(void** _this, size_t size, int32_t handle);
	void* __cdecl VTableLookup(void* _this, int32_t index);

	void* __cdecl F_DCA596035033EDA2(SystemObject* pObject);    // System.RuntimeTypeHandle System.Object.InternalGetRuntimeTypeHandle(System.Object)

	SystemRuntimeTypeData* __cdecl F_45916642A61E51DD(void* pHandle);    // System.RuntimeTypeData* System.RuntimeType.InternalGetRuntimeTypeData(System.RuntimeTypeHandle)
	int8_t* __cdecl F_7E32112E1C16B000(int32_t pOffset);    // System.SByte* System.RuntimeType.InternalGetRuntimeTypeDataString(System.Int32)

	void __cdecl F_55D4A0F5FE5A88D5(SystemString** pString, int32_t pLength);    // System.Void System.String.InternalAllocate(System.String, System.Int32)
	void __cdecl F_204310C5649413C5(SystemString** _this, int16_t* pValue);    // System.Void System.String..ctor(System.Char*)
	void __cdecl F_164E479A86BE82E3(SystemString** _this, int16_t pChar, int32_t pCount);    // System.Void System.String..ctor(System.Char, System.Int32)
	void __cdecl F_9E4B5118337A8FDD(SystemString** _this, int8_t* pValue);    // System.Void System.String..ctor(System.SByte*)

	void __cdecl F_88DF95E6873B512C(SystemString* str);    // System.Void Neutron.Test.Program.ConsoleWrite(System.String str)
};

void __cdecl GCAllocate(void** _this, size_t size, int32_t handle)
{
	*_this = calloc(size, 1);
	SystemObject* obj = (SystemObject*)*_this;
	obj->mRuntimeTypeHandle = (void*)handle;
	// TODO: Link obj to alive list
	printf("GCAllocate: %u bytes for %d\n", size, handle);
}

void* __cdecl VTableLookup(void* _this, int32_t index)
{
	int32_t handle = (int32_t)((SystemObject*)_this)->mRuntimeTypeHandle;
	int32_t vtable = RuntimeTypeDataTable[handle].VTable;
	int32_t offset = vtable + index;
	//printf("VTableLookup: offset = %d\n", offset);
	return RuntimeTypeDataVirtualTable[offset];
}

void* __cdecl F_DCA596035033EDA2(SystemObject* pObject)    // System.RuntimeTypeHandle System.Object.InternalGetRuntimeTypeHandle(System.Object)
{
	//printf("InternalGetRuntimeTypeHandle: handle = %d\n", (int32_t)pObject->mRuntimeTypeHandle);
	return pObject->mRuntimeTypeHandle;
}

SystemRuntimeTypeData* __cdecl F_45916642A61E51DD(void* pHandle)    // System.RuntimeTypeData* System.RuntimeType.InternalGetRuntimeTypeData(System.RuntimeTypeHandle)
{
	//printf("InternalGetRuntimeTypeData: handle = %d, vtable = %d\n", (int32_t)pHandle, RuntimeTypeDataTable[(int32_t)pHandle].VTable);
	return &RuntimeTypeDataTable[(int32_t)pHandle];
}

int8_t* __cdecl F_7E32112E1C16B000(int32_t pOffset)    // System.SByte* System.RuntimeType.InternalGetRuntimeTypeDataString(System.Int32)
{
	//printf("InternalGetRuntimeTypeDataString: offset = %d, value = %s\n", pOffset, &RuntimeTypeDataStringTable[pOffset]);
	return &RuntimeTypeDataStringTable[pOffset];
}

void __cdecl F_55D4A0F5FE5A88D5(SystemString** pString, int32_t pLength)    // System.String System.String.InternalAllocate(System.Int32)
{
	//printf("String.InternalAllocate: %u bytes\n", pLength);
	GCAllocate((void**)pString, sizeof(SystemString) + (pLength << 1), RuntimeTypeHandle_SystemString);
	(*pString)->mArrayLength = pLength + 1;
	(*pString)->mStringLength = pLength;
}

void __cdecl F_204310C5649413C5(SystemString** _this, int16_t* pValue)    // System.Void System.String..ctor(System.Char*)
{
	int32_t stringLength = 0;
	while (pValue[stringLength]) ++stringLength;
	GCAllocate((void**)_this, sizeof(SystemString) + (stringLength << 1), RuntimeTypeHandle_SystemString);
	(*_this)->mRuntimeTypeHandle = NULL;
	(*_this)->mArrayLength = stringLength + 1;
	(*_this)->mStringLength = stringLength;
	int16_t* chars = &(*_this)->mFirstChar;
	for (int32_t index = 0; index < stringLength; ++index)
		chars[index] = pValue[index];
}

void __cdecl F_164E479A86BE82E3(SystemString** _this, int16_t pChar, int32_t pCount)    // System.Void System.String..ctor(System.Char, System.Int32)
{
	GCAllocate((void**)_this, sizeof(SystemString) + (pCount << 1), RuntimeTypeHandle_SystemString);
	(*_this)->mRuntimeTypeHandle = NULL;
	(*_this)->mArrayLength = pCount + 1;
	(*_this)->mStringLength = pCount;
	int16_t* chars = &(*_this)->mFirstChar;
	for (int32_t index = 0; index < pCount; ++index)
		chars[index] = pChar;
}

void __cdecl F_9E4B5118337A8FDD(SystemString** _this, int8_t* pValue)    // System.Void System.String..ctor(System.SByte*)
{
	int32_t stringLength = 0;
	while (pValue[stringLength]) ++stringLength;
	GCAllocate((void**)_this, sizeof(SystemString) + (stringLength << 1), RuntimeTypeHandle_SystemString);
	(*_this)->mRuntimeTypeHandle = NULL;
	(*_this)->mArrayLength = stringLength + 1;
	(*_this)->mStringLength = stringLength;
	int16_t* chars = &(*_this)->mFirstChar;
	for (int32_t index = 0; index < stringLength; ++index)
		chars[index] = pValue[index];
}


void __cdecl F_88DF95E6873B512C(SystemString* str)    // System.Void Neutron.Test.Program.ConsoleWrite(System.String str)
{
	wprintf((wchar_t*)&str->mFirstChar);
}