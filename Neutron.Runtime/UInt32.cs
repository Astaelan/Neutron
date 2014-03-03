namespace System
{
	public struct UInt32 : IComparable, IComparable<uint>, IEquatable<uint>
    {
        public const uint MaxValue = 0xffffffff;
        public const uint MinValue = 0;

#pragma warning disable 0649
        private uint mValue;
#pragma warning restore 0649

        public int CompareTo(object pObject)
        {
            if (pObject == null) return 1;
            if (!(pObject is uint)) throw new ArgumentException();
            return CompareTo((uint)pObject);
        }
        public int CompareTo(uint pValue) { return mValue > pValue ? 1 : (mValue < pValue ? -1 : 0); }

        public override bool Equals(object pObject) { return (pObject is uint) && mValue == (uint)pObject; }
        public bool Equals(uint pValue) { return mValue == pValue; }

        public override int GetHashCode() { return (int)mValue; }

        //public override string ToString() { return NumberFormatter.FormatGeneral(new NumberFormatter.NumberStore(mValue)); }
    }
}
