using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLArrayLengthLocation : HLLocation
    {
        public static HLArrayLengthLocation Create(HLLocation pInstance)
        {
            HLArrayLengthLocation location = new HLArrayLengthLocation(HLDomain.SystemInt32);
            location.mInstance = pInstance;
            return location;
        }

        private HLArrayLengthLocation(HLType pType) : base(pType) { }

        private HLLocation mInstance = null;
        public HLLocation Instance { get { return mInstance; } }

        public override string ToString() { return string.Format("arraylength({0})", mInstance); }
    }
}
