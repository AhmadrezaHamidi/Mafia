using AhmadBase.Application.Query;

namespace Ahmad.Mafia.Application.Query.Queries;

public sealed record RevealedPlayerResponse(long PlayerId, string Nickname, string Role, bool IsAlive);

/// <summary>فقط بعد از پایان بازی قابل‌دسترسیه — قبلش هندلر خطای 409 معادل می‌ده</summary>
public sealed record GetGameResultQueryResponse(
    long GameSessionId,
    string WinningTeam,
    IReadOnlyList<RevealedPlayerResponse> Reveal
);

public record GetGameResultQuery(long GameSessionId) : IQuery<GetGameResultQueryResponse>;
