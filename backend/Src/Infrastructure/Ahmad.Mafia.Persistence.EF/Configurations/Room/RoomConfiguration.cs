using Ahmad.Mafia.Domain.Room.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomAggregate = Ahmad.Mafia.Domain.Room.Aggregates.Room;

namespace Ahmad.Mafia.Persistence.EF.Configurations.Room;

public sealed class RoomConfiguration : IEntityTypeConfiguration<RoomAggregate>
{
    public void Configure(EntityTypeBuilder<RoomAggregate> builder)
    {
        builder.ToTable("Rooms");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.RoomCode).HasMaxLength(10).IsRequired();
        builder.Property(x => x.Capacity).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();

        builder.Property(x => x.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.HasMany(x => x.Members)
               .WithOne()
               .HasForeignKey(x => x.RoomId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.RoomCode).IsUnique();
    }
}

public sealed class RoomMemberConfiguration : IEntityTypeConfiguration<RoomMember>
{
    public void Configure(EntityTypeBuilder<RoomMember> builder)
    {
        builder.ToTable("RoomMembers");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.RoomId).IsRequired();
        builder.Property(x => x.Nickname).HasMaxLength(50).IsRequired();
        builder.Property(x => x.IsHost).IsRequired();
        builder.Property(x => x.JoinedAtUtc).IsRequired();

        builder.HasIndex(x => x.RoomId);
    }
}
