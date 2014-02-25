using Neutron.LLIR.Instructions;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Neutron.LLIR
{
    public sealed class LLFunction
    {
        private string mName = null;
        private bool mExplicitName = false;
        private bool mExternal = false;
        private string mDescription = null;
        private string mIdentifier = null;
        private string mIdentifierHash = null;
        private LLType mReturnType = null;
        private LLParameterList mParameters = null;
        private LLLabelList mLabels = null;
        private LLLocalList mLocals = null;
        private int mTemporaryIndexer = 0;
        private List<LLInstructionBlock> mBlocks = null;
        private LLInstructionBlock mCurrentBlock = null;

        internal LLFunction() { }

        public string Name { get { return mName; } internal set { mName = value; } }
        public bool ExplicitName { get { return mExplicitName; } set { mExplicitName = value; } }
        public bool External { get { return mExternal; } set { mExternal = value; } }
        public string Description { get { return mDescription; } set { mDescription = value; } }
        public string Identifier { get { return mIdentifier; } internal set { mIdentifier = value; } }
        public string IdentifierHash { get { return mIdentifierHash; } internal set { mIdentifierHash = value; } }
        public LLType ReturnType { get { return mReturnType; } internal set { mReturnType = value; } }
        public LLParameterList Parameters { get { return mParameters; } internal set { mParameters = value; } }

        public LLLabelList Labels { get { return mLabels; } }
        public LLLabel CreateLabel(int pIdentifier)
        {
            if (mLabels == null) mLabels = new LLLabelList();
            LLLabel label = LLLabel.Create(pIdentifier);
            mLabels.Add(label);
            return label;
        }

        public LLLocalList Locals { get { return mLocals; } }
        public LLLocal CreateLocal(LLType pType, string pIdentifier)
        {
            if (mLocals == null) mLocals = new LLLocalList();
            LLLocal local = LLLocal.Create(pType.PointerDepthPlusOne, pIdentifier);
            mLocals.Add(local);
            return local;
        }

        public LLTemporary CreateTemporary(LLType pType)
        {
            return LLTemporary.Create(pType, mTemporaryIndexer++);
        }

        public List<LLInstructionBlock> Blocks { get { return mBlocks; } }
        public LLInstructionBlock CurrentBlock { get { return mCurrentBlock; } set { mCurrentBlock = value; } }
        public LLInstructionBlock CreateBlock(LLLabel pStartLabel)
        {
            if (mBlocks == null) mBlocks = new List<LLInstructionBlock>();
            LLInstructionBlock block = LLInstructionBlock.Create(this, pStartLabel);
            mBlocks.Add(block);
            if (mBlocks.Count == 1)
            {
                if (mLocals != null)
                {
                    foreach (LLLocal local in mLocals)
                        block.EmitAllocate(LLLocalLocation.Create(local));
                }
            }
            return block;
        }

        public string Signature { get { return LLModule.GetFunctionSignature(mReturnType, mParameters.ToTypeList()); } }

        public string Declaration
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0} {1}(", mReturnType, this);
                for (int index = 0; index < mParameters.Count; ++index)
                {
                    sb.AppendFormat("{0} {1}", mParameters[index].Type, mParameters[index]);
                    if (index < (mParameters.Count - 1)) sb.Append(", ");
                }
                sb.Append(")");
                return sb.ToString();
            }
        }

        public override string ToString()
        {
            return string.Format("@{0}", mExplicitName ? mName : mIdentifierHash);
        }
    }
}
