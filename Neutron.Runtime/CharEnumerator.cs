using System.Collections;
using System.Collections.Generic;

namespace System
{
    public sealed class CharEnumerator : ICloneable, IEnumerator, IEnumerator<char>
    {
        private string mString;
        private int mIndex;
        private char mCurrent;

        internal CharEnumerator(string pString)
        {
            mString = pString;
            mIndex = -1;
        }

        public object Clone() { return MemberwiseClone(); }

        public void Dispose() { }

        public char Current
        {
            get
            {
                if (mIndex == -1 || mIndex >= mString.Length) throw new InvalidOperationException("The position is not valid.");
                return mCurrent;
            }
        }

        object IEnumerator.Current { get { return Current; } }

        public bool MoveNext()
        {
            if (mIndex < (mString.Length - 1))
            {
                mIndex++;
                mCurrent = mString[mIndex];
                return true;
            }
            mIndex = mString.Length;
            return false;
        }

        public void Reset() { mIndex = -1; }
    }
}
