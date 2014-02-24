using Microsoft.Cci;
using Neutron.LLIR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR
{
    public sealed class HLParameter
    {
        internal HLParameter() { }

        //private IParameterDefinition mDefinition = null;
        //public IParameterDefinition Definition { get { return mDefinition; } internal set { mDefinition = value; } }

        private string mName = null;
        public string Name { get { return mName; } internal set { mName = value; } }

        private string mSignature = null;
        public string Signature { get { return mSignature; } internal set { mSignature = value; } }

        //private HLMethod mMethodContainer = null;
        //public HLMethod MethodContainer { get { return mMethodContainer; } internal set { mMethodContainer = value; } }

        private HLType mType = null;
        public HLType Type { get { return mType; } internal set { mType = value; } }

        private bool mRequiresAddressing = false;
        public bool RequiresAddressing { get { return mRequiresAddressing; } internal set { mRequiresAddressing = value; } }

        private LLLocal mAddressableLocal = null;
        public LLLocal AddressableLocal { get { return mAddressableLocal; } internal set { mAddressableLocal = value; } }

        public override string ToString() { return Name; }
    }
}
