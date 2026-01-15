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
                emitter.Emit(new MappingStart(AnchorName.Empty, TagName.Empty, false, MappingStyle.Block));

                if (serviceVolume.Type != null)
                {
                    emitter.Emit(new Scalar("type"));
                    emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, serviceVolume.Type, ScalarStyle.DoubleQuoted, true, false));
                }

                if (serviceVolume.Source != null)
                {
                    emitter.Emit(new Scalar("source"));
                    emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, serviceVolume.Source, ScalarStyle.DoubleQuoted, true, false));
                }

                if (serviceVolume.Target != null)
                {
                    emitter.Emit(new Scalar("target"));
                    emitter.Emit(new Scalar(AnchorName.Empty, TagName.Empty, serviceVolume.Target, ScalarStyle.DoubleQuoted, true, false));
                }

                if (serviceVolume.ReadOnly != null)
                {
                    emitter.Emit(new Scalar("read_only"));
                    emitter.Emit(new Scalar(serviceVolume.ReadOnly.Value ? "true" : "false"));
                }

                if (serviceVolume.Bind != null)
                {
                    emitter.Emit(new Scalar("bind"));
                    serializer(serviceVolume.Bind, typeof(ServiceVolumeBind));
                }

                if (serviceVolume.Volume != null)
                {
                    emitter.Emit(new Scalar("volume"));
                    serializer(serviceVolume.Volume, typeof(ServiceVolumeVolume));
                }

                if (serviceVolume.Tmpfs != null)
                {
                    emitter.Emit(new Scalar("tmpfs"));
                    serializer(serviceVolume.Tmpfs, typeof(ServiceVolumeTmpfs));
                }

                emitter.Emit(new MappingEnd());
            }
        }
    }
}
