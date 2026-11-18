using System;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Model.Services;

[Serializable]
public class GenericResource
{
    [YamlMember(Alias = "discrete_resource_spec")]
    public DiscreteResourceSpec? DiscreteResourceSpec { get; set; }
}
