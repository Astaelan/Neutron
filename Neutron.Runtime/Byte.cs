namespace System
{
	public struct Byte : IComparable, IComparable<byte>, IEquatable<byte>
    {
        public const byte MinValue = 0;
        public const byte MaxValue = 255;

#pragma warning disable 0649
        private byte mValue;
#pragma warning restore 0649

        public int CompareTo(object pObject)
        {
            if (pObject == null) return 1;
            if (!(pObject is byte)) throw new ArgumentException();
            return CompareTo((byte)pObject);
        }
        public int CompareTo(byte pValue) { return mValue > pValue ? 1 : (mValue < pValue ? -1 : 0); }

        public override bool Equals(object pObject) { return (pObject is byte) && mValue == (byte)pObject; }
        public bool Equals(byte pValue) { return mValue == pValue; }

        public override int GetHashCode() { return (int)mValue; }

        //public override string ToString() { return NumberFormatter.FormatGeneral(new NumberFormatter.NumberStore(mValue)); }
    }
}
