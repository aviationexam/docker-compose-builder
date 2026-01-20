using System.Collections.Generic;
using System.Linq;

namespace DockerComposeBuilder.Model.Services;

public class ServiceVolumeCollection : ServiceItemCollection<ServiceVolume>, IList<string>
{
    string IList<string>.this[int index]
    {
        get => Items[index];
        set => Items[index] = value;
    }

    void ICollection<string>.Add(string item) => Items.Add(item);

    bool ICollection<string>.Contains(string item) => Items.Any(v => (string) v == item);

    void ICollection<string>.CopyTo(string[] array, int arrayIndex)
    {
        for (var i = 0; i < Items.Count; i++)
        {
            array[arrayIndex + i] = Items[i];
        }
    }

    int IList<string>.IndexOf(string item) => Items.FindIndex(v => (string) v == item);

    void IList<string>.Insert(int index, string item) => Items.Insert(index, item);

    bool ICollection<string>.Remove(string item)
    {
        var index = Items.FindIndex(v => (string) v == item);
        if (index >= 0)
        {
            Items.RemoveAt(index);
            return true;
        }

        return false;
    }

    IEnumerator<string> IEnumerable<string>.GetEnumerator() => Items.Select(v => (string) v).GetEnumerator();

    public void AddRange(IEnumerable<string> items)
    {
        foreach (var item in items)
        {
            Items.Add(item);
        }
    }
}
