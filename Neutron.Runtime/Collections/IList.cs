namespace System.Collections
{
    public interface IList : ICollection, IEnumerable
    {
        bool IsFixedSize { get; }
        bool IsReadOnly { get; }
        object this[int pIndex] { get; set; }
        int Add(object pValue);
        void Clear();
        bool Contains(object pValue);
        int IndexOf(object pValue);
        void Insert(int pIndex, object pValue);
        void Remove(object pValue);
        void RemoveAt(int pIndex);
    }
}
