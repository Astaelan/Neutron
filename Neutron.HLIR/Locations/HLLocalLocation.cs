using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLLocalLocation : HLLocation
    {
        public static HLLocalLocation Create(HLLocal pLocal)
        {
            HLLocalLocation location = new HLLocalLocation(pLocal.Type);
            location.mLocal = pLocal;
            return location;
        }

        private HLLocalLocation(HLType pType) : base(pType) { }

        private HLLocal mLocal = null;
        public HLLocal Local { get { return mLocal; } }

        internal override HLLocation AddressOf() { return HLLocalAddressLocation.Create(mLocal); }

        public override string ToString() { return string.Format("({0}){1}", Type, mLocal); }

        internal override LLLocation Load(LLFunction pFunction)
        {
            LLLocation locationLocal = LLLocalLocation.Create(pFunction.Locals[Local.Name]);
            LLLocation locationTemporary = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationLocal.Type.PointerDepthMinusOne));
            pFunction.CurrentBlock.EmitLoad(locationTemporary, locationLocal);
            return locationTemporary;
        }

        internal override void Store(LLFunction pFunction, LLLocation pSource)
        {
            LLLocation locationLocal = LLLocalLocation.Create(pFunction.Locals[Local.Name]);
            LLLocation locationSource = pFunction.CurrentBlock.EmitConversion(pSource, locationLocal.Type.PointerDepthMinusOne);
            pFunction.CurrentBlock.EmitStore(locationLocal, locationSource);
        }
    }
}
