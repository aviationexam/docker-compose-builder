using DockerComposeBuilder.Model.Services;
using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Converters;

public class ServiceVolumeConverter : IYamlTypeConverter
{
    public bool Accepts(Type type) => typeof(ServiceVolume).IsAssignableFrom(type);

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        if (parser.Current is Scalar scalar)
        {
            var value = scalar.Value;
            parser.MoveNext();
            return ServiceVolume.FromShortSyntax(value);
        }

        if (parser.Current is MappingStart)
        {
            return rootDeserializer(typeof(ServiceVolume));
        }

        parser.MoveNext();
        return null;
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        if (value is ServiceVolume serviceVolume)
        {
            if (serviceVolume.ShortSyntax != null)
            {
                emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, serviceVolume.ShortSyntax, ScalarStyle.DoubleQuoted, true, false));
            }
            else
            {
                serializer(value, type);
            }
        }
    }
}
