using DockerComposeBuilder.Model;
using DockerComposeBuilder.Model.Services;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Converters;

internal static class DeployMappingProjection
{
    private static readonly ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>> Members = new();
    private static Dictionary<string, PropertyInfo> Properties(
        Type type
    ) => Members.GetOrAdd(type,
        t => t.GetProperties().Where(p => p.IsDefined(typeof(YamlMemberAttribute)))
            .ToDictionary(p => p.GetCustomAttribute<YamlMemberAttribute>()!.Alias!, p => p));

    internal static Dictionary<string, object?> ToDictionary(DeployMapping model)
    {
        var properties = Properties(model.GetType());
        var entries = new Dictionary<string, object?>();
        foreach (var property in properties)
        {
            var current = property.Value.GetValue(model);
            if (model.Extra.TryGetValue(property.Key, out var preserved) && Equals(current, preserved.Projection))
            {
                entries.Add(property.Key, Restored(current, preserved.Original));
            }
            else if (current != null)
            {
                entries.Add(property.Key, current);
            }
        }

        foreach (var extra in model.Extra)
        {
            if (!properties.ContainsKey(extra.Key))
            {
                entries.Add(extra.Key, extra.Value.Original);
            }
        }

        return entries;
    }

    internal static T FromMap<T>(
        Map map
    ) where T : DeployMapping, new() => (T) FromDictionary(typeof(T), map);

    internal static DeployMapping FromDictionary(Type type, IDictionary source)
    {
        var model = (DeployMapping) Activator.CreateInstance(type)!;
        var properties = Properties(type);
        foreach (DictionaryEntry entry in source)
        {
            var key = (string) entry.Key;
            if (!properties.TryGetValue(key, out var property))
            {
                model.Extra[key] = (entry.Value, null);
                continue;
            }

            var projected = Project(entry.Value, property.PropertyType, out var lossless, out var original);
            property.SetValue(model, projected);
            if (!lossless)
            {
                model.Extra[key] = (original, projected);
            }
        }

        return model;
    }

    // Keep fallbacks only for lossy slots, never for the whole mutable list.
    [Serializable]
    private sealed class ListFallbacks : Dictionary<int, (object? Original, object? Projection)> { }

    private static object? Restored(object? current, object? original)
    {
        if (original is not ListFallbacks slots || current is not IList list) return original;
        return list.Cast<object?>().Select((item, index) =>
            slots.TryGetValue(index, out var slot) && Equals(item, slot.Projection)
                ? Restored(item, slot.Original) : item).ToList();
    }

    private static object? Project(object? value, Type target, out bool lossless, out object? original)
    {
        original = value;
        target = Nullable.GetUnderlyingType(target) ?? target;
        lossless = value != null;
        if (value == null || target.IsInstanceOfType(value))
        {
            return value;
        }

        if (typeof(DeployMapping).IsAssignableFrom(target) && value is IDictionary dict
            && dict.Keys.Cast<object>().All(k => k is string))
        {
            return FromDictionary(target, dict);
        }

        if (target == typeof(PlacementPreference) && value is IDictionary preference
            && preference.Keys.Cast<object>().All(k => k is string))
        {
            var result = new PlacementPreference();
            foreach (DictionaryEntry entry in preference)
            {
                result[(string) entry.Key] = entry.Value!;
            }
            return result;
        }

        if (target.IsGenericType && target.GetGenericTypeDefinition() == typeof(List<>)
            && value is IEnumerable sequence and not string and not IDictionary)
        {
            return ProjectList(sequence, target, out lossless, out original);
        }

        return ProjectScalar(value, target, out lossless);
    }

    private static IList ProjectList(IEnumerable sequence, Type target, out bool lossless, out object? original)
    {
        var list = (IList) Activator.CreateInstance(target)!;
        var slots = new ListFallbacks();
        foreach (var item in sequence)
        {
            var projected = Project(item, target.GetGenericArguments()[0], out var represented, out var raw);
            if (!represented) slots[list.Count] = (raw, projected);
            list.Add(projected);
        }
        lossless = slots.Count == 0;
        original = slots;
        return list;
    }

    private static object? ProjectScalar(object value, Type target, out bool lossless)
    {
        if (target.IsEnum && value is string text)
        {
            foreach (var field in target.GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                var spelling = field.GetCustomAttribute<EnumMemberAttribute>()?.Value;
                if (string.Equals(spelling, text, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(field.Name, text, StringComparison.OrdinalIgnoreCase))
                {
                    lossless = true;
                    return field.GetValue(null);
                }
            }
        }

        // Legacy maps aren't schema constrained: a numeric field may contain a quoted
        // number, interpolation, or even a mapping. Never drop or normalize that value.
        lossless = false;
        try
        {
            var invariant = CultureInfo.InvariantCulture;
            if (target == typeof(int)) return Convert.ToInt32(value, invariant);
            if (target == typeof(double))
            {
                // Convert.ToDouble accepts thousands separators and would read "0,3" as 3.
                return value is string number
                    ? double.Parse(number, NumberStyles.Float, invariant)
                    : Convert.ToDouble(value, invariant);
            }
            if (target == typeof(string)) return Convert.ToString(value, invariant);
        }
        catch (Exception ex) when (ex is FormatException or InvalidCastException or OverflowException)
        {
            // Not representable through the typed API.
        }
        return null;
    }
}
