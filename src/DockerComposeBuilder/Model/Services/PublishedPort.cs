namespace DockerComposeBuilder.Model.Services;

public sealed class PublishedPort
{
    public int? PortAsInt { get; private set; }

    public string? PortAsString { get; private set; }

    public static implicit operator PublishedPort(int port) => new()
    {
        PortAsInt = port,
    };

    public static implicit operator PublishedPort(string port) => new()
    {
        PortAsString = port,
    };
}
