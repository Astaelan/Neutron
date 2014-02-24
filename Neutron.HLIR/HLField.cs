using Microsoft.Cci;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR
{
    public sealed class HLField
    {
        internal HLField() { }

        //private IFieldDefinition mDefinition = null;
        //public IFieldDefinition Definition { get { return mDefinition; } internal set { mDefinition = value; } }

        private string mName = null;
        public string Name { get { return mName; } internal set { mName = value; } }

        private string mSignature = null;
        public string Signature { get { return mSignature; } internal set { mSignature = value; } }

        private HLType mContainer = null;
        public HLType Container { get { return mContainer; } internal set { mContainer = value; } }

        private bool mIsStatic = false;
        public bool IsStatic { get { return mIsStatic; } internal set { mIsStatic = value; } }

        private bool mIsCompileTimeConstant = false;
        public bool IsCompileTimeConstant { get { return mIsCompileTimeConstant; } internal set { mIsCompileTimeConstant = value; } }

        private HLType mType = null;
        public HLType Type { get { return mType; } internal set { mType = value; } }

        private int mOffset = 0;
        public int Offset { get { return mOffset; } internal set { mOffset = value; } }

        public override string ToString() { return Name; }
    }
}
