using Microsoft.Cci.MutableCodeModel;
using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLFieldAddressLocation : HLLocation
    {
        public static HLFieldAddressLocation Create(HLLocation pInstance, HLField pField)
        {
            HLFieldAddressLocation location = new HLFieldAddressLocation(HLDomain.GetOrCreateType(MutableModelHelper.GetManagedPointerTypeReference(pField.Type.Definition, HLDomain.Host.InternFactory, pField.Type.Definition)));
            location.mInstance = pInstance;
            location.mField = pField;
            return location;
        }

        private HLFieldAddressLocation(HLType pType) : base(pType) { }

        private HLLocation mInstance = null;
        public HLLocation Instance { get { return mInstance; } }

        private HLField mField = null;
        public HLField Field { get { return mField; } }

        public override string ToString() { return string.Format("({0})&({1}).{2}", Type, mInstance, mField); }

        internal static LLLocation LoadInstanceFieldPointer(LLFunction pFunction, HLLocation pInstance, HLField pField)
        {
            LLLocation locationInstance = pInstance.Load(pFunction);
            LLLocation locationFieldPointer = pFunction.CurrentBlock.EmitConversion(locationInstance, LLModule.GetOrCreatePointerType(LLModule.GetOrCreateUnsignedType(8), 1));
            if (pField.Offset > 0)
            {
                locationFieldPointer = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationFieldPointer.Type));
                pFunction.CurrentBlock.EmitGetElementPointer(locationFieldPointer, locationInstance, LLLiteralLocation.Create(LLLiteral.Create(LLModule.GetOrCreateUnsignedType(32), pField.Offset.ToString())));
            }
            return pFunction.CurrentBlock.EmitConversion(locationFieldPointer, pField.Type.LLType.PointerDepthPlusOne);
        }

        internal override LLLocation Load(LLFunction pFunction)
        {
            //LLLocation locationFieldPointer = LoadInstanceFieldPointer(pFunction, mInstance, mField);
            //LLLocation locationTemporary = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationFieldPointer.Type.PointerDepthMinusOne));
            //pFunction.CurrentBlock.EmitLoad(locationTemporary, locationFieldPointer);
            //return locationTemporary;
            return LoadInstanceFieldPointer(pFunction, Instance, Field);
        }
    }
}
