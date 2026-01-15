using System;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Model.Services;

[Serializable]
public class ServiceVolumeTmpfs
{
    [YamlMember(Alias = "size")]
    public long? Size { get; set; }

    [YamlMember(Alias = "mode")]
    public int? Mode { get; set; }
}
