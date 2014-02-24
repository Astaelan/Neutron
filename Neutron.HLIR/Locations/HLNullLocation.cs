using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLNullLocation : HLLocation
    {
        public static HLNullLocation Create(HLType pType)
        {
            return new HLNullLocation(pType);
        }

        private HLNullLocation(HLType pType) : base(pType) { }

        public override string ToString() { return string.Format("({0})null", Type); }

        internal override LLLocation Load(LLFunction pFunction)
        {
            return LLLiteralLocation.Create(LLLiteral.Create(Type.LLType, "zeroinitializer"));
        }
    }
}
