using DockerComposeBuilder.Converters;
using DockerComposeBuilder.Emitters;
using DockerComposeBuilder.Model;
using System;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DockerComposeBuilder.Extensions;

public static class ComposeExtensions
{
    public static SerializerBuilder CreateSerializerBuilder(
        string lineEndings
    ) => new SerializerBuilder()
        .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
        .WithTypeConverter(new YamlValueCollectionConverter())
        .WithTypeConverter(new PublishedPortConverter())
        .WithTypeConverter(new ServiceVolumeCollectionConverter())
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .WithEventEmitter(nextEmitter => new FlowStyleStringSequences(nextEmitter))
        .WithEventEmitter(nextEmitter => new FlowStringEnumConverter(nextEmitter))
        .WithEventEmitter(nextEmitter => new ForceQuotedStringValuesEventEmitter(nextEmitter))
        .WithEmissionPhaseObjectGraphVisitor(args => new YamlIEnumerableSkipEmptyObjectGraphVisitor(args.InnerVisitor))
        .WithNewLine(lineEndings);

    public static string Serialize(this Compose serializable, string lineEndings = "\n")
    {
        var serializer = CreateSerializerBuilder(lineEndings)
            .Build();

        return serializer.Serialize(serializable);
    }

    public static DeserializerBuilder CreateDeserializerBuilder(
        bool ignoreUnmatchedProperties = true
    )
    {
        var builder = new DeserializerBuilder()
            .WithTypeConverter(new PublishedPortConverter())
            .WithTypeConverter(new ServiceVolumeCollectionConverter())
            .WithNamingConvention(UnderscoredNamingConvention.Instance);

        if (ignoreUnmatchedProperties)
        {
            builder.IgnoreUnmatchedProperties();
        }

        return builder;
    }

    public static Compose Deserialize(string yaml, bool ignoreUnmatchedProperties = true)
    {
        var deserializer = CreateDeserializerBuilder(ignoreUnmatchedProperties)
            .Build();

        return deserializer.Deserialize<Compose>(yaml);
    }

    public static bool TryDeserialize(string yaml, out Compose? result, bool ignoreUnmatchedProperties = true)
    {
        try
        {
            result = Deserialize(yaml, ignoreUnmatchedProperties);
            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }
}
