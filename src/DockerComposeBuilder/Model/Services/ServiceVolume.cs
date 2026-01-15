using System;
using YamlDotNet.Serialization;

namespace DockerComposeBuilder.Model.Services;

[Serializable]
public class ServiceVolume
{
    [YamlMember(Alias = "type")]
    public string? Type { get; set; }

    [YamlMember(Alias = "source")]
    public string? Source { get; set; }

    [YamlMember(Alias = "target")]
    public string? Target { get; set; }

    [YamlMember(Alias = "read_only")]
    public bool? ReadOnly { get; set; }

    [YamlMember(Alias = "bind")]
    public ServiceVolumeBind? Bind { get; set; }

    [YamlMember(Alias = "volume")]
    public ServiceVolumeVolume? Volume { get; set; }

    [YamlMember(Alias = "tmpfs")]
    public ServiceVolumeTmpfs? Tmpfs { get; set; }

    [YamlIgnore]
    public string? ShortSyntax { get; private set; }

    public static implicit operator ServiceVolume(
        string shortSyntax
    ) => new()
    {
        ShortSyntax = shortSyntax,
    };

    public static implicit operator string(
        ServiceVolume volume
    ) => volume.ShortSyntax ?? BuildShortSyntax(volume);

    public static ServiceVolume FromShortSyntax(
        string shortSyntax
    ) => new() { ShortSyntax = shortSyntax };

    private static string BuildShortSyntax(ServiceVolume volume)
    {
        var result = $"{volume.Source}:{volume.Target}";
        if (volume.ReadOnly == true)
        {
            result += ":ro";
        }

        return result;
    }
}
