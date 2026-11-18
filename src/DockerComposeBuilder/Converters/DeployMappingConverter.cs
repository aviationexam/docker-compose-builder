using DockerComposeBuilder.Model.Services;
using System;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Converters;

internal sealed class DeployMappingConverter : IYamlTypeConverter
{
    public bool Accepts(
        Type type
    ) => typeof(DeployMapping).IsAssignableFrom(type);

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        // Read the mapping with the document's own deserializer so anchors resolve
        // even when a typed property and an extension refer to the same node.
        var entries = DeployScalarDeserializer.ReadMapping(parser, rootDeserializer);
        return entries == null ? null : DeployMappingProjection.FromDictionary(type, entries);
    }

    public void WriteYaml(
        IEmitter emitter,
        object? value,
        Type type,
        ObjectSerializer serializer
    ) => serializer(DeployMappingProjection.ToDictionary((DeployMapping) value!));
}
