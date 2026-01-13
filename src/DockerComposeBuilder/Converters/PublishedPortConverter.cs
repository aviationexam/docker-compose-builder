using DockerComposeBuilder.Model.Services;
using System;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Converters;

public class PublishedPortConverter : IYamlTypeConverter
{
    public bool Accepts(Type type) => typeof(PublishedPort).IsAssignableFrom(type);

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        if (parser.Current is YamlDotNet.Core.Events.Scalar scalar)
        {
            var value = scalar.Value;
            parser.MoveNext();
            
            if (int.TryParse(value, out var intValue))
            {
                return (PublishedPort)intValue;
            }
            
            return (PublishedPort)value;
        }
        
        parser.MoveNext();
        return null;
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        if (value is PublishedPort publishedPort)
        {
            if (publishedPort is { PortAsInt: { } portAsInt })
            {
                serializer(portAsInt, typeof(int));
            }
            else if (publishedPort is { PortAsString: { } portAsString })
            {
                serializer(portAsString, typeof(string));
            }
        }
    }
}
