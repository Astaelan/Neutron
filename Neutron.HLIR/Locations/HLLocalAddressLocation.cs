using Microsoft.Cci.MutableCodeModel;
using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLLocalAddressLocation : HLLocation
    {
        public static HLLocalAddressLocation Create(HLLocal pLocal)
        {
            HLLocalAddressLocation location = new HLLocalAddressLocation(HLDomain.GetOrCreateType(MutableModelHelper.GetManagedPointerTypeReference(pLocal.Type.Definition, HLDomain.Host.InternFactory, pLocal.Type.Definition)));
            location.mLocal = pLocal;
            return location;
        }

        private HLLocalAddressLocation(HLType pType) : base(pType) { }

        private HLLocal mLocal = null;
        public HLLocal Local { get { return mLocal; } }

        public override string ToString()
        {
            return string.Format("({0}){1}{2}", Type, mLocal.IsReference ? "" : "&", mLocal);
        }

        internal override LLLocation Load(LLFunction pFunction)
        {
            LLLocation location = LLLocalLocation.Create(pFunction.Locals[Local.Name]);
            if (mLocal.IsReference)
                location = location.Load(pFunction.CurrentBlock);
            return location;
        }
    }
}
