using AhmadBase.Application;

namespace Ahmad.Mafia.Application.Contract.Identity.Commands;

/// <summary>
/// پاسخ درخواست کد.
///
/// <para><b>IsRegistered</b> به فرانت می‌گوید در مرحله‌ی بعد فیلد «نام نمایشی»
/// را نشان بدهد یا نه — تا کاربرِ قدیمی مجبور نشود هر بار اسمش را بنویسد.</para>
///
/// <para><b>DemoCode</b> فقط در نسخه‌ی آزمایشی پر می‌شود (وقتی درگاه پیامک
/// واقعی وصل نیست) تا بشود بدون پیامک وارد شد. در production باید null باشد.</para>
/// </summary>
public sealed record RequestOtpResult(
    string Mobile,
    int ExpiresInSeconds,
    int ResendAfterSeconds,
    bool IsRegistered,
    string? DemoCode
);

public record RequestOtpCommand(string Mobile) : ICommand<RequestOtpResult>;

public sealed record VerifyOtpResult(
    long PlayerId,
    string Mobile,
    string DisplayName,
    bool IsNewAccount,
    string Token
);

/// <summary>
/// DisplayName فقط برای حساب تازه لازم است؛ برای حسابِ موجود نادیده گرفته
/// می‌شود تا کسی نتواند با یک بار ورود، اسم حساب را عوض کند.
/// </summary>
public record VerifyOtpCommand(
    string Mobile,
    string Code,
    string? DisplayName = null
) : ICommand<VerifyOtpResult>;
