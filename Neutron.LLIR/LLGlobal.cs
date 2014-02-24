using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR
{
    public sealed class LLGlobal
    {
        public static LLGlobal Create(LLType pType, string pIdentifier)
        {
            LLGlobal global = new LLGlobal();
            global.mType = pType;
            global.mIdentifier = pIdentifier;
            return global;
        }

        private LLType mType = null;
        private string mIdentifier = null;

        private LLGlobal() { }

        public LLType Type { get { return mType; } }

        public string Identifier { get { return mIdentifier; } }

        public override string ToString() { return "@" + mIdentifier; }
    }
}
