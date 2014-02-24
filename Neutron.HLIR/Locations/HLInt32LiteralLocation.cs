using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLInt32LiteralLocation : HLLocation
    {
        public static HLInt32LiteralLocation Create(int pLiteral)
        {
            HLInt32LiteralLocation location = new HLInt32LiteralLocation(HLDomain.SystemInt32);
            location.mLiteral = pLiteral;
            return location;
        }

        private HLInt32LiteralLocation(HLType pType) : base(pType) { }

        private int mLiteral = 0;
        public int Literal { get { return mLiteral; } }

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
