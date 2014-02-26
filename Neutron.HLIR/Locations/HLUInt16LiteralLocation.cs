using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLUInt16LiteralLocation : HLLiteralLocation
    {
        public static HLUInt16LiteralLocation Create(ushort pLiteral)
        {
            HLUInt16LiteralLocation location = new HLUInt16LiteralLocation(HLDomain.SystemUInt16);
            location.mLiteral = pLiteral;
            return location;
        }

        private HLUInt16LiteralLocation(HLType pType) : base(pType) { }

        private ushort mLiteral = 0;
        public ushort Literal { get { return mLiteral; } }

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
