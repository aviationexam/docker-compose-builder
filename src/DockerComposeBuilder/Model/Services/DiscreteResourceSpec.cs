using System;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Model.Services;

[Serializable]
public class DiscreteResourceSpec
{
    [YamlMember(Alias = "kind")]
    public string? Kind { get; set; }

    /// <summary>The amount to reserve, as a number or a string (for example, a Compose variable).</summary>
    [YamlMember(Alias = "value")]
    public object? Value { get; set; }
}
