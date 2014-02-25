using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Neutron.LLIR
{
    public static class LLModule
    {
        private static Dictionary<string, LLType> sTypes = new Dictionary<string, LLType>();

        private static Dictionary<string, LLFunction> sFunctions = new Dictionary<string, LLFunction>();

        private static Dictionary<string, LLGlobal> sGlobals = new Dictionary<string, LLGlobal>();

        private static int sPointerSizeInBits = 32;
        public static int PointerSizeInBits { get { return sPointerSizeInBits; } set { sPointerSizeInBits = value; } }


        private static LLType CreatePrimitiveType(LLPrimitive pPrimitive, int pSizeInBits)
        {
            LLType type = new LLType();
            type.Primitive = pPrimitive;
            type.SizeInBits = pSizeInBits;
            sTypes[type.Signature] = type;
            return type;
        }


        private static Lazy<LLType> sVoidType = new Lazy<LLType>(() => CreatePrimitiveType(LLPrimitive.Void, 0));
        public static LLType VoidType { get { return sVoidType.Value; } }
        public static string GetVoidSignature() { return "(void)"; }


        public static string GetSignedSignature(int pSizeInBits) { return string.Format("(signed:{0})", pSizeInBits); }
        public static LLType GetOrCreateSignedType(int pSizeInBits)
        {
            LLType type = null;
            string signature = GetSignedSignature(pSizeInBits);
            if (!sTypes.TryGetValue(signature, out type)) type = CreatePrimitiveType(LLPrimitive.Signed, pSizeInBits);
            return type;
        }

        private static Lazy<LLType> sBooleanType = new Lazy<LLType>(() => CreatePrimitiveType(LLPrimitive.Unsigned, 1));
        public static LLType BooleanType { get { return sBooleanType.Value; } }

        public static string GetUnsignedSignature(int pSizeInBits) { return string.Format("(unsigned:{0})", pSizeInBits); }
        public static LLType GetOrCreateUnsignedType(int pSizeInBits)
        {
            if (pSizeInBits == 1) return BooleanType;
            LLType type = null;
            string signature = GetUnsignedSignature(pSizeInBits);
            if (!sTypes.TryGetValue(signature, out type)) type = CreatePrimitiveType(LLPrimitive.Unsigned, pSizeInBits);
            return type;
        }


        private static Lazy<LLType> sFloat32Type = new Lazy<LLType>(() => CreatePrimitiveType(LLPrimitive.Float, 32));
        public static LLType Float32Type { get { return sFloat32Type.Value; } }

        private static Lazy<LLType> sFloat64Type = new Lazy<LLType>(() => CreatePrimitiveType(LLPrimitive.Float, 64));
        public static LLType Float64Type { get { return sFloat64Type.Value; } }

        public static string GetFloatSignature(int pSizeInBits) { return string.Format("(float:{0})", pSizeInBits); }


        private static LLType CreatePointerType(LLType pPointerBase, int pPointerDepth)
        {
            LLType type = new LLType();
            type.Primitive = LLPrimitive.Pointer;
            type.SizeInBits = LLModule.PointerSizeInBits;
            type.PointerBase = pPointerBase;
            type.PointerDepth = pPointerDepth;
            sTypes[type.Signature] = type;
            return type;
        }
        public static string GetPointerSignature(LLType pPointerBase, int pPointerDepth) { return string.Format("(pointer:{0}:{1})", pPointerBase.Signature, pPointerDepth); }
        public static LLType GetOrCreatePointerType(LLType pPointerBase, int pPointerDepth)
        {
            LLType type = null;
            string signature = GetPointerSignature(pPointerBase, pPointerDepth);
            if (!sTypes.TryGetValue(signature, out type)) type = CreatePointerType(pPointerBase, pPointerDepth);
            return type;
        }


        private static LLType CreateFunctionType(LLType pReturnType, List<LLType> pParameterTypes)
        {
            LLType type = new LLType();
            type.Primitive = LLPrimitive.Function;
            type.SizeInBits = 0;
            type.FunctionReturn = pReturnType;
            type.FunctionParameters = new List<LLType>(pParameterTypes);
            sTypes[type.Signature] = type;
            return type;
        }
        public static string GetFunctionSignature(LLType pReturnType, List<LLType> pParameterTypes)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("(function:{0}", pReturnType.Signature);
            foreach (LLType type in pParameterTypes)
                sb.AppendFormat(":{0}", type.Signature);
            sb.Append(")");
            return sb.ToString();
        }
        public static LLType GetOrCreateFunctionType(LLType pReturnType, List<LLType> pParameterTypes)
        {
            LLType type = null;
            string signature = GetFunctionSignature(pReturnType, pParameterTypes);
            if (!sTypes.TryGetValue(signature, out type)) type = CreateFunctionType(pReturnType, pParameterTypes);
            return type;
        }


        private static LLType CreateVectorType(LLType pElementType, int pElementCount)
        {
            LLType type = new LLType();
            type.Primitive = LLPrimitive.Vector;
            type.SizeInBits = pElementCount * pElementType.SizeInBits;
            type.ElementCount = pElementCount;
            type.ElementType = pElementType;
            sTypes[type.Signature] = type;
            return type;
        }
        public static string GetVectorSignature(LLType pElementType, int pElementCount) { return string.Format("(vector:{0}:{1})", pElementType.Signature, pElementCount); }
        public static LLType GetOrCreateVectorType(LLType pElementType, int pElementCount)
        {
            LLType type = null;
            string signature = GetVectorSignature(pElementType, pElementCount);
            if (!sTypes.TryGetValue(signature, out type)) type = CreateVectorType(pElementType, pElementCount);
            return type;
        }


        private static LLType CreateArrayType(LLType pElementType, int pElementCount)
        {
            LLType type = new LLType();
            type.Primitive = LLPrimitive.Array;
            type.SizeInBits = pElementCount * pElementType.SizeInBits;
            type.ElementCount = pElementCount;
            type.ElementType = pElementType;
            sTypes[type.Signature] = type;
            return type;
        }
        public static string GetArraySignature(LLType pElementType, int pElementCount) { return string.Format("(array:{0}:{1})", pElementType.Signature, pElementCount); }
        public static LLType GetOrCreateArrayType(LLType pElementType, int pElementCount)
        {
            LLType type = null;
            string signature = GetArraySignature(pElementType, pElementCount);
            if (!sTypes.TryGetValue(signature, out type)) type = CreateArrayType(pElementType, pElementCount);
            return type;
        }


        private static LLType CreateStructureType(string pName, bool pPacked, List<LLType> pFieldTypes)
        {
            LLType type = new LLType();
            type.Primitive = LLPrimitive.Structure;
            type.SizeInBits = pFieldTypes.Sum(t => t.SizeInBits);
            type.StructureName = pName;
            type.StructurePacked = pPacked;
            type.StructureFields = new List<LLType>(pFieldTypes);
            sTypes[type.Signature] = type;
            return type;
        }
        public static string GetStructureSignature(bool pPacked, List<LLType> pFieldTypes)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("(structure:{0}", pPacked);
            foreach (LLType typeField in pFieldTypes)
                sb.AppendFormat(":{0}", typeField.Signature);
            sb.Append(")");
            return sb.ToString();
        }
        public static LLType GetOrCreateStructureType(string pName, bool pPacked, List<LLType> pFieldTypes)
        {
            LLType type = null;
            string signature = GetStructureSignature(pPacked, pFieldTypes);
            if (!sTypes.TryGetValue(signature, out type)) type = CreateStructureType(pName, pPacked, pFieldTypes);
            return type;
        }


        public static string GetFunctionIdentifier(string pName, LLType pReturnType, List<LLParameter> pParameters)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}:{1}", pReturnType, pName);
            foreach (LLParameter parameter in pParameters)
                sb.AppendFormat(":{0}", parameter.Type);
            return sb.ToString();
        }
        private static LLFunction CreateFunction(string pName, bool pExplicitName, LLType pReturnType, List<LLParameter> pParameters, bool pExternal, bool pIntrinsic)
        {
            LLFunction function = new LLFunction();
            function.Name = pName;
            function.ExplicitName = pExplicitName;
            function.Identifier = LLModule.GetFunctionIdentifier(pName, pReturnType, pParameters);
            function.IdentifierHash = "F_" + BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(function.Identifier))).Replace("-", "").ToUpper().Substring(0, 16);
            function.ReturnType = pReturnType;
            function.Parameters = new LLParameterList(pParameters);
            function.External = pExternal;
            function.Intrinsic = pIntrinsic;
            sFunctions[function.Identifier] = function;
            return function;
        }
        public static LLFunction GetOrCreateFunction(string pName, bool pExplicitName, LLType pReturnType, List<LLParameter> pParameters, bool pExternal, bool pIntrinsic)
        {
            LLFunction function = null;
            string identifier = GetFunctionIdentifier(pName, pReturnType, pParameters);
            if (!sFunctions.TryGetValue(identifier, out function)) function = CreateFunction(pName, pExplicitName, pReturnType, pParameters, pExternal, pIntrinsic);
            return function;
        }
        public static LLFunction GetFunction(string pIdentifier)
        {
            LLFunction function = null;
            sFunctions.TryGetValue(pIdentifier, out function);
            return function;
        }


        public static LLGlobal CreateGlobal(LLType pType, string pIdentifier)
        {
            LLGlobal global = LLGlobal.Create(pType, pIdentifier);
            sGlobals.Add(global.Identifier, global);
            return global;
        }
        public static LLGlobal GetGlobal(string pIdentifier)
        {
            LLGlobal global = null;
            sGlobals.TryGetValue(pIdentifier, out global);
            return global;
        }


        public static LLType BinaryResult(LLType pLeftType, LLType pRightType)
        {
            if (pLeftType.Primitive == LLPrimitive.Void || pRightType.Primitive == LLPrimitive.Void) throw new NotSupportedException();
            if (pLeftType.Primitive == LLPrimitive.Function || pRightType.Primitive == LLPrimitive.Function) throw new NotSupportedException();
            if (pLeftType.Primitive == LLPrimitive.Vector || pRightType.Primitive == LLPrimitive.Vector) throw new NotSupportedException();
            if (pLeftType.Primitive == LLPrimitive.Array || pRightType.Primitive == LLPrimitive.Array) throw new NotSupportedException();
            if (pLeftType.Primitive == LLPrimitive.Structure || pRightType.Primitive == LLPrimitive.Structure) throw new NotSupportedException();

            if (pLeftType == pRightType) return pLeftType;
            // u1, i8, u8, i16, u16, i32, u32, i64, u64, f32, f64, ptr
            if (pLeftType.Primitive == LLPrimitive.Pointer) return pLeftType;
            if (pRightType.Primitive == LLPrimitive.Pointer) return pRightType;
            // u1, i8, u8, i16, u16, i32, u32, i64, u64, f32, f64
            if (pLeftType.Primitive == LLPrimitive.Float && pRightType.Primitive == LLPrimitive.Float) return pLeftType.SizeInBits >= pRightType.SizeInBits ? pLeftType : pRightType;
            if (pLeftType.Primitive == LLPrimitive.Float) return pLeftType;
            if (pRightType.Primitive == LLPrimitive.Float) return pRightType;
            // u1, i8, u8, i16, u16, i32, u32, i64, u64
            if (pLeftType.SizeInBits > pRightType.SizeInBits) return pLeftType;
            if (pRightType.SizeInBits > pLeftType.SizeInBits) return pRightType;
            // i, u
            if (pLeftType.Primitive == LLPrimitive.Unsigned) return pLeftType;
            if (pRightType.Primitive == LLPrimitive.Unsigned) return pRightType;
            // i... should not get here, means both are Signed of the same size, should be same type handled earlier
            throw new NotSupportedException();
        }


        public static void Dump(StreamWriter pWriter)
        {
            foreach (LLGlobal global in sGlobals.Values)
                pWriter.WriteLine("{0} = global {1} zeroinitializer", global, global.Type.PointerDepthMinusOne);
            if (sGlobals.Count > 0) pWriter.WriteLine();

            List<LLType> typesStructure = sTypes.Values.Where(t => t.Primitive == LLPrimitive.Structure).ToList();
            foreach (LLType type in typesStructure)
            {
                pWriter.Write("{0} = type {1}{{ ", type, type.StructurePacked ? "<" : "");
                for (int index = 0; index < type.StructureFields.Count; ++index)
                {
                    if (index > 0) pWriter.Write(", ");
                    pWriter.Write(type.StructureFields[index]);
                }
                pWriter.WriteLine(" }}{0}", type.StructurePacked ? ">" : "");
            }
            if (typesStructure.Count > 0) pWriter.WriteLine();

            List<LLFunction> functionsExternal = sFunctions.Values.Where(f => f.External && !f.Intrinsic).ToList();
            foreach (LLFunction function in functionsExternal)
            {
                pWriter.Write("declare {0}", function.Declaration);
                if (function.Description != null) pWriter.Write("    ; {0}", function.Description);
                pWriter.WriteLine();
            }
            if (functionsExternal.Count > 0) pWriter.WriteLine();

            List<LLFunction> functionsInternal = sFunctions.Values.Where(f => !f.External && !f.Intrinsic).ToList();
            foreach (LLFunction function in functionsInternal)
            {
                pWriter.Write("define {0}", function.Declaration);
                if (function.Description != null) pWriter.Write("    ; {0}", function.Description);
                pWriter.WriteLine();
                pWriter.WriteLine("{");
                foreach (LLInstructionBlock block in function.Blocks)
                {
                    foreach (LLInstruction instruction in block.Instruction)
                    {
                        pWriter.Write("    ");
                        pWriter.WriteLine(instruction.ToString());
                    }
                }
                pWriter.WriteLine("}");
                pWriter.WriteLine();
            }
        }
    }
}
