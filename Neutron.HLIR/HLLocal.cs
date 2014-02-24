using Microsoft.Cci;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR
{
    public sealed class HLLocal
    {
        internal HLLocal() { }

        //private ILocalDefinition mDefinition = null;
        //public ILocalDefinition Definition { get { return mDefinition; } internal set { mDefinition = value; } }

        private string mName = null;
        public string Name { get { return mName; } internal set { mName = value; } }

        private string mSignature = null;
        public string Signature { get { return mSignature; } internal set { mSignature = value; } }

        //private HLMethod mContainer = null;
        //public HLMethod Container { get { return mContainer; } internal set { mContainer = value; } }

        private HLType mType = null;
        public HLType Type { get { return mType; } internal set { mType = value; } }

        public override string ToString() { return Name; }
    }
}
