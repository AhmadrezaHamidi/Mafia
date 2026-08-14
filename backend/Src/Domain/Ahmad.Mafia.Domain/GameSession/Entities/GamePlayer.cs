using AhmadBase.Doamin;
using Ahmad.Mafia.Domain.GameSession.Enums;

namespace Ahmad.Mafia.Domain.GameSession.Entities;

public sealed class GamePlayer : TEntity<long>
{
    public long GameSessionId { get; private set; }
    public string Nickname { get; private set; } = string.Empty;
    public Role Role { get; private set; }
    public bool IsAlive { get; private set; } = true;
    public ConnectionState Connection { get; private set; } = ConnectionState.Connected;

    private GamePlayer() { }

    internal GamePlayer(long playerId, long gameSessionId, string nickname, Role role)
    {
        Id = playerId;
        GameSessionId = gameSessionId;
        Nickname = nickname;
        Role = role;
    }

    internal void Eliminate() => IsAlive = false;

    internal void SetConnectionState(ConnectionState state) => Connection = state;

    internal void ResetForRematch()
    {
        IsAlive = true;
        Connection = ConnectionState.Connected;
    }

    internal void AssignRole(Role role) => Role = role;
}
