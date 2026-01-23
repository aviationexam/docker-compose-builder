using System.Runtime.Serialization;

namespace DockerComposeBuilder.Enums;

public enum EEndpointMode
{
    [EnumMember(Value = "vip")]
    Vip,

    [EnumMember(Value = "dnsrr")]
    DnsRoundRobin,
}
