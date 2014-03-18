using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLEnumLiteralLocation : HLLiteralLocation
    {
        public static HLEnumLiteralLocation Create(HLType pType, string pLiteral)
        {
            HLEnumLiteralLocation location = new HLEnumLiteralLocation(pType);
            location.mLiteral = pLiteral;
            return location;
        }

        private HLEnumLiteralLocation(HLType pType) : base(pType) { }

        private string mLiteral = null;
        public string Literal { get { return mLiteral; } }

        public override string ToString()
        {
            return string.Format("({0}){1}", Type, mLiteral);
        }

        internal override LLLocation Load(LLFunction pFunction)
        {
            return LLLiteralLocation.Create(LLLiteral.Create(Type.LLType, Literal));
        }

        public override string LiteralAsString { get { return Literal; } }
    }
}
