using Microsoft.Cci.MutableCodeModel;
using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLParameterAddressLocation : HLLocation
    {
        public static HLParameterAddressLocation Create(HLParameter pParameter)
        {
            HLParameterAddressLocation location = new HLParameterAddressLocation(HLDomain.GetOrCreateType(MutableModelHelper.GetManagedPointerTypeReference(pParameter.Type.Definition, HLDomain.Host.InternFactory, pParameter.Type.Definition)));
            location.mParameter = pParameter;

            pParameter.RequiresAddressing = true;
            return location;
        }

        private HLParameterAddressLocation(HLType pType) : base(pType) { }

        private HLParameter mParameter = null;
        public HLParameter Parameter { get { return mParameter; } }

        public override string ToString() { return string.Format("({0})&{1}", Type, mParameter); }

        internal override LLLocation Load(LLFunction pFunction)
        {
            if (!Parameter.RequiresAddressing) throw new NotSupportedException();
            return LLLocalLocation.Create(Parameter.AddressableLocal);
        }
    }
}
