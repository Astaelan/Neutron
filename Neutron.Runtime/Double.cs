namespace System
{
	public struct Double : IComparable, IComparable<double>, IEquatable<double>
    {
        public const double Epsilon = 4.9406564584124650e-324;
        public const double MaxValue = 1.7976931348623157e308;
        public const double MinValue = -1.7976931348623157e308;
        public const double NaN = 0.0d / 0.0d;
        public const double NegativeInfinity = -1.0d / 0.0d;
        public const double PositiveInfinity = 1.0d / 0.0d;

#pragma warning disable 1718
        public static bool IsNaN(double pValue) { return pValue != pValue; }
#pragma warning restore 1718

        public static bool IsNegativeInfinity(double pValue) { return (pValue < 0.0d && (pValue == NegativeInfinity || pValue == PositiveInfinity)); }

        public static bool IsPositiveInfinity(double pValue) { return (pValue > 0.0d && (pValue == NegativeInfinity || pValue == PositiveInfinity)); }

        public static bool IsInfinity(double pValue) { return (pValue == PositiveInfinity || pValue == NegativeInfinity); }


#pragma warning disable 0649
        private double mValue;
#pragma warning restore 0649

        public int CompareTo(object pObject)
        {
            if (pObject == null) return 1;
            if (!(pObject is double)) throw new ArgumentException();
            return CompareTo((double)pObject);
        }
        public int CompareTo(double pValue)
        {
            if (IsNaN(mValue)) return IsNaN(pValue) ? 0 : -1;
            if (IsNaN(pValue)) return 1;
            return mValue > pValue ? 1 : (mValue < pValue ? -1 : 0);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is double)) return false;
            if (IsNaN((double)obj)) return IsNaN(mValue);
            return ((double)obj).mValue == mValue;
        }
        public bool Equals(double pValue)
        {
            if (IsNaN(mValue)) return IsNaN(pValue);
            return mValue == pValue;
        }

        public unsafe override int GetHashCode()
        {
            if (mValue == 0.0) return 0;
            double value = mValue;
            long hash = *((long*)&value);
            return (int)hash ^ (int)(hash >> 0x20);
        }

        //public override string ToString() { return NumberFormatter.FormatGeneral(new NumberFormatter.NumberStore(mValue)); }
    }
}
