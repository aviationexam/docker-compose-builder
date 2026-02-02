using DockerComposeBuilder.Builders.Base;
using DockerComposeBuilder.Enums;
using DockerComposeBuilder.Model;
using DockerComposeBuilder.Model.Services;
using System;
using System.Collections.Generic;
using System.Linq;

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
        WorkingObject.UpdateConfig = MapToUpdateConfig(map);
        return this;
    }

    [Obsolete($"Use {nameof(WithRestartPolicy)} with {nameof(RestartPolicy)} or {nameof(RestartPolicyBuilder)} instead.", error: false)]
    public DeployBuilder WithRestartPolicy(Action<MapBuilder> restartPolicy)
    {
        var mb = new MapBuilder();
        restartPolicy(mb);
        var map = mb.Build();
        WorkingObject.RestartPolicy = MapToRestartPolicy(map);
        return this;
    }

    [Obsolete($"Use {nameof(WithPlacement)} with {nameof(Placement)} or {nameof(PlacementBuilder)} instead.", error: false)]
    public DeployBuilder WithPlacement(Action<MapBuilder> placement)
    {
        var mb = new MapBuilder();
        placement(mb);
        var map = mb.Build();
        WorkingObject.Placement = MapToPlacement(map);
        return this;
    }

    [Obsolete($"Use {nameof(WithResources)} with {nameof(Resources)} or {nameof(ResourcesBuilder)} instead.", error: false)]
    public DeployBuilder WithResources(Action<MapBuilder> resources)
    {
        var mb = new MapBuilder();
        resources(mb);
        var map = mb.Build();
        WorkingObject.Resources = MapToResources(map);
        return this;
    }

    private static UpdateConfig MapToUpdateConfig(Map map)
    {
        var config = new UpdateConfig();
        if (map.TryGetValue("parallelism", out var p) && p != null)
        {
            config.Parallelism = Convert.ToInt32(p);
        }

        if (map.TryGetValue("delay", out var d))
        {
            config.Delay = d?.ToString();
        }

        if (map.TryGetValue("failure_action", out var fa) && fa != null)
        {
            config.FailureAction = ParseUpdateFailureAction(fa.ToString()!);
        }

        if (map.TryGetValue("monitor", out var m))
        {
            config.Monitor = m?.ToString();
        }

        if (map.TryGetValue("max_failure_ratio", out var mfr) && mfr != null)
        {
            config.MaxFailureRatio = Convert.ToDouble(mfr);
        }

        if (map.TryGetValue("order", out var o) && o != null)
        {
            config.Order = ParseUpdateOrder(o.ToString()!);
        }

        return config;
    }

    private static RestartPolicy MapToRestartPolicy(Map map)
    {
        var policy = new RestartPolicy();
        if (map.TryGetValue("condition", out var c) && c != null)
        {
            policy.Condition = ParseRestartCondition(c.ToString()!);
        }

        if (map.TryGetValue("delay", out var d))
        {
            policy.Delay = d?.ToString();
        }

        if (map.TryGetValue("max_attempts", out var ma) && ma != null)
        {
            policy.MaxAttempts = Convert.ToInt32(ma);
        }

        if (map.TryGetValue("window", out var w))
        {
            policy.Window = w?.ToString();
        }

        return policy;
    }

    private static Placement MapToPlacement(Map map)
    {
        var placement = new Placement();

        if (map.TryGetValue("constraints", out var c) && c is IEnumerable<object> constraints)
        {
            placement.Constraints = constraints.Select(x => x.ToString()!).ToArray();
        }

        if (map.TryGetValue("preferences", out var p) && p is IEnumerable<object> prefs)
        {
            placement.Preferences = prefs
                .Select(x =>
                {
                    if (x is PlacementPreference { Spread: var spread})
                    {
                        return new PlacementPreference { Spread = spread };
                    }

                    if (x is IDictionary<string, object> dict && dict.TryGetValue("spread", out var s))
                    {
                        return new PlacementPreference { Spread = s?.ToString()! };
                    }

                    return null;
                })
                .Where(x => x != null)
                .ToList()!;
        }

        if (map.TryGetValue("max_replicas_per_node", out var mr) && mr != null)
        {
            placement.MaxReplicasPerNode = Convert.ToInt32(mr);
        }

        return placement;
    }

    private static Resources MapToResources(Map map)
    {
        var resources = new Resources();
        if (map.TryGetValue("limits", out var l))
        {
            resources.Limits = ConvertToResourceSpec(l);
        }

        if (map.TryGetValue("reservations", out var r))
        {
            resources.Reservations = ConvertToResourceSpec(r);
        }

        return resources;
    }

    private static ResourceSpec? ConvertToResourceSpec(object? value)
    {
        if (value == null) return null;

        var spec = new ResourceSpec();

        if (value is IDictionary<string, object> dictObj)
        {
            if (dictObj.TryGetValue("cpus", out var c)) spec.Cpus = c?.ToString();
            if (dictObj.TryGetValue("memory", out var m)) spec.Memory = m?.ToString();
            if (dictObj.TryGetValue("pids", out var p) && p != null) spec.Pids = Convert.ToInt32(p);
        }
        else if (value is IDictionary<string, string> dictStr)
        {
            if (dictStr.TryGetValue("cpus", out var c)) spec.Cpus = c;
            if (dictStr.TryGetValue("memory", out var m)) spec.Memory = m;
            if (dictStr.TryGetValue("pids", out var p)) spec.Pids = int.Parse(p);
        }

        return spec;
    }

    private static EUpdateFailureAction ParseUpdateFailureAction(
        string value
    ) => value.ToLowerInvariant() switch
    {
        "continue" => EUpdateFailureAction.Continue,
        "rollback" => EUpdateFailureAction.Rollback,
        "pause" => EUpdateFailureAction.Pause,
        _ => Enum.TryParse<EUpdateFailureAction>(value, true, out var result)
            ? result
            : throw new ArgumentException($"Unknown update failure action: {value}")
    };

    private static EUpdateOrder ParseUpdateOrder(
        string value
    ) => value.ToLowerInvariant() switch
    {
        "stop-first" or "stopfirst" => EUpdateOrder.StopFirst,
        "start-first" or "startfirst" => EUpdateOrder.StartFirst,
        _ => Enum.TryParse<EUpdateOrder>(value, true, out var result)
            ? result
            : throw new ArgumentException($"Unknown update order: {value}")
    };

    private static ERestartCondition ParseRestartCondition(
        string value
    ) => value.ToLowerInvariant() switch
    {
        "none" => ERestartCondition.None,
        "on-failure" or "onfailure" => ERestartCondition.OnFailure,
        "any" => ERestartCondition.Any,
        _ => Enum.TryParse<ERestartCondition>(value, true, out var result)
            ? result
            : throw new ArgumentException($"Unknown restart condition: {value}")
    };
}
