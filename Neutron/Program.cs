using Neutron.HLIR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neutron
{
    internal static class Program
    {
        private static void Main(string[] pArguments)
        {
            HLDomain.Process(pArguments[0]);
        }
    }
}
