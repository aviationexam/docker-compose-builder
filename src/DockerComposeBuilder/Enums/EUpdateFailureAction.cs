using System.Runtime.Serialization;

namespace DockerComposeBuilder.Enums;

public enum EUpdateFailureAction
{
    [EnumMember(Value = "continue")]
    Continue,

    [EnumMember(Value = "rollback")]
    Rollback,

    [EnumMember(Value = "pause")]
    Pause,
}
