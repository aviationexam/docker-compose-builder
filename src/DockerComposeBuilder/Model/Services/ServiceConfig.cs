using System;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Model.Services;

[Serializable]
public class ServiceConfig
{
    [YamlMember(Alias = "source")]
    public string? Source { get; set; }

    [YamlMember(Alias = "target")]
    public string? Target { get; set; }

    [YamlMember(Alias = "uid", ScalarStyle = ScalarStyle.DoubleQuoted)]
    public string? Uid { get; set; }

    [YamlMember(Alias = "gid", ScalarStyle = ScalarStyle.DoubleQuoted)]
    public string? Gid { get; set; }

    [YamlMember(Alias = "mode")]
    public UnixFileMode? Mode { get; set; }

    [YamlIgnore]
    public string? ShortSyntax { get; private set; }

    [YamlIgnore]
    public bool IsShortSyntax => ShortSyntax != null || (Target == null && Uid == null && Gid == null && Mode == null);

    public static implicit operator ServiceConfig(string shortSyntax) => new()
    {
        ShortSyntax = shortSyntax,
        Source = shortSyntax
    };

    public static implicit operator string(ServiceConfig config) => config.ShortSyntax ?? config.Source ?? string.Empty;

    public static ServiceConfig FromShortSyntax(string shortSyntax) => new()
    {
        ShortSyntax = shortSyntax,
        Source = shortSyntax
    };
}
