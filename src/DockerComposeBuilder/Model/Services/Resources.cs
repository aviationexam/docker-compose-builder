using System;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Model.Services;

[Serializable]
public class Resources
{
    [YamlMember(Alias = "limits")]
    public ResourceSpec? Limits { get; set; }

    [YamlMember(Alias = "reservations")]
    public ResourceSpec? Reservations { get; set; }
}
