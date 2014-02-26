using Neutron.LLIR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR
{
    public abstract class HLLiteralLocation : HLLocation
    {
        protected HLLiteralLocation(HLType pType) : base(pType) { }

        public abstract string LiteralAsString { get; }
    }
}
