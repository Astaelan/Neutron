using Microsoft.Cci.MutableCodeModel;
using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLIndirectAddressLocation : HLLocation
    {
        public static HLIndirectAddressLocation Create(HLLocation pAddress, HLType pType)
        {
            HLIndirectAddressLocation location = new HLIndirectAddressLocation(HLDomain.GetOrCreateType(MutableModelHelper.GetManagedPointerTypeReference(pType.Definition, HLDomain.Host.InternFactory, pType.Definition)));
            location.mAddress = pAddress;
            return location;
        }

        private HLIndirectAddressLocation(HLType pType) : base(pType) { }

        private HLLocation mAddress = null;
        public HLLocation Address { get { return mAddress; } }
        
        public override string ToString() { return string.Format("({0})({1})", Type, mAddress); }

        internal override LLLocation Load(LLFunction pFunction)
        {
            LLLocation locationAddress = mAddress.Load(pFunction);
            locationAddress = pFunction.CurrentBlock.EmitConversion(locationAddress, Type.LLType);
            return locationAddress.Load(pFunction.CurrentBlock);
        }

        internal override void Store(LLFunction pFunction, LLLocation pSource)
        {
            LLLocation locationAddress = mAddress.Load(pFunction);
            locationAddress = pFunction.CurrentBlock.EmitConversion(locationAddress, Type.LLType);
            LLLocation locationSource = pFunction.CurrentBlock.EmitConversion(pSource, locationAddress.Type.PointerDepthMinusOne);
            pFunction.CurrentBlock.EmitStore(locationAddress, locationSource);
        }
    }
}
