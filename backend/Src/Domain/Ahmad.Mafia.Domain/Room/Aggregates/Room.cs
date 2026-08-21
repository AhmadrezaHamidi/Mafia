using AhmadBase.Doamin;
using Ahmad.Mafia.Domain.Room.Args;
using Ahmad.Mafia.Domain.Room.Entities;
using Ahmad.Mafia.Domain.Room.Enums;
using Ahmad.Mafia.Domain.Room.Events;
using Ahmad.Mafia.Domain.Room.Exceptions;

namespace Ahmad.Mafia.Domain.Room.Aggregates;

public sealed class Room : AggregateRoot<long>
{
    private readonly List<RoomMember> _members = [];

    public string RoomCode { get; private set; } = string.Empty;
    public int Capacity { get; private set; }
    public RoomStatus Status { get; private set; }
    public RoomVisibility Visibility { get; private set; }
    public ScenarioType Scenario { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    public IReadOnlyCollection<RoomMember> Members => _members.AsReadOnly();
    public long? HostPlayerId => _members.FirstOrDefault(m => m.IsHost)?.Id;

    private Room() { }

    private Room(CreateRoomArg arg) : base(arg.Id)
    {
        GuardCapacityRange(arg.Capacity);

        RoomCode = arg.RoomCode;
        Capacity = arg.Capacity;
        Status = RoomStatus.WaitingForPlayers;
        Visibility = arg.Visibility;
        Scenario = arg.Scenario;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static Room Create(CreateRoomArg arg)
    {
        var room = new Room(arg);
        room.RaiseDomainEvent(new RoomCreatedEvent(arg.Id, arg.RoomCode, arg.HostPlayerId, arg.Capacity));

        var host = new RoomMember(arg.HostPlayerId, room.Id, arg.HostNickname, isHost: true);
        room._members.Add(host);
        room.RaiseDomainEvent(new PlayerJoinedRoomEvent(room.Id, host.Id, host.Nickname, room._members.Count, room.Capacity));

        return room;
    }

    public RoomMember Join(JoinRoomArg arg)
    {
        GuardOpenForJoining();
        GuardNotFull();

        var member = new RoomMember(arg.PlayerId, Id, arg.Nickname, isHost: false);
        _members.Add(member);

        RaiseDomainEvent(new PlayerJoinedRoomEvent(Id, member.Id, member.Nickname, _members.Count, Capacity));

        if (_members.Count == Capacity)
        {
            Status = RoomStatus.ReadyToStart;
            RaiseDomainEvent(new RoomBecameReadyEvent(Id));
        }

        return member;
    }

    public void Leave(long playerId)
    {
        var member = _members.FirstOrDefault(m => m.Id == playerId);
        GuardMemberExists(member);

        var wasHost = member!.IsHost;
        _members.Remove(member);
        RaiseDomainEvent(new PlayerLeftRoomEvent(Id, playerId));

        if (Status == RoomStatus.ReadyToStart && _members.Count < Capacity)
            Status = RoomStatus.WaitingForPlayers;

        if (wasHost && _members.Count > 0)
        {
            var newHost = _members.OrderBy(m => m.JoinedAtUtc).First();
            newHost.PromoteToHost();
            RaiseDomainEvent(new HostTransferredEvent(Id, newHost.Id));
        }
    }

    public void Start(long requestingPlayerId)
    {
        GuardIsHost(requestingPlayerId);
        GuardStatusIsOpen();
        GuardCapacityReached();

        Status = RoomStatus.InProgress;
        RaiseDomainEvent(new RoomGameStartedEvent(Id));
    }

    public void Close(long requestingPlayerId)
    {
        GuardIsHost(requestingPlayerId);
        if (Status == RoomStatus.Closed) return;

        Status = RoomStatus.Closed;
        RaiseDomainEvent(new RoomClosedEvent(Id));
    }

    // ── Guards ────────────────────────────────────────────

    private static void GuardCapacityRange(int capacity)
    {
        if (capacity < 6 || capacity > 15)
            throw new InvalidCapacityException();
    }

    private void GuardOpenForJoining()
    {
        if (Status is RoomStatus.InProgress or RoomStatus.Closed)
            throw new RoomAlreadyStartedException();
    }

    private void GuardNotFull()
    {
        if (_members.Count >= Capacity)
            throw new RoomFullException();
    }

    private void GuardStatusIsOpen()
    {
        if (Status == RoomStatus.InProgress)
            throw new RoomAlreadyStartedException();
        if (Status == RoomStatus.Closed)
            throw new RoomClosedException();
    }

    private void GuardCapacityReached()
    {
        if (_members.Count != Capacity)
            throw new RoomNotFullException();
    }

    private static void GuardMemberExists(RoomMember? member)
    {
        if (member is null) throw new PlayerNotInRoomException();
    }

    private void GuardIsHost(long playerId)
    {
        if (HostPlayerId != playerId)
            throw new OnlyHostCanPerformActionException();
    }
}
