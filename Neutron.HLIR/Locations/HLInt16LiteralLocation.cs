using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLInt16LiteralLocation : HLLiteralLocation
    {
        public static HLInt16LiteralLocation Create(short pLiteral)
        {
            HLInt16LiteralLocation location = new HLInt16LiteralLocation(HLDomain.SystemInt16);
            location.mLiteral = pLiteral;
            return location;
        }

        private HLInt16LiteralLocation(HLType pType) : base(pType) { }

        private short mLiteral = 0;
        public short Literal { get { return mLiteral; } }

        public override string ToString()
        {
            return string.Format("({0}){1}", Type, mLiteral);
        }

        internal override LLLocation Load(LLFunction pFunction)
        {
            return LLLiteralLocation.Create(LLLiteral.Create(Type.LLType, Literal.ToString()));
        }

        public override string LiteralAsString { get { return Literal.ToString(); } }
    }
}
