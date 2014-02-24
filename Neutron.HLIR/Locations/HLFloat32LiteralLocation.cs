﻿using Neutron.LLIR;
using Neutron.LLIR.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR.Locations
{
    public sealed class HLFloat32LiteralLocation : HLLocation
    {
        public static HLFloat32LiteralLocation Create(float pLiteral)
        {
            HLFloat32LiteralLocation location = new HLFloat32LiteralLocation(HLDomain.SystemSingle);
            location.mLiteral = pLiteral;
            return location;
        }

        private HLFloat32LiteralLocation(HLType pType) : base(pType) { }

        private float mLiteral = 0;
        public float Literal { get { return mLiteral; } }

        public override string ToString()
        {
            return string.Format("({0}){1}", Type, mLiteral);
        }

        internal override LLLocation Load(LLFunction pFunction)
        {
            return LLLiteralLocation.Create(LLLiteral.Create(Type.LLType, Literal.ToString()));
        }
    }
}
