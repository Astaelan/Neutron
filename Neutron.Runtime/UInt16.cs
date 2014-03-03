namespace System
{
	public struct UInt16 : IComparable, IComparable<ushort>, IEquatable<ushort>
    {
        public const ushort MaxValue = 0xffff;
        public const ushort MinValue = 0;

#pragma warning disable 0649
        private ushort mValue;
#pragma warning restore 0649

        public int CompareTo(object pObject)
        {
            if (pObject == null) return 1;
            if (!(pObject is ushort)) throw new ArgumentException();
            return CompareTo((ushort)pObject);
        }
        public int CompareTo(ushort pValue) { return mValue > pValue ? 1 : (mValue < pValue ? -1 : 0); }

        public override bool Equals(object pObject) { return (pObject is ushort) && mValue == (ushort)pObject; }
        public bool Equals(ushort pValue) { return mValue == pValue; }

        public override int GetHashCode() { return (int)mValue; }

        //public override string ToString() { return NumberFormatter.FormatGeneral(new NumberFormatter.NumberStore(mValue)); }
    }
}
