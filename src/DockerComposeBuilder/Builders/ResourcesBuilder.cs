using DockerComposeBuilder.Builders.Base;
using DockerComposeBuilder.Model.Services;
using System;

namespace DockerComposeBuilder.Builders;

public class ResourcesBuilder : BaseBuilder<ResourcesBuilder, Resources>
{
    internal ResourcesBuilder()
    {
    }

    public ResourcesBuilder WithLimits(ResourceSpec limits)
    {
        WorkingObject.Limits = limits;
        return this;
    }

    public ResourcesBuilder WithLimits(Action<ResourceSpecBuilder> limits)
    {
        var builder = new ResourceSpecBuilder();
        limits(builder);
        WorkingObject.Limits = builder.Build();
        return this;
    }

    public ResourcesBuilder WithReservations(ResourceSpec reservations)
    {
        WorkingObject.Reservations = reservations;
        return this;
    }

    public ResourcesBuilder WithReservations(Action<ResourceSpecBuilder> reservations)
    {
        var builder = new ResourceSpecBuilder();
        reservations(builder);
        WorkingObject.Reservations = builder.Build();
        return this;
    }
}
