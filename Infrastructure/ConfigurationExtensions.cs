namespace Microservices.Chassis.Infrastructure;

internal static class ConfigurationExtensions
{
    public static string GetServiceFile(this IConfiguration configuration)
    {
        var file = configuration["Service:File"];

        if (!File.Exists(file))
        {
            throw new Exception($"File does not exist '{file}'");
        }

        return Path.GetFullPath(file);
    }

    public static string GetServiceContentRootPath(this IConfiguration configuration)
    {
        var path = configuration["Service:ContentRootPath"];

        if (!Directory.Exists(path))
        {
            throw new Exception($"Directory does not exist '{path}'");
        }

        return Path.GetFullPath(path);
    }
}