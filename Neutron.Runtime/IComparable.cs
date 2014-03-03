namespace System
{
    public interface IComparable
    {
        int CompareTo(object pValue);
    }

    public interface IComparable<T>
    {
        int CompareTo(T pValue);
    }
}
