using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLSizeOfLocation : HLLocation
    {
        public static HLSizeOfLocation Create(HLType pSizeOfType)
        {
            HLSizeOfLocation location = new HLSizeOfLocation(HLDomain.SystemInt32);
            location.mSizeOfType = pSizeOfType;
            return location;
        }

        private HLSizeOfLocation(HLType pType) : base(pType) { }

        private HLType mSizeOfType = null;
        public HLType SizeOfType { get { return mSizeOfType; } }

        public override string ToString() { return string.Format("sizeof({0})", mSizeOfType); }

        internal override LLLocation Load(LLFunction pFunction)
        {
            return LLLiteralLocation.Create(LLLiteral.Create(Type.LLType, SizeOfType.CalculatedSize.ToString()));
        }
    }
}
