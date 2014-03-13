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

	struct SystemDelegate : public SystemObject
	{
		SystemObject* mTargetObj;
		void* mTargetMethod;
		SystemDelegate* mNext;
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

	struct SystemRuntimeMethodData
	{
		int32_t Handle;
		int32_t Flags;
		int32_t Name;
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

	extern int32_t RuntimeTypeDataVirtualTable[];
	extern int32_t RuntimeTypeDataVirtualTableCount;

	extern SystemRuntimeMethodData RuntimeMethodDataTable[];
	extern int32_t RuntimeMethodDataTableCount;

	extern int8_t RuntimeMethodDataStringTable[];

	extern void* RuntimeMethodDataFunctionTable[];
	extern int32_t RuntimeMethodDataFunctionTableCount;

	void __cdecl GCAllocate(void** _this, size_t size, int32_t handle);
	int32_t __cdecl VTableHandleLookup(void* _this, int32_t index);
	void* __cdecl VTableFunctionLookup(void* _this, int32_t index);
	void* __cdecl DelegateLookup(SystemDelegate* _this);
	void* __cdecl DelegateInstance(SystemDelegate* _this);

	void* __cdecl F_699DAC97BED1D622(void* _this, SystemObject* pObject);    // System.RuntimeTypeHandle System.Object.InternalGetRuntimeTypeHandle(System.Object)

	SystemRuntimeTypeData* __cdecl F_08D4265147985EF9(void* _this, void* pHandle);    // System.RuntimeTypeData* System.RuntimeType.InternalGetRuntimeTypeData(System.RuntimeTypeHandle)
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
	obj->mRuntimeTypeHandle = (void*)handle;
	// TODO: Link obj to alive list
	printf("GCAllocate: %u bytes for %d\n", size, handle);
}

int32_t __cdecl VTableHandleLookup(void* _this, int32_t index)
{
	int32_t handle = (int32_t)((SystemObject*)_this)->mRuntimeTypeHandle;
	int32_t vtable = RuntimeTypeDataTable[handle].VTable;
	int32_t offset = vtable + index;
	int32_t mhandle = RuntimeTypeDataVirtualTable[offset];
	//printf("VTableHandleLookup: handle = %d\n", mhandle);
	return mhandle;
}

void* __cdecl VTableFunctionLookup(void* _this, int32_t index)
{
	int32_t mhandle = VTableHandleLookup(_this, index);
	//printf("VTableFunctionLookup: handle = %d\n", mhandle);
	return RuntimeMethodDataFunctionTable[mhandle];
}

void* __cdecl DelegateLookup(SystemDelegate* _this)
{
	return RuntimeMethodDataFunctionTable[(int32_t)_this->mTargetMethod];
}

void* __cdecl DelegateInstance(SystemDelegate* _this)
{
	void* result = (void*)_this->mTargetObj;
	uint8_t boxed = (RuntimeTypeDataTable[(int32_t)_this->mTargetObj->mRuntimeTypeHandle].Flags & (1 << 0)) != 0;
	if (boxed) result = (void*)(((uint8_t*)result) + sizeof(SystemObject));
	return result;
}

void* __cdecl F_699DAC97BED1D622(void* _this, SystemObject* pObject)    // System.RuntimeTypeHandle System.Object.InternalGetRuntimeTypeHandle(System.Object)
{
	//printf("InternalGetRuntimeTypeHandle: %d\n", (int32_t)pObject);
	//printf("InternalGetRuntimeTypeHandle: handle = %d\n", (int32_t)pObject->mRuntimeTypeHandle);
	return pObject->mRuntimeTypeHandle;
}

SystemRuntimeTypeData* __cdecl F_08D4265147985EF9(void* _this, void* pHandle)    // System.RuntimeTypeData* System.RuntimeType.InternalGetRuntimeTypeData(System.RuntimeTypeHandle)
{
	//printf("InternalGetRuntimeTypeData: handle = %d, vtable = %d\n", (int32_t)pHandle, RuntimeTypeDataTable[(int32_t)pHandle].VTable);
	return &RuntimeTypeDataTable[(int32_t)pHandle];
}

int8_t* __cdecl F_38731037EF6816B8(void* _this, int32_t pOffset)    // System.SByte* System.RuntimeType.InternalGetRuntimeTypeDataString(System.Int32)
{
	//printf("InternalGetRuntimeTypeDataString: offset = %d, value = %s\n", pOffset, &RuntimeTypeDataStringTable[pOffset]);
	return &RuntimeTypeDataStringTable[pOffset];
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


void __cdecl F_88B29A12102B9B7B(void* _this, SystemString* str)    // System.Void Neutron.Test.Program.ConsoleWrite(System.String str)
{
	wprintf((wchar_t*)&str->mFirstChar);
}