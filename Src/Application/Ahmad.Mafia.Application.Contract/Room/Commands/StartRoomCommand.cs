using AhmadBase.Application;
using System.Text.Json.Serialization;

namespace Ahmad.Mafia.Application.Contract.Room.Commands;

public record StartRoomCommand(
    [property: JsonIgnore] long RoomId,
    long RequestingPlayerId
) : ICommand<long>;
