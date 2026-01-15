using DockerComposeBuilder.Model.Services;
using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Converters;

public class ServiceVolumeCollectionConverter : IYamlTypeConverter
{
    private readonly ServiceVolumeConverter _itemConverter = new();

    public bool Accepts(Type type) => typeof(ServiceVolumeCollection).IsAssignableFrom(type);

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        if (parser.Current is not SequenceStart)
        {
            return null;
        }

        parser.MoveNext();
        var collection = new ServiceVolumeCollection();

        while (parser.Current is not SequenceEnd)
        {
            var item = _itemConverter.ReadYaml(parser, typeof(ServiceVolume), rootDeserializer);
            if (item is ServiceVolume volume)
            {
                collection.Add(volume);
            }
        }

        parser.MoveNext();
        return collection;
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        if (value is not ServiceVolumeCollection collection)
        {
            return;
        }

        emitter.Emit(new SequenceStart(AnchorName.Empty, TagName.Empty, false, SequenceStyle.Block));

        foreach (var item in collection)
        {
            _itemConverter.WriteYaml(emitter, item, typeof(ServiceVolume), serializer);
        }

        emitter.Emit(new SequenceEnd());
    }
}
