using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLUInt32LiteralLocation : HLLiteralLocation
    {
        public static HLUInt32LiteralLocation Create(uint pLiteral)
        {
            HLUInt32LiteralLocation location = new HLUInt32LiteralLocation(HLDomain.SystemUInt32);
            location.mLiteral = pLiteral;
            return location;
        }

        private HLUInt32LiteralLocation(HLType pType) : base(pType) { }

        private uint mLiteral = 0;
        public uint Literal { get { return mLiteral; } }

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
