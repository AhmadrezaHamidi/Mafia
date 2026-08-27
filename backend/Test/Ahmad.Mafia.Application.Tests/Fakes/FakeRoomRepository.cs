using Ahmad.Mafia.Domain.Repositories;
using Ahmad.Mafia.Domain.Room.Enums;
using RoomAgg = Ahmad.Mafia.Domain.Room.Aggregates.Room;

namespace Ahmad.Mafia.Application.Tests.Fakes;

public class FakeRoomRepository : IRoomRepository
{
    private readonly Dictionary<long, RoomAgg> _store = new();
    private long _nextId = 1;

    public RoomAgg? Added { get; private set; }
    public RoomAgg? Updated { get; private set; }

    public Task<RoomAgg?> GetByIdAsync(long id, CancellationToken token = default)
        => Task.FromResult(_store.GetValueOrDefault(id));

    public Task<RoomAgg?> GetByCodeAsync(string roomCode, CancellationToken token = default)
        => Task.FromResult(_store.Values.FirstOrDefault(r => r.RoomCode == roomCode));

    /// <summary>همان منطق repository واقعی: قدیمی‌ترین روم عمومیِ هنوز منتظر.</summary>
    public Task<RoomAgg?> GetOpenPublicRoomAsync(CancellationToken token = default)
        => Task.FromResult(_store.Values
            .Where(r => r.Visibility == RoomVisibility.Public && r.Status == RoomStatus.WaitingForPlayers)
            .OrderBy(r => r.CreatedAtUtc)
            .FirstOrDefault());

    public Task AddAsync(RoomAgg room, CancellationToken token = default)
    {
        Added = room;
        _store[room.Id] = room;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(RoomAgg room, CancellationToken token = default)
    {
        Updated = room;
        _store[room.Id] = room;
        return Task.CompletedTask;
    }

    public Task<long> GetNextIdAsync() => Task.FromResult(_nextId++);

    public void Seed(RoomAgg room) => _store[room.Id] = room;
}
