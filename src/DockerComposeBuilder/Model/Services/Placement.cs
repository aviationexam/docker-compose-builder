using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Model.Services;

[Serializable]
public class Placement
{
    [YamlMember(Alias = "constraints")]
    public List<string>? Constraints { get; set; }

    [YamlMember(Alias = "preferences")]
    public List<PlacementPreference>? Preferences { get; set; }

    [YamlMember(Alias = "max_replicas_per_node")]
    public int? MaxReplicasPerNode { get; set; }
}
