using AhmadBase.Application;
using System.Text.Json.Serialization;

namespace Ahmad.Mafia.Application.Contract.GameSession.Commands;

public record CastVoteCommand(
    [property: JsonIgnore] long GameSessionId,
    long VoterId,
    long TargetId
) : ICommand<long>;
