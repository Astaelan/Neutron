using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLInt8LiteralLocation : HLLiteralLocation
    {
        public static HLInt8LiteralLocation Create(sbyte pLiteral)
        {
            HLInt8LiteralLocation location = new HLInt8LiteralLocation(HLDomain.SystemSByte);
            location.mLiteral = pLiteral;
            return location;
        }

        private HLInt8LiteralLocation(HLType pType) : base(pType) { }

        private sbyte mLiteral = 0;
        public sbyte Literal { get { return mLiteral; } }

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
