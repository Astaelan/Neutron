using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLUInt64LiteralLocation : HLLocation
    {
        public static HLUInt64LiteralLocation Create(ulong pLiteral)
        {
            HLUInt64LiteralLocation location = new HLUInt64LiteralLocation(HLDomain.SystemUInt64);
            location.mLiteral = pLiteral;
            return location;
        }

        private HLUInt64LiteralLocation(HLType pType) : base(pType) { }

        private ulong mLiteral = 0;
        public ulong Literal { get { return mLiteral; } }

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
