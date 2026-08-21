using Ahmad.Mafia.Domain.Room.Enums;

namespace Ahmad.Mafia.Domain.Room.Args;

public sealed record CreateRoomArg(
    long Id,
    string RoomCode,
    long HostPlayerId,
    string HostNickname,
    int Capacity,
    RoomVisibility Visibility = RoomVisibility.Private,
    ScenarioType Scenario = ScenarioType.RussianMafia
);

public sealed record JoinRoomArg(
    long PlayerId,
    string Nickname
);
