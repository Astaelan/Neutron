using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLTypeOfLocation : HLLocation
    {
        public static HLTypeOfLocation Create(HLType pTypeOfType)
        {
            HLTypeOfLocation location = new HLTypeOfLocation(HLDomain.SystemType);
            location.mTypeOfType = pTypeOfType;
            return location;
        }

        private HLTypeOfLocation(HLType pType) : base(pType) { }

        private HLType mTypeOfType = null;
        public HLType TypeOfType { get { return mTypeOfType; } }

        public override string ToString() { return string.Format("typeof({0})", mTypeOfType); }
    }
}
