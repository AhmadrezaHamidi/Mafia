using Ahmad.Mafia.Application.Contract.Room.Commands;
using Ahmad.Mafia.Domain.Room.Args;

namespace Ahmad.Mafia.Application.Room.Mapper;

public static class RoomMapper
{
    public static CreateRoomArg Map(this CreateRoomCommand command, long id, string roomCode, long hostPlayerId)
        => new(id, roomCode, hostPlayerId, command.HostNickname, command.Capacity);
}
