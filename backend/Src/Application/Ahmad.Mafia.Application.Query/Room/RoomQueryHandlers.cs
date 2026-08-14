using Ahmad.Mafia.Domain.Room.Enums;
using Ahmad.Mafia.Domain.Room.Exceptions;

namespace Ahmad.Mafia.Application.Query.Handlers;

public sealed class RoomQueryHandlers(IRoomRepository repository, IGameSessionRepository gameSessionRepository) :
    IQueryHandler<GetRoomQuery, GetRoomQueryResponse>
{
    public async Task<GetRoomQueryResponse> HandleAsync(GetRoomQuery query, CancellationToken token)
    {
        var room = await repository.GetByCodeAsync(query.RoomCode, token)
            ?? throw new RoomNotFoundException();

        long? gameSessionId = null;
        if (room.Status == RoomStatus.InProgress)
        {
            var session = await gameSessionRepository.GetByRoomIdAsync(room.Id, token);
            gameSessionId = session?.Id;
        }

        return room.ToResponse(gameSessionId);
    }
}
