using System.Text.Json;
using Ahmad.Mafia.Domain.GameSession.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GameSessionAggregate = Ahmad.Mafia.Domain.GameSession.Aggregates.GameSession;

namespace Ahmad.Mafia.Persistence.EF.Configurations.GameSession;

public sealed class GameSessionConfiguration : IEntityTypeConfiguration<GameSessionAggregate>
{
    public void Configure(EntityTypeBuilder<GameSessionAggregate> builder)
    {
        builder.ToTable("GameSessions");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.RoomId).IsRequired();
        builder.Property(x => x.Round).IsRequired();
        builder.Property(x => x.PhaseDeadlineUtc).IsRequired();
        builder.Property(x => x.NightDurationSeconds).IsRequired();
        builder.Property(x => x.DayDurationSeconds).IsRequired();
        builder.Property(x => x.NightTargetPlayerId);

        builder.Property(x => x.Phase).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.WinningTeam).HasConversion<string>().HasMaxLength(20).IsRequired();

        var votesComparer = new ValueComparer<Dictionary<long, long>>(
            (a, b) => (a ?? new()).OrderBy(kv => kv.Key).SequenceEqual((b ?? new()).OrderBy(kv => kv.Key)),
            v => v.Aggregate(0, (hash, kv) => HashCode.Combine(hash, kv.Key, kv.Value)),
            v => new Dictionary<long, long>(v));

        builder.Property<Dictionary<long, long>>("_votes")
               .HasField("_votes")
               .UsePropertyAccessMode(PropertyAccessMode.Field)
               .HasConversion(
                   v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                   v => JsonSerializer.Deserialize<Dictionary<long, long>>(v, (JsonSerializerOptions?)null) ?? new())
               .Metadata.SetValueComparer(votesComparer);

        builder.Property<Dictionary<long, long>>("_votes").HasColumnName("VotesJson").HasMaxLength(4000);

        builder.HasMany(x => x.Players)
               .WithOne()
               .HasForeignKey(x => x.GameSessionId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.RoomId);
    }
}

public sealed class GamePlayerConfiguration : IEntityTypeConfiguration<GamePlayer>
{
    public void Configure(EntityTypeBuilder<GamePlayer> builder)
    {
        builder.ToTable("GamePlayers");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.GameSessionId).IsRequired();
        builder.Property(x => x.Nickname).HasMaxLength(50).IsRequired();
        builder.Property(x => x.IsAlive).IsRequired();
        builder.Property(x => x.IsMafiaLeader).IsRequired();

        builder.Property(x => x.Role).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.Connection).HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.HasIndex(x => x.GameSessionId);
    }
}
