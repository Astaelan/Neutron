using Neutron.LLIR;
using Neutron.LLIR.Locations;
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

        internal override LLLocation Load(LLFunction pFunction)
        {
            LLLocation locationArrayPointer = Instance.Load(pFunction);
            locationArrayPointer = pFunction.CurrentBlock.EmitConversion(locationArrayPointer, LLModule.GetOrCreatePointerType(LLModule.GetOrCreateUnsignedType(8), 1));
            LLLocation locationArraySizePointer = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationArrayPointer.Type));
            pFunction.CurrentBlock.EmitGetElementPointer(locationArraySizePointer, locationArrayPointer, LLLiteralLocation.Create(LLLiteral.Create(LLModule.GetOrCreateSignedType(32), HLDomain.SystemArray.Fields["mLength"].Offset.ToString())));
            locationArraySizePointer = pFunction.CurrentBlock.EmitConversion(locationArraySizePointer, LLModule.GetOrCreatePointerType(LLModule.GetOrCreateSignedType(32), 1));
            LLLocation locationTemporary = LLTemporaryLocation.Create(pFunction.CreateTemporary(LLModule.GetOrCreateSignedType(32)));
            pFunction.CurrentBlock.EmitLoad(locationTemporary, locationArraySizePointer);
            return pFunction.CurrentBlock.EmitConversion(locationTemporary, Type.LLType);
        }
    }
}
