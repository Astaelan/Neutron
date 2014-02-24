using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR
{
    public sealed class LLParameter
    {
        public static LLParameter Create(LLType pType, string pIdentifier)
        {
            LLParameter parameter = new LLParameter();
            parameter.mType = pType;
            parameter.mIdentifier = pIdentifier;
            return parameter;
        }

        private LLType mType = null;
        private string mIdentifier = null;

        private LLParameter() { }

        public LLType Type { get { return mType; } }

        public string Identifier { get { return mIdentifier; } }

        public override string ToString() { return "%" + mIdentifier; }
    }
}
