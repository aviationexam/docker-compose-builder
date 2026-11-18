using DockerComposeBuilder.Model;
using DockerComposeBuilder.Model.Services;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Converters;

internal sealed class DeployMappingConverter : IYamlTypeConverter
{
    private static readonly ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>> Members = new();
    private static Dictionary<string, PropertyInfo> Properties(Type type) => Members.GetOrAdd(type,
        t => t.GetProperties().Where(p => p.IsDefined(typeof(YamlMemberAttribute)))
            .ToDictionary(p => p.GetCustomAttribute<YamlMemberAttribute>()!.Alias!, p => p));

    public bool Accepts(Type type) => typeof(DeployMapping).IsAssignableFrom(type);

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        // Read the mapping with the document's own deserializer so anchors resolve
        // even when a typed property and an extension refer to the same node.
        DeployScalarDeserializer.Active.Add(parser, this);
        try
        {
            var entries = (IDictionary?) rootDeserializer(typeof(Dictionary<string, object?>));
            return entries == null ? null : FromDictionary(type, entries);
        }
        finally
        {
            DeployScalarDeserializer.Active.Remove(parser);
        }
    }

    // Only infer scalars in deploy maps. Keep the normal mapping, sequence and
    // alias deserializers (and the rest of the Compose document) untouched.
    internal sealed class DeployScalarDeserializer : INodeDeserializer
    {
        internal static readonly ConditionalWeakTable<IParser, DeployMappingConverter> Active = new();
        private static readonly IDeserializer Scalars = new DeserializerBuilder()
            .WithAttemptingUnquotedStringTypeDeserialization().Build();

        public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object?> nestedObjectDeserializer,
            out object? value, ObjectDeserializer rootDeserializer)
        {
            if (expectedType == typeof(object) && reader.Current is Scalar && Active.TryGetValue(reader, out _))
            {
                value = Scalars.Deserialize<object>(reader);
                return true;
            }

            value = null;
            return false;
        }
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        var model = (DeployMapping) value!;
        var entries = new Dictionary<string, object?>();
        foreach (var property in Properties(model.GetType()))
        {
            var current = property.Value.GetValue(model);
            if (model.Extra.TryGetValue(property.Key, out var preserved) && Equals(current, preserved.Projection))
            {
                entries.Add(property.Key, preserved.Original);
            }
            else if (current != null)
            {
                entries.Add(property.Key, current);
            }
        }

        foreach (var extra in model.Extra)
        {
            if (!Properties(model.GetType()).ContainsKey(extra.Key))
            {
                entries.Add(extra.Key, extra.Value.Original);
            }
        }

        serializer(entries);
    }

    internal static T FromMap<T>(Map map) where T : DeployMapping, new() => (T) FromDictionary(typeof(T), map);

    private static DeployMapping FromDictionary(Type type, IDictionary source)
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

            var projected = Project(entry.Value, property.PropertyType, out var lossless);
            property.SetValue(model, projected);
            if (!lossless)
            {
                model.Extra[key] = (entry.Value, projected);
            }
        }

        return model;
    }

    private static object? Project(object? value, Type target, out bool lossless)
    {
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
            var list = (IList) Activator.CreateInstance(target)!;
            foreach (var item in sequence)
            {
                list.Add(Project(item, target.GetGenericArguments()[0], out var represented));
                lossless &= represented;
            }
            return list;
        }

        if (target.IsEnum && value is string text)
        {
            foreach (var field in target.GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                var spelling = field.GetCustomAttribute<EnumMemberAttribute>()?.Value;
                if (spelling == text || string.Equals(field.Name, text, StringComparison.OrdinalIgnoreCase))
                {
                    lossless = spelling == text;
                    return field.GetValue(null);
                }
            }
        }

        // Legacy maps aren't schema constrained: a numeric field may contain a quoted
        // number, interpolation, or even a mapping. Never drop or normalize that value.
        lossless = false;
        try
        {
            if (target == typeof(int)) return Convert.ToInt32(value);
            if (target == typeof(double)) return Convert.ToDouble(value);
            if (target == typeof(string)) return value.ToString();
        }
        catch (Exception ex) when (ex is FormatException or InvalidCastException or OverflowException)
        {
            // Not representable through the typed API.
        }
        return null;
    }
}
