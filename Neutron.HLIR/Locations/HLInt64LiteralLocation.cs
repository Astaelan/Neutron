using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLInt64LiteralLocation : HLLocation
    {
        public static HLInt64LiteralLocation Create(long pLiteral)
        {
            HLInt64LiteralLocation location = new HLInt64LiteralLocation(HLDomain.SystemInt64);
            location.mLiteral = pLiteral;
            return location;
        }

        private HLInt64LiteralLocation(HLType pType) : base(pType) { }

        private long mLiteral = 0;
        public long Literal { get { return mLiteral; } }

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
