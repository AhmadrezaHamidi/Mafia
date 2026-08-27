using AhmadBase.Doamin;
using Ahmad.Mafia.Domain.Identity.Args;
using Ahmad.Mafia.Domain.Identity.Exceptions;

namespace Ahmad.Mafia.Domain.Identity.Aggregates;

/// <summary>
/// یک درخواست کد یک‌بارمصرف برای یک شماره.
///
/// کد خام ذخیره نمی‌شود؛ فقط هش با نمکِ مخصوص همین رکورد. کد ۶ رقمی ذاتاً
/// فضای کوچکی دارد و هش به‌تنهایی امنش نمی‌کند — چیزی که امنش می‌کند عمر
/// کوتاه و سقف تلاش است. هش فقط جلوی خواندنِ مستقیم از دیتابیس را می‌گیرد.
/// </summary>
public sealed class OtpChallenge : AggregateRoot<long>
{
    /// <summary>عمر کد. کوتاه است چون سقف تلاش تنها محافظ دیگر است.</summary>
    public const int LifetimeSeconds = 120;

    /// <summary>فاصله‌ی لازم تا ارسال دوباره — جلوی بمباران پیامک را می‌گیرد.</summary>
    public const int ResendCooldownSeconds = 60;

    /// <summary>بعد از این تعداد اشتباه، کد می‌سوزد و باید کد تازه گرفت.</summary>
    public const int MaxFailedAttempts = 5;

    public string Mobile { get; private set; } = string.Empty;
    public string CodeHash { get; private set; } = string.Empty;
    public string Salt { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime? ConsumedAtUtc { get; private set; }
    public int FailedAttempts { get; private set; }

    private OtpChallenge() { }

    private OtpChallenge(IssueOtpArg arg) : base(arg.Id)
    {
        Mobile = arg.Mobile;
        CodeHash = arg.CodeHash;
        Salt = arg.Salt;
        CreatedAtUtc = arg.NowUtc;
        ExpiresAtUtc = arg.NowUtc.AddSeconds(LifetimeSeconds);
    }

    public static OtpChallenge Issue(IssueOtpArg arg) => new(arg);

    public bool IsUsable(DateTime nowUtc)
        => ConsumedAtUtc is null && nowUtc < ExpiresAtUtc && FailedAttempts < MaxFailedAttempts;

    /// <summary>ثانیه‌های باقی‌مانده تا اجازه‌ی ارسال دوباره؛ صفر یعنی همین حالا.</summary>
    public int SecondsUntilResendAllowed(DateTime nowUtc)
    {
        var elapsed = (nowUtc - CreatedAtUtc).TotalSeconds;
        var remaining = ResendCooldownSeconds - elapsed;
        return remaining <= 0 ? 0 : (int)Math.Ceiling(remaining);
    }

    public int SecondsUntilExpiry(DateTime nowUtc)
    {
        var remaining = (ExpiresAtUtc - nowUtc).TotalSeconds;
        return remaining <= 0 ? 0 : (int)Math.Ceiling(remaining);
    }

    /// <summary>
    /// هشِ کد ورودی را می‌سنجد. اشتباه که باشد، شمارنده بالا می‌رود و رکورد
    /// باید ذخیره شود — پس فراخوان در هر دو حالت باید commit کند، نه فقط موفق.
    /// </summary>
    public void Verify(string candidateHash, DateTime nowUtc)
    {
        if (ConsumedAtUtc is not null) throw new OtpAlreadyUsedException();
        if (nowUtc >= ExpiresAtUtc) throw new OtpExpiredException();
        if (FailedAttempts >= MaxFailedAttempts) throw new OtpTooManyAttemptsException();

        if (!FixedTimeEquals(CodeHash, candidateHash))
        {
            FailedAttempts++;
            throw new InvalidOtpCodeException();
        }

        ConsumedAtUtc = nowUtc;
    }

    /// <summary>
    /// مقایسه‌ی ثابت‌زمان. با == معمولی، زمانِ مقایسه به تعداد کاراکترهای درست
    /// وابسته می‌شد و در تئوری قابل اندازه‌گیری است.
    /// </summary>
    private static bool FixedTimeEquals(string a, string b)
    {
        if (a.Length != b.Length) return false;
        var diff = 0;
        for (var i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
        return diff == 0;
    }
}
