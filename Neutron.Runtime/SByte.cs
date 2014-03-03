namespace System
{
	public struct SByte : IComparable, IComparable<sbyte>, IEquatable<sbyte>
    {
        public const sbyte MinValue = -128;
        public const sbyte MaxValue = 127;

#pragma warning disable 0649
        private sbyte mValue;
#pragma warning restore 0649

        public int CompareTo(object pObject)
        {
            if (pObject == null) return 1;
            if (!(pObject is sbyte)) throw new ArgumentException();
            return CompareTo((sbyte)pObject);
        }
        public int CompareTo(sbyte pValue) { return mValue > pValue ? 1 : (mValue < pValue ? -1 : 0); }

        public override bool Equals(object pObject) { return (pObject is sbyte) && mValue == (sbyte)pObject; }
        public bool Equals(sbyte pValue) { return mValue == pValue; }

        public override int GetHashCode() { return (int)mValue; }

        //public override string ToString() { return NumberFormatter.FormatGeneral(new NumberFormatter.NumberStore(mValue)); }
    }
}
