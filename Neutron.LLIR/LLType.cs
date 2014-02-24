using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR
{
    public sealed class LLType
    {
        internal LLType() { }

        private LLPrimitive mPrimitive = LLPrimitive.Void;
        public LLPrimitive Primitive { get { return mPrimitive; } internal set { mPrimitive = value; } }

        private int mSizeInBits = 0;
        public int SizeInBits { get { return mSizeInBits; } internal set { mSizeInBits = value; } }


        private LLType mPointerBase = null;
        public LLType PointerBase { get { return mPointerBase; } internal set { mPointerBase = value; } }

        private int mPointerDepth = 0;
        public int PointerDepth { get { return mPointerDepth; } internal set { mPointerDepth = value; } }

        public LLType AdjustPointerDepth(int pPointerDepth)
        {
            if (pPointerDepth == 0) return this;
            if (pPointerDepth < 0)
            {
                if (mPointerDepth == 0) return this;
                int depth = mPointerDepth + pPointerDepth;
                if (depth <= 0) return mPointerBase;
                return LLModule.GetOrCreatePointerType(mPointerBase, depth);
            }
            if (mPointerDepth == 0) return LLModule.GetOrCreatePointerType(this, pPointerDepth);
            return LLModule.GetOrCreatePointerType(mPointerBase, mPointerDepth + pPointerDepth);
        }

        public LLType PointerDepthPlusOne { get { return AdjustPointerDepth(1); } }

        public LLType PointerDepthMinusOne { get { return AdjustPointerDepth(-1); } }


        private LLType mFunctionReturn = null;
        public LLType FunctionReturn { get { return mFunctionReturn; } internal set { mFunctionReturn = value; } }

        private List<LLType> mFunctionParameters = null;
        public List<LLType> FunctionParameters { get { return mFunctionParameters; } internal set { mFunctionParameters = value; } }


        private LLType mElementType = null;
        public LLType ElementType { get { return mElementType; } internal set { mElementType = value; } }

        private int mElementCount = 0;
        public int ElementCount { get { return mElementCount; } internal set { mElementCount = value; } }


        private string mStructureName = null;
        public string StructureName { get { return mStructureName; } internal set { mStructureName = value; } }
        
        private bool mStructurePacked = false;
        public bool StructurePacked { get { return mStructurePacked; } internal set { mStructurePacked = value; } }
        
        private List<LLType> mStructureFields = null;
        public List<LLType> StructureFields { get { return mStructureFields; } internal set { mStructureFields = value; } }


        //public bool IsNumericPrimitive
        //{
        //    get
        //    {
        //        switch (mPrimitive)
        //        {
        //            case LLPrimitive.Signed:
        //            case LLPrimitive.Unsigned:
        //            case LLPrimitive.Float: return true;
        //            default: return false;
        //        }
        //    }
        //}

        public string Signature
        {
            get
            {
                switch (mPrimitive)
                {
                    case LLPrimitive.Void: return LLModule.GetVoidSignature();
                    case LLPrimitive.Signed: return LLModule.GetSignedSignature(mSizeInBits);
                    case LLPrimitive.Unsigned: return LLModule.GetUnsignedSignature(mSizeInBits);
                    case LLPrimitive.Float: return LLModule.GetFloatSignature(mSizeInBits);
                    case LLPrimitive.Pointer: return LLModule.GetPointerSignature(mPointerBase, mPointerDepth);
                    case LLPrimitive.Function: return LLModule.GetFunctionSignature(mFunctionReturn, mFunctionParameters);
                    case LLPrimitive.Vector: return LLModule.GetVectorSignature(mElementType, mElementCount);
                    case LLPrimitive.Array: return LLModule.GetArraySignature(mElementType, mElementCount);
                    case LLPrimitive.Structure: return LLModule.GetStructureSignature(mStructurePacked, mStructureFields);
                    default: throw new NotSupportedException();
                }
            }
        }

        public override string ToString()
        {
            switch (mPrimitive)
            {
                case LLPrimitive.Void: return "void";
                case LLPrimitive.Signed:
                case LLPrimitive.Unsigned: return "i" + mSizeInBits;
                case LLPrimitive.Float: return mSizeInBits == 32 ? "float" : "double";
                case LLPrimitive.Pointer: return mPointerBase.ToString() + new string('*', mPointerDepth);
                case LLPrimitive.Function:
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(mFunctionReturn.ToString());
                        sb.Append(" (");
                        for (int index = 0; index < mFunctionParameters.Count; ++index)
                        {
                            if (index > 0) sb.Append(", ");
                            sb.Append(mFunctionParameters[index].ToString());
                        }
                        sb.Append(")");
                        return sb.ToString();
                    }
                case LLPrimitive.Vector: return "<" + mElementCount + " x " + mElementType.ToString() + ">";
                case LLPrimitive.Array: return "[" + mElementCount + " x " + mElementType.ToString() + "]";
                case LLPrimitive.Structure: return "%" + mStructureName;
                default: throw new NotSupportedException();
            }
        }
    }
}
