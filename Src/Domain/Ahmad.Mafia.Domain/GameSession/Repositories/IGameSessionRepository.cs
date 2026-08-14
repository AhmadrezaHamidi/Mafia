using GameSessionAgg = Ahmad.Mafia.Domain.GameSession.Aggregates;

namespace Ahmad.Mafia.Domain.Repositories;

public interface IGameSessionRepository
{
    Task<GameSessionAgg.GameSession?> GetByIdAsync(long id, CancellationToken token = default);
    Task<GameSessionAgg.GameSession?> GetByRoomIdAsync(long roomId, CancellationToken token = default);
    Task<List<GameSessionAgg.GameSession>> GetSessionsPastDeadlineAsync(CancellationToken token = default);
    Task AddAsync(GameSessionAgg.GameSession session, CancellationToken token = default);
    Task UpdateAsync(GameSessionAgg.GameSession session, CancellationToken token = default);
    Task<long> GetNextIdAsync();
}
