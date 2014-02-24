using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLTemporaryLocation : HLLocation
    {
        public static HLTemporaryLocation Create(HLTemporary pTemporary)
        {
            HLTemporaryLocation location = new HLTemporaryLocation(pTemporary.Type);
            location.mTemporary = pTemporary;
            return location;
        }

        private HLTemporaryLocation(HLType pType) : base(pType) { }

        private HLTemporary mTemporary = null;
        public HLTemporary Temporary { get { return mTemporary; } }

        internal override HLLocation AddressOf() { return HLTemporaryAddressLocation.Create(mTemporary); }

        public override string ToString() { return string.Format("({0}){1}", Type, mTemporary); }

        internal override LLLocation Load(LLFunction pFunction)
        {
            LLLocation locationLocal = LLLocalLocation.Create(pFunction.Locals[Temporary.Name]);
            LLLocation locationTemporary = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationLocal.Type.PointerDepthMinusOne));
            pFunction.CurrentBlock.EmitLoad(locationTemporary, locationLocal);
            return locationTemporary;
        }

        internal override void Store(LLFunction pFunction, LLLocation pSource)
        {
            LLLocation locationLocal = LLLocalLocation.Create(pFunction.Locals[Temporary.Name]);
            LLLocation locationSource = pFunction.CurrentBlock.EmitConversion(pSource, locationLocal.Type.PointerDepthMinusOne);
            pFunction.CurrentBlock.EmitStore(locationLocal, locationSource);
        }
    }
}
