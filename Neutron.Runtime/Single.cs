namespace System
{
	public struct Single : IComparable, IComparable<float>, IEquatable<float>
    {
        public const float Epsilon = 1.4e-45f;
        public const float MaxValue = 3.40282346638528859e38f;
        public const float MinValue = -3.40282346638528859e38f;
        public const float NaN = 0.0f / 0.0f;
        public const float PositiveInfinity = 1.0f / 0.0f;
        public const float NegativeInfinity = -1.0f / 0.0f;

#pragma warning disable 1718
        public static bool IsNaN(float pValue) { return pValue != pValue; }
#pragma warning restore 1718

        public static bool IsNegativeInfinity(float pValue) { return (pValue < 0.0f && (pValue == NegativeInfinity || pValue == PositiveInfinity)); }

        public static bool IsPositiveInfinity(float pValue) { return (pValue > 0.0f && (pValue == NegativeInfinity || pValue == PositiveInfinity)); }

        public static bool IsInfinity(float pValue) { return (pValue == PositiveInfinity || pValue == NegativeInfinity); }


#pragma warning disable 0649
        private float mValue;
#pragma warning restore 0649

        public int CompareTo(object pObject)
        {
            if (pObject == null) return 1;
            if (!(pObject is float)) throw new ArgumentException();
            return CompareTo((float)pObject);
        }
        public int CompareTo(float pValue)
        {
            if (IsNaN(mValue)) return IsNaN(pValue) ? 0 : -1;
            if (IsNaN(pValue)) return 1;
            return mValue > pValue ? 1 : (mValue < pValue ? -1 : 0);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is float)) return false;
            if (IsNaN((float)obj)) return IsNaN(mValue);
            return ((float)obj).mValue == mValue;
        }
        public bool Equals(float pValue)
        {
            if (IsNaN(mValue)) return IsNaN(pValue);
            return mValue == pValue;
        }

        public unsafe override int GetHashCode()
        {
            if (mValue == 0.0) return 0;
            float value = mValue;
            return *((int*)&value);
        }

        //public override string ToString() { return NumberFormatter.FormatGeneral(new NumberFormatter.NumberStore(mValue)); }
    }
}
