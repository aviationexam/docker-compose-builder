using DockerComposeBuilder.Builders;
using DockerComposeBuilder.Extensions;
using System.Collections.Generic;
using System.IO;
using Xunit;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace DockerComposeBuilder.Tests;

public class ResourceReservationBuilderTests
{
    [Fact]
    public void TypedBuilderEmitsDeviceAndGenericReservations()
    {
        var compose = Builder.MakeCompose()
            .WithServices(Builder.MakeService("app").WithSwarm().WithDeploy(deploy => deploy
                .WithResources(resources => resources.WithReservations(spec => spec
                    .WithDevice(device => device
                        .WithCapabilities("gpu", "nvidia-compute")
                        .WithDriver("nvidia")
                        .WithCount(2)
                        .WithOptions(new Dictionary<string, object?> { ["virtualization"] = false }))
                    .WithDevice(device => device.WithCapabilities("tpu").WithAllDevices())
                    .WithDevice(device => device.WithCapabilities("gpu").WithDeviceIds("GPU-42")
                        .WithOptions("mode=shared"))
                    .WithDiscreteResource("SSD", 3)
                ))
            ).Build())
            .Build();

        var yaml = new YamlStream();
        yaml.Load(new StringReader(compose.Serialize()));
        var reservations = yaml.Documents[0].RootNode["services"]["app"]["deploy"]["resources"]["reservations"];
        var devices = (YamlSequenceNode) reservations["devices"];
        Assert.Equal(3, devices.Children.Count);
        Assert.Equal("gpu", devices[0]["capabilities"][0].ToString());
        Assert.Equal("nvidia-compute", devices[0]["capabilities"][1].ToString());
        Assert.Equal("nvidia", devices[0]["driver"].ToString());
        Assert.Equal("2", devices[0]["count"].ToString());
        Assert.Equal(ScalarStyle.Plain, ((YamlScalarNode) devices[0]["count"]).Style);
        Assert.Equal("false", devices[0]["options"]["virtualization"].ToString());
        Assert.Equal("all", devices[1]["count"].ToString());
        Assert.Equal("GPU-42", devices[2]["device_ids"][0].ToString());
        Assert.Equal("mode=shared", devices[2]["options"][0].ToString());
        var discreteResource = reservations["generic_resources"][0]["discrete_resource_spec"];
        Assert.Equal("SSD", discreteResource["kind"].ToString());
        Assert.Equal("3", discreteResource["value"].ToString());
    }
}
