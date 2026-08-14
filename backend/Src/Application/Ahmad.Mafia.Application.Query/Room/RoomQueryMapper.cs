using RoomAgg = Ahmad.Mafia.Domain.Room.Aggregates.Room;

namespace Ahmad.Mafia.Application.Query.Mappers;

internal static class RoomQueryMapper
{
    internal static GetRoomQueryResponse ToResponse(this RoomAgg room, long? gameSessionId) => new(
        RoomId: room.Id,
        RoomCode: room.RoomCode,
        Capacity: room.Capacity,
        Status: room.Status.ToString(),
        Members: room.Members
            .Select(m => new GetRoomMemberResponse(m.Id, m.Nickname, m.IsHost))
            .ToList(),
        GameSessionId: gameSessionId
    );
}
