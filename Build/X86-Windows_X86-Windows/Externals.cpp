#include <stddef.h>
#include <stdint.h>
#include <malloc.h>
#include <stdio.h>

extern "C" {
#pragma pack(push)
#pragma pack(1)
    struct SystemRuntimeTypeHandle
    {
		int32_t mValue;
	};

	struct SystemRuntimeMethodHandle
	{
		int32_t mValue;
	};

	struct SystemRuntimeFieldHandle
	{
		int32_t mValue;
	};

    struct SystemObject
    {
		SystemRuntimeTypeHandle mRuntimeTypeHandle;
	};

	struct SystemDelegate : public SystemObject
	{
		SystemObject* mTargetObj;
		SystemRuntimeMethodHandle mTargetMethod;
		SystemDelegate* mNext;
	};

	struct SystemArray : public SystemObject
	{
		int32_t mLength;
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
		int32_t NameOffset;
		int32_t NamespaceOffset;
		int32_t VTableOffset;
		int32_t EnumOffset;
		int32_t EnumCount;
	};

	struct SystemRuntimeMethodData
	{
		int32_t Handle;
		int32_t Flags;
		int32_t Name;
	};

	struct SystemRuntimeFieldData
	{
		int32_t Handle;
		int32_t Flags;
		int32_t Name;
		int32_t Type;
		int32_t Offset;
	};

	struct SystemRuntimeType : public SystemType
	{
		SystemRuntimeTypeHandle mHandle;
		SystemRuntimeTypeData* mData;
	};
#pragma pack(pop)

	extern int32_t RuntimeTypeHandle_SystemString;

	extern SystemRuntimeTypeData RuntimeTypeDataTable[];
	extern int32_t RuntimeTypeDataTableCount;

	extern int32_t RuntimeTypeDataVirtualTable[];
	extern int32_t RuntimeTypeDataVirtualTableCount;

	extern uint8_t RuntimeTypeDataEnum8ValueTable[];
	extern int32_t RuntimeTypeDataEnum8NameTable[];

	extern uint16_t RuntimeTypeDataEnum16ValueTable[];
	extern int32_t RuntimeTypeDataEnum16NameTable[];

	extern uint32_t RuntimeTypeDataEnum32ValueTable[];
	extern int32_t RuntimeTypeDataEnum32NameTable[];

	extern uint64_t RuntimeTypeDataEnum64ValueTable[];
	extern int32_t RuntimeTypeDataEnum64NameTable[];

	extern SystemRuntimeMethodData RuntimeMethodDataTable[];
	extern int32_t RuntimeMethodDataTableCount;

	extern void* RuntimeMethodDataFunctionTable[];
	extern int32_t RuntimeMethodDataFunctionTableCount;

	extern SystemRuntimeFieldData RuntimeFieldDataTable[];
	extern int32_t RuntimeFieldDataTableCount;

	extern int8_t RuntimeDataStringTable[];

	void __cdecl GCAllocate(void** _this, size_t size, int32_t handle);
	int32_t __cdecl VTableHandleLookup(SystemObject* _this, int32_t index);
	void* __cdecl VTableFunctionLookup(SystemObject* _this, int32_t index);
	void* __cdecl DelegateLookup(SystemDelegate* _this);
	void* __cdecl DelegateInstance(SystemDelegate* _this);

	//void __cdecl F_3D504A6BCA77A8BB(void* _this, SystemRuntimeTypeHandle pHandle, int32_t pIndex, uint64_t* pValue, SystemString** pName);    // System.Void System.Enum.InternalGetEnumData(System.RuntimeTypeHandle, System.Int32, System.UInt64, System.String)
	int32_t __cdecl F_A7EEA569618A38D1(void* _this, SystemRuntimeTypeHandle pHandle, uint64_t pValue);    // System.Int32 System.Enum.InternalGetEnumIndex(System.RuntimeTypeHandle, System.UInt64)
	int32_t __cdecl F_3E29D22882E08664(void* _this, SystemRuntimeTypeHandle pHandle, int32_t pIndex);    // System.Int32 System.Enum.InternalGetEnumName(System.RuntimeTypeHandle, System.Int32)
	uint64_t __cdecl F_0D59668FE083EE48(void* _this, SystemObject* pObject);    // System.UInt64 System.Enum.InternalGetEnumValue(System.Object)

	SystemRuntimeTypeHandle __cdecl F_699DAC97BED1D622(void* _this, SystemObject* pObject);    // System.RuntimeTypeHandle System.Object.InternalGetRuntimeTypeHandle(System.Object)

	SystemRuntimeTypeData* __cdecl F_488236944CAE6668(void* _this, SystemRuntimeTypeHandle pHandle);    // System.RuntimeTypeData* System.RuntimeType.InternalGetRuntimeTypeData(System.RuntimeTypeHandle)
	int8_t* __cdecl F_38731037EF6816B8(void* _this, int32_t pOffset);    // System.SByte* System.RuntimeType.InternalGetRuntimeTypeDataString(System.Int32)

	void __cdecl F_3D1FB7A7FAEF7651(void* _this, SystemString** pString, int32_t pLength);    // System.Void System.String.InternalAllocate(System.String, System.Int32)
	void __cdecl F_204310C5649413C5(SystemString** _this, int16_t* pValue);    // System.Void System.String..ctor(System.Char*)
	void __cdecl F_164E479A86BE82E3(SystemString** _this, int16_t pChar, int32_t pCount);    // System.Void System.String..ctor(System.Char, System.Int32)
	void __cdecl F_9E4B5118337A8FDD(SystemString** _this, int8_t* pValue);    // System.Void System.String..ctor(System.SByte*)

	void __cdecl F_88B29A12102B9B7B(void* _this, SystemString* str);    // System.Void Neutron.Test.Program.ConsoleWrite(System.String str)
};

void __cdecl GCAllocate(void** _this, size_t size, int32_t handle)
{
	*_this = calloc(size, 1);
	SystemObject* obj = (SystemObject*)*_this;
	obj->mRuntimeTypeHandle.mValue = handle;
	// TODO: Link obj to alive list
	printf("GCAllocate: %u bytes for %d\n", size, handle);
}

int32_t __cdecl VTableHandleLookup(SystemObject* _this, int32_t index)
{
	int32_t handle = _this->mRuntimeTypeHandle.mValue;
	int32_t vtable = RuntimeTypeDataTable[handle].VTableOffset;
	int32_t offset = vtable + index;
	int32_t mhandle = RuntimeTypeDataVirtualTable[offset];
	//printf("VTableHandleLookup: handle = %d, vtable = %d, offset = %d, mhandle = %d\n", handle, vtable, offset, mhandle);
	return mhandle;
}

void* __cdecl VTableFunctionLookup(SystemObject* _this, int32_t index)
{
	int32_t mhandle = VTableHandleLookup(_this, index);
	//printf("VTableFunctionLookup: handle = %d\n", mhandle);
	return RuntimeMethodDataFunctionTable[mhandle];
}

void* __cdecl DelegateLookup(SystemDelegate* _this)
{
	return RuntimeMethodDataFunctionTable[_this->mTargetMethod.mValue];
}

void* __cdecl DelegateInstance(SystemDelegate* _this)
{
	uint8_t* result = (uint8_t*)_this->mTargetObj;
	uint8_t boxed = (RuntimeTypeDataTable[_this->mTargetObj->mRuntimeTypeHandle.mValue].Flags & (1 << 0)) != 0;
	if (boxed) result += sizeof(SystemObject);
	return (void*)result;
}

/*
void __cdecl F_3D504A6BCA77A8BB(void* _this, SystemRuntimeTypeHandle pHandle, int32_t pIndex, uint64_t* pValue, int32_t* pNameOffset)    // System.Void System.Enum.InternalGetEnumData(System.RuntimeTypeHandle, System.Int32, System.UInt64, System.Int32)
{
	SystemRuntimeTypeData* dataType = &RuntimeTypeDataTable[pHandle.mValue];
	int offset = dataType->EnumOffset + pIndex;
	printf("InternalGetEnumData: handle = %d, index = %d, size = %d, count = %d\n", pHandle.mValue, pIndex, dataType->Size, dataType->EnumCount);
	*pValue = 0;
	*pNameOffset = 0;
	switch (dataType->Size)
	{
		case 1: *pValue = RuntimeTypeDataEnum8ValueTable[offset]; *pNameOffset = RuntimeTypeDataEnum8NameTable[offset]; break;
		case 2: *pValue = RuntimeTypeDataEnum16ValueTable[offset]; *pNameOffset = RuntimeTypeDataEnum16NameTable[offset]; break;
		case 4: *pValue = RuntimeTypeDataEnum32ValueTable[offset]; *pNameOffset = RuntimeTypeDataEnum32NameTable[offset]; break;
		case 8: *pValue = RuntimeTypeDataEnum64ValueTable[offset]; *pNameOffset = RuntimeTypeDataEnum64NameTable[offset]; break;
		default: break;
	}
}
*/

int32_t __cdecl F_A7EEA569618A38D1(void* _this, SystemRuntimeTypeHandle pHandle, uint64_t pValue)    // System.Int32 System.Enum.InternalGetEnumIndex(System.RuntimeTypeHandle, System.UInt64)
{
	SystemRuntimeTypeData* dataType = &RuntimeTypeDataTable[pHandle.mValue];
	//printf("InternalGetEnumIndex: handle = %d, value = %llu\n", pHandle.mValue, pValue);
	for (int index = 0; index < dataType->EnumCount; ++index)
	{
		int offset = dataType->EnumOffset + index;
		switch (dataType->Size)
		{
			case 1: if (RuntimeTypeDataEnum8ValueTable[offset] == pValue) return index; break;
			case 2: if (RuntimeTypeDataEnum16ValueTable[offset] == pValue) return index; break;
			case 4: if (RuntimeTypeDataEnum32ValueTable[offset] == pValue) return index; break;
			default: if (RuntimeTypeDataEnum64ValueTable[offset] == pValue) return index; break;
		}
	}
	return -1;
}

int32_t __cdecl F_3E29D22882E08664(void* _this, SystemRuntimeTypeHandle pHandle, int32_t pIndex)    // System.Int32 System.Enum.InternalGetEnumName(System.RuntimeTypeHandle, System.Int32)
{
	SystemRuntimeTypeData* dataType = &RuntimeTypeDataTable[pHandle.mValue];
	int offset = dataType->EnumOffset + pIndex;
	//printf("InternalGetEnumName: handle = %d, index = %d\n", pHandle.mValue, pIndex);
	switch (dataType->Size)
	{
		case 1: return RuntimeTypeDataEnum8NameTable[offset];
		case 2: return RuntimeTypeDataEnum16NameTable[offset];
		case 4: return RuntimeTypeDataEnum32NameTable[offset];
		case 8: return RuntimeTypeDataEnum64NameTable[offset];
		default: return 0;
	}
}

uint64_t __cdecl F_0D59668FE083EE48(void* _this, SystemObject* pObject)    // System.UInt64 System.Enum.InternalGetEnumValue(System.Object)
{
	SystemRuntimeTypeData* dataType = &RuntimeTypeDataTable[pObject->mRuntimeTypeHandle.mValue];
	uint8_t* data = ((uint8_t*)pObject) + sizeof(SystemObject);
	//printf("InternalGetEnumValue: handle = %d, size = %d\n", dataType->Handle, dataType->Size);
	switch (dataType->Size)
	{
		case 1: return *data;
		case 2: return *((uint16_t*)data);
		case 4: return *((uint32_t*)data);
		default: return *((uint64_t*)data);
	}
}

SystemRuntimeTypeHandle __cdecl F_699DAC97BED1D622(void* _this, SystemObject* pObject)    // System.RuntimeTypeHandle System.Object.InternalGetRuntimeTypeHandle(System.Object)
{
	//printf("InternalGetRuntimeTypeHandle: %d\n", (int32_t)pObject);
	//printf("InternalGetRuntimeTypeHandle: handle = %d\n", pObject->mRuntimeTypeHandle.mValue);
	return pObject->mRuntimeTypeHandle;
}

SystemRuntimeTypeData* __cdecl F_488236944CAE6668(void* _this, SystemRuntimeTypeHandle pHandle)    // System.RuntimeTypeData* System.RuntimeType.InternalGetRuntimeTypeData(System.RuntimeTypeHandle)
{
	//printf("InternalGetRuntimeTypeData: handle = %d, vtable = %d\n", pHandle.mValue, RuntimeTypeDataTable[pHandle.mValue].VTableOffset);
	return &RuntimeTypeDataTable[pHandle.mValue];
}

int8_t* __cdecl F_38731037EF6816B8(void* _this, int32_t pOffset)    // System.SByte* System.RuntimeType.InternalGetRuntimeTypeDataString(System.Int32)
{
	//printf("InternalGetRuntimeTypeDataString: offset = %d, value = %s\n", pOffset, &RuntimeTypeDataStringTable[pOffset]);
	return &RuntimeDataStringTable[pOffset];
}

void __cdecl F_3D1FB7A7FAEF7651(void* _this, SystemString** pString, int32_t pLength)    // System.String System.String.InternalAllocate(System.Int32)
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
	(*_this)->mArrayLength = stringLength + 1;
	(*_this)->mStringLength = stringLength;
	int16_t* chars = &(*_this)->mFirstChar;
	for (int32_t index = 0; index < stringLength; ++index)
		chars[index] = pValue[index];
}

void __cdecl F_164E479A86BE82E3(SystemString** _this, int16_t pChar, int32_t pCount)    // System.Void System.String..ctor(System.Char, System.Int32)
{
	GCAllocate((void**)_this, sizeof(SystemString) + (pCount << 1), RuntimeTypeHandle_SystemString);
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
	(*_this)->mArrayLength = stringLength + 1;
	(*_this)->mStringLength = stringLength;
	int16_t* chars = &(*_this)->mFirstChar;
	for (int32_t index = 0; index < stringLength; ++index)
		chars[index] = pValue[index];
}


void __cdecl F_88B29A12102B9B7B(void* _this, SystemString* str)    // System.Void Neutron.Test.Program.ConsoleWrite(System.String str)
{
	wprintf((wchar_t*)&str->mFirstChar);
}
