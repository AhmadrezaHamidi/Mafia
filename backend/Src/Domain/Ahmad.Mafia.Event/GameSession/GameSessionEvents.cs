using AhmadBase.Doamin;

namespace Ahmad.Mafia.Domain.GameSession.Events;

public sealed record GameSessionStartedEvent(
    long GameSessionId,
    long RoomId,
    int PlayerCount
) : IEvent;

public sealed record NightActionSubmittedEvent(
    long GameSessionId,
    long ActorId,
    long TargetId
) : IEvent;

public sealed record NightPhaseResolvedEvent(
    long GameSessionId,
    int Round,
    long? EliminatedPlayerId,
    /// <summary>فقط وقتی هم‌زمان دو نفر یک شب حذف بشن (مثلاً مافیا + قاتل زنجیره‌ای مستقل).</summary>
    long? SecondEliminatedPlayerId = null
) : IEvent;

public sealed record VoteCastEvent(
    long GameSessionId,
    long VoterId,
    long TargetId
) : IEvent;

public sealed record DayPhaseResolvedEvent(
    long GameSessionId,
    int Round,
    long? EliminatedPlayerId
) : IEvent;

public sealed record GameEndedEvent(
    long GameSessionId,
    int WinningTeam
) : IEvent;
