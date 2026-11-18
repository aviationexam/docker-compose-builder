using DockerComposeBuilder.Builders.Base;
using DockerComposeBuilder.Converters;
using DockerComposeBuilder.Enums;
using DockerComposeBuilder.Model.Services;
using System;
using System.Collections.Generic;

namespace DockerComposeBuilder.Builders;

public class DeployBuilder : BaseBuilder<DeployBuilder, Deploy>
{
    internal DeployBuilder()
    {
    }

    public DeployBuilder WithEndpointMode(EEndpointMode endpointMode)
    {
        WorkingObject.EndpointMode = endpointMode;
        return this;
    }

    public DeployBuilder WithLabels(IDictionary<string, string> labels)
    {
        WorkingObject.Labels ??= new Dictionary<string, string>();

        return AddToDictionary(WorkingObject.Labels, labels);
    }

    public DeployBuilder WithLabels(Action<IDictionary<string, string>> environmentExpression)
    {
        WorkingObject.Labels ??= new Dictionary<string, string>();

        environmentExpression(WorkingObject.Labels);

        return this;
    }

    public DeployBuilder WithMode(EReplicationMode mode)
    {
        WorkingObject.Mode = mode;
        return this;
    }

    public DeployBuilder WithReplicas(int replicas)
    {
        WorkingObject.Replicas = replicas;
        return this;
    }

    public DeployBuilder WithUpdateConfig(UpdateConfig updateConfig)
    {
        WorkingObject.UpdateConfig = updateConfig;
        return this;
    }

    public DeployBuilder WithUpdateConfig(Action<UpdateConfigBuilder> updateConfig)
    {
        var builder = new UpdateConfigBuilder();
        updateConfig(builder);
        WorkingObject.UpdateConfig = builder.Build();
        return this;
    }

    public DeployBuilder WithRestartPolicy(RestartPolicy restartPolicy)
    {
        WorkingObject.RestartPolicy = restartPolicy;
        return this;
    }

    public DeployBuilder WithRestartPolicy(Action<RestartPolicyBuilder> restartPolicy)
    {
        var builder = new RestartPolicyBuilder();
        restartPolicy(builder);
        WorkingObject.RestartPolicy = builder.Build();
        return this;
    }

    public DeployBuilder WithPlacement(Placement placement)
    {
        WorkingObject.Placement = placement;
        return this;
    }

    public DeployBuilder WithPlacement(Action<PlacementBuilder> placement)
    {
        var builder = new PlacementBuilder();
        placement(builder);
        WorkingObject.Placement = builder.Build();
        return this;
    }

    public DeployBuilder WithResources(Resources resources)
    {
        WorkingObject.Resources = resources;
        return this;
    }

    public DeployBuilder WithResources(Action<ResourcesBuilder> resources)
    {
        var builder = new ResourcesBuilder();
        resources(builder);
        WorkingObject.Resources = builder.Build();
        return this;
    }

    [Obsolete($"Use {nameof(WithUpdateConfig)} with {nameof(UpdateConfig)} or {nameof(UpdateConfigBuilder)} instead.", error: false)]
    public DeployBuilder WithUpdateConfig(Action<MapBuilder> updateConfig)
    {
        WorkingObject.UpdateConfig = BuildMap<UpdateConfig>(updateConfig);
        return this;
    }

    [Obsolete($"Use {nameof(WithRestartPolicy)} with {nameof(RestartPolicy)} or {nameof(RestartPolicyBuilder)} instead.", error: false)]
    public DeployBuilder WithRestartPolicy(Action<MapBuilder> restartPolicy)
    {
        WorkingObject.RestartPolicy = BuildMap<RestartPolicy>(restartPolicy);
        return this;
    }

    [Obsolete($"Use {nameof(WithPlacement)} with {nameof(Placement)} or {nameof(PlacementBuilder)} instead.", error: false)]
    public DeployBuilder WithPlacement(Action<MapBuilder> placement)
    {
        WorkingObject.Placement = BuildMap<Placement>(placement);
        return this;
    }

    [Obsolete($"Use {nameof(WithResources)} with {nameof(Resources)} or {nameof(ResourcesBuilder)} instead.", error: false)]
    public DeployBuilder WithResources(Action<MapBuilder> resources)
    {
        WorkingObject.Resources = BuildMap<Resources>(resources);
        return this;
    }

    private static T BuildMap<T>(Action<MapBuilder> configure) where T : DeployMapping, new()
    {
        var builder = new MapBuilder();
        configure(builder);
        return DeployMappingConverter.FromMap<T>(builder.Build());
    }
}
