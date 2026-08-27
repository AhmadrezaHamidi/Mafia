using System.Text;
using Ahmad.Mafia.Domain.Identity.Exceptions;

namespace Ahmad.Mafia.Domain.Identity.ValueObjects;

/// <summary>
/// نرمال‌سازی شماره موبایل ایران به شکل واحد «۹۸۹XXXXXXXXX».
///
/// چرا نرمال‌سازی لازم است: یک نفر ممکن است ۰۹۱۲…، +۹۸۹۱۲…، ۹۸۹۱۲… یا ۹۱۲…
/// وارد کند. اگر خام ذخیره کنیم، همان آدم چند حساب جدا می‌گیرد و OTPاش هم به
/// رکورد دیگری می‌خورد. ارقام فارسی/عربی هم تبدیل می‌شوند چون کیبورد فارسی
/// به‌طور پیش‌فرض همان‌ها را می‌فرستد.
/// </summary>
public static class MobileNumber
{
    private const string CountryCode = "98";

    public static string Normalize(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) throw new InvalidMobileException();

        var digits = ToLatinDigits(raw);

        // +989121234567 / 989121234567 → 9121234567
        if (digits.StartsWith(CountryCode, StringComparison.Ordinal) && digits.Length == 12)
            digits = digits[2..];
        // 09121234567 → 9121234567
        else if (digits.StartsWith('0') && digits.Length == 11)
            digits = digits[1..];

        // بعد از حذف پیشوندها باید دقیقاً ۱۰ رقم بماند که با ۹ شروع شود
        if (digits.Length != 10 || digits[0] != '9')
            throw new InvalidMobileException();

        return CountryCode + digits;
    }

    /// <summary>«۹۸۹۱۲۱۲۳۴۵۶۷» → «۰۹۱۲۱۲۳۴۵۶۷» — فقط برای نمایش.</summary>
    public static string ToLocalFormat(string normalized)
        => normalized.StartsWith(CountryCode, StringComparison.Ordinal)
            ? "0" + normalized[2..]
            : normalized;

    /// <summary>ارقام فارسی و عربی را به لاتین می‌برد و هر چیز دیگری را دور می‌ریزد.</summary>
    private static string ToLatinDigits(string input)
    {
        var sb = new StringBuilder(input.Length);
        foreach (var ch in input)
        {
            if (ch is >= '0' and <= '9') sb.Append(ch);
            else if (ch is >= '\u06F0' and <= '\u06F9') sb.Append((char)('0' + (ch - '\u06F0'))); // فارسی
            else if (ch is >= '\u0660' and <= '\u0669') sb.Append((char)('0' + (ch - '\u0660'))); // عربی
        }
        return sb.ToString();
    }
}
