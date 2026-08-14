using Ahmad.Mafia.Domain.Repositories;
using AhmadBase.Persistence.NHiLoHelper;
using Microsoft.EntityFrameworkCore;
using RoomAggregate = Ahmad.Mafia.Domain.Room.Aggregates.Room;

namespace Ahmad.Mafia.Persistence.EF.Repositories;

public sealed class RoomRepository(
    MafiaDbContext context,
    IHiLoIdGenerator hiLoGenerator) : IRoomRepository
{
    public async Task<RoomAggregate?> GetByIdAsync(long id, CancellationToken token = default)
        => await context.Rooms.Include(x => x.Members).FirstOrDefaultAsync(x => x.Id == id, token);

    public async Task<RoomAggregate?> GetByCodeAsync(string roomCode, CancellationToken token = default)
        => await context.Rooms.Include(x => x.Members).FirstOrDefaultAsync(x => x.RoomCode == roomCode, token);

    public async Task AddAsync(RoomAggregate room, CancellationToken token = default)
        => await context.Rooms.AddAsync(room, token);

    public Task UpdateAsync(RoomAggregate room, CancellationToken token = default)
    {
        context.Rooms.Update(room);
        return Task.CompletedTask;
    }

    public Task<long> GetNextIdAsync()
        => Task.FromResult(hiLoGenerator.GetNextId<RoomAggregate>());
}
