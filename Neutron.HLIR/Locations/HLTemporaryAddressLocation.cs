using Microsoft.Cci.MutableCodeModel;
using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLTemporaryAddressLocation : HLLocation
    {
        public static HLTemporaryAddressLocation Create(HLTemporary pTemporary)
        {
            HLTemporaryAddressLocation location = new HLTemporaryAddressLocation(HLDomain.GetOrCreateType(MutableModelHelper.GetManagedPointerTypeReference(pTemporary.Type.Definition, HLDomain.Host.InternFactory, pTemporary.Type.Definition)));
            location.mTemporary = pTemporary;
            return location;
        }

        private HLTemporaryAddressLocation(HLType pType) : base(pType) { }

        private HLTemporary mTemporary = null;
        public HLTemporary Temporary { get { return mTemporary; } }

        public override string ToString() { return string.Format("({0})&{1}", Type, mTemporary); }

        internal override LLLocation Load(LLFunction pFunction)
        {
            return LLLocalLocation.Create(pFunction.Locals[Temporary.Name]);
        }
    }
}
