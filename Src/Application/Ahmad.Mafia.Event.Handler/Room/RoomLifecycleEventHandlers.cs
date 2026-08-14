namespace Ahmad.Mafia.Application.EventHandlers.Room;

public sealed class RoomCreatedEventHandler(ILogger<RoomCreatedEventHandler> logger)
    : IEventHandlerAsync<RoomCreatedEvent>
{
    public Task HandleAsync(RoomCreatedEvent @event, CancellationToken token)
    {
        logger.LogInformation("روم {RoomId} با کد {RoomCode} و ظرفیت {Capacity} ساخته شد.",
            @event.RoomId, @event.RoomCode, @event.Capacity);
        return Task.CompletedTask;
    }
}

public sealed class PlayerJoinedRoomEventHandler(ILogger<PlayerJoinedRoomEventHandler> logger)
    : IEventHandlerAsync<PlayerJoinedRoomEvent>
{
    public Task HandleAsync(PlayerJoinedRoomEvent @event, CancellationToken token)
    {
        logger.LogInformation("بازیکن {PlayerId} به روم {RoomId} پیوست ({CurrentCount}/{Capacity}).",
            @event.PlayerId, @event.RoomId, @event.CurrentCount, @event.Capacity);
        return Task.CompletedTask;
    }
}

public sealed class PlayerLeftRoomEventHandler(ILogger<PlayerLeftRoomEventHandler> logger)
    : IEventHandlerAsync<PlayerLeftRoomEvent>
{
    public Task HandleAsync(PlayerLeftRoomEvent @event, CancellationToken token)
    {
        logger.LogInformation("بازیکن {PlayerId} از روم {RoomId} خارج شد.", @event.PlayerId, @event.RoomId);
        return Task.CompletedTask;
    }
}

public sealed class RoomBecameReadyEventHandler(ILogger<RoomBecameReadyEventHandler> logger)
    : IEventHandlerAsync<RoomBecameReadyEvent>
{
    public Task HandleAsync(RoomBecameReadyEvent @event, CancellationToken token)
    {
        logger.LogInformation("روم {RoomId} پر شد و آماده‌ی شروعه.", @event.RoomId);
        return Task.CompletedTask;
    }
}

public sealed class RoomClosedEventHandler(ILogger<RoomClosedEventHandler> logger)
    : IEventHandlerAsync<RoomClosedEvent>
{
    public Task HandleAsync(RoomClosedEvent @event, CancellationToken token)
    {
        logger.LogInformation("روم {RoomId} بسته شد.", @event.RoomId);
        return Task.CompletedTask;
    }
}

public sealed class HostTransferredEventHandler(ILogger<HostTransferredEventHandler> logger)
    : IEventHandlerAsync<HostTransferredEvent>
{
    public Task HandleAsync(HostTransferredEvent @event, CancellationToken token)
    {
        logger.LogInformation("مالکیت روم {RoomId} به بازیکن {NewHostPlayerId} منتقل شد.",
            @event.RoomId, @event.NewHostPlayerId);
        return Task.CompletedTask;
    }
}
