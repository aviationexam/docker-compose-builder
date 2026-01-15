using System;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Model.Services;

[Serializable]
public class ServiceVolumeBind
{
    [YamlMember(Alias = "propagation")]
    public string? Propagation { get; set; }

    [YamlMember(Alias = "create_host_path")]
    public bool? CreateHostPath { get; set; }

    [YamlMember(Alias = "selinux")]
    public string? Selinux { get; set; }
}
