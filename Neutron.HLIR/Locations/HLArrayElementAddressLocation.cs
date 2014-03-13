using Microsoft.Cci.MutableCodeModel;
using Neutron.LLIR;
using Neutron.LLIR.Locations;
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

        internal static LLLocation LoadArrayElementPointer(LLFunction pFunction, HLLocation pInstance, HLLocation pIndex, HLType pElementType)
        {
            LLLocation locationIndex = pIndex.Load(pFunction);
            LLLocation locationElementOffset = locationIndex;
            long? literalElementOffset = null;
            if (locationIndex is LLLiteralLocation)
            {
                literalElementOffset = Convert.ToInt64(((LLLiteralLocation)locationIndex).Literal.Value) * pElementType.VariableSize;
            }
            else if (pElementType.VariableSize > 1)
            {
                locationElementOffset = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationIndex.Type));
                pFunction.CurrentBlock.EmitMultiply(locationElementOffset, locationIndex, LLLiteralLocation.Create(LLLiteral.Create(locationElementOffset.Type, pElementType.VariableSize.ToString())));
            }

            LLLocation locationArrayPointer = pInstance.Load(pFunction);
            locationArrayPointer = pFunction.CurrentBlock.EmitConversion(locationArrayPointer, LLModule.GetOrCreatePointerType(LLModule.GetOrCreateUnsignedType(8), 1));

            LLLocation locationElementPointer = null;
            if (literalElementOffset.HasValue)
            {
                locationElementOffset = LLLiteralLocation.Create(LLLiteral.Create(LLModule.GetOrCreateSignedType(64), (literalElementOffset.Value + HLDomain.SystemArray.CalculatedSize).ToString()));
                locationElementPointer = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationArrayPointer.Type));
                pFunction.CurrentBlock.EmitGetElementPointer(locationElementPointer, locationArrayPointer, locationElementOffset);
            }
            else
            {
                LLLocation locationTemporary = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationArrayPointer.Type));
                pFunction.CurrentBlock.EmitGetElementPointer(locationTemporary, locationArrayPointer, LLLiteralLocation.Create(LLLiteral.Create(LLModule.GetOrCreateSignedType(32), (HLDomain.SystemArray.CalculatedSize).ToString())));
                locationElementPointer = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationArrayPointer.Type));
                pFunction.CurrentBlock.EmitGetElementPointer(locationElementPointer, locationTemporary, locationElementOffset);
            }
            return pFunction.CurrentBlock.EmitConversion(locationElementPointer, pElementType.LLType.PointerDepthPlusOne);
        }

        internal override LLLocation Load(LLFunction pFunction)
        {
            return LoadArrayElementPointer(pFunction, Instance, Index, ElementType);
        }
    }
}
