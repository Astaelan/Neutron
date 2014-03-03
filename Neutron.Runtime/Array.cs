using System.Collections;
using System.Runtime.CompilerServices;

namespace System
{
    public abstract class Array : ICloneable, ICollection, IEnumerable, IList
    {
        private sealed class SZArrayEnumerator : ICloneable, IEnumerator
        {
            private Array mArray;
            private int mEndIndex;
            private int mIndex;

            internal SZArrayEnumerator(Array pArray)
            {
                mArray = pArray;
                mEndIndex = pArray.mLength;
                mIndex = -1;
            }

            public object Clone() { return MemberwiseClone(); }

            public object Current
            {
                get
                {
                    if (mIndex < 0) throw new InvalidOperationException();
                    if (mIndex >= mEndIndex) throw new InvalidOperationException();
                    return mArray.GetValue(mIndex);
                }
            }

            public bool MoveNext()
            {
                if (mIndex < mEndIndex) mIndex++;
                return mIndex < mEndIndex;
            }

            public void Reset() { mIndex = -1; }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public static extern void Clear(Array pArray, int pStartIndex, int pLength);

        public static void Copy(Array pSourceArray, int pSourceStartIndex, Array pDestinationArray, int pDestinationStartIndex, int pLength)
        {
            if (pSourceArray == null) throw new ArgumentNullException("pSourceArray");
            if (pDestinationArray == null) throw new ArgumentNullException("pDestinationArray");
            if (pSourceStartIndex < 0 || pDestinationStartIndex < 0 || pLength < 0) throw new ArgumentOutOfRangeException();
            if ((pSourceStartIndex + pLength) > pSourceArray.mLength || (pDestinationStartIndex + pLength) > pDestinationArray.mLength) throw new ArgumentException();

            int startIndex = 0;
            int endIndex = pLength;
            int increment = 1;
            if (pDestinationStartIndex < pSourceStartIndex)
            {
                startIndex = pLength - 1;
                endIndex = -1;
                increment = -1;
            }
            for (int index = startIndex; index != endIndex; index += increment)
            {
                object item = pSourceArray.GetValue(pSourceStartIndex + index);
                pDestinationArray.SetValue(item, pDestinationStartIndex + index);
            }
        }

        public static void Copy(Array pSourceArray, Array pDestinationArray, int pLength) { Copy(pSourceArray, 0, pDestinationArray, 0, pLength); }

        public static int IndexOf(Array pArray, object pValue) { return IndexOf(pArray, pValue, 0, pArray.mLength); }

        public static int IndexOf(Array pArray, object pValue, int pStartIndex) { return IndexOf(pArray, pValue, pStartIndex, pArray.mLength - pStartIndex); }

        public static int IndexOf(Array pArray, object pValue, int pStartIndex, int pLength)
        {
            if (pArray == null) throw new ArgumentNullException("pArray");
            int length = pStartIndex + pLength;
            if (pStartIndex < 0 || length > pArray.mLength) throw new ArgumentOutOfRangeException();
            for (int i = pStartIndex; i < length; i++)
            {
                if (object.Equals(pValue, pArray.GetValue(i))) return i;
            }
            return -1;
        }

#pragma warning disable 0649
        private int mLength;
#pragma warning restore 0649

        private Array() { }

        public int Length { get { return mLength; } }

        public object Clone() { return MemberwiseClone(); }

        int ICollection.Count { get { return mLength; } }

        public bool IsSynchronized { get { return false; } }

        public object SyncRoot { get { return this; } }

        public void CopyTo(Array pArray, int pStartIndex) { Copy(this, 0, pArray, pStartIndex, mLength); }

        int IList.Add(object pValue) { throw new NotSupportedException("Collection is read-only"); }

        void IList.Clear() { Array.Clear(this, 0, mLength); }

        bool IList.Contains(object pValue) { return (IndexOf(this, pValue) >= 0); }

        public IEnumerator GetEnumerator() { return new SZArrayEnumerator(this); }

        int IList.IndexOf(object pValue) { return IndexOf(this, pValue); }

        void IList.Insert(int pIndex, object pValue) { throw new NotSupportedException("Collection is read-only"); }

        public bool IsFixedSize { get { return true; } }

        public bool IsReadOnly { get { return false; } }

        void IList.Remove(object pValue) { throw new NotSupportedException("Collection is read-only"); }

        void IList.RemoveAt(int pIndex) { throw new NotSupportedException("Collection is read-only"); }

        object IList.this[int pIndex]
        {
            get
            {
                if (pIndex < 0 || pIndex >= mLength) throw new ArgumentOutOfRangeException("index");
                return GetValue(pIndex);
            }
            set
            {
                if (pIndex < 0 || pIndex >= mLength) throw new ArgumentOutOfRangeException("index");
                SetValue(value, pIndex);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe void InternalGetValueReference(TypedReference* pElementReference, int pRank, int* pIndexes);

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe void InternalSetValueReference(TypedReference* pElementReference, object pValue);

        public unsafe object GetValue(int pIndex)
        {
            TypedReference reference = new TypedReference();
            InternalGetValueReference(&reference, 1, &pIndex);
            return TypedReference.InternalToObject(&reference);
        }

        public unsafe void SetValue(object pValue, int pIndex)
        {
            TypedReference reference = new TypedReference();
            InternalGetValueReference(&reference, 1, &pIndex);
            InternalSetValueReference(&reference, pValue);
        }
    }
}
