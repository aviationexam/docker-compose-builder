using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Model.Services;

[Serializable]
public class DeviceReservation
{
    [YamlMember(Alias = "capabilities")]
    public List<string>? Capabilities { get; set; }

    [YamlMember(Alias = "driver")]
    public string? Driver { get; set; }

    /// <summary>An integer or the string <c>all</c>. Mutually exclusive with <see cref="DeviceIds"/>.</summary>
    [YamlMember(Alias = "count")]
    public object? Count { get; set; }

    [YamlMember(Alias = "device_ids")]
    public List<string>? DeviceIds { get; set; }

    /// <summary>Driver options as a mapping or a list of KEY=VALUE strings.</summary>
    [YamlMember(Alias = "options")]
    public object? Options { get; set; }
}
