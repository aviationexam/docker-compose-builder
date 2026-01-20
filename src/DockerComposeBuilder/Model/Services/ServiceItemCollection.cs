using System.Collections;
using System.Collections.Generic;

namespace DockerComposeBuilder.Model.Services;

public abstract class ServiceItemCollection<T> : IList<T>
{
    protected readonly List<T> Items = new();

    public int Count => Items.Count;

    public bool IsReadOnly => false;

    public T this[int index]
    {
        get => Items[index];
        set => Items[index] = value;
    }

    public void Add(T item) => Items.Add(item);

    public bool Contains(T item) => Items.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);

    public int IndexOf(T item) => Items.IndexOf(item);

    public void Insert(int index, T item) => Items.Insert(index, item);

    public void Clear() => Items.Clear();

    public bool Remove(T item) => Items.Remove(item);

    public void RemoveAt(int index) => Items.RemoveAt(index);

    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();

    public void AddRange(IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            Items.Add(item);
        }
    }
}
