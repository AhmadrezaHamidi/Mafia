using Ahmad.Mafia.Application.Query.Queries;
using AhmadBase.Application.Query;
using Microsoft.AspNetCore.SignalR;

namespace Ahmad.Mafia.Rest.Hubs;

/// <summary>
/// چت بلادرنگ بازی.
///
/// قانون بنیادی (سند ۰۷): «سرور باید کانال را تعیین کند، نه client».
/// کلاینت هرگز نمی‌گوید در کدام thread است — فقط roomCode و playerId می‌فرستد
/// و سرور از روی وضعیت واقعی بازی (فاز، نقش، زنده بودن) تصمیم می‌گیرد.
/// اگر این را به کلاینت بسپاریم، بازیکن حذف‌شده می‌تواند در کانال زنده‌ها
/// حرف بزند و کل مکانیک مخفی‌کاری بازی بی‌اعتبار می‌شود.
/// </summary>
public sealed class ChatHub : Hub
{
    private readonly IQueryBus _queryBus;

    public ChatHub(IQueryBus queryBus) => _queryBus = queryBus;

    public const string LobbyThread = "lobby";
    public const string DayThread = "dayPublic";
    public const string NightMafiaThread = "nightMafia";
    public const string DeadThread = "deadChat";

    private static string Group(string roomCode, string thread) => $"{roomCode}:{thread}";

    /// <summary>
    /// عضویت در کانالِ مجاز. کلاینت بعد از هر تغییر فاز دوباره صدا می‌زند
    /// تا سرور عضویتش را بازمحاسبه کند (همان ForceChannelSwitch سند ۰۷).
    /// </summary>
    public async Task<string?> Join(string roomCode, long playerId)
    {
        var thread = await ResolveThreadAsync(roomCode, playerId);

        // از تمام گروه‌های قبلی این اتصال خارج شو تا بعد از مرگ یا تغییر فاز
        // پیام کانال قبلی به او نرسد
        foreach (var t in new[] { LobbyThread, DayThread, NightMafiaThread, DeadThread })
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Group(roomCode, t));

        if (thread is null) return null;   // این لحظه اجازه‌ی هیچ کانالی ندارد (مثلاً شهروند در شب)

        await Groups.AddToGroupAsync(Context.ConnectionId, Group(roomCode, thread));
        return thread;
    }

    /// <summary>ارسال پیام. thread دوباره سمت سرور محاسبه می‌شود، نه از ورودی کلاینت.</summary>
    public async Task Send(string roomCode, long playerId, string nickname, string text)
    {
        var body = (text ?? string.Empty).Trim();
        if (body.Length == 0) return;
        if (body.Length > 500) body = body[..500];

        var thread = await ResolveThreadAsync(roomCode, playerId);
        if (thread is null)
            throw new HubException("در این فاز اجازه‌ی ارسال پیام نداری.");

        await Clients.Group(Group(roomCode, thread)).SendAsync("message", new
        {
            id = Guid.NewGuid().ToString("N"),
            thread,
            senderId = playerId.ToString(),
            senderName = nickname,
            text = body,
            sentAtMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
        });
    }

    /// <summary>
    /// کانالِ مجاز این بازیکن در همین لحظه — تنها منبع حقیقت.
    /// null یعنی الان اجازه‌ی صحبت/شنیدن ندارد.
    /// </summary>
    private async Task<string?> ResolveThreadAsync(string roomCode, long playerId)
    {
        var room = await _queryBus.DispatchAsync<GetRoomQueryResponse>(
            new GetRoomQuery(roomCode), CancellationToken.None);

        if (room is null) return null;

        // هنوز بازی شروع نشده → همه در کانال عمومی لابی
        if (room.GameSessionId is not { } sessionId) return LobbyThread;

        var state = await _queryBus.DispatchAsync<GetGameStateQueryResponse>(
            new GetGameStateQuery(sessionId, playerId), CancellationToken.None);

        if (state is null) return LobbyThread;

        // حذف‌شده‌ها فقط با حذف‌شده‌ها — این قانون هیچ استثنایی ندارد
        if (!state.IAmAlive) return DeadThread;

        return state.Phase switch
        {
            "Night" => state.MyRole == "SimpleMafia" ? NightMafiaThread : null,
            "Day" or "Voting" => DayThread,
            _ => LobbyThread,
        };
    }
}
