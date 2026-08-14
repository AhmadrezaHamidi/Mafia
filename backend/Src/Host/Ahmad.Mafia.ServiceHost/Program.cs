using AhmadBase.Config.Applications;
using Ahmad.Mafia.ServiceHost;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<PhaseTimerBackgroundService>();

await new WebBuilder(builder)
    .PrintApplicationName()
    .ConfigureEnvironmentVariable()
    .ConfigureSerilog()
    .ConfigureAutofac()
    .ConfigureController()
    .ConfigureSwagger()
    .ConfigureEndPoints()
    .ConfigureApiVersion()
    .ConfigureCors()
    .RunAsync();
