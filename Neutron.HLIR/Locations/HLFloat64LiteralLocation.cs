using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLFloat64LiteralLocation : HLLiteralLocation
    {
        public static HLFloat64LiteralLocation Create(double pLiteral)
        {
            HLFloat64LiteralLocation location = new HLFloat64LiteralLocation(HLDomain.SystemDouble);
            location.mLiteral = pLiteral;
            return location;
        }

        private HLFloat64LiteralLocation(HLType pType) : base(pType) { }

        private double mLiteral = 0;
        public double Literal { get { return mLiteral; } }

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
