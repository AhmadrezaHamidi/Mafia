using AhmadBase.Application;

namespace Ahmad.Mafia.Application.Contract.GameSession.Commands;

public record RequestRematchCommand(
    long GameSessionId
) : ICommand<long>;
