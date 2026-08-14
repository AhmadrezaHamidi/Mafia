using Ahmad.Mafia.Domain.GameSession.Args;
using Ahmad.Mafia.Domain.Room.Exceptions;
using Ahmad.Mafia.Persistence.EF;
using GameSessionAgg = Ahmad.Mafia.Domain.GameSession.Aggregates.GameSession;

namespace Ahmad.Mafia.Application.EventHandlers.Room;

/// <summary>
/// وقتی Room.Start() صدا زده می‌شود و RoomGameStartedEvent منتشر می‌شود
/// → یک GameSession جدید از روی اعضای روم ساخته می‌شود (نقش‌ها همینجا تصادفی تخصیص می‌یابند)
/// </summary>
public sealed class RoomGameStartedEventHandler(
    IRoomRepository roomRepository,
    IGameSessionRepository gameSessionRepository,
    MafiaDbContext context,
    ILogger<RoomGameStartedEventHandler> logger)
    : IEventHandlerAsync<RoomGameStartedEvent>
{
    public async Task HandleAsync(RoomGameStartedEvent @event, CancellationToken token)
    {
        var room = await roomRepository.GetByIdAsync(@event.RoomId, token)
            ?? throw new RoomNotFoundException();

        var sessionId = await gameSessionRepository.GetNextIdAsync();
        var players = room.Members
            .Select(m => new GamePlayerSeed(m.Id, m.Nickname))
            .ToList();

        var session = GameSessionAgg.Create(new CreateGameSessionArg(sessionId, room.Id, players));

        await gameSessionRepository.AddAsync(session, token);
        await context.CommitAsync(token);

        logger.LogInformation("بازی روم {RoomId} شروع شد — GameSession {GameSessionId} با {PlayerCount} بازیکن ساخته شد.",
            room.Id, session.Id, players.Count);
    }
}
