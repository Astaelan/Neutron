using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLBooleanLiteralLocation : HLLiteralLocation
    {
        public static HLBooleanLiteralLocation Create(bool pLiteral)
        {
            HLBooleanLiteralLocation location = new HLBooleanLiteralLocation(HLDomain.SystemBoolean);
            location.mLiteral = pLiteral;
            return location;
        }

        private HLBooleanLiteralLocation(HLType pType) : base(pType) { }

        private bool mLiteral = false;
        public bool Literal { get { return mLiteral; } }

        public override string ToString()
        {
            return string.Format("({0}){1}", Type, mLiteral);
        }

        internal override LLLocation Load(LLFunction pFunction)
        {
            return LLLiteralLocation.Create(LLLiteral.Create(Type.LLType, Literal.ToString().ToLower()));
        }

        public override string LiteralAsString { get { return Literal.ToString().ToLower(); } }
    }
}
