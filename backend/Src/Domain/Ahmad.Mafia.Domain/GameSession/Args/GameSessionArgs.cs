using Ahmad.Mafia.Domain.Room.Enums;

namespace Ahmad.Mafia.Domain.GameSession.Args;

public sealed record GamePlayerSeed(long PlayerId, string Nickname);

public sealed record CreateGameSessionArg(
    long Id,
    long RoomId,
    IReadOnlyList<GamePlayerSeed> Players,
    ScenarioType Scenario = ScenarioType.RussianMafia,
    int NightDurationSeconds = 45,
    int DayDurationSeconds = 90
);
