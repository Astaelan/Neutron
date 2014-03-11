using Microsoft.Cci;
using Neutron.LLIR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR
{
    public sealed class HLType
    {
        private static int sRuntimeTypeHandle = 0;

        internal HLType()
        {
            mRuntimeTypeHandle = sRuntimeTypeHandle++;
        }

        private int mRuntimeTypeHandle = 0;
        public int RuntimeTypeHandle { get { return mRuntimeTypeHandle; } }

        private ITypeDefinition mDefinition = null;
        public ITypeDefinition Definition { get { return mDefinition; } internal set { mDefinition = value; } }

        private string mName = null;
        public string Name { get { return mName; } internal set { mName = value; } }

        private string mSignature = null;
        public string Signature { get { return mSignature; } internal set { mSignature = value; } }

        private HLType mBaseType = null;
        public HLType BaseType { get { return mBaseType; } internal set { mBaseType = value; } }

        private List<HLField> mFields = new List<HLField>();
        public List<HLField> Fields { get { return mFields; } }

        public List<HLField> StaticFields { get { return mFields.FindAll(f => f.IsStatic); } }

        public List<HLField> MemberFields { get { return mFields.FindAll(f => !f.IsStatic); } }

        private List<HLMethod> mMethods = new List<HLMethod>();
        public List<HLMethod> Methods { get { return mMethods; } }

        private HLMethod mStaticConstructor = null;
        public HLMethod StaticConstructor { get { return mStaticConstructor; } internal set { mStaticConstructor = value; } }

        private Dictionary<HLMethod, HLMethod> mVirtualMap = new Dictionary<HLMethod, HLMethod>();
        public Dictionary<HLMethod, HLMethod> VirtualMap { get { return mVirtualMap; } }

        private bool mVirtualTableBuilt = false;
        public bool VirtualTableBuilt { get { return mVirtualTableBuilt; } }

        private List<HLMethod> mVirtualTable = new List<HLMethod>();
        public List<HLMethod> VirtualTable { get { return mVirtualTable; } }

        private Dictionary<HLMethod, int> mVirtualLookup = new Dictionary<HLMethod, int>();
        public Dictionary<HLMethod, int> VirtualLookup { get { return mVirtualLookup; } }

        public int CalculatedSize
        {
            get
            {
                switch (Definition.TypeCode)
                {
                    case PrimitiveTypeCode.Void: return 0;
                    case PrimitiveTypeCode.Boolean:
                    case PrimitiveTypeCode.Int8:
                    case PrimitiveTypeCode.UInt8: return 1;
                    case PrimitiveTypeCode.Char:
                    case PrimitiveTypeCode.Int16:
                    case PrimitiveTypeCode.UInt16: return 2;
                    case PrimitiveTypeCode.Float32:
                    case PrimitiveTypeCode.Int32:
                    case PrimitiveTypeCode.UInt32: return 4;
                    case PrimitiveTypeCode.Float64:
                    case PrimitiveTypeCode.Int64:
                    case PrimitiveTypeCode.UInt64: return 8;
                    case PrimitiveTypeCode.Pointer:
                    case PrimitiveTypeCode.Reference:
                    case PrimitiveTypeCode.IntPtr:
                    case PrimitiveTypeCode.UIntPtr: return HLDomain.SizeOfPointer;
                    case PrimitiveTypeCode.String:
                    case PrimitiveTypeCode.NotPrimitive:
                        {
                            int calculatedSize = 0;
                            if (Definition.IsReferenceType) calculatedSize += HLDomain.SizeOfPointer;
                            return calculatedSize + MemberFields.Sum(f => f.Type.VariableSize);
                        }
                    default: throw new NotSupportedException();
                }
            }
        }

        public int VariableSize { get { return Definition.IsReferenceType ? HLDomain.SizeOfPointer : CalculatedSize; } }

        internal void LayoutFields()
        {
            int offset = 0;
            if (Definition.IsReferenceType) offset += HLDomain.SizeOfPointer;
            foreach (HLField field in MemberFields)
            {
                field.Offset = offset;
                offset += field.Type.VariableSize;
            }
        }

        internal void MapVirtualMethods()
        {
            if (BaseType != null)
            {
                BaseType.MapVirtualMethods();
                foreach (KeyValuePair<HLMethod, HLMethod> kv in BaseType.VirtualMap)
                    VirtualMap[kv.Key] = kv.Value;
            }
            foreach (HLMethod method in VirtualMap.Keys.ToList())
            {
                IMethodDefinition definition = TypeHelper.GetMethod(Definition, method.Definition, true);
                if (definition != Dummy.MethodDefinition)
                    VirtualMap[method] = HLDomain.GetOrCreateMethod(definition);
            }
            List<HLMethod> methodsVirtuals = Methods.FindAll(m => m.IsVirtual);
            foreach (HLMethod method in methodsVirtuals)
            {
                IMethodDefinition definition = TypeHelper.GetMethod(Definition, method.Definition, true);
                if (definition == Dummy.MethodDefinition) VirtualMap[method] = method;
                else VirtualMap[method] = HLDomain.GetOrCreateMethod(definition);
            }
        }

        internal void BuildVirtualTable()
        {
            if (VirtualTableBuilt) return;
            mVirtualTableBuilt = true;
            if (BaseType != null)
            {
                BaseType.BuildVirtualTable();
                VirtualTable.AddRange(BaseType.VirtualTable);
                foreach (KeyValuePair<HLMethod, int> kv in BaseType.VirtualLookup)
                    VirtualLookup.Add(kv.Key, kv.Value);
                foreach (KeyValuePair<HLMethod, HLMethod> kv in VirtualMap.Where(p => p.Key.Container != this))
                {
                    int index = VirtualLookup[kv.Key];
                    VirtualTable[index] = kv.Value;
                }
            }
            foreach (KeyValuePair<HLMethod, HLMethod> kv in VirtualMap.Where(p => p.Key.Container == this))
            {
                int index = VirtualTable.Count;
                VirtualTable.Add(kv.Value);
                VirtualLookup.Add(kv.Key, index);
            }
        }


        public LLType LLType
        {
            get
            {
                switch (Definition.TypeCode)
                {
                    case PrimitiveTypeCode.Void: return LLModule.VoidType;
                    case PrimitiveTypeCode.Boolean: return LLModule.BooleanType;
                    case PrimitiveTypeCode.Int8: return LLModule.GetOrCreateSignedType(8);
                    case PrimitiveTypeCode.UInt8: return LLModule.GetOrCreateUnsignedType(8);
                    case PrimitiveTypeCode.Char: return LLModule.GetOrCreateSignedType(16);
                    case PrimitiveTypeCode.Int16: return LLModule.GetOrCreateSignedType(16);
                    case PrimitiveTypeCode.UInt16: return LLModule.GetOrCreateUnsignedType(16);
                    case PrimitiveTypeCode.Float32: return LLModule.Float32Type;
                    case PrimitiveTypeCode.Int32: return LLModule.GetOrCreateSignedType(32);
                    case PrimitiveTypeCode.UInt32: return LLModule.GetOrCreateUnsignedType(32);
                    case PrimitiveTypeCode.Float64: return LLModule.Float64Type;
                    case PrimitiveTypeCode.Int64: return LLModule.GetOrCreateSignedType(64);
                    case PrimitiveTypeCode.UInt64: return LLModule.GetOrCreateUnsignedType(64);
                    case PrimitiveTypeCode.Pointer:
                        {
                            IPointerType definitionPointer = Definition as IPointerType;
                            HLType typePointerTarget = HLDomain.GetOrCreateType(definitionPointer.TargetType);
                            if (typePointerTarget == HLDomain.SystemVoid) typePointerTarget = HLDomain.SystemByte;
                            return typePointerTarget.LLType.PointerDepthPlusOne;
                        }
                    case PrimitiveTypeCode.Reference:
                        {
                            IManagedPointerType definitionReference = Definition as IManagedPointerType;
                            HLType typeReferenceTarget = HLDomain.GetOrCreateType(definitionReference.TargetType);
                            return typeReferenceTarget.LLType.PointerDepthPlusOne;
                        }
                    case PrimitiveTypeCode.IntPtr: return LLModule.GetOrCreatePointerType(LLModule.GetOrCreateUnsignedType(8), 1);
                    case PrimitiveTypeCode.UIntPtr: return LLModule.GetOrCreatePointerType(LLModule.GetOrCreateUnsignedType(8), 1);
                    case PrimitiveTypeCode.String: return LLModule.GetOrCreatePointerType(LLModule.GetOrCreateUnsignedType(8), 1);
                    case PrimitiveTypeCode.NotPrimitive:
                        {
                            if (Definition is IArrayType)
                            {
                                IArrayType definitionArray = Definition as IArrayType;
                                HLType typeElement = HLDomain.GetOrCreateType(definitionArray.ElementType);
                                return LLModule.GetOrCreatePointerType(LLModule.GetOrCreateUnsignedType(8), 1);
                            }
                            if (Definition.IsReferenceType || Definition.IsClass) return LLModule.GetOrCreatePointerType(LLModule.GetOrCreateUnsignedType(8), 1);
                            if (Definition.IsValueType)
                            {
                                switch (CalculatedSize)
                                {
                                    case 1: return LLModule.GetOrCreateUnsignedType(8);
                                    case 2: return LLModule.GetOrCreateUnsignedType(16);
                                    case 4: return LLModule.GetOrCreateUnsignedType(32);
                                    case 8: return LLModule.GetOrCreateUnsignedType(64);
                                    default: return LLModule.GetOrCreateArrayType(LLModule.GetOrCreateUnsignedType(8), CalculatedSize);
                                }
                            }
                            throw new NotSupportedException();
                        }
                    default: throw new NotSupportedException();
                }
            }
        }

        public override string ToString() { return Name; }
    }
}
