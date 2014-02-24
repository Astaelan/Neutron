using Microsoft.Cci.MutableCodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLArrayElementAddressLocation : HLLocation
    {
        public static HLArrayElementAddressLocation Create(HLLocation pInstance, HLLocation pIndex, HLType pElementType)
        {
            HLArrayElementAddressLocation location = new HLArrayElementAddressLocation(HLDomain.GetOrCreateType(MutableModelHelper.GetManagedPointerTypeReference(pElementType.Definition, HLDomain.Host.InternFactory, pElementType.Definition)));
            location.mInstance = pInstance;
            location.mIndex = pIndex;
            location.mElementType = pElementType;
            return location;
        }

        private HLArrayElementAddressLocation(HLType pType) : base(pType) { }

        private HLLocation mInstance = null;
        public HLLocation Instance { get { return mInstance; } }

        private HLLocation mIndex = null;
        public HLLocation Index { get { return mIndex; } }

        private HLType mElementType = null;
        public HLType ElementType { get { return mElementType; } }

        public override string ToString() { return string.Format("({0})&({1})[{2}]", Type, mInstance, mIndex); }
    }
}
