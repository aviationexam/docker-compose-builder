using DockerComposeBuilder.Builders;
using DockerComposeBuilder.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace DockerComposeBuilder.Tests;

// These tests exercise the legacy map overloads deliberately.
#pragma warning disable CS0618
public class DeployMapCultureTests
{
    [Theory]
    [InlineData("")]
    [InlineData("cs-CZ")]
    [InlineData("de-DE")]
    public void DeployMapEmitsInvariantScalars(string culture)
    {
        using var scope = new CultureScope(culture);
        var yaml = Serialize(deploy => deploy
            .WithUpdateConfig(c => c.WithProperty("parallelism", 2L)
                .WithProperty("delay", 1.25m).WithProperty("monitor", 2.75)
                .WithProperty("max_failure_ratio", "0.3"))
            .WithRestartPolicy(c => c.WithProperty("delay", 3.25)
                .WithProperty("max_attempts", "4").WithProperty("window", 5.75m))
            .WithPlacement(c => c.WithProperty("max_replicas_per_node", 3)
                .WithProperty("constraints", new object[] { 6.25m })
                .WithProperty("preferences", new[] { new Dictionary<string, object> { ["spread"] = 7.75 } }))
            .WithResources(c => c.WithProperty("limits", new Dictionary<string, object>
                {
                    ["cpus"] = 0.5, ["memory"] = 8.25m, ["pids"] = 9m,
                })
                .WithProperty("reservations", new Dictionary<string, string>
                {
                    ["cpus"] = "0.50", ["memory"] = "512M", ["pids"] = "10",
                })));

        Assert.Contains(
            """
                  update_config:
                    parallelism: 2
                    delay: "1.25"
                    monitor: "2.75"
                    max_failure_ratio: 0.3
                  restart_policy:
                    delay: "3.25"
                    max_attempts: 4
                    window: "5.75"
                  placement:
                    constraints:
                    - "6.25"
                    preferences:
                    - spread: "7.75"
                    max_replicas_per_node: 3
                  resources:
                    limits:
                      cpus: 0.5
                      memory: "8.25"
                      pids: 9
                    reservations:
                      cpus: "0.50"
                      memory: "512M"
                      pids: 10
            """, yaml);
    }

    [Theory]
    [InlineData("")]
    [InlineData("cs-CZ")]
    [InlineData("de-DE")]
    public void InvalidNumbersIdentifyTheMapKey(string culture)
    {
        using var scope = new CultureScope(culture);
        var ratio = Assert.Throws<ArgumentException>(() => Serialize(d =>
            d.WithUpdateConfig(c => c.WithProperty("max_failure_ratio", "0,3"))));
        Assert.Contains("max_failure_ratio", ratio.Message);

        var pids = Assert.Throws<ArgumentException>(() => Serialize(d =>
            d.WithResources(c => c.WithProperty("limits", new Dictionary<string, string> { ["pids"] = "many" }))));
        Assert.Contains("pids", pids.Message);
    }

    private static string Serialize(Action<DeployBuilder> configure) => Builder.MakeCompose()
        .WithServices(Builder.MakeService("app").WithImage("app").WithSwarm().WithDeploy(configure).Build())
        .Build().Serialize();

    private sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo _previous = CultureInfo.CurrentCulture;

        public CultureScope(string name)
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(name);
        }

        public void Dispose() => CultureInfo.CurrentCulture = _previous;
    }
}
#pragma warning restore CS0618
