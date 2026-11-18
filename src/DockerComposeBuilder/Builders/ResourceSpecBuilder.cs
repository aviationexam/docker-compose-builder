using DockerComposeBuilder.Builders.Base;
using DockerComposeBuilder.Model.Services;
using System;

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

    public ResourceSpecBuilder WithDevice(DeviceReservation device)
    {
        (WorkingObject.Devices ??= []).Add(device);
        return this;
    }

    public ResourceSpecBuilder WithDevice(Action<DeviceReservationBuilder> device)
    {
        var builder = new DeviceReservationBuilder();
        device(builder);
        return WithDevice(builder.Build());
    }

    public ResourceSpecBuilder WithGenericResource(GenericResource resource)
    {
        (WorkingObject.GenericResources ??= []).Add(resource);
        return this;
    }

    public ResourceSpecBuilder WithDiscreteResource(
        string kind,
        decimal value
    ) => WithGenericResource(new GenericResource { DiscreteResourceSpec = new DiscreteResourceSpec { Kind = kind, Value = value } });

    public ResourceSpecBuilder WithDiscreteResource(
        string kind,
        string value
    ) => WithGenericResource(new GenericResource { DiscreteResourceSpec = new DiscreteResourceSpec { Kind = kind, Value = value } });
}
