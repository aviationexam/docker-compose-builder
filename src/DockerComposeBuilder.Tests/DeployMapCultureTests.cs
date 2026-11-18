using DockerComposeBuilder.Builders;
using DockerComposeBuilder.Extensions;
using DockerComposeBuilder.Model;
using DockerComposeBuilder.Model.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace DockerComposeBuilder.Tests;

// These tests exercise the legacy map overloads deliberately.
#pragma warning disable CS0618
public class DeployMapCultureTests
{
    public static TheoryData<string> Cultures => ["", "cs-CZ", "de-DE"];

    [Theory]
    [MemberData(nameof(Cultures))]
    public void NumericCpusStaysAnUnquotedNumber(string culture)
    {
        using var scope = new CultureScope(culture);
        var compose = Build(d => d.WithResources(c => c.WithProperty("limits",
            new Dictionary<string, object> { ["cpus"] = 0.5 })));

        Assert.Contains("cpus: 0.5\n", compose.Serialize());
        Assert.Equal("0.5", DeployOf(compose).Resources!.Limits!.Cpus);
    }

    [Theory]
    [MemberData(nameof(Cultures))]
    public void QuotedRatioStaysAQuotedString(string culture)
    {
        using var scope = new CultureScope(culture);
        var compose = Build(d => d.WithUpdateConfig(c => c.WithProperty("max_failure_ratio", "0.3")));

        Assert.Contains("max_failure_ratio: \"0.3\"\n", compose.Serialize());
        Assert.Equal(0.3, DeployOf(compose).UpdateConfig!.MaxFailureRatio);
    }

    [Theory]
    [MemberData(nameof(Cultures))]
    public void EveryNumericTypeKeepsItsYamlForm(string culture)
    {
        using var scope = new CultureScope(culture);
        var compose = Build(d => d
            .WithUpdateConfig(c => c.WithProperty("parallelism", 2L).WithProperty("max_failure_ratio", 0.3m))
            .WithRestartPolicy(c => c.WithProperty("max_attempts", "4"))
            .WithPlacement(c => c.WithProperty("max_replicas_per_node", 3))
            .WithResources(c => c
                .WithProperty("limits", new Dictionary<string, object> { ["cpus"] = 1.5m, ["pids"] = 9L })
                .WithProperty("reservations", new Dictionary<string, string> { ["cpus"] = "0.50", ["pids"] = "10" })));

        var yaml = compose.Serialize();
        Assert.Contains("parallelism: 2\n", yaml);
        Assert.Contains("max_failure_ratio: 0.3\n", yaml);
        Assert.Contains("max_attempts: \"4\"\n", yaml);
        Assert.Contains("max_replicas_per_node: 3\n", yaml);
        Assert.Contains("cpus: 1.5\n", yaml);
        Assert.Contains("pids: 9\n", yaml);
        Assert.Contains("cpus: \"0.50\"\n", yaml);
        Assert.Contains("pids: \"10\"\n", yaml);

        var deploy = DeployOf(compose);
        Assert.Equal(2, deploy.UpdateConfig!.Parallelism);
        Assert.Equal(0.3, deploy.UpdateConfig.MaxFailureRatio);
        Assert.Equal(4, deploy.RestartPolicy!.MaxAttempts);
        Assert.Equal(3, deploy.Placement!.MaxReplicasPerNode);
        Assert.Equal("1.5", deploy.Resources!.Limits!.Cpus);
        Assert.Equal(9, deploy.Resources.Limits.Pids);
        Assert.Equal("0.50", deploy.Resources.Reservations!.Cpus);
        Assert.Equal(10, deploy.Resources.Reservations.Pids);
    }

    [Theory]
    [MemberData(nameof(Cultures))]
    public void StringsThatAreNotInvariantNumbersPassThrough(string culture)
    {
        using var scope = new CultureScope(culture);
        var compose = Build(d => d.WithUpdateConfig(c => c
            .WithProperty("parallelism", "${PARALLELISM}")
            .WithProperty("max_failure_ratio", "0,3")));

        var yaml = compose.Serialize();
        Assert.Contains("parallelism: \"${PARALLELISM}\"\n", yaml);
        Assert.Contains("max_failure_ratio: \"0,3\"\n", yaml);
        Assert.Null(DeployOf(compose).UpdateConfig!.Parallelism);
        Assert.Null(DeployOf(compose).UpdateConfig!.MaxFailureRatio);
    }

    [Theory]
    [MemberData(nameof(Cultures))]
    public void TypedEditReplacesTheNumericMapValue(string culture)
    {
        using var scope = new CultureScope(culture);
        var compose = Build(d => d.WithResources(c => c.WithProperty("limits",
            new Dictionary<string, object> { ["cpus"] = 0.5 })));
        var limits = DeployOf(compose).Resources!.Limits!;

        limits.Cpus = "1.50";
        Assert.Equal("1.50", limits.Cpus);
        Assert.Contains("cpus: \"1.50\"\n", compose.Serialize());

        limits.Cpus = null;
        Assert.DoesNotContain("cpus:", compose.Serialize());
    }

    [Theory]
    [MemberData(nameof(Cultures))]
    public void DeserializedNumbersReadTheSameInEveryCulture(string culture)
    {
        using var scope = new CultureScope(culture);
        var compose = ComposeExtensions.Deserialize(
            "services:\n  app:\n    deploy:\n      resources:\n        limits:\n          cpus: 0.5\n");

        Assert.Equal("0.5", DeployOf(compose).Resources!.Limits!.Cpus);
        Assert.Contains("cpus: 0.5\n", compose.Serialize());
    }

    private static Compose Build(Action<DeployBuilder> configure) => Builder.MakeCompose()
        .WithServices(Builder.MakeService("app").WithImage("app").WithSwarm().WithDeploy(configure).Build())
        .Build();

    private static Deploy DeployOf(Compose compose) => compose.Services!["app"].Deploy!;

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
