using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLDefaultLocation : HLLocation
    {
        public static HLDefaultLocation Create(HLType pType)
        {
            return new HLDefaultLocation(pType);
        }

        private HLDefaultLocation(HLType pType) : base(pType) { }

        public override string ToString() { return string.Format("default({0})", Type); }

        internal override LLLocation Load(LLFunction pFunction)
        {
            return LLLiteralLocation.Create(LLLiteral.Create(Type.LLType, "zeroinitializer"));
        }
    }
}
