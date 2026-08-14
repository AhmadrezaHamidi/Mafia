using AhmadBase.Doamin;
using Ahmad.Mafia.Domain.GameSession.Entities;
using Ahmad.Mafia.Domain.Room.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using RoomAggregate = Ahmad.Mafia.Domain.Room.Aggregates.Room;
using GameSessionAggregate = Ahmad.Mafia.Domain.GameSession.Aggregates.GameSession;

namespace Ahmad.Mafia.Persistence.EF;

public sealed class MafiaDbContext : DbContext, IUnitOfWork
{
    public MafiaDbContext(DbContextOptions<MafiaDbContext> options) : base(options) { }

    public DbSet<RoomAggregate> Rooms => Set<RoomAggregate>();
    public DbSet<RoomMember> RoomMembers => Set<RoomMember>();
    public DbSet<GameSessionAggregate> GameSessions => Set<GameSessionAggregate>();
    public DbSet<GamePlayer> GamePlayers => Set<GamePlayer>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(MafiaDbContext).Assembly);
    }

    public async Task<int> CommitAsync(CancellationToken token = default)
    {
        var aggregates = ChangeTracker
            .Entries<IAggregateRoot>()
            .Select(e => e.Entity)
            .Where(a => a.DomainEvents.Any())
            .ToList();

        var result = await SaveChangesAsync(token);

        foreach (var aggregate in aggregates)
        {
            foreach (var domainEvent in aggregate.DomainEvents)
            {
                // EventDispatcher.PublishAsync<T> اینجا اگه با نوع استاتیک IEvent صدا زده بشه،
                // AutofacEventHandlerFactory سعی می‌کنه handler واقعی (مثلاً IEventHandlerAsync<RoomCreatedEvent>) رو
                // به IEventHandlerAsync<IEvent> کست کنه که به خاطر contravariance شکست می‌خوره.
                // با dynamic، T بر اساس نوع واقعی runtime بایند می‌شه.
                await EventDispatcher.PublishAsync((dynamic)domainEvent, token);
            }

            aggregate.ClearDomainEvents();
        }

        return result;
    }
}

public class MafiaDbContextFactory : IDesignTimeDbContextFactory<MafiaDbContext>
{
    public MafiaDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MafiaDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\MSSQLLocalDB;Database=Ahmad.MafiaDb;Integrated Security=True;TrustServerCertificate=True;");
        return new MafiaDbContext(optionsBuilder.Options);
    }
}
