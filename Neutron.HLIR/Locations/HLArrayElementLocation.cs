using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLArrayElementLocation : HLLocation
    {
        public static HLArrayElementLocation Create(HLLocation pInstance, HLLocation pIndex, HLType pElementType)
        {
            HLArrayElementLocation location = new HLArrayElementLocation(pElementType);
            location.mInstance = pInstance;
            location.mIndex = pIndex;
            location.mElementType = pElementType;
            return location;
        }

        private HLArrayElementLocation(HLType pType) : base(pType) { }

        private HLLocation mInstance = null;
        public HLLocation Instance { get { return mInstance; } }

        private HLLocation mIndex = null;
        public HLLocation Index { get { return mIndex; } }

        private HLType mElementType = null;
        public HLType ElementType { get { return mElementType; } }

        internal override HLLocation AddressOf() { return HLArrayElementAddressLocation.Create(mInstance, mIndex, mElementType); }

        public override string ToString() { return string.Format("({0})({1})[{2}]", Type, mInstance, mIndex); }
    }
}
