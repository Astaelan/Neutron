﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Neutron.Test
{
    internal static class Program
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern static void ConsoleWrite(string str);

        private static int Main()
        {
            //int x = 0;
            //while (x != 42) x++;
            //if (x == 0) x = 42;
            //if (x == 42) return x;
            //else return 0;
            //return x;
            //int x = 0;
            //x = x + 1;
            //return x == 0 ? 0 : 42;

            //object o = new object();
            string s = new string('A', 42);
            //TestX x = new TestX();
            //return x.Test(s.Length);

            ConsoleWrite(s);
            TestX x = new TestX();
            return x.Test(s.Length);
        }

        private struct TestX
        {
            //public int X;
            public int Test(int x)
            {
                //X = x;
                //return X;
                switch (x)
                {
                    case 0: x = 0; break;
                    case 1: x = 1; break;
                    default: x = x + 1; break;

                }
                return x;
            }
        }
    }
}
