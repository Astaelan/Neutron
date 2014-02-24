using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR
{
    public sealed class LLTemporary
    {
        public static LLTemporary Create(LLType pType, int pIdentifier)
        {
            LLTemporary temporary = new LLTemporary();
            temporary.mType = pType;
            temporary.mIdentifier = pIdentifier;
            return temporary;
        }

        private LLType mType = null;
        private int mIdentifier = 0;

        private LLTemporary() { }

        public LLType Type { get { return mType; } }

        public int Identifier { get { return mIdentifier; } }

        public override string ToString() { return "%" + mIdentifier; }
    }
}
