namespace Ahmad.Mafia.Domain.Room.Args;

public sealed record CreateRoomArg(
    long Id,
    string RoomCode,
    long HostPlayerId,
    string HostNickname,
    int Capacity
);

public sealed record JoinRoomArg(
    long PlayerId,
    string Nickname
);
