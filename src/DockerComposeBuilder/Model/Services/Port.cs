using System;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Model.Services;

/// <summary>
/// Defines port mappings between the host machine and the container, specifying how ports
/// on the host are exposed to the container's network.
/// </summary>
[Serializable]
public class Port
{
    /// <summary>
    /// Specifies the internal container port to which the external port will be mapped.
    /// </summary>
    [YamlMember(Alias = "target")]
    public int? Target { get; set; }

    /// <summary>
    /// Specifies the publicly exposed port. Can be set as a range using the syntax <c>start-end</c>,
    /// where an available port within the range is assigned.<br/><br/>
    /// <list type="table">
    /// <item>
    /// <term>Single Port</term>
    /// <description><c>8000</c></description>
    /// </item>
    /// <item>
    /// <term>Port Range</term>
    /// <description><c>8000-9000</c></description>
    /// </item>
    /// </list>
    /// </summary>
    [YamlMember(Alias = "published")]
    public PublishedPort? Published { get; set; }

    /// <summary>
    /// Specifies the port protocol, either <c>tcp</c> or <c>udp</c>. Defaults to <c>tcp</c>.
    /// </summary>
    [YamlMember(Alias = "protocol")]
    public string? Protocol { get; set; }

    /// <summary>
    /// Specifies the port mode. Defaults to <c>ingress</c>.
    /// <list type="bullet">
    /// <item>
    /// <description><c>host</c>: Publishes a host port on each node.</description>
    /// </item>
    /// <item>
    /// <description><c>ingress</c>: Enables load balancing for the port.</description>
    /// </item>
    /// </list>
    /// </summary>
    [YamlMember(Alias = "mode")]
    public string? Mode { get; set; }

    /// <summary>
    /// Specifies the host IP address to bind the port. If unspecified, it defaults to all network interfaces (<c>0.0.0.0</c>).
    /// </summary>
    [YamlMember(Alias = "host_ip")]
    public string? HostIp { get; set; }

    /// <summary>
    /// A human-readable name for the port, used to document its purpose or usage within the service.
    /// </summary>
    [YamlMember(Alias = "name")]
    public string? Name { get; set; }
}
