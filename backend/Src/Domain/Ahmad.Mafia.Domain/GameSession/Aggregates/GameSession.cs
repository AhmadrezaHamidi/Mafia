using AhmadBase.Doamin;
using Ahmad.Mafia.Domain.GameSession.Args;
using Ahmad.Mafia.Domain.GameSession.Entities;
using Ahmad.Mafia.Domain.GameSession.Enums;
using Ahmad.Mafia.Domain.GameSession.Events;
using Ahmad.Mafia.Domain.GameSession.Exceptions;

namespace Ahmad.Mafia.Domain.GameSession.Aggregates;

public sealed class GameSession : AggregateRoot<long>
{
    private readonly List<GamePlayer> _players = [];
    private readonly Dictionary<long, long> _votes = [];

    public long RoomId { get; private set; }
    public GamePhase Phase { get; private set; }
    public int Round { get; private set; }
    public DateTime PhaseDeadlineUtc { get; private set; }
    public WinningTeam WinningTeam { get; private set; }
    public long? NightTargetPlayerId { get; private set; }
    public int NightDurationSeconds { get; private set; }
    public int DayDurationSeconds { get; private set; }

    public IReadOnlyCollection<GamePlayer> Players => _players.AsReadOnly();
    public IReadOnlyDictionary<long, long> Votes => _votes;

    private GameSession() { }

    private GameSession(CreateGameSessionArg arg) : base(arg.Id)
    {
        RoomId = arg.RoomId;
        NightDurationSeconds = arg.NightDurationSeconds;
        DayDurationSeconds = arg.DayDurationSeconds;
        Phase = GamePhase.Night;
        Round = 1;
        WinningTeam = WinningTeam.None;
        PhaseDeadlineUtc = DateTime.UtcNow.AddSeconds(arg.NightDurationSeconds);
    }

    public static GameSession Create(CreateGameSessionArg arg)
    {
        GuardEnoughPlayers(arg.Players.Count);

        var session = new GameSession(arg);
        var roles = AssignRoles(arg.Players.Count);

        for (var i = 0; i < arg.Players.Count; i++)
        {
            var seed = arg.Players[i];
            session._players.Add(new GamePlayer(seed.PlayerId, session.Id, seed.Nickname, roles[i]));
        }

        session.RaiseDomainEvent(new GameSessionStartedEvent(session.Id, arg.RoomId, arg.Players.Count));
        return session;
    }

    // ── Night ─────────────────────────────────────────────

    public void SubmitNightAction(long actorId, long targetId)
    {
        GuardPhaseIs(GamePhase.Night);

        var actor = GetAlivePlayerOrThrow(actorId);
        if (actor.Role != Role.SimpleMafia)
            throw new ActionNotAllowedForRoleException();

        GetAlivePlayerOrThrow(targetId);

        NightTargetPlayerId = targetId;
        RaiseDomainEvent(new NightActionSubmittedEvent(Id, actorId, targetId));
    }

    public void ResolveNightPhase()
    {
        GuardPhaseIs(GamePhase.Night);

        long? eliminatedId = null;
        if (Round == 1 && NightTargetPlayerId is { } targetId)
        {
            var target = _players.First(p => p.Id == targetId);
            target.Eliminate();
            eliminatedId = targetId;
        }

        NightTargetPlayerId = null;
        RaiseDomainEvent(new NightPhaseResolvedEvent(Id, Round, eliminatedId));

        if (TryEndGame()) return;

        _votes.Clear();
        Phase = GamePhase.Day;
        PhaseDeadlineUtc = DateTime.UtcNow.AddSeconds(DayDurationSeconds);
    }

    // ── Day ───────────────────────────────────────────────

    public void CastVote(long voterId, long targetId)
    {
        GuardPhaseIs(GamePhase.Day);
        GetAlivePlayerOrThrow(voterId);
        GetAlivePlayerOrThrow(targetId);

        _votes[voterId] = targetId;
        RaiseDomainEvent(new VoteCastEvent(Id, voterId, targetId));
    }

    public void RetractVote(long voterId)
    {
        GuardPhaseIs(GamePhase.Day);
        _votes.Remove(voterId);
    }

    public void ResolveVoting()
    {
        GuardPhaseIs(GamePhase.Day);

        long? eliminatedId = null;
        if (_votes.Count > 0)
        {
            var tally = _votes.Values
                .GroupBy(targetId => targetId)
                .Select(g => new { TargetId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            var topCount = tally[0].Count;
            var topVoted = tally.Where(x => x.Count == topCount).ToList();

            if (topVoted.Count == 1)
            {
                eliminatedId = topVoted[0].TargetId;
                _players.First(p => p.Id == eliminatedId).Eliminate();
            }
        }

        _votes.Clear();
        RaiseDomainEvent(new DayPhaseResolvedEvent(Id, Round, eliminatedId));

        if (TryEndGame()) return;

        Round++;
        Phase = GamePhase.Night;
        PhaseDeadlineUtc = DateTime.UtcNow.AddSeconds(NightDurationSeconds);
    }

    // ── Connection & Rematch ──────────────────────────────

    public void SetPlayerConnectionState(long playerId, ConnectionState state)
    {
        var player = _players.FirstOrDefault(p => p.Id == playerId);
        if (player is null) throw new PlayerNotInGameException();
        player.SetConnectionState(state);
    }

    public void RequestRematch()
    {
        if (Phase != GamePhase.Ended)
            throw new GameNotEndedException();

        var roles = AssignRoles(_players.Count);
        for (var i = 0; i < _players.Count; i++)
        {
            _players[i].ResetForRematch();
            _players[i].AssignRole(roles[i]);
        }

        _votes.Clear();
        NightTargetPlayerId = null;
        WinningTeam = WinningTeam.None;
        Round = 1;
        Phase = GamePhase.Night;
        PhaseDeadlineUtc = DateTime.UtcNow.AddSeconds(NightDurationSeconds);
    }

    // ── Internals ─────────────────────────────────────────

    private static IReadOnlyList<Role> AssignRoles(int playerCount)
    {
        var mafiaCount = Math.Max(1, (int)Math.Round(playerCount / 4.0, MidpointRounding.AwayFromZero));
        var roles = new List<Role>(playerCount);
        for (var i = 0; i < playerCount; i++)
            roles.Add(i < mafiaCount ? Role.SimpleMafia : Role.SimpleCitizen);

        var rng = Random.Shared;
        for (var i = roles.Count - 1; i > 0; i--)
        {
            var j = rng.Next(i + 1);
            (roles[i], roles[j]) = (roles[j], roles[i]);
        }
        return roles;
    }

    private bool TryEndGame()
    {
        var alive = _players.Where(p => p.IsAlive).ToList();
        var mafiaAlive = alive.Count(p => p.Role == Role.SimpleMafia);
        var townAlive = alive.Count - mafiaAlive;

        if (mafiaAlive == 0)
        {
            EndGame(Enums.WinningTeam.Town);
            return true;
        }
        if (mafiaAlive >= townAlive)
        {
            EndGame(Enums.WinningTeam.Mafia);
            return true;
        }
        return false;
    }

    private void EndGame(WinningTeam winner)
    {
        Phase = GamePhase.Ended;
        WinningTeam = winner;
        RaiseDomainEvent(new GameEndedEvent(Id, (int)winner));
    }

    private GamePlayer GetAlivePlayerOrThrow(long playerId)
    {
        var player = _players.FirstOrDefault(p => p.Id == playerId);
        if (player is null) throw new PlayerNotInGameException();
        if (!player.IsAlive) throw new PlayerAlreadyEliminatedException();
        return player;
    }

    private void GuardPhaseIs(GamePhase phase)
    {
        if (Phase != phase) throw new WrongPhaseForActionException();
    }

    private static void GuardEnoughPlayers(int count)
    {
        if (count < 6)
            throw new NotEnoughPlayersException();
    }
}
