using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System
{
    public class String : ICloneable, IComparable, IComparable<string>, IEnumerable, IEnumerable<char>, IEquatable<string>
    {
        public static readonly string Empty = "";

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static unsafe extern string InternalAllocate(int pLength);

        public static bool IsNullOrEmpty(string pValue) { return (pValue == null) || (pValue.mStringLength == 0); }

        #region Compare
        private static unsafe int InternalCompareOrdinal(string pStringA, string pStringB)
        {
            if (pStringA == null)
            {
                if (pStringB == null) return 0;
                return -1;
            }
            if (pStringB == null) return 1;
            int length = pStringA.mStringLength <= pStringB.mStringLength ? pStringA.mStringLength : pStringB.mStringLength;
            fixed (char* aChars = &pStringA.mFirstChar)
            {
                fixed (char* bChars = &pStringB.mFirstChar)
                {
                    for (int index = 0; index < length; ++index)
                    {
                        if (aChars[index] != bChars[index]) return (aChars[index] - bChars[index]);
                    }
                }
            }
            return pStringA.mStringLength - pStringB.mStringLength;
        }

        public static int Compare(string pStringA, string pStringB)
        {
            return CompareOrdinal(pStringA, pStringB);
        }

        public static int Compare(string pStringA, int pIndexA, string pStringB, int pIndexB, int pLength)
        {
            return CompareOrdinal(pStringA.Substring(pIndexA, pLength), pStringB.Substring(pIndexB, pLength));
        }

        public static int CompareOrdinal(string pStringA, string pStringB)
        {
            return InternalCompareOrdinal(pStringA, pStringB);
        }
        #endregion

        #region Concat
        private static unsafe string InternalConcat(string pStringA, string pStringB)
        {
            int aLength = pStringA.mStringLength;
            int bLength = pStringB.mStringLength;
            string str = InternalAllocate(aLength + bLength);
            fixed (char* strChars = &str.mFirstChar)
            {
                fixed (char* aChars = &pStringA.mFirstChar)
                {
                    for (int index = 0; index < aLength; ++index)
                        strChars[index] = aChars[index];
                }
                fixed (char* bChars = &pStringB.mFirstChar)
                {
                    for (int index = 0; index < bLength; ++index)
                        strChars[aLength + index] = bChars[index];
                }
            }
            return str;
        }

        private static string InternalConcatWithChecks(string pStringA, string pStringB)
        {
            if (IsNullOrEmpty(pStringA))
            {
                if (IsNullOrEmpty(pStringB)) return Empty;
                return pStringB;
            }
            if (IsNullOrEmpty(pStringB)) return pStringA;
            return InternalConcat(pStringA, pStringB);
        }

        public static string Concat(string pStringA, string pStringB)
        {
            return InternalConcatWithChecks(pStringA, pStringB);
        }

        public static string Concat(string pStringA, string pStringB, string pStringC)
        {
            return Concat(Concat(pStringA, pStringB), pStringC);
        }

        public static string Concat(string pStringA, string pStringB, string pStringC, string pStringD)
        {
            return Concat(Concat(pStringA, pStringB), Concat(pStringC, pStringD));
        }

        public static unsafe string Concat(params string[] pStrings)
        {
            int totalLength = 0;
            int count = pStrings.Length;
            for (int index = 0; index < count; ++index)
            {
                if (!IsNullOrEmpty(pStrings[index]))
                    totalLength += pStrings[index].mStringLength;
            }
            string str = InternalAllocate(totalLength);
            fixed (char* strChars = &str.mFirstChar)
            {
                int offset = 0;
                for (int index = 0; index < count; ++index)
                {
                    int tempLength = pStrings[index].mStringLength;
                    fixed (char* tempChars = &pStrings[index].mFirstChar)
                    {
                        for (int tempIndex = 0; tempIndex < tempLength; ++tempIndex)
                            strChars[offset + tempIndex] = tempChars[tempIndex];
                    }
                    offset += tempLength;
                }
            }
            return str;
        }
        #endregion


        private int mArrayLength;
        private int mStringLength;
        private char mFirstChar;

        [MethodImpl(MethodImplOptions.InternalCall)]
        public unsafe extern String(char* pValue);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public unsafe extern String(char* pValue, int pIndex, int pLength);
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern String(char pChar, int pCount);

        public int Length { get { return mStringLength; } }

        public override bool Equals(object pObject) { return (pObject is string) && Equals((string)pObject); }

        public override unsafe int GetHashCode()
        {
            int hash = 0x15051505;
            int temp = hash;
            fixed (char* ptr = &mFirstChar)
            {
                int* iterator = (int*)ptr;
                for (int index = mStringLength; index > 0; index -= 4, iterator += 2)
                {
                    hash = (((hash << 5) + hash) + (hash >> 0x1B)) ^ iterator[0];
                    if (index <= 2) break;
                    temp = (((temp << 5) + temp) + (temp >> 0x1B)) ^ iterator[1];
                }
            }
            return hash + (temp * 0x5D588B65);
        }

        [IndexerName("Chars")]
        public unsafe char this[int pIndex]
        {
            get
            {
                if (pIndex < 0 || pIndex >= mStringLength) throw new ArgumentOutOfRangeException("pIndex");
                fixed (char* ptr = &mFirstChar)
                {
                    return ptr[pIndex];
                }
            }
        }

        public object Clone() { return this; }

        #region IComparable
        public int CompareTo(object pObject)
        {
            if (pObject == null) return 1;
            if (!(pObject is string)) throw new ArgumentException();
            return CompareTo((string)pObject);
        }
        public int CompareTo(string pValue)
        {
            if (pValue == null) return 1;
            return Compare(this, pValue);
        }
        #endregion

        #region IEnumerable
        public IEnumerator GetEnumerator() { return new CharEnumerator(this); }
        IEnumerator<char> IEnumerable<char>.GetEnumerator() { return new CharEnumerator(this); }
        #endregion

        #region IEquatable
        public bool Equals(string pValue)
        {
            if (pValue == null) return false;
            return CompareOrdinal(this, pValue) == 0;
        }
        #endregion

        #region Substring
        private unsafe string InternalSubstring(int pStartIndex, int pLength, bool pAlwaysCopy)
        {
            if (pStartIndex == 0 && pLength == mStringLength && !pAlwaysCopy) return this;
            string str = InternalAllocate(pLength);
            fixed (char* thisChars = &mFirstChar)
            {
                fixed (char* strChars = &str.mFirstChar)
                {
                    for (int index = 0; index < pLength; ++index)
                        strChars[index] = thisChars[pStartIndex + index];
                }
            }
            return str;
        }

        private string InternalSubstringWithChecks(int pStartIndex, int pLength, bool pAlwaysCopy)
        {
            if (pStartIndex < 0) throw new ArgumentOutOfRangeException("pStartIndex");
            if (pStartIndex > mStringLength) throw new ArgumentOutOfRangeException("pStartIndex");
            if (pLength < 0) throw new ArgumentOutOfRangeException("pLength");
            if (pStartIndex > (mStringLength - pLength)) throw new ArgumentOutOfRangeException("pLength");
            if (pLength == 0) return Empty;
            return InternalSubstring(pStartIndex, pLength, pAlwaysCopy);
        }

        public string Substring(int pStartIndex, int pLength) { return InternalSubstringWithChecks(pStartIndex, pLength, false); }

        public string Substring(int pStartIndex) { return Substring(pStartIndex, mStringLength - pStartIndex); }

        #endregion
    }
}
