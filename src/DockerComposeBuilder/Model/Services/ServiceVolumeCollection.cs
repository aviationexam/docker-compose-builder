using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DockerComposeBuilder.Model.Services;

public class ServiceVolumeCollection : IList<ServiceVolume>, IList<string>
{
    private readonly List<ServiceVolume> _items = new();

    public ServiceVolume this[int index]
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

    public void Add(ServiceVolume item) => _items.Add(item);

    void ICollection<string>.Add(string item) => _items.Add(item);

    public void Clear() => _items.Clear();

    public bool Contains(ServiceVolume item) => _items.Contains(item);

    bool ICollection<string>.Contains(string item) => _items.Any(v => (string) v == item);

    public void CopyTo(ServiceVolume[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

    void ICollection<string>.CopyTo(string[] array, int arrayIndex)
    {
        for (var i = 0; i < _items.Count; i++)
        {
            array[arrayIndex + i] = _items[i];
        }
    }

    public int IndexOf(ServiceVolume item) => _items.IndexOf(item);

    int IList<string>.IndexOf(string item) => _items.FindIndex(v => (string) v == item);

    public void Insert(int index, ServiceVolume item) => _items.Insert(index, item);

    void IList<string>.Insert(int index, string item) => _items.Insert(index, item);

    public bool Remove(ServiceVolume item) => _items.Remove(item);

    bool ICollection<string>.Remove(string item)
    {
        var index = _items.FindIndex(v => (string) v == item);
        if (index >= 0)
        {
            _items.RemoveAt(index);
            return true;
        }
        return false;
    }

    public void RemoveAt(int index) => _items.RemoveAt(index);

    public IEnumerator<ServiceVolume> GetEnumerator() => _items.GetEnumerator();

    IEnumerator<string> IEnumerable<string>.GetEnumerator() => _items.Select(v => (string) v).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

    public void AddRange(IEnumerable<ServiceVolume> items)
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
