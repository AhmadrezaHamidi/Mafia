using AhmadBase.Doamin;

namespace Ahmad.Mafia.Domain.Identity.Exceptions;

public sealed class InvalidMobileException : BusinessException
{
    public InvalidMobileException() : base("شماره موبایل معتبر نیست. مثل ۰۹۱۲۳۴۵۶۷۸۹ وارد کن.") { }
}

public sealed class InvalidDisplayNameException : BusinessException
{
    public InvalidDisplayNameException() : base("نام نمایشی باید بین ۲ تا ۲۰ حرف باشد.") { }
}

public sealed class OtpNotFoundException : BusinessException
{
    public OtpNotFoundException() : base("کدی برای این شماره ارسال نشده. دوباره درخواست بده.") { }
}

public sealed class OtpExpiredException : BusinessException
{
    public OtpExpiredException() : base("کد منقضی شده. کد جدید بگیر.") { }
}

public sealed class OtpAlreadyUsedException : BusinessException
{
    public OtpAlreadyUsedException() : base("این کد قبلاً استفاده شده. کد جدید بگیر.") { }
}

public sealed class OtpTooManyAttemptsException : BusinessException
{
    public OtpTooManyAttemptsException() : base("تعداد تلاش‌های اشتباه زیاد شد. کد جدید بگیر.") { }
}

public sealed class InvalidOtpCodeException : BusinessException
{
    public InvalidOtpCodeException() : base("کد وارد شده درست نیست.") { }
}

/// <summary>پیام شامل ثانیه‌های باقی‌مانده است تا کاربر بداند چقدر صبر کند.</summary>
public sealed class OtpResendTooSoonException : BusinessException
{
    public OtpResendTooSoonException(int secondsRemaining)
        : base($"برای ارسال دوباره {secondsRemaining} ثانیه صبر کن.") { }
}
