using System.Collections.Generic;
using System.Linq;

namespace DockerComposeBuilder.Model.Services;

public class ServiceSecretCollection : ServiceItemCollection<ServiceSecret>, IList<string>
{
    string IList<string>.this[int index]
    {
        get => Items[index];
        set => Items[index] = value;
    }

    void ICollection<string>.Add(string item) => Items.Add(item);

    bool ICollection<string>.Contains(string item) => Items.Any(s => (string) s == item);

    void ICollection<string>.CopyTo(string[] array, int arrayIndex)
    {
        for (var i = 0; i < Items.Count; i++)
        {
            array[arrayIndex + i] = Items[i];
        }
    }

    int IList<string>.IndexOf(string item) => Items.FindIndex(s => (string) s == item);

    void IList<string>.Insert(int index, string item) => Items.Insert(index, item);

    bool ICollection<string>.Remove(string item)
    {
        var index = Items.FindIndex(s => (string) s == item);
        if (index >= 0)
        {
            Items.RemoveAt(index);
            return true;
        }

        return false;
    }

    IEnumerator<string> IEnumerable<string>.GetEnumerator() => Items.Select(s => (string) s).GetEnumerator();

    public void AddRange(IEnumerable<string> items)
    {
        foreach (var item in items)
        {
            Items.Add(item);
        }
    }
}
