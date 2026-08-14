using AhmadBase.Application;
using System.Text.Json.Serialization;

namespace Ahmad.Mafia.Application.Contract.GameSession.Commands;

public record SubmitNightActionCommand(
    [property: JsonIgnore] long GameSessionId,
    long ActorId,
    long TargetId
) : ICommand<long>;
