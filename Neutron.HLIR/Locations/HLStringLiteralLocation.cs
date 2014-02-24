using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLStringLiteralLocation : HLLocation
    {
        public static HLStringLiteralLocation Create(string pLiteral)
        {
            HLStringLiteralLocation location = new HLStringLiteralLocation(HLDomain.SystemString);
            location.mLiteral = pLiteral;
            return location;
        }

        private HLStringLiteralLocation(HLType pType) : base(pType) { }

        private string mLiteral = null;
        public string Literal { get { return mLiteral; } }

        public override string ToString() { return string.Format("({0})\"{1}\"", Type, mLiteral); }
    }
}
