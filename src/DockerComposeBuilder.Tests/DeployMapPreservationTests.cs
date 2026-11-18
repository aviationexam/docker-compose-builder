using DockerComposeBuilder.Builders;
using DockerComposeBuilder.Extensions;
using System.Collections.Generic;
using Xunit;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Tests;

public class DeployMapPreservationTests
{
    private const string DeploySections = """
        update_config:
          parallelism: 2
          x-custom: "y"
        restart_policy:
          x-enabled: false
        placement:
          x-weights: [2, 1.5, "2"]
        resources:
          vendor-setting: true
          limits:
            x-burst: 8
          reservations:
            memory: "1g"
            devices:
              - driver: nvidia
                count: 1
                capabilities: [gpu]
            x-profile: {active: true, labels: [fast, "false"]}
        """;

    [Fact]
    public void MapBuilderPreservesDevicesAndUnknownKeysInEverySection()
    {
#pragma warning disable CS0618 // Exercise the legacy map overloads.
        var compose = Builder.MakeCompose()
            .WithServices(Builder.MakeService("s").WithImage("x")
                .WithSwarm().WithDeploy(deploy => deploy
                    .WithUpdateConfig(c => c.WithProperty("parallelism", 2).WithProperty("x-custom", "y"))
                    .WithRestartPolicy(c => c.WithProperty("x-enabled", false))
                    .WithPlacement(c => c.WithProperty("x-weights", new object[] { 2, 1.5, "2" }))
                    .WithResources(c => c.WithProperty("vendor-setting", true)
                        .WithMap("limits", l => l.WithProperty("x-burst", 8))
                        .WithProperty("reservations", new Dictionary<string, object>
                        {
                            ["memory"] = "1g",
                            ["devices"] = new[]
                            {
                                new Dictionary<string, object>
                                {
                                    ["driver"] = "nvidia",
                                    ["count"] = 1,
                                    ["capabilities"] = new[] { "gpu" },
                                },
                            },
                            ["x-profile"] = new Dictionary<string, object>
                            {
                                ["active"] = true,
                                ["labels"] = new[] { "fast", "false" },
                            },
                        })))
                .Build())
            .Build();
#pragma warning restore CS0618

        AssertDeploySections(DeploySections, compose.Serialize());
    }

    [Theory]
    [InlineData(DeploySections)]
    [InlineData("""
        resources:
          reservations:
            devices:
              - driver: nvidia
                count: 1
                capabilities: [gpu]
        """)]
    public void DeserializeSerializePreservesDeploySections(string deployYaml)
    {
        var yaml = "services:\n  s:\n    image: x\n    deploy:\n      " + deployYaml.Replace("\n", "\n      ");

        AssertDeploySections(deployYaml, ComposeExtensions.Deserialize(yaml).Serialize());
    }

    private static void AssertDeploySections(string expectedDeploy, string actualCompose)
    {
        // Compare YAML values independently of the typed model, including 2 vs "2" and false vs "false".
        var reader = new DeserializerBuilder().WithAttemptingUnquotedStringTypeDeserialization().Build();
        var compose = reader.Deserialize<Dictionary<object, object>>(actualCompose);
        var services = Assert.IsType<Dictionary<object, object>>(compose["services"]);
        var service = Assert.IsType<Dictionary<object, object>>(services["s"]);
        var deploy = Assert.IsType<Dictionary<object, object>>(service["deploy"]);
        Assert.Equal(reader.Deserialize<Dictionary<object, object>>(expectedDeploy), deploy);
    }
}
