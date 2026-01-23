using System.Runtime.Serialization;

namespace DockerComposeBuilder.Enums;

public enum ERestartCondition
{
    [EnumMember(Value = "none")]
    None,

    [EnumMember(Value = "on-failure")]
    OnFailure,

    [EnumMember(Value = "any")]
    Any,
}
