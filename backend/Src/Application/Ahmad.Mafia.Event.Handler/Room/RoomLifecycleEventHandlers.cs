using Ahmad.Mafia.Domain.Room.Enums;
using Ahmad.Mafia.Persistence.EF;

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

/// <summary>
/// وقتی روم پر می‌شود: اگه عمومی بود خودمون Start رو صدا می‌زنیم (میزبانی برای
/// کاربرِ matchmaking عمومی مفهومی نداره که منتظر کلیک اون بمونیم)؛ روم خصوصی
/// مثل قبل با کلیک دستی Host شروع می‌شه.
/// </summary>
public sealed class RoomBecameReadyEventHandler(
    IRoomRepository roomRepository,
    MafiaDbContext context,
    ILogger<RoomBecameReadyEventHandler> logger)
    : IEventHandlerAsync<RoomBecameReadyEvent>
{
    public async Task HandleAsync(RoomBecameReadyEvent @event, CancellationToken token)
    {
        logger.LogInformation("روم {RoomId} پر شد و آماده‌ی شروعه.", @event.RoomId);

        var room = await roomRepository.GetByIdAsync(@event.RoomId, token);
        if (room is null || room.Visibility != RoomVisibility.Public) return;
        if (room.HostPlayerId is not { } hostPlayerId) return;

        room.Start(hostPlayerId);
        await roomRepository.UpdateAsync(room, token);
        await context.CommitAsync(token);

        logger.LogInformation("روم عمومی {RoomId} خودکار شروع شد.", @event.RoomId);
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
