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

var host = builder.Build();

host.AddModules(Assembly.GetExecutingAssembly());
host.UseGatewayEventHandlers();

await host.RunAsync();