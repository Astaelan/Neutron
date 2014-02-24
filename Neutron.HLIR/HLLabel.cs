using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.HLIR
{
    public sealed class HLLabel
    {
        public static HLLabel Create(int pIdentifier)
        {
            HLLabel label = new HLLabel();
            label.mIdentifier = pIdentifier;
            return label;
        }

        private int mIdentifier = 0;

        private HLLabel() { }

        public int Identifier { get { return mIdentifier; } }

        public override string ToString() { return string.Format("L_{0}", mIdentifier); }
    }
}
