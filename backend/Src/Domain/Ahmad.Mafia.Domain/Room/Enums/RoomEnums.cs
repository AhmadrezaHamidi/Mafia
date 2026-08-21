namespace Ahmad.Mafia.Domain.Room.Enums;

public enum RoomStatus
{
    WaitingForPlayers = 0,
    ReadyToStart = 1,
    InProgress = 2,
    Closed = 3,
}

/// <summary>
/// عمومی: هرکسی با «بازی سریع» وارد صف matchmaking می‌شود و به یکی از روم‌های
/// عمومیِ در انتظار وصل می‌شود (یا اگر نبود، یکی ساخته می‌شود) — پر شدن ظرفیت
/// خودش یعنی شروع خودکار بازی، چون میزبانی برای کاربر بی‌معناست.
/// خصوصی: سازنده لینک/کد روم را برای دوستانش می‌فرستد و خودش با کلیک روی
/// «شروع بازی» تصمیم می‌گیرد بازی کی شروع شود.
/// </summary>
public enum RoomVisibility
{
    Private = 0,
    Public = 1,
}
