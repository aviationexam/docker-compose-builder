using System;
using System.Collections.Generic;

namespace DockerComposeBuilder.Model.Services;

/// <summary>A typed Compose mapping that retains entries beyond the modeled schema.</summary>
[Serializable]
public abstract class DeployMapping
{
    // Keep the original value if it cannot be represented by its typed property
    // (e.g. "${PARALLELISM}"). A subsequent typed edit takes precedence.
    internal Dictionary<string, (object? Original, object? Projection)> Extra { get; } = new();
}
