using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Locations
{
    public sealed class LLLiteralLocation : LLLocation
    {
        public static LLLiteralLocation Create(LLLiteral pLiteral)
        {
            LLLiteralLocation location = new LLLiteralLocation(pLiteral.Type);
            location.mLiteral = pLiteral;
            return location;
        }

        private LLLiteral mLiteral = null;

        private LLLiteralLocation(LLType pType) : base(pType) { }

        public LLLiteral Literal { get { return mLiteral; } }

        public override string ToString() { return mLiteral.ToString(); }

        public override LLLocation Load(LLInstructionBlock pBlock) { return this; }
    }
}
