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
        var mb = new MapBuilder();
        updateConfig(mb);
        var map = mb.Build();
        WorkingObject.UpdateConfig = DeployMappingConverter.FromMap<UpdateConfig>(map);
        return this;
    }

    [Obsolete($"Use {nameof(WithRestartPolicy)} with {nameof(RestartPolicy)} or {nameof(RestartPolicyBuilder)} instead.", error: false)]
    public DeployBuilder WithRestartPolicy(Action<MapBuilder> restartPolicy)
    {
        var mb = new MapBuilder();
        restartPolicy(mb);
        var map = mb.Build();
        WorkingObject.RestartPolicy = DeployMappingConverter.FromMap<RestartPolicy>(map);
        return this;
    }

    [Obsolete($"Use {nameof(WithPlacement)} with {nameof(Placement)} or {nameof(PlacementBuilder)} instead.", error: false)]
    public DeployBuilder WithPlacement(Action<MapBuilder> placement)
    {
        var mb = new MapBuilder();
        placement(mb);
        var map = mb.Build();
        WorkingObject.Placement = DeployMappingConverter.FromMap<Placement>(map);
        return this;
    }

    [Obsolete($"Use {nameof(WithResources)} with {nameof(Resources)} or {nameof(ResourcesBuilder)} instead.", error: false)]
    public DeployBuilder WithResources(Action<MapBuilder> resources)
    {
        var mb = new MapBuilder();
        resources(mb);
        var map = mb.Build();
        WorkingObject.Resources = DeployMappingConverter.FromMap<Resources>(map);
        return this;
    }
}
