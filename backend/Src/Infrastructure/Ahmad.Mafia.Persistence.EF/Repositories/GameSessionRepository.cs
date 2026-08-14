using Ahmad.Mafia.Domain.GameSession.Enums;
using Ahmad.Mafia.Domain.Repositories;
using AhmadBase.Persistence.NHiLoHelper;
using Microsoft.EntityFrameworkCore;
using GameSessionAggregate = Ahmad.Mafia.Domain.GameSession.Aggregates.GameSession;

namespace Ahmad.Mafia.Persistence.EF.Repositories;

public sealed class GameSessionRepository(
    MafiaDbContext context,
    IHiLoIdGenerator hiLoGenerator) : IGameSessionRepository
{
    public async Task<GameSessionAggregate?> GetByIdAsync(long id, CancellationToken token = default)
        => await context.GameSessions.Include(x => x.Players).FirstOrDefaultAsync(x => x.Id == id, token);

    public async Task<GameSessionAggregate?> GetByRoomIdAsync(long roomId, CancellationToken token = default)
        => await context.GameSessions.Include(x => x.Players).FirstOrDefaultAsync(x => x.RoomId == roomId, token);

    public async Task<List<GameSessionAggregate>> GetSessionsPastDeadlineAsync(CancellationToken token = default)
        => await context.GameSessions
            .Include(x => x.Players)
            .Where(x => x.Phase != GamePhase.Ended && x.PhaseDeadlineUtc <= DateTime.UtcNow)
            .ToListAsync(token);

    public async Task AddAsync(GameSessionAggregate session, CancellationToken token = default)
        => await context.GameSessions.AddAsync(session, token);

    public Task UpdateAsync(GameSessionAggregate session, CancellationToken token = default)
    {
        context.GameSessions.Update(session);
        return Task.CompletedTask;
    }

    public Task<long> GetNextIdAsync()
        => Task.FromResult(hiLoGenerator.GetNextId<GameSessionAggregate>());
}
