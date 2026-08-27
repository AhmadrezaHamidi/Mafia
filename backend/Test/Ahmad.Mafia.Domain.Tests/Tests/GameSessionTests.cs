using Ahmad.Mafia.Domain.GameSession.Args;
using Ahmad.Mafia.Domain.GameSession.Entities;
using Ahmad.Mafia.Domain.GameSession.Enums;
using Ahmad.Mafia.Domain.GameSession.Events;
using Ahmad.Mafia.Domain.GameSession.Exceptions;
using GameSessionAgg = Ahmad.Mafia.Domain.GameSession.Aggregates.GameSession;

namespace Ahmad.Mafia.Domain.Tests.Tests;

public class GameSessionTests
{
    private static GameSessionAgg CreateSession(int playerCount = 6)
    {
        var players = Enumerable.Range(1, playerCount)
            .Select(i => new GamePlayerSeed(i, $"بازیکن{i}"))
            .ToList();

        return GameSessionAgg.Create(new CreateGameSessionArg(1, RoomId: 10, players,
            NightDurationSeconds: 45, DayDurationSeconds: 90));
    }

    // ─── Create / Role assignment ─────────────────────────

    [Theory]
    [InlineData(6, 2)]
    [InlineData(8, 2)]
    [InlineData(10, 3)]
    [InlineData(12, 3)]
    public void Create_Should_AssignMafiaCount_AccordingToRatio(int playerCount, int expectedMafia)
    {
        var session = CreateSession(playerCount);

        var mafiaCount = session.Players.Count(p => p.Role == Role.SimpleMafia);

        Assert.Equal(expectedMafia, mafiaCount);
        Assert.Equal(GamePhase.Night, session.Phase);
        Assert.Equal(1, session.Round);
    }

    [Fact]
    public void Create_Should_Raise_GameSessionStartedEvent()
    {
        var session = CreateSession();

        Assert.Contains(session.DomainEvents, e => e is GameSessionStartedEvent);
    }

    // ─── Night phase ──────────────────────────────────────

    [Fact]
    public void SubmitNightAction_ByCitizen_Should_Throw_ActionNotAllowedForRoleException()
    {
        var session = CreateSession();
        var citizen = session.Players.First(p => p.Role == Role.SimpleCitizen);
        var anyTarget = session.Players.First(p => p.Id != citizen.Id);

        Assert.Throws<ActionNotAllowedForRoleException>(
            () => session.SubmitNightAction(citizen.Id, anyTarget.Id));
    }

    [Fact]
    public void SubmitNightAction_ByMafia_Should_SetNightTarget()
    {
        var session = CreateSession();
        var mafia = KillDecider(session);
        var target = session.Players.First(p => p.Id != mafia.Id);

        session.SubmitNightAction(mafia.Id, target.Id);

        Assert.Equal(target.Id, session.NightTargetPlayerId);
        Assert.Contains(session.DomainEvents, e => e is NightActionSubmittedEvent);
    }

    [Fact]
    public void SubmitNightAction_TargetNotInGame_Should_Throw_PlayerNotInGameException()
    {
        var session = CreateSession();
        var mafia = KillDecider(session);

        Assert.Throws<PlayerNotInGameException>(() => session.SubmitNightAction(mafia.Id, 999));
    }

    [Fact]
    public void ResolveNightPhase_Round1_WithTarget_Should_EliminateTarget()
    {
        var session = CreateSession();
        var mafia = KillDecider(session);
        var target = session.Players.First(p => p.Role == Role.SimpleCitizen);
        session.SubmitNightAction(mafia.Id, target.Id);

        session.ResolveNightPhase();

        Assert.False(session.Players.First(p => p.Id == target.Id).IsAlive);
        Assert.Equal(GamePhase.Day, session.Phase);
        Assert.Null(session.NightTargetPlayerId);
    }

    [Fact]
    public void ResolveNightPhase_Round1_WithoutTarget_Should_NotEliminateAnyone()
    {
        var session = CreateSession();

        session.ResolveNightPhase();

        Assert.All(session.Players, p => Assert.True(p.IsAlive));
        Assert.Equal(GamePhase.Day, session.Phase);
    }

    [Fact]
    public void ResolveVoting_Then_ResolveNightPhase_Round2_Should_NotKill_EvenIfTargetSubmitted()
    {
        var session = CreateSession(8); // 2 mafia, 6 citizens — keeps game alive past round 1
        session.ResolveNightPhase(); // round 1, no kill (no action submitted)
        session.ResolveVoting();     // round 1 day, no votes -> no elimination -> round becomes 2

        Assert.Equal(2, session.Round);
        Assert.Equal(GamePhase.Night, session.Phase);

        var mafia = KillDecider(session);
        var target = session.Players.First(p => p.Role == Role.SimpleCitizen && p.IsAlive);
        session.SubmitNightAction(mafia.Id, target.Id);

        session.ResolveNightPhase();

        Assert.True(session.Players.First(p => p.Id == target.Id).IsAlive);
    }

    // ─── Day phase / voting ───────────────────────────────

    [Fact]
    public void CastVote_Then_ResolveVoting_Should_EliminateTopVotedPlayer()
    {
        var session = CreateSession(8);
        session.ResolveNightPhase(); // -> Day, round 1

        var alive = session.Players.ToList();
        var target = alive[0];
        foreach (var voter in alive.Where(p => p.Id != target.Id).Take(3))
            session.CastVote(voter.Id, target.Id);

        session.ResolveVoting();

        Assert.False(session.Players.First(p => p.Id == target.Id).IsAlive);
    }

    [Fact]
    public void ResolveVoting_OnTie_Should_NotEliminateAnyone()
    {
        var session = CreateSession(8);
        session.ResolveNightPhase(); // -> Day

        var alive = session.Players.ToList();
        session.CastVote(alive[0].Id, alive[2].Id);
        session.CastVote(alive[1].Id, alive[3].Id);

        session.ResolveVoting();

        Assert.All(session.Players, p => Assert.True(p.IsAlive));
    }

    [Fact]
    public void CastVote_ByEliminatedPlayer_Should_Throw_PlayerAlreadyEliminatedException()
    {
        var session = CreateSession(8);
        var mafia = KillDecider(session);
        var deadCitizen = session.Players.First(p => p.Role == Role.SimpleCitizen);
        session.SubmitNightAction(mafia.Id, deadCitizen.Id);
        session.ResolveNightPhase(); // deadCitizen eliminated, -> Day

        Assert.Throws<PlayerAlreadyEliminatedException>(
            () => session.CastVote(deadCitizen.Id, mafia.Id));
    }

    [Fact]
    public void CastVote_WhenPhaseIsNight_Should_Throw_WrongPhaseForActionException()
    {
        var session = CreateSession();
        var p1 = session.Players.First();
        var p2 = session.Players.Skip(1).First();

        Assert.Throws<WrongPhaseForActionException>(() => session.CastVote(p1.Id, p2.Id));
    }

    // ─── Win condition ────────────────────────────────────

    [Fact]
    public void Game_Should_EndWithTownWin_WhenAllMafiaEliminated()
    {
        var session = CreateSession(6); // 2 mafia, 4 citizens
        session.ResolveNightPhase();     // -> Day round 1, no kill

        var mafias = session.Players.Where(p => p.Role == Role.SimpleMafia).ToList();
        var citizens = session.Players.Where(p => p.Role == Role.SimpleCitizen).ToList();

        // Round 1 day: vote out first mafia
        VoteOutUnanimously(session, mafias[0].Id);
        session.ResolveVoting(); // -> Night round 2

        // Round 2 night: no kill possible (rule: only round 1 kills)
        session.ResolveNightPhase(); // -> Day round 2

        // Round 2 day: vote out second mafia -> town should win (0 mafia left)
        VoteOutUnanimously(session, mafias[1].Id);
        session.ResolveVoting();

        Assert.Equal(GamePhase.Ended, session.Phase);
        Assert.Equal(WinningTeam.Town, session.WinningTeam);
        Assert.Contains(session.DomainEvents, e => e is GameEndedEvent);
    }

    [Fact]
    public void Game_Should_EndWithMafiaWin_WhenMafiaReachesParityWithTown()
    {
        var session = CreateSession(6); // 2 mafia, 4 citizens
        var mafia = KillDecider(session);
        var firstVictim = session.Players.First(p => p.Role == Role.SimpleCitizen);

        session.SubmitNightAction(mafia.Id, firstVictim.Id);
        session.ResolveNightPhase(); // 1 citizen dead -> 2 mafia vs 3 citizens, game continues

        var citizens = session.Players.Where(p => p.Role == Role.SimpleCitizen && p.IsAlive).ToList();

        // Round 1 day: town mistakenly votes out one of their own -> 2 mafia vs 2 citizens -> mafia wins
        VoteOutUnanimously(session, citizens[0].Id);
        session.ResolveVoting();

        Assert.Equal(GamePhase.Ended, session.Phase);
        Assert.Equal(WinningTeam.Mafia, session.WinningTeam);
    }

    // ─── Rematch ──────────────────────────────────────────

    [Fact]
    public void RequestRematch_WhenGameNotEnded_Should_Throw_GameNotEndedException()
    {
        var session = CreateSession();

        Assert.Throws<GameNotEndedException>(() => session.RequestRematch());
    }

    [Fact]
    public void RequestRematch_WhenGameEnded_Should_ResetAllPlayersAlive_AndReassignRoles()
    {
        var session = CreateSession(6);
        var mafia = KillDecider(session);
        var target = session.Players.First(p => p.Role == Role.SimpleCitizen);
        session.SubmitNightAction(mafia.Id, target.Id);
        session.ResolveNightPhase();

        var citizens = session.Players.Where(p => p.Role == Role.SimpleCitizen && p.IsAlive).ToList();
        VoteOutUnanimously(session, citizens[0].Id);
        session.ResolveVoting();
        Assert.Equal(GamePhase.Ended, session.Phase);

        session.RequestRematch();

        Assert.All(session.Players, p => Assert.True(p.IsAlive));
        Assert.Equal(GamePhase.Night, session.Phase);
        Assert.Equal(1, session.Round);
        Assert.Equal(WinningTeam.None, session.WinningTeam);
    }

    /// <summary>
    /// مافیایی که اجازه‌ی ثبتِ کشتن دارد. با بیش از یک مافیای زنده، دامین فقط
    /// از رئیس می‌پذیرد — پس تست‌ها نباید «اولین مافیا» را بردارند.
    /// </summary>
    private static GamePlayer KillDecider(GameSessionAgg session)
        => session.Players.First(p => p.IsAlive && p.Role is Role.SimpleMafia or Role.GodFather && p.IsMafiaLeader);

    private static void VoteOutUnanimously(GameSessionAgg session, long targetId)
    {
        foreach (var voter in session.Players.Where(p => p.IsAlive && p.Id != targetId))
            session.CastVote(voter.Id, targetId);
    }
}
