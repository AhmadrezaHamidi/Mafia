namespace Ahmad.Mafia.Application.Contract.Identity.Services;

public interface IJwtService
{
    string GenerateToken(long playerId, string mobile, string displayName);
}

/// <summary>
/// ارسال کد به کاربر. پیاده‌سازی فعلی فقط لاگ می‌کند چون درگاه پیامکی وصل
/// نیست؛ وقتی وصل شد، فقط همین را عوض می‌کنیم و بقیه‌ی کد دست نمی‌خورد.
/// </summary>
public interface IOtpSender
{
    /// <summary>
    /// اگر true برگرداند یعنی کد واقعاً برای کاربر ارسال شده و نباید در پاسخ
    /// API لو برود. false یعنی نسخه‌ی آزمایشی است و کد باید در UI نشان داده شود.
    /// </summary>
    Task<bool> SendAsync(string mobile, string code, CancellationToken token = default);
}
