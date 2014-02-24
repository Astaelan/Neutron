using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLFieldLocation : HLLocation
    {
        public static HLFieldLocation Create(HLLocation pInstance, HLField pField)
        {
            HLFieldLocation location = new HLFieldLocation(pField.Type);
            location.mInstance = pInstance;
            location.mField = pField;
            return location;
        }

        private HLFieldLocation(HLType pType) : base(pType) { }

        private HLLocation mInstance = null;
        public HLLocation Instance { get { return mInstance; } }

        private HLField mField = null;
        public HLField Field { get { return mField; } }

        internal override HLLocation AddressOf() { return HLFieldAddressLocation.Create(mInstance, mField); }

        public override string ToString() { return string.Format("({0})({1}).{2}", Type, mInstance, mField); }

        internal override LLLocation Load(LLFunction pFunction)
        {
            LLLocation locationFieldPointer = HLFieldAddressLocation.LoadInstanceFieldPointer(pFunction, Instance, Field);
            LLLocation locationTemporary = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationFieldPointer.Type.PointerDepthMinusOne));
            pFunction.CurrentBlock.EmitLoad(locationTemporary, locationFieldPointer);
            return locationTemporary;
        }

        internal override void Store(LLFunction pFunction, LLLocation pSource)
        {
            LLLocation locationFieldPointer = HLFieldAddressLocation.LoadInstanceFieldPointer(pFunction, Instance, Field);
            LLLocation locationSource = pFunction.CurrentBlock.EmitConversion(pSource, locationFieldPointer.Type.PointerDepthMinusOne);
            pFunction.CurrentBlock.EmitStore(locationFieldPointer, locationSource);
        }
    }
}
