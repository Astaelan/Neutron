using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLStaticFieldLocation : HLLocation
    {
        public static HLStaticFieldLocation Create(HLField pStaticField)
        {
            HLStaticFieldLocation location = new HLStaticFieldLocation(pStaticField.Type);
            location.mStaticField = pStaticField;
            return location;
        }

        private HLStaticFieldLocation(HLType pType) : base(pType) { }

        private HLField mStaticField = null;
        public HLField StaticField { get { return mStaticField; } }

        internal override HLLocation AddressOf() { return HLStaticFieldAddressLocation.Create(mStaticField); }

        public override string ToString() { return string.Format("({0}){1}.{2}", Type, mStaticField.Container, mStaticField); }

        internal override LLLocation Load(LLFunction pFunction)
        {
            HLStaticFieldAddressLocation.CheckStaticConstructorCalled(pFunction, StaticField.Container);
            LLLocation locationFieldPointer = LLGlobalLocation.Create(LLModule.GetGlobal(StaticField.Container.ToString() + "." + StaticField.ToString()));
            LLLocation locationTemporary = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationFieldPointer.Type.PointerDepthMinusOne));
            pFunction.CurrentBlock.EmitLoad(locationTemporary, locationFieldPointer);
            return locationTemporary;
        }

        internal override void Store(LLFunction pFunction, LLLocation pSource)
        {
            HLStaticFieldAddressLocation.CheckStaticConstructorCalled(pFunction, StaticField.Container);
            LLLocation locationFieldPointer = LLGlobalLocation.Create(LLModule.GetGlobal(StaticField.Container.ToString() + "." + StaticField.ToString()));
            LLLocation locationSource = pFunction.CurrentBlock.EmitConversion(pSource, locationFieldPointer.Type.PointerDepthMinusOne);
            pFunction.CurrentBlock.EmitStore(locationFieldPointer, locationSource);
        }
    }
}
