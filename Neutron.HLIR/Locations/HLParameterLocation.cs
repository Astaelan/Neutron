using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLParameterLocation : HLLocation
    {
        public static HLParameterLocation Create(HLParameter pParameter)
        {
            HLParameterLocation location = new HLParameterLocation(pParameter.Type);
            location.mParameter = pParameter;
            return location;
        }

        private HLParameterLocation(HLType pType) : base(pType) { }

        private HLParameter mParameter = null;
        public HLParameter Parameter { get { return mParameter; } }

        internal override HLLocation AddressOf() { return HLParameterAddressLocation.Create(mParameter); }

        public override string ToString() { return string.Format("({0}){1}", Type, mParameter); }

        internal override LLLocation Load(LLFunction pFunction)
        {
            LLLocation locationTemporary = null;
            if (!Parameter.RequiresAddressing) locationTemporary = LLParameterLocation.Create(pFunction.Parameters[Parameter.Name]);
            else
            {
                LLLocation locationLocal = LLLocalLocation.Create(Parameter.AddressableLocal);
                locationTemporary = LLTemporaryLocation.Create(pFunction.CreateTemporary(locationLocal.Type.PointerDepthMinusOne));
                pFunction.CurrentBlock.EmitLoad(locationTemporary, locationLocal);
            }
            return locationTemporary;
        }

        internal override void Store(LLFunction pFunction, LLLocation pSource)
        {
            if (!Parameter.RequiresAddressing) throw new NotSupportedException();
            LLLocation locationLocal = LLLocalLocation.Create(Parameter.AddressableLocal);
            LLLocation locationSource = pFunction.CurrentBlock.EmitConversion(pSource, locationLocal.Type.PointerDepthMinusOne);
            pFunction.CurrentBlock.EmitStore(locationLocal, locationSource);
        }
    }
}
