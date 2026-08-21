namespace Ahmad.Mafia.Domain.GameSession.Enums;

public enum Role
{
    SimpleCitizen = 0,
    SimpleMafia = 1,
    /// <summary>سناریوی «شب‌های مافیا» — شب‌ها یک نفر رو نجات می‌ده؛ اگه هدفِ کشتنِ مافیا باشه، اون شب کسی نمی‌میره.</summary>
    Doctor = 2,
    /// <summary>سناریوی «شب‌های مافیا» — شب‌ها یک نفر رو استعلام می‌کنه؛ نتیجه فقط برای خودش نمایش داده می‌شه.</summary>
    Detective = 3,
    /// <summary>سناریوی «شب‌های مافیا» — رئیس ثابت تیم مافیاست؛ طبق قانون سناریوی اصلی، جلوی کارآگاه بی‌گناه دیده می‌شه.</summary>
    GodFather = 4,
    /// <summary>سناریوی «انتخابات شهر» — یک شهروند عادیه با یه امتیاز: روزها رأیش دو نفر حساب می‌شه.</summary>
    Mayor = 5,
    /// <summary>سناریوی «محافظ سایه» — هر شب از یک نفر محافظت می‌کنه؛ اگه مافیا همون رو هدف بگیره، به‌جاش خودِ بادیگارد کشته می‌شه.</summary>
    Bodyguard = 6,
    /// <summary>سناریوی «شکار روانی» — نقشی مستقل و بیرون از تیم مافیا/شهر؛ هر شب یک نفر رو می‌کشه، برنده‌ی تنها می‌خواد بمونه.</summary>
    SerialKiller = 7,
}

/// <summary>نوع اکشن شب — چون از فاز ۲ به بعد بیش از یک نقش قابلیت شب داره، دیگه فقط «کشتن» نیست.</summary>
public enum NightActionType
{
    Kill = 0,
    Save = 1,
    Investigate = 2,
    /// <summary>فقط بادیگارد — انتخاب کسی که امشب ازش محافظت می‌کنه.</summary>
    Guard = 3,
}

public enum GamePhase
{
    Night = 0,
    Day = 1,
    Ended = 2,
}

public enum WinningTeam
{
    None = 0,
    Town = 1,
    Mafia = 2,
    /// <summary>فقط سناریوی «شکار روانی» — قاتل زنجیره‌ای تنهای تنها موند و برد.</summary>
    SerialKiller = 3,
}

public enum ConnectionState
{
    Connected = 0,
    Disconnected = 1,
}
