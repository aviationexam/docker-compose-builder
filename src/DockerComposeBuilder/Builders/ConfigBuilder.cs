using DockerComposeBuilder.Builders.Base;
using DockerComposeBuilder.Model;

namespace DockerComposeBuilder.Builders;

public class ConfigBuilder : BuilderBase<ConfigBuilder, Config>
{
    internal ConfigBuilder()
    {
    }

    public ConfigBuilder WithFile(string file)
    {
        return WithProperty("file", file);
    }

    public ConfigBuilder SetExternal(bool isExternal)
    {
        return WithProperty("external", isExternal);
    }

    public ConfigBuilder WithEnvironment(string environmentVariable)
    {
        return WithProperty("environment", environmentVariable);
    }

    public ConfigBuilder WithContent(string content)
    {
        return WithProperty("content", content);
    }
}
