using Ahmad.Mafia.Domain.Repositories;
using GameSessionAgg = Ahmad.Mafia.Domain.GameSession.Aggregates.GameSession;

namespace Ahmad.Mafia.Application.Tests.Fakes;

public class FakeGameSessionRepository : IGameSessionRepository
{
    private readonly Dictionary<long, GameSessionAgg> _store = new();
    private long _nextId = 1;

    public GameSessionAgg? Added { get; private set; }
    public GameSessionAgg? Updated { get; private set; }

    public Task<GameSessionAgg?> GetByIdAsync(long id, CancellationToken token = default)
        => Task.FromResult(_store.GetValueOrDefault(id));

    public Task<GameSessionAgg?> GetByRoomIdAsync(long roomId, CancellationToken token = default)
        => Task.FromResult(_store.Values.FirstOrDefault(s => s.RoomId == roomId));

    public Task<List<GameSessionAgg>> GetSessionsPastDeadlineAsync(CancellationToken token = default)
        => Task.FromResult(_store.Values.Where(s => s.PhaseDeadlineUtc <= DateTime.UtcNow).ToList());

    public Task AddAsync(GameSessionAgg session, CancellationToken token = default)
    {
        Added = session;
        _store[session.Id] = session;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(GameSessionAgg session, CancellationToken token = default)
    {
        Updated = session;
        _store[session.Id] = session;
        return Task.CompletedTask;
    }

    public Task<long> GetNextIdAsync() => Task.FromResult(_nextId++);

    public void Seed(GameSessionAgg session) => _store[session.Id] = session;
}
