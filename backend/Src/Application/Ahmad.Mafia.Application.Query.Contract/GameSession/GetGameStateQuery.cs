using AhmadBase.Application.Query;

namespace Ahmad.Mafia.Application.Query.Queries;

public sealed record GamePlayerView(
    long PlayerId,
    string Nickname,
    bool IsAlive,
    string Connection
);

/// <summary>
/// State بازی فیلترشده برای یک بازیکن خاص — نقش بقیه‌ی بازیکنان هرگز در این DTO وجود ندارد،
/// فقط نقش خودِ درخواست‌دهنده (MyRole) برگردانده می‌شود.
/// </summary>
public sealed record GetGameStateQueryResponse(
    long GameSessionId,
    string Phase,
    int Round,
    int TimeLeftSeconds,
    string? MyRole,
    bool IAmAlive,
    bool? MyIsMafiaLeader,
    long? MyNightTarget,
    IReadOnlyList<GamePlayerView> Players,
    IReadOnlyDictionary<long, long>? Votes
);

public record GetGameStateQuery(long GameSessionId, long RequestingPlayerId) : IQuery<GetGameStateQueryResponse>;
