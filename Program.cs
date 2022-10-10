var logs = LoggerFactory
    .Create(x =>
    {
        x.AddConsole();
    })
    .CreateLogger(typeof(Program));
logs.LogInformation("Chassis Starting");

var builder = WebApplication.CreateBuilder(args);

logs.LogInformation($"Configuration:\n{builder.Configuration.GetDebugView()}");

builder.Logging
    .AddConsole()
    .AddDebug();
builder
    .SetupChassisService(logs);

var app = builder.Build();
app.UseChassisService(logs);
app.Run();