using Microsoft.Cci;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR
{
    public sealed class HLTemporary
    {
        internal HLTemporary() { }

        private string mName = null;
        public string Name { get { return mName; } internal set { mName = value; } }

        //private HLMethod mContainer = null;
        //public HLMethod Container { get { return mContainer; } internal set { mContainer = value; } }

        private HLType mType = null;
        public HLType Type { get { return mType; } internal set { mType = value; } }

        public override string ToString() { return Name; }
    }
}
