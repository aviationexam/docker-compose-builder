using DockerComposeBuilder.Extensions;
using System.Collections.Generic;
using Xunit;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Tests;

public class DeployListProjectionTests
{
    [Fact]
    public void InPlaceEditsSurviveAlongsideUnrepresentableListItems()
    {
        const string yaml = """
            services:
              app:
                image: x
                deploy:
                  placement:
                    constraints: [2]
                    preferences: [false, {spread: node.labels.old}]
            """;
        var compose = ComposeExtensions.Deserialize(yaml);
        var placement = compose.Services!["app"].Deploy!.Placement!;
        placement.Constraints!.Add("node.role == worker");
        placement.Preferences![1].Spread = "node.labels.new";

        var reader = new DeserializerBuilder().WithAttemptingUnquotedStringTypeDeserialization().Build();
        var expected = yaml.Replace("[2]", "[2, node.role == worker]").Replace("labels.old", "labels.new");
        Assert.Equal(reader.Deserialize<Dictionary<object, object>>(expected),
            reader.Deserialize<Dictionary<object, object>>(compose.Serialize()));
    }
}
