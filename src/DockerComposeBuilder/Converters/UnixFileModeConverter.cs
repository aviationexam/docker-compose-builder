using DockerComposeBuilder.Model.Services;
using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Converters;

public class UnixFileModeConverter : IYamlTypeConverter
{
    public bool Accepts(Type type) => type == typeof(UnixFileMode) || type == typeof(UnixFileMode?);

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        if (parser.Current is not Scalar scalar)
        {
            parser.MoveNext();
            return null;
        }

        var value = scalar.Value;
        parser.MoveNext();

        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        return UnixFileMode.Parse(value);
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        if (value is not UnixFileMode mode)
        {
            return;
        }

        emitter.Emit(new Scalar(mode.ToNotationString()));
    }
}
