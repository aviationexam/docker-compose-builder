using DockerComposeBuilder.Builders.Base;
using DockerComposeBuilder.Model.Services;
using System.Collections.Generic;

namespace DockerComposeBuilder.Builders;

public class DeviceReservationBuilder : BaseBuilder<DeviceReservationBuilder, DeviceReservation>
{
    internal DeviceReservationBuilder()
    {
    }

    public DeviceReservationBuilder WithCapabilities(params IReadOnlyCollection<string> capabilities)
    {
        (WorkingObject.Capabilities ??= []).AddRange(capabilities);
        return this;
    }

    public DeviceReservationBuilder WithDriver(string driver)
    {
        WorkingObject.Driver = driver;
        return this;
    }

    public DeviceReservationBuilder WithCount(int count)
    {
        WorkingObject.Count = count;
        return this;
    }

    public DeviceReservationBuilder WithAllDevices()
    {
        WorkingObject.Count = "all";
        return this;
    }

    public DeviceReservationBuilder WithDeviceIds(params IReadOnlyCollection<string> deviceIds)
    {
        (WorkingObject.DeviceIds ??= []).AddRange(deviceIds);
        return this;
    }

    public DeviceReservationBuilder WithOptions(IDictionary<string, object?> options)
    {
        WorkingObject.Options = options;
        return this;
    }

    public DeviceReservationBuilder WithOptions(params IReadOnlyCollection<string> options)
    {
        WorkingObject.Options = options;
        return this;
    }
}
