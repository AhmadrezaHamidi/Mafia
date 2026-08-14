using AhmadBase.Doamin;

namespace Ahmad.Mafia.Domain.Room.Entities;

public sealed class RoomMember : TEntity<long>
{
    public long RoomId { get; private set; }
    public string Nickname { get; private set; } = string.Empty;
    public bool IsHost { get; private set; }
    public DateTime JoinedAtUtc { get; private set; }

    private RoomMember() { }

    internal RoomMember(long playerId, long roomId, string nickname, bool isHost)
    {
        GuardNickname(nickname);

        Id = playerId;
        RoomId = roomId;
        Nickname = nickname;
        IsHost = isHost;
        JoinedAtUtc = DateTime.UtcNow;
    }

    internal void PromoteToHost() => IsHost = true;

    internal void DemoteFromHost() => IsHost = false;

    private static void GuardNickname(string nickname)
    {
        if (string.IsNullOrWhiteSpace(nickname) || nickname.Length < 2)
            throw new Exceptions.InvalidNicknameException();
    }
}
