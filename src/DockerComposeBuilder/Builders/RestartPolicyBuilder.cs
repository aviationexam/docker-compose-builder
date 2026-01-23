using DockerComposeBuilder.Builders.Base;
using DockerComposeBuilder.Enums;
using DockerComposeBuilder.Model.Services;

namespace DockerComposeBuilder.Builders;

public class RestartPolicyBuilder : BaseBuilder<RestartPolicyBuilder, RestartPolicy>
{
    internal RestartPolicyBuilder()
    {
    }

    public RestartPolicyBuilder WithCondition(ERestartCondition condition)
    {
        WorkingObject.Condition = condition;
        return this;
    }

    public RestartPolicyBuilder WithDelay(string delay)
    {
        WorkingObject.Delay = delay;
        return this;
    }

    public RestartPolicyBuilder WithMaxAttempts(int maxAttempts)
    {
        WorkingObject.MaxAttempts = maxAttempts;
        return this;
    }

    public RestartPolicyBuilder WithWindow(string window)
    {
        WorkingObject.Window = window;
        return this;
    }
}
