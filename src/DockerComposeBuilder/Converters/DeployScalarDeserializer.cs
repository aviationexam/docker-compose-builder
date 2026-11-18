using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Converters;

// Only infer scalars in deploy maps. Keep the normal mapping, sequence and
// alias deserializers (and the rest of the Compose document) untouched.
internal sealed class DeployScalarDeserializer : INodeDeserializer
{
    private static readonly ConditionalWeakTable<IParser, object> Active = new();
    private static readonly IDeserializer Scalars = new DeserializerBuilder()
        .WithAttemptingUnquotedStringTypeDeserialization().Build();

    internal static IDictionary? ReadMapping(IParser parser, ObjectDeserializer rootDeserializer)
    {
        Active.Add(parser, new object());
        try
        {
            return (IDictionary?) rootDeserializer(typeof(Dictionary<string, object?>));
        }
        finally
        {
            Active.Remove(parser);
        }
    }

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
