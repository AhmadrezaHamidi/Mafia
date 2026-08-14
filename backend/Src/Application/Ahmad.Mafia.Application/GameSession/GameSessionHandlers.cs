using Ahmad.Mafia.Application.Contract.GameSession.Commands;
using Ahmad.Mafia.Domain.GameSession.Exceptions;
using Ahmad.Mafia.Domain.Repositories;
using Ahmad.Mafia.Persistence.EF;

namespace Ahmad.Mafia.Application.Handlers;

public sealed class GameSessionHandlers(
    IGameSessionRepository repository,
    MafiaDbContext context) :
    ICommandHandler<SubmitNightActionCommand, long>,
    ICommandHandler<CastVoteCommand, long>,
    ICommandHandler<RetractVoteCommand, long>,
    ICommandHandler<RequestRematchCommand, long>,
    ICommandHandler<ResolveNightPhaseCommand, long>,
    ICommandHandler<ResolveVotingCommand, long>
{
    public async Task<long> Handle(SubmitNightActionCommand command, CancellationToken token)
    {
        var session = await GetOrThrow(command.GameSessionId, token);
        session.SubmitNightAction(command.ActorId, command.TargetId);
        await repository.UpdateAsync(session, token);
        await context.CommitAsync(token);
        return session.Id;
    }

    public async Task<long> Handle(CastVoteCommand command, CancellationToken token)
    {
        var session = await GetOrThrow(command.GameSessionId, token);
        session.CastVote(command.VoterId, command.TargetId);
        await repository.UpdateAsync(session, token);
        await context.CommitAsync(token);
        return session.Id;
    }

    public async Task<long> Handle(RetractVoteCommand command, CancellationToken token)
    {
        var session = await GetOrThrow(command.GameSessionId, token);
        session.RetractVote(command.VoterId);
        await repository.UpdateAsync(session, token);
        await context.CommitAsync(token);
        return session.Id;
    }

    public async Task<long> Handle(RequestRematchCommand command, CancellationToken token)
    {
        var session = await GetOrThrow(command.GameSessionId, token);
        session.RequestRematch();
        await repository.UpdateAsync(session, token);
        await context.CommitAsync(token);
        return session.Id;
    }

    public async Task<long> Handle(ResolveNightPhaseCommand command, CancellationToken token)
    {
        var session = await GetOrThrow(command.GameSessionId, token);
        session.ResolveNightPhase();
        await repository.UpdateAsync(session, token);
        await context.CommitAsync(token);
        return session.Id;
    }

    public async Task<long> Handle(ResolveVotingCommand command, CancellationToken token)
    {
        var session = await GetOrThrow(command.GameSessionId, token);
        session.ResolveVoting();
        await repository.UpdateAsync(session, token);
        await context.CommitAsync(token);
        return session.Id;
    }

    private async Task<Domain.GameSession.Aggregates.GameSession> GetOrThrow(long id, CancellationToken token)
        => await repository.GetByIdAsync(id, token) ?? throw new GameSessionNotFoundException();
}
