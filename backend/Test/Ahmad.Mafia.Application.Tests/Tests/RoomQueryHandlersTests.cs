using Ahmad.Mafia.Application.Query.Handlers;
using Ahmad.Mafia.Application.Query.Queries;
using Ahmad.Mafia.Application.Tests.Fakes;
using Ahmad.Mafia.Domain.GameSession.Args;
using Ahmad.Mafia.Domain.Room.Args;
using Ahmad.Mafia.Domain.Room.Exceptions;
using GameSessionAgg = Ahmad.Mafia.Domain.GameSession.Aggregates.GameSession;
using RoomAgg = Ahmad.Mafia.Domain.Room.Aggregates.Room;

namespace Ahmad.Mafia.Application.Tests.Tests;

public class RoomQueryHandlersTests
{
    private readonly FakeRoomRepository _roomRepo = new();
    private readonly FakeGameSessionRepository _sessionRepo = new();
    private readonly RoomQueryHandlers _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public RoomQueryHandlersTests() => _sut = new RoomQueryHandlers(_roomRepo, _sessionRepo);

    private static RoomAgg MakeRoom(int capacity = 6, long hostId = 1)
        => RoomAgg.Create(new CreateRoomArg(1, "AB12CD", hostId, "احمد", capacity));

    [Fact]
    public async Task GetRoom_Should_ReturnMembers_And_NullGameSessionId_WhileWaiting()
    {
        var room = MakeRoom();
        room.Join(new JoinRoomArg(2, "نگار"));
        _roomRepo.Seed(room);

        var response = await _sut.HandleAsync(new GetRoomQuery("AB12CD"), _ct);

        Assert.Equal(2, response.Members.Count);
        Assert.Equal("WaitingForPlayers", response.Status);
        Assert.Null(response.GameSessionId);
    }

    [Fact]
    public async Task GetRoom_When_CodeNotFound_Should_Throw_RoomNotFoundException()
    {
        await Assert.ThrowsAsync<RoomNotFoundException>(
            () => _sut.HandleAsync(new GetRoomQuery("ZZZZZZ"), _ct));
    }

    [Fact]
    public async Task GetRoom_When_InProgress_Should_ReturnGameSessionId()
    {
        var room = MakeRoom(capacity: 6, hostId: 1);
        for (long id = 2; id <= 6; id++)
            room.Join(new JoinRoomArg(id, $"بازیکن{id}"));
        room.Start(1);
        _roomRepo.Seed(room);

        var players = room.Members.Select(m => new GamePlayerSeed(m.Id, m.Nickname)).ToList();
        var session = GameSessionAgg.Create(new CreateGameSessionArg(500, room.Id, players));
        _sessionRepo.Seed(session);

        var response = await _sut.HandleAsync(new GetRoomQuery("AB12CD"), _ct);

        Assert.Equal("InProgress", response.Status);
        Assert.Equal(500, response.GameSessionId);
    }
}
