namespace System
{
	public struct Int32 : IComparable, IComparable<int>, IEquatable<int>
    {
        public const int MaxValue = 0x7fffffff;
        public const int MinValue = -2147483648;

#pragma warning disable 0649
        private int mValue;
#pragma warning restore 0649

        public int CompareTo(object pObject)
        {
            if (pObject == null) return 1;
            if (!(pObject is int)) throw new ArgumentException();
            return CompareTo((int)pObject);
        }
        public int CompareTo(int pValue) { return mValue > pValue ? 1 : (mValue < pValue ? -1 : 0); }

        public override bool Equals(object pObject) { return (pObject is int) && mValue == (int)pObject; }
        public bool Equals(int pValue) { return mValue == pValue; }

        public override int GetHashCode() { return mValue; }

        //public override string ToString() { return NumberFormatter.FormatGeneral(new NumberFormatter.NumberStore(mValue)); }
    }
}
