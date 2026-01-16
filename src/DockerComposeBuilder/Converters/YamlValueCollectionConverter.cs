using DockerComposeBuilder.Model.Infrastructure;
using System;
using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Converters;

public class YamlValueCollectionConverter : IYamlTypeConverter
{
    public bool Accepts(Type type) => typeof(IValueCollection).IsAssignableFrom(type);

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer) => throw new NotImplementedException();

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        if (value is IValueCollection valueCollection)
        {
            var items = new List<string>();
            foreach (var item in valueCollection)
            {
                var stringValue = item switch
                {
                    IKeyValue keyValue => $"{keyValue.Key}={keyValue.Value}",
                    IKey key => key.Key,
                    _ => null,
                };

                if (stringValue != null)
                {
                    items.Add(stringValue);
                }
            }

            serializer(items, typeof(List<string>));
        }
    }
}
