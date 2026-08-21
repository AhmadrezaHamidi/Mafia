using RoomAgg = Ahmad.Mafia.Domain.Room.Aggregates;

namespace Ahmad.Mafia.Domain.Repositories;

public interface IRoomRepository
{
    Task<RoomAgg.Room?> GetByIdAsync(long id, CancellationToken token = default);
    Task<RoomAgg.Room?> GetByCodeAsync(string roomCode, CancellationToken token = default);
    /// <summary>قدیمی‌ترین روم عمومیِ هنوز منتظرِ بازیکن — برای matchmaking «بازی سریع».</summary>
    Task<RoomAgg.Room?> GetOpenPublicRoomAsync(CancellationToken token = default);
    Task AddAsync(RoomAgg.Room room, CancellationToken token = default);
    Task UpdateAsync(RoomAgg.Room room, CancellationToken token = default);
    Task<long> GetNextIdAsync();
}
