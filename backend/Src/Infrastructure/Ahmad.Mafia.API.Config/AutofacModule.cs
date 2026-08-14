using AhmadBase.Config.Containers;
using AhmadBase.IOC;
using Autofac;
using Ahmad.Mafia.Config.Middlewares;
using Ahmad.Mafia.Application.Validators;
using Ahmad.Mafia.Domain.Repositories;
using Ahmad.Mafia.Persistence.EF.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Ahmad.Mafia.Config;

public class AutofacModule : Autofac.Module
{
    private readonly IConfiguration _configuration;

    public AutofacModule(IConfiguration configuration) => _configuration = configuration;

    protected override void Load(ContainerBuilder builder)
    {
        // ── ۱. Framework Auto-Registration ────────────────────────────────────
        Container.Setup(builder, _configuration)
            .RegisterDatabaseModels()    // MafiaDbContext + HiLo
            .RegisterMainServices()
            .RegisterHandlers()          // CommandBus, QueryBus, EventHandlers
            .RegisterAuxiliaryServices();

        // ── ۲. Domain Repositories ─────────────────────────────────────────────
        builder.RegisterType<RoomRepository>().As<IRoomRepository>().InstancePerLifetimeScope();
        builder.RegisterType<GameSessionRepository>().As<IGameSessionRepository>().InstancePerLifetimeScope();

        // ── ۳. FluentValidation — validators + pipeline behavior ───────────────
        builder.RegisterType<ValidationExceptionStartupFilter>()
               .As<IStartupFilter>()
               .SingleInstance();

        builder.RegisterAssemblyTypes(typeof(ValidationBehavior<,>).Assembly)
               .AsClosedTypesOf(typeof(IValidator<>))
               .InstancePerLifetimeScope();

        builder.RegisterGeneric(typeof(ValidationBehavior<,>))
               .As(typeof(IPipelineBehavior<,>))
               .InstancePerLifetimeScope();
    }
}
