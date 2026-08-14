using Ahmad.Mafia.Domain.Room.Args;
using Ahmad.Mafia.Domain.Room.Enums;
using Ahmad.Mafia.Domain.Room.Events;
using Ahmad.Mafia.Domain.Room.Exceptions;
using RoomAgg = Ahmad.Mafia.Domain.Room.Aggregates.Room;

namespace Ahmad.Mafia.Domain.Tests.Tests;

public class RoomTests
{
    private static RoomAgg CreateRoom(int capacity = 6)
        => RoomAgg.Create(new CreateRoomArg(1, "AB12CD", HostPlayerId: 100, HostNickname: "احمد", Capacity: capacity));

    /// <summary>روم را با حداقل ظرفیت مجاز (۶) می‌سازد و بجز میزبان، بقیه را پر می‌کند.</summary>
    private static RoomAgg CreateAlmostFullRoom(out long lastPlayerId)
    {
        var room = CreateRoom(capacity: 6);
        for (long id = 2; id <= 5; id++)
            room.Join(new JoinRoomArg(id, $"بازیکن{id}"));

        lastPlayerId = 6;
        return room;
    }

    // ─── Create ───────────────────────────────────────────

    [Fact]
    public void Create_Should_AddHostAsFirstMember()
    {
        var room = CreateRoom();

        Assert.Single(room.Members);
        Assert.Equal(100, room.HostPlayerId);
        Assert.Equal(RoomStatus.WaitingForPlayers, room.Status);
    }

    [Fact]
    public void Create_Should_Raise_RoomCreatedEvent_And_PlayerJoinedRoomEvent()
    {
        var room = CreateRoom();

        Assert.Contains(room.DomainEvents, e => e is RoomCreatedEvent);
        Assert.Contains(room.DomainEvents, e => e is PlayerJoinedRoomEvent);
    }

    [Theory]
    [InlineData(5)]
    [InlineData(16)]
    public void Create_When_CapacityOutOfRange_Should_Throw(int capacity)
    {
        Assert.Throws<InvalidCapacityException>(
            () => RoomAgg.Create(new CreateRoomArg(1, "AB12CD", 100, "احمد", capacity)));
    }

    // ─── Join ─────────────────────────────────────────────

    [Fact]
    public void Join_Should_AddMember_And_NotBeReady_WhenBelowCapacity()
    {
        var room = CreateRoom(capacity: 6);

        room.Join(new JoinRoomArg(2, "نگار"));

        Assert.Equal(2, room.Members.Count);
        Assert.Equal(RoomStatus.WaitingForPlayers, room.Status);
    }

    [Fact]
    public void Join_Should_BecomeReadyToStart_WhenCapacityReached()
    {
        var room = CreateAlmostFullRoom(out var lastPlayerId);

        room.Join(new JoinRoomArg(lastPlayerId, "آخرین‌نفر"));

        Assert.Equal(RoomStatus.ReadyToStart, room.Status);
        Assert.Contains(room.DomainEvents, e => e is RoomBecameReadyEvent);
    }

    [Fact]
    public void Join_When_RoomFull_Should_Throw_RoomFullException()
    {
        var room = CreateAlmostFullRoom(out var lastPlayerId);
        room.Join(new JoinRoomArg(lastPlayerId, "آخرین‌نفر"));

        Assert.Throws<RoomFullException>(() => room.Join(new JoinRoomArg(7, "اضافی")));
    }

    [Fact]
    public void Join_When_RoomInProgress_Should_Throw_RoomAlreadyStartedException()
    {
        var room = CreateAlmostFullRoom(out var lastPlayerId);
        room.Join(new JoinRoomArg(lastPlayerId, "آخرین‌نفر"));
        room.Start(100);

        Assert.Throws<RoomAlreadyStartedException>(() => room.Join(new JoinRoomArg(7, "اضافی")));
    }

    [Theory]
    [InlineData("")]
    [InlineData("ا")]
    public void Join_When_NicknameInvalid_Should_Throw_InvalidNicknameException(string nickname)
    {
        var room = CreateRoom(capacity: 6);

        Assert.Throws<InvalidNicknameException>(() => room.Join(new JoinRoomArg(2, nickname)));
    }

    // ─── Leave & Host transfer ────────────────────────────

    [Fact]
    public void Leave_When_HostLeaves_Should_TransferHostToOldestRemainingMember()
    {
        var room = CreateRoom(capacity: 6);
        room.Join(new JoinRoomArg(2, "نگار"));
        room.Join(new JoinRoomArg(3, "رضا"));

        room.Leave(100); // host leaves

        Assert.Equal(2, room.HostPlayerId);
        Assert.Contains(room.DomainEvents, e => e is HostTransferredEvent);
    }

    [Fact]
    public void Leave_When_PlayerNotInRoom_Should_Throw_PlayerNotInRoomException()
    {
        var room = CreateRoom();

        Assert.Throws<PlayerNotInRoomException>(() => room.Leave(999));
    }

    [Fact]
    public void Leave_When_RoomWasReady_ShouldRevertTo_WaitingForPlayers()
    {
        var room = CreateAlmostFullRoom(out var lastPlayerId);
        room.Join(new JoinRoomArg(lastPlayerId, "آخرین‌نفر"));
        Assert.Equal(RoomStatus.ReadyToStart, room.Status);

        room.Leave(lastPlayerId);

        Assert.Equal(RoomStatus.WaitingForPlayers, room.Status);
    }

    // ─── Start ────────────────────────────────────────────

    [Fact]
    public void Start_When_NotFull_Should_Throw_RoomNotFullException()
    {
        var room = CreateRoom(capacity: 6);
        room.Join(new JoinRoomArg(2, "نگار"));

        Assert.Throws<RoomNotFullException>(() => room.Start(100));
    }

    [Fact]
    public void Start_ByNonHost_Should_Throw_OnlyHostCanPerformActionException()
    {
        var room = CreateAlmostFullRoom(out var lastPlayerId);
        room.Join(new JoinRoomArg(lastPlayerId, "آخرین‌نفر"));

        Assert.Throws<OnlyHostCanPerformActionException>(() => room.Start(lastPlayerId));
    }

    [Fact]
    public void Start_When_Full_Should_SetStatusInProgress()
    {
        var room = CreateAlmostFullRoom(out var lastPlayerId);
        room.Join(new JoinRoomArg(lastPlayerId, "آخرین‌نفر"));

        room.Start(100);

        Assert.Equal(RoomStatus.InProgress, room.Status);
        Assert.Contains(room.DomainEvents, e => e is RoomGameStartedEvent);
    }

    [Fact]
    public void Start_WhenAlreadyStarted_Should_Throw_RoomAlreadyStartedException()
    {
        var room = CreateAlmostFullRoom(out var lastPlayerId);
        room.Join(new JoinRoomArg(lastPlayerId, "آخرین‌نفر"));
        room.Start(100);

        Assert.Throws<RoomAlreadyStartedException>(() => room.Start(100));
    }
}
