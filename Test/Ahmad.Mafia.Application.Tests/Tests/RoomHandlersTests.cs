using Ahmad.Mafia.Application.Contract.Room.Commands;
using Ahmad.Mafia.Application.Handlers;
using Ahmad.Mafia.Application.Tests.Fakes;
using Ahmad.Mafia.Domain.Room.Args;
using Ahmad.Mafia.Domain.Room.Exceptions;
using RoomAgg = Ahmad.Mafia.Domain.Room.Aggregates.Room;

namespace Ahmad.Mafia.Application.Tests.Tests;

public class RoomHandlersTests
{
    private readonly FakeRoomRepository _repo = new();
    private readonly RoomHandlers _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public RoomHandlersTests() => _sut = new RoomHandlers(_repo, FakeAppDb.Create());

    private static RoomAgg MakeRoom(int capacity = 6, long hostId = 1)
        => RoomAgg.Create(new CreateRoomArg(1, "AB12CD", hostId, "احمد", capacity));

    [Fact]
    public async Task Create_Should_AddRoom_And_ReturnRoomCodeAndHostPlayerId()
    {
        var result = await _sut.Handle(new CreateRoomCommand("احمد", 6), _ct);

        Assert.NotNull(_repo.Added);
        Assert.Equal(result.RoomCode, _repo.Added!.RoomCode);
        Assert.Equal(result.HostPlayerId, _repo.Added.HostPlayerId);
        Assert.Single(_repo.Added.Members);
    }

    [Fact]
    public async Task Join_Should_AddMember_ToExistingRoom()
    {
        var room = MakeRoom();
        _repo.Seed(room);

        var result = await _sut.Handle(new JoinRoomCommand("AB12CD", "نگار"), _ct);

        Assert.Equal(2, room.Members.Count);
        Assert.Equal(room.Id, result.RoomId);
    }

    [Fact]
    public async Task Join_When_RoomCodeNotFound_Should_Throw_RoomNotFoundException()
    {
        await Assert.ThrowsAsync<RoomNotFoundException>(
            () => _sut.Handle(new JoinRoomCommand("ZZZZZZ", "نگار"), _ct));
    }

    [Fact]
    public async Task Leave_Should_RemoveMember_FromRoom()
    {
        var room = MakeRoom();
        room.Join(new JoinRoomArg(2, "نگار"));
        _repo.Seed(room);

        await _sut.Handle(new LeaveRoomCommand(room.Id, 2), _ct);

        Assert.Single(room.Members);
    }

    [Fact]
    public async Task Start_ByHost_When_RoomFull_Should_SetInProgress()
    {
        var room = MakeRoom(capacity: 6, hostId: 1);
        for (long id = 2; id <= 6; id++)
            room.Join(new JoinRoomArg(id, $"بازیکن{id}"));
        _repo.Seed(room);

        await _sut.Handle(new StartRoomCommand(room.Id, 1), _ct);

        Assert.Equal(Domain.Room.Enums.RoomStatus.InProgress, room.Status);
    }

    [Fact]
    public async Task Start_ByNonHost_Should_Throw_OnlyHostCanPerformActionException()
    {
        var room = MakeRoom(capacity: 6, hostId: 1);
        for (long id = 2; id <= 6; id++)
            room.Join(new JoinRoomArg(id, $"بازیکن{id}"));
        _repo.Seed(room);

        await Assert.ThrowsAsync<OnlyHostCanPerformActionException>(
            () => _sut.Handle(new StartRoomCommand(room.Id, 2), _ct));
    }

    [Fact]
    public async Task Start_When_RoomNotFull_Should_Throw_RoomNotFullException()
    {
        var room = MakeRoom(capacity: 6, hostId: 1);
        _repo.Seed(room);

        await Assert.ThrowsAsync<RoomNotFullException>(
            () => _sut.Handle(new StartRoomCommand(room.Id, 1), _ct));
    }

    [Fact]
    public async Task Join_When_RoomAlreadyFull_Should_Throw_RoomFullException()
    {
        var room = MakeRoom(capacity: 6, hostId: 1);
        for (long id = 2; id <= 6; id++)
            room.Join(new JoinRoomArg(id, $"بازیکن{id}"));
        _repo.Seed(room);

        await Assert.ThrowsAsync<RoomFullException>(
            () => _sut.Handle(new JoinRoomCommand("AB12CD", "اضافی"), _ct));
    }

    [Fact]
    public async Task Leave_When_HostLeaves_Should_TransferHost_ToOldestRemainingMember()
    {
        var room = MakeRoom(capacity: 6, hostId: 1);
        room.Join(new JoinRoomArg(2, "نگار"));
        room.Join(new JoinRoomArg(3, "رضا"));
        _repo.Seed(room);

        await _sut.Handle(new LeaveRoomCommand(room.Id, 1), _ct);

        Assert.Equal(2, room.HostPlayerId);
    }
}
