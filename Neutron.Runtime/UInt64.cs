namespace System
{
	public struct UInt64 : IComparable, IComparable<ulong>, IEquatable<ulong>
    {
        public const ulong MinValue = 0;
        public const ulong MaxValue = 0xffffffffffffffffL;

#pragma warning disable 0649
        private ulong mValue;
#pragma warning restore 0649

        public int CompareTo(object pObject)
        {
            if (pObject == null) return 1;
            if (!(pObject is ulong)) throw new ArgumentException();
            return CompareTo((ulong)pObject);
        }
        public int CompareTo(ulong pValue) { return mValue > pValue ? 1 : (mValue < pValue ? -1 : 0); }

        public override bool Equals(object pObject) { return (pObject is ulong) && mValue == (ulong)pObject; }
        public bool Equals(ulong pValue) { return mValue == pValue; }

        public override int GetHashCode() { return (int)(mValue & 0xffffffff) ^ (int)(mValue >> 32); }

        //public override string ToString() { return NumberFormatter.FormatGeneral(new NumberFormatter.NumberStore(mValue)); }
    }
}
