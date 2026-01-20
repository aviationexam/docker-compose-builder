using DockerComposeBuilder.Model.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Converters;

public class ServiceConfigCollectionConverter : IYamlTypeConverter
{
    public bool Accepts(Type type) => typeof(ServiceConfigCollection).IsAssignableFrom(type);

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        if (parser.Current is not SequenceStart)
        {
            return null;
        }

        parser.MoveNext();
        var collection = new ServiceConfigCollection();

        while (parser.Current is not SequenceEnd)
        {
            if (parser.Current is Scalar scalar)
            {
                var value = scalar.Value;
                parser.MoveNext();
                collection.Add(ServiceConfig.FromShortSyntax(value));
            }
            else if (parser.Current is MappingStart)
            {
                parser.MoveNext();

                string? source = null;
                string? target = null;
                string? uid = null;
                string? gid = null;
                int? mode = null;

                while (parser.Current is not MappingEnd)
                {
                    if (parser.Current is Scalar keyScalar)
                    {
                        var key = keyScalar.Value;
                        parser.MoveNext();

                        if (parser.Current is Scalar valueScalar)
                        {
                            var value = valueScalar.Value;
                            parser.MoveNext();

                            switch (key)
                            {
                                case "source":
                                    source = value;
                                    break;
                                case "target":
                                    target = value;
                                    break;
                                case "uid":
                                    uid = value;
                                    break;
                                case "gid":
                                    gid = value;
                                    break;
                                case "mode":
                                    mode = ParseMode(value);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        parser.MoveNext();
                    }
                }

                parser.MoveNext(); // consume MappingEnd

                collection.Add(new ServiceConfig
                {
                    Source = source,
                    Target = target,
                    Uid = uid,
                    Gid = gid,
                    Mode = mode
                });
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
        if (value is not ServiceConfigCollection collection)
        {
            return;
        }

        var items = collection.Select<ServiceConfig, object>(item =>
            item.ShortSyntax != null ? item.ShortSyntax : item
        ).ToList();

        serializer(items, typeof(List<object>));
    }

    private static int ParseMode(string value)
    {
        if (value.StartsWith("0") && value.Length > 1)
            return Convert.ToInt32(value, 8);
        return int.Parse(value);
    }
}
