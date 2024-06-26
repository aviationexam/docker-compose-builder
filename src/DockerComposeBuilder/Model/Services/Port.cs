using System;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Model.Services;

[Serializable]
public class Port
{
    [YamlMember(Alias = "target")]
    public int? Target { get; set; }

    [YamlMember(Alias = "published")]
    public int? Published { get; set; }

    [YamlMember(Alias = "protocol")]
    public string? Protocol { get; set; }

    [YamlMember(Alias = "mode")]
    public string? Mode { get; set; }
}
