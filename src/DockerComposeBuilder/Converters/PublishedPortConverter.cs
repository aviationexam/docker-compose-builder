using DockerComposeBuilder.Model.Services;
using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Converters;

public class PublishedPortConverter : IYamlTypeConverter
{
    public bool Accepts(Type type) => typeof(PublishedPort).IsAssignableFrom(type);

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer) => throw new NotImplementedException();

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        if (value is PublishedPort publishedPort)
        {
            if (publishedPort is { PortAsInt: { } portAsInt })
            {
                emitter.Emit(new Scalar(portAsInt.ToString()));
            }
            else if (publishedPort is { PortAsString: { } portAsString })
            {
                emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, portAsString, ScalarStyle.DoubleQuoted, true, false));
            }
        }
    }
}
