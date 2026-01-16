using DockerComposeBuilder.Builders;
using DockerComposeBuilder.Extensions;
using DockerComposeBuilder.Model;
using DockerComposeBuilder.Model.Services;
using DockerComposeBuilder.Model.Services.BuildArguments;
using System.Collections.Generic;
using Xunit;

namespace DockerComposeBuilder.Tests;

public class ComposeBuilderTests
{
    [Fact]
    public void EmptyComposeBuilderTest()
    {
        var compose = Builder.MakeCompose()
            .Build();

        var result = compose.Serialize();

        Assert.Equal(
            // language=yaml
            """
            version: "3.8"

            """,
            result
        );
    }

    [Fact]
    public void ServiceWithImageTest()
    {
        var compose = Builder.MakeCompose()
            .WithServices(Builder.MakeService("a-service")
                .WithImage("aviationexam/a-service")
                .Build()
            )
            .Build();

        var result = compose.Serialize();

        Assert.Equal(
            // language=yaml
            """
            version: "3.8"
            services:
              a-service:
                image: "aviationexam/a-service"

            """,
            result
        );
    }

    [Fact]
    public void PrivilegedServiceWithImageTest()
    {
        var compose = Builder.MakeCompose()
            .WithServices(Builder.MakeService("a-service")
                .WithImage("aviationexam/a-service")
                .WithPrivileged()
                .Build()
            )
            .Build();

        var result = compose.Serialize();

        Assert.Equal(
            // language=yaml
            """
            version: "3.8"
            services:
              a-service:
                image: "aviationexam/a-service"
                privileged: true

            """,
            result
        );
    }

    [Fact]
    public void ServiceWithBuildTest()
    {
        var compose = Builder.MakeCompose()
            .WithServices(Builder.MakeService("a-service")
                .WithImage("aviationexam/a-service")
                .WithBuild(x => x
                    .WithDockerfile("a.dockerfile")
                )
                .Build()
            )
            .Build();

        var result = compose.Serialize();

        Assert.Equal(
            // language=yaml
            """
            version: "3.8"
            services:
              a-service:
                image: "aviationexam/a-service"
                build:
                  dockerfile: "a.dockerfile"

            """,
            result
        );
    }

    [Fact]
    public void ServiceWithBuildArgumentsTest()
    {
        var compose = Builder.MakeCompose()
            .WithServices(Builder.MakeService("a-service")
                .WithImage("aviationexam/a-service")
                .WithBuild(x => x
                    .WithContext(".")
                    .WithDockerfile("a.dockerfile")
                    .WithArguments(a => a
                        .AddWithoutValue("ENV_1")
                        .Add(new KeyValuePair<string, string>("ENV_2", "value"))
                        .Add(new BuildArgument("ENV_3", "value"))
                    )
                )
                .Build()
            )
            .Build();

        var result = compose.Serialize();

        Assert.Equal(
            // language=yaml
            """
            version: "3.8"
            services:
              a-service:
                image: "aviationexam/a-service"
                build:
                  context: "."
                  dockerfile: "a.dockerfile"
                  args:
                  - "ENV_1"
                  - "ENV_2=value"
                  - "ENV_3=value"

            """,
            result
        );
    }

    [Fact]
    public void DeserializeSimpleComposeTest()
    {
        var yaml = // language=yaml
            """
            version: "3.8"
            services:
              web:
                image: nginx:latest
                hostname: webserver
              db:
                image: postgres:13
            """;

        var compose = ComposeExtensions.Deserialize(yaml);

        Assert.NotNull(compose);
        Assert.Equal("3.8", compose.Version);
        Assert.NotNull(compose.Services);
        Assert.Equal(2, compose.Services.Count);
        var webService = Assert.Contains("web", compose.Services);
        var dbService = Assert.Contains("db", compose.Services);
        Assert.Equal("nginx:latest", webService.Image);
        Assert.Equal("webserver", webService.Hostname);
        Assert.Equal("postgres:13", dbService.Image);
    }

    [Fact]
    public void DeserializeWithEnvironmentTest()
    {
        var yaml = // language=yaml
            """
            version: "3.8"
            services:
              db:
                image: mysql:5.7
                environment:
                  MYSQL_ROOT_PASSWORD: secret
                  MYSQL_DATABASE: mydb
            """;

        var compose = ComposeExtensions.Deserialize(yaml);

        Assert.NotNull(compose.Services);
        var dbService = compose.Services["db"];
        Assert.NotNull(dbService.Environment);
        Assert.Equal("secret", dbService.Environment["MYSQL_ROOT_PASSWORD"]);
        Assert.Equal("mydb", dbService.Environment["MYSQL_DATABASE"]);
    }

    [Fact]
    public void RoundTripSerializationTest()
    {
        // Create a compose using the builder
        var compose = Builder.MakeCompose()
            .WithServices(
                Builder.MakeService("web")
                    .WithImage("nginx:latest")
                    .WithHostname("webserver")
                    .Build()
            )
            .Build();

        // Serialize to YAML
        var yaml1 = compose.Serialize();

        // Deserialize back to object
        var compose2 = ComposeExtensions.Deserialize(yaml1);

        // Serialize again
        var yaml2 = compose2.Serialize();

        // The two YAML strings should be identical
        Assert.Equal(yaml1, yaml2);
    }

    [Fact]
    public void TryDeserializeSuccessTest()
    {
        var yaml = // language=yaml
            """
            version: "3.8"
            services:
              app:
                image: myapp:latest
            """;

        var success = ComposeExtensions.TryDeserialize(yaml, out var compose);

        Assert.True(success);
        Assert.NotNull(compose);
        Assert.Equal("3.8", compose.Version);
    }

    [Fact]
    public void TryDeserializeFailureTest()
    {
        var invalidYaml = "not: [valid: yaml: structure";

        var success = ComposeExtensions.TryDeserialize(invalidYaml, out var compose);

        Assert.False(success);
        Assert.Null(compose);
    }

    [Fact]
    public void DeserializeWithPortsTest()
    {
        var yaml = // language=yaml
            """
            version: "3.8"
            services:
              web:
                image: nginx:latest
                ports:
                  - target: 80
                    published: 8080
                  - target: 443
                    published: 8443
            """;

        var compose = ComposeExtensions.Deserialize(yaml);

        Assert.NotNull(compose.Services);
        var webService = compose.Services["web"];
        Assert.NotNull(webService.Ports);
        Assert.Equal(2, webService.Ports.Count);

        Assert.Equal(80, webService.Ports[0].Target);
        Assert.NotNull(webService.Ports[0].Published);
        Assert.Equal(8080, webService.Ports[0].Published!.PortAsInt);

        Assert.Equal(443, webService.Ports[1].Target);
        Assert.NotNull(webService.Ports[1].Published);
        Assert.Equal(8443, webService.Ports[1].Published!.PortAsInt);
    }

    [Fact]
    public void DeserializeWithPortRangesTest()
    {
        var yaml = // language=yaml
            """
            version: "3.8"
            services:
              web:
                image: nginx:latest
                ports:
                  - published: "8000-9000"
                    protocol: tcp
                  - target: 443
                    published: 8443
            """;

        var compose = ComposeExtensions.Deserialize(yaml);

        Assert.NotNull(compose.Services);
        var webService = compose.Services["web"];
        Assert.NotNull(webService.Ports);
        Assert.Equal(2, webService.Ports.Count);

        Assert.Null(webService.Ports[0].Target);
        Assert.NotNull(webService.Ports[0].Published);
        Assert.Equal("8000-9000", webService.Ports[0].Published!.PortAsString);
        Assert.Equal("tcp", webService.Ports[0].Protocol);

        Assert.Equal(443, webService.Ports[1].Target);
        Assert.NotNull(webService.Ports[1].Published);
        Assert.Equal(8443, webService.Ports[1].Published!.PortAsInt);
    }

    [Fact]
    public void DeserializeWithVolumesShortSyntaxTest()
    {
        var yaml = // language=yaml
            """
            version: "3.8"
            services:
              app:
                image: myapp:latest
                volumes:
                  - ./data:/app/data
                  - cache:/app/cache
            volumes:
              cache:
            """;

        var compose = ComposeExtensions.Deserialize(yaml);

        Assert.NotNull(compose.Services);
        var appService = compose.Services["app"];
        Assert.NotNull(appService.Volumes);
        Assert.Equal(2, appService.Volumes.Count);

        // Works as string (backwards compatible via implicit conversion)
        Assert.Equal("./data:/app/data", appService.Volumes[0]);
        Assert.Equal("cache:/app/cache", appService.Volumes[1]);

        // Also works as ServiceVolume (typed access)
        ServiceVolume firstVolume = appService.Volumes[0];
        Assert.Equal("./data:/app/data", firstVolume.ShortSyntax);
    }

    [Fact]
    public void DeserializeWithVolumesLongSyntaxTest()
    {
        var yaml = // language=yaml
            """
            version: "3.8"
            services:
              app:
                image: myapp:latest
                volumes:
                  - type: bind
                    source: ./data
                    target: /app/data
                    read_only: true
                  - type: volume
                    source: cache
                    target: /app/cache
            volumes:
              cache:
            """;

        var compose = ComposeExtensions.Deserialize(yaml);

        Assert.NotNull(compose.Services);
        var appService = compose.Services["app"];
        Assert.NotNull(appService.Volumes);
        Assert.Equal(2, appService.Volumes.Count);

        Assert.Equal("bind", appService.Volumes[0].Type);
        Assert.Equal("./data", appService.Volumes[0].Source);
        Assert.Equal("/app/data", appService.Volumes[0].Target);
        Assert.True(appService.Volumes[0].ReadOnly);

        Assert.Equal("volume", appService.Volumes[1].Type);
        Assert.Equal("cache", appService.Volumes[1].Source);
        Assert.Equal("/app/cache", appService.Volumes[1].Target);
        Assert.Null(appService.Volumes[1].ReadOnly);
    }

    [Fact]
    public void VolumesSupportsForEachWithStringTest()
    {
        var compose = Builder.MakeCompose()
            .WithServices(
                Builder.MakeService("app")
                    .WithImage("myapp:latest")
                    .WithVolumes("./data:/app/data", "cache:/app/cache")
                    .Build()
            )
            .Build();

        var appService = compose.Services!["app"];
        var volumeStrings = new List<string>();

        // This is the backwards-compatible pattern that must work
        foreach (string v in appService.Volumes!)
        {
            volumeStrings.Add(v);
        }

        Assert.Equal(2, volumeStrings.Count);
        Assert.Equal("./data:/app/data", volumeStrings[0]);
        Assert.Equal("cache:/app/cache", volumeStrings[1]);
    }

    [Fact]
    public void SerializeWithVolumesShortSyntaxTest()
    {
        var compose = Builder.MakeCompose()
            .WithServices(
                Builder.MakeService("app")
                    .WithImage("myapp:latest")
                    .WithVolumes("./data:/app/data", "cache:/app/cache")
                    .Build()
            )
            .Build();

        var result = compose.Serialize();

        Assert.Equal(
            // language=yaml
            """
            version: "3.8"
            services:
              app:
                image: "myapp:latest"
                volumes:
                - "./data:/app/data"
                - "cache:/app/cache"

            """,
            result
        );
    }

    [Fact]
    public void SerializeWithVolumesLongSyntaxTest()
    {
        var compose = Builder.MakeCompose()
            .WithServices(
                Builder.MakeService("app")
                    .WithImage("myapp:latest")
                    .WithVolumes(
                        new ServiceVolume
                        {
                            Type = "bind",
                            Source = "./data",
                            Target = "/app/data",
                            ReadOnly = true
                        },
                        new ServiceVolume
                        {
                            Type = "volume",
                            Source = "cache",
                            Target = "/app/cache"
                        }
                    )
                    .Build()
            )
            .Build();

        var result = compose.Serialize();

        Assert.Equal(
            // language=yaml
            """
            version: "3.8"
            services:
              app:
                image: "myapp:latest"
                volumes:
                - type: "bind"
                  source: "./data"
                  target: "/app/data"
                  read_only: true
                - type: "volume"
                  source: "cache"
                  target: "/app/cache"

            """,
            result
        );
    }

    [Fact]
    public void SerializeJaegerServiceTest()
    {
        var compose = Builder.MakeCompose()
            .WithServices(
                Builder.MakeService("jaeger")
                    .WithImage("jaegertracing/jaeger:2.14.1")
                    .WithHealthCheck(healthCheck =>
                    {
                        healthCheck.TestCommand =
                        [
                            "CMD-SHELL",
                            "wget --no-verbose --tries=1 -q -O - http://localhost:14269/status | grep -q '\"healthy\":true' > /dev/null || exit 1"
                        ];
                        healthCheck.Interval = "1m";
                        healthCheck.Timeout = "10s";
                        healthCheck.Retries = 3;
                        healthCheck.StartPeriod = "15s";
                    })
                    .WithHostname("jaeger")
                    .WithNetworks("app_network")
                    .WithLabels(labels =>
                    {
                        labels.Add("app.service", "jaeger");
                        labels.Add("app.metrics.port", "14269");
                        labels.Add("app.container.group", "services");
                        labels.Add("app.stack.name", "production");
                    })
                    .WithEnvironment(env => { env.Add("GOMEMLIMIT", "1280MiB"); })
                    .WithVolumes("/data/config/jaeger:/config:ro")
                    .WithPortMappings(
                        new Port
                        {
                            Target = 16686,
                            Published = 16686,
                            Protocol = "tcp",
                        },
                        new Port
                        {
                            Target = 14269,
                            Protocol = "tcp",
                        }
                    )
                    .WithExtraHosts(
                        "prometheus:${PROMETHEUS_HOST}",
                        "elasticsearch:192.168.1.100"
                    )
                    .WithCommands(
                        "--config=/config/jaeger.yaml",
                        "--set=extensions.jaeger_query.base_path=/tracing"
                    )
                    .WithSwarm()
                    .WithDeploy(deploy => deploy
                        .WithLabels(labels =>
                        {
                            labels.Add("app.service", "jaeger");
                            labels.Add("app.metrics.port", "14269");
                            labels.Add("app.container.group", "services");
                            labels.Add("app.stack.name", "production");
                        })
                        .WithReplicas(2)
                        .WithUpdateConfig(c => c.WithProperty("delay", "30s"))
                        .WithRestartPolicy(c => c
                            .WithProperty("delay", "30s")
                            .WithProperty("max_attempts", 10)
                            .WithProperty("window", "120s")
                        )
                        .WithPlacement(c => c
                            .WithProperty("constraints", new[] { "node.role == worker" })
                            .WithProperty("preferences", new[] { new PlacementPreferences { Spread = "node.labels.zone" } })
                        )
                        .WithResources(c => c.WithProperty("limits", new Dictionary<string, string>
                        {
                            ["memory"] = "1600m",
                        }))
                    )
                    .Build()
            )
            .Build();

        var result = compose.Serialize();

        Assert.Equal(
            // language=yaml
            """
            version: "3.8"
            services:
              jaeger:
                image: "jaegertracing/jaeger:2.14.1"
                healthcheck:
                  test: ["CMD-SHELL", "wget --no-verbose --tries=1 -q -O - http://localhost:14269/status | grep -q '\"healthy\":true' > /dev/null || exit 1"]
                  interval: "1m"
                  timeout: "10s"
                  retries: 3
                  start_period: "15s"
                hostname: "jaeger"
                networks:
                - "app_network"
                labels:
                  app.service: "jaeger"
                  app.metrics.port: "14269"
                  app.container.group: "services"
                  app.stack.name: "production"
                environment:
                  GOMEMLIMIT: "1280MiB"
                volumes:
                - "/data/config/jaeger:/config:ro"
                ports:
                - target: 16686
                  published: 16686
                  protocol: "tcp"
                - target: 14269
                  protocol: "tcp"
                extra_hosts:
                - "prometheus:${PROMETHEUS_HOST}"
                - "elasticsearch:192.168.1.100"
                command:
                - "--config=/config/jaeger.yaml"
                - "--set=extensions.jaeger_query.base_path=/tracing"
                deploy:
                  labels:
                    app.service: "jaeger"
                    app.metrics.port: "14269"
                    app.container.group: "services"
                    app.stack.name: "production"
                  replicas: 2
                  update_config:
                    delay: "30s"
                  restart_policy:
                    delay: "30s"
                    max_attempts: 10
                    window: "120s"
                  placement:
                    constraints: ["node.role == worker"]
                    preferences:
                    - spread: "node.labels.zone"
                  resources:
                    limits:
                      memory: "1600m"

            """,
            result
        );
    }
}
