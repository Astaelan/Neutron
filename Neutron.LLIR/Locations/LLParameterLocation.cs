using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR.Locations
{
    public sealed class LLParameterLocation : LLLocation
    {
        public static LLParameterLocation Create(LLParameter pParameter)
        {
            LLParameterLocation location = new LLParameterLocation(pParameter.Type);
            location.mParameter = pParameter;
            return location;
        }

        private LLParameter mParameter = null;

        private LLParameterLocation(LLType pType) : base(pType) { }

        public LLParameter Parameter { get { return mParameter; } }

        public override string ToString() { return mParameter.ToString(); }

        public override LLLocation Load(LLInstructionBlock pBlock) { return this; }
    }
}
