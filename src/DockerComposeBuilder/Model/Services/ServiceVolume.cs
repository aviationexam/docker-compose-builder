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

    public string? ShortSyntax { get; private set; }

    public static implicit operator ServiceVolume(string shortSyntax) => new()
    {
        ShortSyntax = shortSyntax,
    };

    public static implicit operator string(ServiceVolume volume) =>
        volume.ShortSyntax ?? BuildShortSyntax(volume);

    public static ServiceVolume FromShortSyntax(string shortSyntax)
    {
        return new ServiceVolume { ShortSyntax = shortSyntax };
    }

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

[Serializable]
public class ServiceVolumeBind
{
    [YamlMember(Alias = "propagation")]
    public string? Propagation { get; set; }

    [YamlMember(Alias = "create_host_path")]
    public bool? CreateHostPath { get; set; }

    [YamlMember(Alias = "selinux")]
    public string? Selinux { get; set; }
}

[Serializable]
public class ServiceVolumeVolume
{
    [YamlMember(Alias = "nocopy")]
    public bool? Nocopy { get; set; }
}

[Serializable]
public class ServiceVolumeTmpfs
{
    [YamlMember(Alias = "size")]
    public long? Size { get; set; }

    [YamlMember(Alias = "mode")]
    public int? Mode { get; set; }
}
