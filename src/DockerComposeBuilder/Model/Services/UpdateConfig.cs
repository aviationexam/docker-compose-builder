using DockerComposeBuilder.Enums;
using System;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Model.Services;

[Serializable]
public class UpdateConfig
{
    [YamlMember(Alias = "parallelism")]
    public int? Parallelism { get; set; }

    [YamlMember(Alias = "delay")]
    public string? Delay { get; set; }

    [YamlMember(Alias = "failure_action")]
    public EUpdateFailureAction? FailureAction { get; set; }

    [YamlMember(Alias = "monitor")]
    public string? Monitor { get; set; }

    [YamlMember(Alias = "max_failure_ratio")]
    public double? MaxFailureRatio { get; set; }

    [YamlMember(Alias = "order")]
    public EUpdateOrder? Order { get; set; }
}
