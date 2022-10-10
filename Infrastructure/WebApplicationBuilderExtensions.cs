using System.Reflection;

namespace Microservices.Chassis.Infrastructure;

public static class WebApplicationBuilderExtensions
{
    public static void SetupChassisService(this WebApplicationBuilder builder, ILogger logs)
    {
        logs.LogInformation("Loading settings");
        var settings = LoadSettings(builder.Configuration);

        var file = builder.Configuration.GetServiceFile();
        
        logs.LogInformation($"Loading service assembly {file}");
        var assembly = LoadAssembly(file);

        logs.LogInformation($"Getting entrypoint type from service assembly: {assembly}");
        var type = GetEntryPointType(assembly);

        logs.LogInformation($"Getting service instance from service type: {type}");
        var instance = GetEntryPointInstance(type);

        logs.LogInformation($"Configuring web app builder for instance {instance}");
        instance.ConfigureWebApplicationBuilder(builder, settings);

        builder.Services.AddSingleton(instance);
    }

    public static void UseChassisService(this WebApplication app, ILogger logs)
    {
        var service = app.Services.GetRequiredService<IServiceEntryPoint>();
        if (service == null)
        {
            throw new Exception("No service can be found in the container");
        }

        logs.LogInformation($"Configuring web app for instance {service}");

        service.ConfigureWebApplication(app);
    }

    private static IServiceSettings LoadSettings(IConfiguration configuration)
    {
        var content = configuration.GetServiceContentRootPath();
        return new ServiceSettings(content);
    }

    private static Assembly LoadAssembly(string file)
    {
        return Assembly.LoadFrom(file);
    }

    private static IServiceEntryPoint GetEntryPointInstance(Type type)
    {
        if (Activator.CreateInstance(type) is not IServiceEntryPoint instance)
        {
            throw new Exception($"Entrypoint cannot be instantiated from {type}");
        }

        return instance;
    }

    private static Type GetEntryPointType(Assembly assembly)
    {
        // The predicate used to identify the entrypoint
        Func<Type, bool> IsEntryPoint()
        {
            return type => typeof(IServiceEntryPoint).IsAssignableFrom(type);
        }

        // Find the entrypoint of the service
        var types = assembly
            .GetTypes()
            .Where(IsEntryPoint())
            .ToList();

        if (!types.Any())
        {
            throw new Exception($"No Entrypoint found in {assembly}");
        }

        if (types.Count > 1)
        {
            throw new Exception($"More than one entrypoint found in {assembly}");
        }

        return types.Single();
    }
}