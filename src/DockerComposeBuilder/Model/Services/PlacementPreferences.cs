using DockerComposeBuilder.Model.Base;
using System;

namespace DockerComposeBuilder.Model.Services;

[Serializable]
public class PlacementPreference : ObjectBase
{
    public string Spread
    {
        get => GetProperty<string>("spread")!;
        set => SetProperty("spread", value);
    }
}
