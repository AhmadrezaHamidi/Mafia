using Ahmad.Mafia.Application.Contract.Identity.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ahmad.Mafia.Persistence.EF.Services;

/// <summary>
/// نسخه‌ی آزمایشی: درگاه پیامکی وصل نیست، پس کد فقط در لاگ می‌نشیند و
/// false برمی‌گردد تا لایه‌ی بالاتر آن را در پاسخ API بگذارد و UI نشانش بدهد.
///
/// وقتی درگاه واقعی آمد، یک پیاده‌سازی تازه از IOtpSender بساز که true
/// برگرداند — همان‌جا کد از پاسخ API حذف می‌شود و جای دیگری تغییر نمی‌کند.
///
/// با Otp:ExposeCode=false می‌شود همین حالا هم جلوی برگشتنِ کد را گرفت
/// (کد آن‌وقت فقط در لاگ سرور می‌ماند).
/// </summary>
public sealed class LoggingOtpSender(
    IConfiguration configuration,
    ILogger<LoggingOtpSender> logger) : IOtpSender
{
    private readonly bool _exposeCode =
        !bool.TryParse(configuration["Otp:ExposeCode"], out var v) || v;

    public Task<bool> SendAsync(string mobile, string code, CancellationToken token = default)
    {
        logger.LogWarning("OTP برای {Mobile}: {Code} (درگاه پیامکی وصل نیست)", mobile, code);
        return Task.FromResult(!_exposeCode);
    }
}
