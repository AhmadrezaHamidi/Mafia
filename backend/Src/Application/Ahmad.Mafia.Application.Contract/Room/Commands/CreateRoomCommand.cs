using AhmadBase.Application;

namespace Ahmad.Mafia.Application.Contract.Room.Commands;

public sealed record CreateRoomResult(long RoomId, string RoomCode, long HostPlayerId);

public record CreateRoomCommand(
    string HostNickname,
    int Capacity
) : ICommand<CreateRoomResult>;
