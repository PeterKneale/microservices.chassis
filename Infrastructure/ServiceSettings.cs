namespace Microservices.Chassis.Infrastructure;

internal class ServiceSettings : IServiceSettings
{
    public ServiceSettings(string contentRootPath)
    {
        ContentRootPath = contentRootPath;
    }

    public string ContentRootPath { get; }
}