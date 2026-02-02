using System.Runtime.Serialization;

namespace DockerComposeBuilder.Enums;

public enum EUpdateOrder
{
    [EnumMember(Value = "stop-first")]
    StopFirst,

    [EnumMember(Value = "start-first")]
    StartFirst,
}
