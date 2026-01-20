using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DockerComposeBuilder.Model.Services;

public class ServiceSecretCollection : IList<ServiceSecret>, IList<string>
{
    private readonly List<ServiceSecret> _items = new();

    public ServiceSecret this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }

    string IList<string>.this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }

    public int Count => _items.Count;

    public bool IsReadOnly => false;

    public void Add(ServiceSecret item) => _items.Add(item);

    void ICollection<string>.Add(string item) => _items.Add(item);

    public void Clear() => _items.Clear();

    public bool Contains(ServiceSecret item) => _items.Contains(item);

    bool ICollection<string>.Contains(string item) => _items.Any(s => (string) s == item);

    public void CopyTo(ServiceSecret[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

    void ICollection<string>.CopyTo(string[] array, int arrayIndex)
    {
        for (var i = 0; i < _items.Count; i++)
        {
            array[arrayIndex + i] = _items[i];
        }
    }

    public int IndexOf(ServiceSecret item) => _items.IndexOf(item);

    int IList<string>.IndexOf(string item) => _items.FindIndex(s => (string) s == item);

    public void Insert(int index, ServiceSecret item) => _items.Insert(index, item);

    void IList<string>.Insert(int index, string item) => _items.Insert(index, item);

    public bool Remove(ServiceSecret item) => _items.Remove(item);

    bool ICollection<string>.Remove(string item)
    {
        var index = _items.FindIndex(s => (string) s == item);
        if (index >= 0)
        {
            _items.RemoveAt(index);
            return true;
        }
        return false;
    }

    public void RemoveAt(int index) => _items.RemoveAt(index);

    public IEnumerator<ServiceSecret> GetEnumerator() => _items.GetEnumerator();

    IEnumerator<string> IEnumerable<string>.GetEnumerator() => _items.Select(s => (string) s).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

    public void AddRange(IEnumerable<ServiceSecret> items)
    {
        foreach (var item in items)
        {
            _items.Add(item);
        }
    }

    public void AddRange(IEnumerable<string> items)
    {
        foreach (var item in items)
        {
            _items.Add(item);
        }
    }
}
