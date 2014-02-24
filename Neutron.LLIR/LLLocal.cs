using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR
{
    public sealed class LLLocal
    {
        public static LLLocal Create(LLType pType, string pIdentifier)
        {
            LLLocal local = new LLLocal();
            local.mType = pType;
            local.mIdentifier = pIdentifier;
            return local;
        }

        private LLType mType = null;
        private string mIdentifier = null;

        private LLLocal() { }

        public LLType Type { get { return mType; } }

        public string Identifier { get { return mIdentifier; } }

        public override string ToString() { return "%" + mIdentifier; }
    }
}
