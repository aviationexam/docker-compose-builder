using DockerComposeBuilder.Model.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Converters;

public class ServiceSecretCollectionConverter : IYamlTypeConverter
{
    public bool Accepts(Type type) => typeof(ServiceSecretCollection).IsAssignableFrom(type);

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        if (parser.Current is not SequenceStart)
        {
            return null;
        }

        parser.MoveNext();
        var collection = new ServiceSecretCollection();

        while (parser.Current is not SequenceEnd)
        {
            if (parser.Current is Scalar scalar)
            {
                var value = scalar.Value;
                parser.MoveNext();
                collection.Add(ServiceSecret.FromShortSyntax(value));
            }
            else if (parser.Current is MappingStart)
            {
                var item = rootDeserializer(typeof(ServiceSecret));
                if (item is ServiceSecret secret)
                {
                    collection.Add(secret);
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
        if (value is not ServiceSecretCollection collection)
        {
            return;
        }

        var items = collection.Select<ServiceSecret, object>(item =>
            item.ShortSyntax != null ? item.ShortSyntax : item
        ).ToList();

        serializer(items, typeof(List<object>));
    }
}
