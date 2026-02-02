using DockerComposeBuilder.Builders.Base;
using DockerComposeBuilder.Model.Services;
using System.Collections.Generic;
using System.Linq;

namespace DockerComposeBuilder.Builders;

public class PlacementBuilder : BaseBuilder<PlacementBuilder, Placement>
{
    internal PlacementBuilder()
    {
    }

    public PlacementBuilder WithConstraints(params IReadOnlyList<string> constraints)
    {
        if (WorkingObject.Constraints == null)
        {
            WorkingObject.Constraints = constraints;
        }
        else
        {
            WorkingObject.Constraints = WorkingObject.Constraints.Concat(constraints).ToArray();
        }

        return this;
    }

    public PlacementBuilder WithPreference(string spread)
    {
        WorkingObject.Preferences ??= [];
        WorkingObject.Preferences.Add(new PlacementPreference { Spread = spread });
        return this;
    }

    public PlacementBuilder WithMaxReplicasPerNode(int maxReplicasPerNode)
    {
        WorkingObject.MaxReplicasPerNode = maxReplicasPerNode;
        return this;
    }
}
