using Ahmad.Mafia.Domain.Identity.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ahmad.Mafia.Persistence.EF.Configurations.Identity;

public sealed class PlayerAccountConfiguration : IEntityTypeConfiguration<PlayerAccount>
{
    public void Configure(EntityTypeBuilder<PlayerAccount> builder)
    {
        builder.ToTable("PlayerAccounts");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Mobile).HasMaxLength(15).IsRequired();
        builder.Property(x => x.DisplayName).HasMaxLength(PlayerAccount.MaxNameLength).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.LastLoginAtUtc).IsRequired();

        // یکتا بودنش در دیتابیس تضمین می‌شود، نه فقط در کد: دو درخواست همزمان
        // می‌توانند هر دو «حساب نیست» ببینند و دو حساب برای یک شماره بسازند.
        builder.HasIndex(x => x.Mobile).IsUnique();
    }
}

public sealed class OtpChallengeConfiguration : IEntityTypeConfiguration<OtpChallenge>
{
    public void Configure(EntityTypeBuilder<OtpChallenge> builder)
    {
        builder.ToTable("OtpChallenges");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Mobile).HasMaxLength(15).IsRequired();
        builder.Property(x => x.CodeHash).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Salt).HasMaxLength(32).IsRequired();
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.ExpiresAtUtc).IsRequired();
        builder.Property(x => x.FailedAttempts).IsRequired();

        // پرس‌وجوی همیشگی «تازه‌ترین کد این شماره» است
        builder.HasIndex(x => new { x.Mobile, x.CreatedAtUtc });
    }
}
