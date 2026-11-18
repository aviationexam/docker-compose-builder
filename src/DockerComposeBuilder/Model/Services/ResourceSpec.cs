using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Model.Services;

[Serializable]
public class ResourceSpec : DeployMapping
{
    [YamlMember(Alias = "cpus")]
    public string? Cpus { get; set; }

    [YamlMember(Alias = "memory")]
    public string? Memory { get; set; }

    [YamlMember(Alias = "pids")]
    public int? Pids { get; set; }

    [YamlMember(Alias = "devices")]
    public List<DeviceReservation>? Devices { get; set; }

    [YamlMember(Alias = "generic_resources")]
    public List<GenericResource>? GenericResources { get; set; }
}
