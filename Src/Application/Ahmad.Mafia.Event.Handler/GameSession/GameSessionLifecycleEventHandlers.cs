namespace Ahmad.Mafia.Application.EventHandlers.GameSession;

public sealed class GameSessionStartedEventHandler(ILogger<GameSessionStartedEventHandler> logger)
    : IEventHandlerAsync<GameSessionStartedEvent>
{
    public Task HandleAsync(GameSessionStartedEvent @event, CancellationToken token)
    {
        logger.LogInformation("GameSession {GameSessionId} برای روم {RoomId} با {PlayerCount} بازیکن شروع شد.",
            @event.GameSessionId, @event.RoomId, @event.PlayerCount);
        return Task.CompletedTask;
    }
}

public sealed class NightActionSubmittedEventHandler(ILogger<NightActionSubmittedEventHandler> logger)
    : IEventHandlerAsync<NightActionSubmittedEvent>
{
    public Task HandleAsync(NightActionSubmittedEvent @event, CancellationToken token)
    {
        logger.LogInformation("بازی {GameSessionId}: بازیکن {ActorId} هدف {TargetId} رو انتخاب کرد.",
            @event.GameSessionId, @event.ActorId, @event.TargetId);
        return Task.CompletedTask;
    }
}

public sealed class NightPhaseResolvedEventHandler(ILogger<NightPhaseResolvedEventHandler> logger)
    : IEventHandlerAsync<NightPhaseResolvedEvent>
{
    public Task HandleAsync(NightPhaseResolvedEvent @event, CancellationToken token)
    {
        logger.LogInformation("بازی {GameSessionId}: پایان شب راند {Round} — حذف‌شده: {EliminatedPlayerId}.",
            @event.GameSessionId, @event.Round, @event.EliminatedPlayerId);
        return Task.CompletedTask;
    }
}

public sealed class VoteCastEventHandler(ILogger<VoteCastEventHandler> logger)
    : IEventHandlerAsync<VoteCastEvent>
{
    public Task HandleAsync(VoteCastEvent @event, CancellationToken token)
    {
        logger.LogInformation("بازی {GameSessionId}: بازیکن {VoterId} به {TargetId} رأی داد.",
            @event.GameSessionId, @event.VoterId, @event.TargetId);
        return Task.CompletedTask;
    }
}

public sealed class DayPhaseResolvedEventHandler(ILogger<DayPhaseResolvedEventHandler> logger)
    : IEventHandlerAsync<DayPhaseResolvedEvent>
{
    public Task HandleAsync(DayPhaseResolvedEvent @event, CancellationToken token)
    {
        logger.LogInformation("بازی {GameSessionId}: پایان رأی‌گیری راند {Round} — حذف‌شده: {EliminatedPlayerId}.",
            @event.GameSessionId, @event.Round, @event.EliminatedPlayerId);
        return Task.CompletedTask;
    }
}
