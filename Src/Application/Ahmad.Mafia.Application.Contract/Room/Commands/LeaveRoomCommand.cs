using AhmadBase.Application;
using System.Text.Json.Serialization;

namespace Ahmad.Mafia.Application.Contract.Room.Commands;

public record LeaveRoomCommand(
    [property: JsonIgnore] long RoomId,
    long PlayerId
) : ICommand<long>;
