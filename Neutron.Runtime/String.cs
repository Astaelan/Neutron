using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System
{
    public class String : ICloneable, IEnumerable, IEnumerable<char>, IComparable, IComparable<string>, IEquatable<string>
    {
        private int mArrayLength;
        private int mStringLength;

        [MethodImpl(MethodImplOptions.InternalCall)]
        public unsafe extern String(char* pValue);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern String(char pChar, int pCount);
    }
}
