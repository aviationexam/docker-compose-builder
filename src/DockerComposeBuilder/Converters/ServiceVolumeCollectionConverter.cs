using DockerComposeBuilder.Model.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Converters;

public class ServiceVolumeCollectionConverter : IYamlTypeConverter
{
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
            if (parser.Current is Scalar scalar)
            {
                var value = scalar.Value;
                parser.MoveNext();
                collection.Add(ServiceVolume.FromShortSyntax(value));
            }
            else if (parser.Current is MappingStart)
            {
                var item = rootDeserializer(typeof(ServiceVolume));
                if (item is ServiceVolume volume)
                {
                    collection.Add(volume);
                }
            }
            else
            {
                parser.MoveNext();
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

        var items = collection.Select<ServiceVolume, object>(item =>
            item.ShortSyntax != null ? item.ShortSyntax : item
        ).ToList();

        serializer(items, typeof(List<object>));
    }
}
