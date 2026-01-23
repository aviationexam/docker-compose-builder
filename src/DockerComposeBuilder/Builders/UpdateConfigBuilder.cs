using DockerComposeBuilder.Builders.Base;
using DockerComposeBuilder.Enums;
using DockerComposeBuilder.Model.Services;

namespace DockerComposeBuilder.Builders;

public class UpdateConfigBuilder : BaseBuilder<UpdateConfigBuilder, UpdateConfig>
{
    internal UpdateConfigBuilder()
    {
    }

    public UpdateConfigBuilder WithParallelism(int parallelism)
    {
        WorkingObject.Parallelism = parallelism;
        return this;
    }

    public UpdateConfigBuilder WithDelay(string delay)
    {
        WorkingObject.Delay = delay;
        return this;
    }

    public UpdateConfigBuilder WithFailureAction(EUpdateFailureAction failureAction)
    {
        WorkingObject.FailureAction = failureAction;
        return this;
    }

    public UpdateConfigBuilder WithMonitor(string monitor)
    {
        WorkingObject.Monitor = monitor;
        return this;
    }

    public UpdateConfigBuilder WithMaxFailureRatio(double maxFailureRatio)
    {
        WorkingObject.MaxFailureRatio = maxFailureRatio;
        return this;
    }

    public UpdateConfigBuilder WithOrder(EUpdateOrder order)
    {
        WorkingObject.Order = order;
        return this;
    }
}
