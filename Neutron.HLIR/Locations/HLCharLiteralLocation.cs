using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLCharLiteralLocation : HLLiteralLocation
    {
        public static HLCharLiteralLocation Create(char pLiteral)
        {
            HLCharLiteralLocation location = new HLCharLiteralLocation(HLDomain.SystemChar);
            location.mLiteral = pLiteral;
            return location;
        }

        private HLCharLiteralLocation(HLType pType) : base(pType) { }

        private char mLiteral = '\0';
        public char Literal { get { return mLiteral; } }

        public override string ToString()
        {
            return string.Format("({0})'{1}'", Type, mLiteral);
        }

        internal override LLLocation Load(LLFunction pFunction)
        {
            return LLLiteralLocation.Create(LLLiteral.Create(Type.LLType, ((short)Literal).ToString()));
        }

        public override string LiteralAsString { get { return ((short)Literal).ToString(); } }
    }
}
