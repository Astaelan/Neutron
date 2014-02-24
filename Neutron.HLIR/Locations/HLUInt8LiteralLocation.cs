using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLUInt8LiteralLocation : HLLocation
    {
        public static HLUInt8LiteralLocation Create(byte pLiteral)
        {
            HLUInt8LiteralLocation location = new HLUInt8LiteralLocation(HLDomain.SystemByte);
            location.mLiteral = pLiteral;
            return location;
        }

        private HLUInt8LiteralLocation(HLType pType) : base(pType) { }

        private byte mLiteral = 0;
        public byte Literal { get { return mLiteral; } }

        public override string ToString()
        {
            return string.Format("({0}){1}", Type, mLiteral);
        }

        internal override LLLocation Load(LLFunction pFunction)
        {
            return LLLiteralLocation.Create(LLLiteral.Create(Type.LLType, Literal.ToString()));
        }
    }
}
