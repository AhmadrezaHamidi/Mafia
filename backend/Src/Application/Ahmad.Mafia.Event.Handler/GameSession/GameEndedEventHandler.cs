namespace Ahmad.Mafia.Application.EventHandlers.GameSession;

public sealed class GameEndedEventHandler(ILogger<GameEndedEventHandler> logger)
    : IEventHandlerAsync<GameEndedEvent>
{
    public Task HandleAsync(GameEndedEvent @event, CancellationToken token)
    {
        logger.LogInformation("بازی {GameSessionId} تمام شد — تیم برنده: {WinningTeam}",
            @event.GameSessionId, @event.WinningTeam);
        return Task.CompletedTask;
    }
}
