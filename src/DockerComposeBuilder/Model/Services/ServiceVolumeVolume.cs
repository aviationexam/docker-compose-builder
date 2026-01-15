using System;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Model.Services;

[Serializable]
public class ServiceVolumeVolume
{
    [YamlMember(Alias = "nocopy")]
    public bool? Nocopy { get; set; }
}
