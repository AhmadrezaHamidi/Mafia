using AhmadBase.Application.Query;

namespace Ahmad.Mafia.Application.Query.Queries;

public sealed record GetRoomMemberResponse(long PlayerId, string Nickname, bool IsHost);

public sealed record GetRoomQueryResponse(
    long RoomId,
    string RoomCode,
    int Capacity,
    string Status,
    string Visibility,
    string Scenario,
    IReadOnlyList<GetRoomMemberResponse> Members,
    long? GameSessionId
);

public record GetRoomQuery(string RoomCode) : IQuery<GetRoomQueryResponse>;
