using Ahmad.Mafia.Application.Contract.Room.Commands;
using Ahmad.Mafia.Domain.Room.Args;
using Ahmad.Mafia.Domain.Room.Enums;

namespace Ahmad.Mafia.Application.Room.Mapper;

public static class RoomMapper
{
    public static CreateRoomArg Map(this CreateRoomCommand command, long id, string roomCode, long hostPlayerId)
        => new(id, roomCode, hostPlayerId, command.HostNickname, command.Capacity,
            ParseVisibility(command.Visibility), ParseScenario(command.Scenario));

    private static RoomVisibility ParseVisibility(string? value)
        => Enum.TryParse<RoomVisibility>(value, ignoreCase: true, out var parsed) ? parsed : RoomVisibility.Private;

    private static ScenarioType ParseScenario(string? value)
        => Enum.TryParse<ScenarioType>(value, ignoreCase: true, out var parsed) ? parsed : ScenarioType.RussianMafia;
}
