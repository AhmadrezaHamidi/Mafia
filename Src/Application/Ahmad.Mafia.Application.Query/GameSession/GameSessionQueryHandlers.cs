using Ahmad.Mafia.Domain.GameSession.Enums;
using Ahmad.Mafia.Domain.GameSession.Exceptions;

namespace Ahmad.Mafia.Application.Query.Handlers;

public sealed class GameSessionQueryHandlers(IGameSessionRepository repository) :
    IQueryHandler<GetGameStateQuery, GetGameStateQueryResponse>,
    IQueryHandler<GetGameResultQuery, GetGameResultQueryResponse>
{
    public async Task<GetGameStateQueryResponse> HandleAsync(GetGameStateQuery query, CancellationToken token)
    {
        var session = await repository.GetByIdAsync(query.GameSessionId, token)
            ?? throw new GameSessionNotFoundException();

        return session.ToStateResponse(query.RequestingPlayerId);
    }

    public async Task<GetGameResultQueryResponse> HandleAsync(GetGameResultQuery query, CancellationToken token)
    {
        var session = await repository.GetByIdAsync(query.GameSessionId, token)
            ?? throw new GameSessionNotFoundException();

        if (session.Phase != GamePhase.Ended)
            throw new GameNotEndedException();

        return session.ToResultResponse();
    }
}
