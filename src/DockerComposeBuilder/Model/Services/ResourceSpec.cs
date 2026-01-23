using System;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Model.Services;

[Serializable]
public class ResourceSpec
{
    [YamlMember(Alias = "cpus")]
    public string? Cpus { get; set; }

    [YamlMember(Alias = "memory")]
    public string? Memory { get; set; }

    [YamlMember(Alias = "pids")]
    public int? Pids { get; set; }
}
