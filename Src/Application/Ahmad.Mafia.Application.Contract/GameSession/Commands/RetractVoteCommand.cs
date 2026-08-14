using AhmadBase.Application;
using System.Text.Json.Serialization;

namespace Ahmad.Mafia.Application.Contract.GameSession.Commands;

public record RetractVoteCommand(
    [property: JsonIgnore] long GameSessionId,
    long VoterId
) : ICommand<long>;
