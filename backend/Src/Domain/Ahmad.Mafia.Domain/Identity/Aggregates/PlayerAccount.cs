using AhmadBase.Doamin;
using Ahmad.Mafia.Domain.Identity.Args;
using Ahmad.Mafia.Domain.Identity.Exceptions;

namespace Ahmad.Mafia.Domain.Identity.Aggregates;

/// <summary>
/// حساب بازیکن — هویتی که به شماره موبایل تأیید‌شده گره خورده.
///
/// تا پیش از این، بازیکن فقط یک nickname تایپ‌شده بود و بین دو بازی هیچ چیز
/// از او نمی‌ماند. با این حساب، نام و سابقه‌اش به شماره‌اش وصل می‌شود.
/// </summary>
public sealed class PlayerAccount : AggregateRoot<long>
{
    public const int MinNameLength = 2;
    public const int MaxNameLength = 20;

    /// <summary>همیشه نرمال‌شده: ۹۸۹XXXXXXXXX</summary>
    public string Mobile { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime LastLoginAtUtc { get; private set; }

    private PlayerAccount() { }

    private PlayerAccount(RegisterPlayerArg arg) : base(arg.Id)
    {
        GuardDisplayName(arg.DisplayName);

        Mobile = arg.Mobile;
        DisplayName = arg.DisplayName.Trim();
        CreatedAtUtc = DateTime.UtcNow;
        LastLoginAtUtc = CreatedAtUtc;
    }

    public static PlayerAccount Register(RegisterPlayerArg arg) => new(arg);

    public void RecordLogin() => LastLoginAtUtc = DateTime.UtcNow;

    public void Rename(string displayName)
    {
        GuardDisplayName(displayName);
        DisplayName = displayName.Trim();
    }

    private static void GuardDisplayName(string? name)
    {
        var trimmed = name?.Trim();
        if (string.IsNullOrEmpty(trimmed) || trimmed.Length < MinNameLength || trimmed.Length > MaxNameLength)
            throw new InvalidDisplayNameException();
    }
}
