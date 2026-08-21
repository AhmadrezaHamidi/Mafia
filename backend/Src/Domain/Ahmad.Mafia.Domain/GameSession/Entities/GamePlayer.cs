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

    /// <summary>
    /// وقتی بیش از یک مافیا زنده باشد، فقط رئیس (Leader) اکشن شب را نهایی می‌کند —
    /// بقیه‌ی مافیا فقط از طریق کانال چت nightMafia نظر می‌دهند.
    /// </summary>
    public bool IsMafiaLeader { get; private set; }

    /// <summary>فقط برای Detective پر می‌شه — آخرین نفری که استعلام گرفته + نتیجه (مافیاست یا نه).</summary>
    public long? LastInvestigationTargetId { get; private set; }
    public bool? LastInvestigationIsMafia { get; private set; }

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
        IsMafiaLeader = false;
        LastInvestigationTargetId = null;
        LastInvestigationIsMafia = null;
    }

    internal void AssignRole(Role role) => Role = role;

    internal void SetMafiaLeader(bool value) => IsMafiaLeader = value;

    internal void SetInvestigationResult(long targetId, bool isMafia)
    {
        LastInvestigationTargetId = targetId;
        LastInvestigationIsMafia = isMafia;
    }
}
