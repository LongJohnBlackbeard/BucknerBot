using System.Reflection;
using BucknerBot;
using BucknerBot.Discord;
using BucknerBot.ElevenLabs;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;

var builder = Host.CreateApplicationBuilder(args);

// Add Services
builder.Services.AddSingleton<ElevenLabsTtsService>();
builder.Services.AddSingleton<TtsQueueService>();
builder.Services.AddDiscordGateway();
builder.Services.AddApplicationCommands();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var host = builder.Build();

try
{
    host.AddModules(Assembly.GetExecutingAssembly());
    host.UseGatewayEventHandlers();

    await host.RunAsync();
}
catch (Exception ex)
{
    var logger = host.Services.GetRequiredService<ILogger<Program>>();
    logger.LogCritical(ex, "Application failed to start.");
    throw;
}