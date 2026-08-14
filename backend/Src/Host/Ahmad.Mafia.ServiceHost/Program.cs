using AhmadBase.Config.Applications;
using Ahmad.Mafia.ServiceHost;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<PhaseTimerBackgroundService>();
// SignalR برای چت بلادرنگ — باید قبل از ساخت app ثبت شود چون WebBuilder
// خودش Build/Run را انجام می‌دهد و بعدش دیگر به Services دسترسی نداریم.
builder.Services.AddSignalR();

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
