namespace System
{
	public struct Int16 : IComparable, IComparable<short>, IEquatable<short>
    {
        public const short MaxValue = 0x7fff;
        public const short MinValue = -32768;

#pragma warning disable 0649
        private short mValue;
#pragma warning restore 0649

        public int CompareTo(object pObject)
        {
            if (pObject == null) return 1;
            if (!(pObject is short)) throw new ArgumentException();
            return CompareTo((short)pObject);
        }
        public int CompareTo(short pValue) { return mValue > pValue ? 1 : (mValue < pValue ? -1 : 0); }

        public override bool Equals(object pObject) { return (pObject is short) && mValue == (short)pObject; }
        public bool Equals(short pValue) { return mValue == pValue; }

        public override int GetHashCode() { return (int)mValue; }

        //public override string ToString() { return NumberFormatter.FormatGeneral(new NumberFormatter.NumberStore(mValue)); }
    }
}
