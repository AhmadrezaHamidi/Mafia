using AhmadBase.Doamin;

namespace Ahmad.Mafia.Domain.Room.Events;

public sealed record RoomCreatedEvent(
    long RoomId,
    string RoomCode,
    long HostPlayerId,
    int Capacity
) : IEvent;

public sealed record PlayerJoinedRoomEvent(
    long RoomId,
    long PlayerId,
    string Nickname,
    int CurrentCount,
    int Capacity
) : IEvent;

public sealed record PlayerLeftRoomEvent(
    long RoomId,
    long PlayerId
) : IEvent;

public sealed record RoomBecameReadyEvent(
    long RoomId
) : IEvent;

public sealed record RoomGameStartedEvent(
    long RoomId
) : IEvent;

public sealed record RoomClosedEvent(
    long RoomId
) : IEvent;

public sealed record HostTransferredEvent(
    long RoomId,
    long NewHostPlayerId
) : IEvent;
