namespace System
{
	public struct Int64 : IComparable, IComparable<long>, IEquatable<long>
    {
        public const long MaxValue = 0x7fffffffffffffff;
        public const long MinValue = unchecked((long)0x8000000000000000);

#pragma warning disable 0649
        private long mValue;
#pragma warning restore 0649

        public int CompareTo(object pObject)
        {
            if (pObject == null) return 1;
            if (!(pObject is long)) throw new ArgumentException();
            return CompareTo((long)pObject);
        }
        public int CompareTo(long pValue) { return mValue > pValue ? 1 : (mValue < pValue ? -1 : 0); }

        public override bool Equals(object pObject) { return (pObject is long) && mValue == (long)pObject; }
        public bool Equals(long pValue) { return mValue == pValue; }

        public override int GetHashCode() { return (int)(mValue & 0xffffffff) ^ (int)(mValue >> 32); }

        //public override string ToString() { return NumberFormatter.FormatGeneral(new NumberFormatter.NumberStore(mValue)); }
    }
}
