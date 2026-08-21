using AhmadBase.Application;

namespace Ahmad.Mafia.Application.Contract.Room.Commands;

public sealed record QuickJoinResult(long RoomId, string RoomCode, long PlayerId, bool IsHost);

/// <summary>
/// «بازی سریع» — matchmaking عمومی. کاربر فقط nickname می‌فرستد؛ سرور یا او را به
/// یک روم عمومیِ منتظر وصل می‌کند یا یکی می‌سازد. پر شدن ظرفیت یعنی شروع خودکار
/// (بدون نیاز به کلیک Host) — چون در این حالت میزبانی برای کاربر مفهومی ندارد.
/// </summary>
public record QuickJoinCommand(
    string Nickname
) : ICommand<QuickJoinResult>;
