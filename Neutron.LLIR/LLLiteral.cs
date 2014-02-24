using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR
{
    public sealed class LLLiteral
    {
        public static LLLiteral Create(LLType pType, string pValue)
        {
            LLLiteral literal = new LLLiteral();
            literal.mType = pType;
            literal.mValue = pValue;
            return literal;
        }

        private LLType mType = null;
        private string mValue = null;

        private LLLiteral() { }

        public LLType Type { get { return mType; } }

        public string Value { get { return mValue; } }

        public override string ToString() { return mValue; }
    }
}
