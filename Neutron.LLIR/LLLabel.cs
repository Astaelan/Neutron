using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron.LLIR
{
    public sealed class LLLabel
    {
        public static LLLabel Create(int pIdentifier)
        {
            LLLabel label = new LLLabel();
            label.mIdentifier = pIdentifier;
            return label;
        }

        private int mIdentifier = 0;

        private LLLabel() { }

        public int Identifier { get { return mIdentifier; } }

        public override string ToString() { return "L_" + mIdentifier; }
    }
}
