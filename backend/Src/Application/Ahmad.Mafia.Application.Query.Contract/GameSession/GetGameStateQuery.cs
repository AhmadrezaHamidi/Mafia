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
public sealed record InvestigationResultView(long TargetId, bool IsMafia);

public sealed record GetGameStateQueryResponse(
    long GameSessionId,
    string Scenario,
    string Phase,
    int Round,
    int TimeLeftSeconds,
    string? MyRole,
    bool IAmAlive,
    bool? MyIsMafiaLeader,
    long? MyNightTarget,
    /// <summary>فقط برای دکتر — هدفی که این شب برای نجات انتخاب کرده.</summary>
    long? MyNightSaveTarget,
    /// <summary>فقط برای کارآگاه — هدفی که این شب برای استعلام انتخاب کرده.</summary>
    long? MyNightInvestigateTarget,
    /// <summary>فقط برای کارآگاه — نتیجه‌ی آخرین استعلامی که تا الان انجام داده (بین شب‌ها هم می‌مونه).</summary>
    InvestigationResultView? MyLastInvestigation,
    IReadOnlyList<GamePlayerView> Players,
    IReadOnlyDictionary<long, long>? Votes
);

public record GetGameStateQuery(long GameSessionId, long RequestingPlayerId) : IQuery<GetGameStateQueryResponse>;
