using DockerComposeBuilder.Builders.Base;
using DockerComposeBuilder.Model.Services;

namespace DockerComposeBuilder.Builders;

public class ResourceSpecBuilder : BaseBuilder<ResourceSpecBuilder, ResourceSpec>
{
    internal ResourceSpecBuilder()
    {
    }

    public ResourceSpecBuilder WithCpus(string cpus)
    {
        WorkingObject.Cpus = cpus;
        return this;
    }

    public ResourceSpecBuilder WithMemory(string memory)
    {
        WorkingObject.Memory = memory;
        return this;
    }

    public ResourceSpecBuilder WithPids(int pids)
    {
        WorkingObject.Pids = pids;
        return this;
    }
}
