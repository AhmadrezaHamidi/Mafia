using AhmadBase.Application;

namespace Ahmad.Mafia.Application.Contract.Room.Commands;

public sealed record JoinRoomResult(long RoomId, long PlayerId);

public record JoinRoomCommand(
    string RoomCode,
    string Nickname
) : ICommand<JoinRoomResult>;
