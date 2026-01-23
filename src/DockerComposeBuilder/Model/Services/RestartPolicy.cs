using DockerComposeBuilder.Enums;
using System;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Model.Services;

[Serializable]
public class RestartPolicy
{
    [YamlMember(Alias = "condition")]
    public ERestartCondition? Condition { get; set; }

    [YamlMember(Alias = "delay")]
    public string? Delay { get; set; }

    [YamlMember(Alias = "max_attempts")]
    public int? MaxAttempts { get; set; }

    [YamlMember(Alias = "window")]
    public string? Window { get; set; }
}
