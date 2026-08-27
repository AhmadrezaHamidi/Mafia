using Ahmad.Mafia.Application.Contract.GameSession.Commands;
using Ahmad.Mafia.Application.Handlers;
using Ahmad.Mafia.Application.Tests.Fakes;
using Ahmad.Mafia.Domain.GameSession.Args;
using Ahmad.Mafia.Domain.GameSession.Entities;
using Ahmad.Mafia.Domain.GameSession.Enums;
using Ahmad.Mafia.Domain.GameSession.Exceptions;
using GameSessionAgg = Ahmad.Mafia.Domain.GameSession.Aggregates.GameSession;

namespace Ahmad.Mafia.Application.Tests.Tests;

public class GameSessionHandlersTests
{
    private readonly FakeGameSessionRepository _repo = new();
    private readonly GameSessionHandlers _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public GameSessionHandlersTests() => _sut = new GameSessionHandlers(_repo, FakeAppDb.Create());

    private static GameSessionAgg MakeSession(int playerCount = 6)
    {
        var players = Enumerable.Range(1, playerCount)
            .Select(i => new GamePlayerSeed(i, $"بازیکن{i}"))
            .ToList();
        return GameSessionAgg.Create(new CreateGameSessionArg(1, RoomId: 10, players));
    }

    [Fact]
    public async Task SubmitNightAction_ByMafia_Should_SetNightTarget()
    {
        var session = MakeSession();
        _repo.Seed(session);
        var mafia = KillDecider(session);
        var target = session.Players.First(p => p.Id != mafia.Id);

        await _sut.Handle(new SubmitNightActionCommand(session.Id, mafia.Id, target.Id), _ct);

        Assert.Equal(target.Id, session.NightTargetPlayerId);
    }

    [Fact]
    public async Task SubmitNightAction_WhenSessionNotFound_Should_Throw_GameSessionNotFoundException()
    {
        await Assert.ThrowsAsync<GameSessionNotFoundException>(
            () => _sut.Handle(new SubmitNightActionCommand(999, 1, 2), _ct));
    }

    [Fact]
    public async Task CastVote_Should_RecordVote_ForCorrectRound()
    {
        var session = MakeSession();
        session.ResolveNightPhase(); // -> Day
        _repo.Seed(session);

        await _sut.Handle(new CastVoteCommand(session.Id, 1, 2), _ct);

        Assert.Equal(2, session.Votes[1]);
    }

    [Fact]
    public async Task RetractVote_Should_RemoveVote()
    {
        var session = MakeSession();
        session.ResolveNightPhase();
        session.CastVote(1, 2);
        _repo.Seed(session);

        await _sut.Handle(new RetractVoteCommand(session.Id, 1), _ct);

        Assert.False(session.Votes.ContainsKey(1));
    }

    [Fact]
    public async Task ResolveNightPhase_Should_TransitionToDay()
    {
        var session = MakeSession();
        _repo.Seed(session);

        await _sut.Handle(new ResolveNightPhaseCommand(session.Id), _ct);

        Assert.Equal(GamePhase.Day, session.Phase);
    }

    [Fact]
    public async Task ResolveVoting_Should_TransitionToNight_AndIncrementRound()
    {
        var session = MakeSession();
        session.ResolveNightPhase(); // round 1 -> Day
        _repo.Seed(session);

        await _sut.Handle(new ResolveVotingCommand(session.Id), _ct);

        Assert.Equal(GamePhase.Night, session.Phase);
        Assert.Equal(2, session.Round);
    }

    [Fact]
    public async Task RequestRematch_WhenNotEnded_Should_Throw_GameNotEndedException()
    {
        var session = MakeSession();
        _repo.Seed(session);

        await Assert.ThrowsAsync<GameNotEndedException>(
            () => _sut.Handle(new RequestRematchCommand(session.Id), _ct));
    }

    [Fact]
    public async Task SubmitNightAction_ByCitizen_Should_Throw_ActionNotAllowedForRoleException()
    {
        var session = MakeSession();
        _repo.Seed(session);
        var citizen = session.Players.First(p => p.Role == Role.SimpleCitizen);
        var anyTarget = session.Players.First(p => p.Id != citizen.Id);

        await Assert.ThrowsAsync<ActionNotAllowedForRoleException>(
            () => _sut.Handle(new SubmitNightActionCommand(session.Id, citizen.Id, anyTarget.Id), _ct));
    }

    [Fact]
    public async Task CastVote_ByEliminatedPlayer_Should_Throw_PlayerAlreadyEliminatedException()
    {
        var session = MakeSession(8);
        var mafia = KillDecider(session);
        var victim = session.Players.First(p => p.Role == Role.SimpleCitizen);
        session.SubmitNightAction(mafia.Id, victim.Id);
        session.ResolveNightPhase(); // victim eliminated -> Day
        _repo.Seed(session);

        await Assert.ThrowsAsync<PlayerAlreadyEliminatedException>(
            () => _sut.Handle(new CastVoteCommand(session.Id, victim.Id, mafia.Id), _ct));
    }

    /// <summary>سناریوی برد شهر: حذف تدریجی مافیا از طریق handler تا رسیدن به GamePhase.Ended</summary>
    [Fact]
    public async Task ResolveVoting_Should_EndGame_WhenLastMafiaEliminated()
    {
        var session = MakeSession(6); // 2 mafia, 4 citizens
        _repo.Seed(session);
        var mafias = session.Players.Where(p => p.Role == Role.SimpleMafia).ToList();

        await _sut.Handle(new ResolveNightPhaseCommand(session.Id), _ct); // -> Day round 1
        VoteOutUnanimously(session, mafias[0].Id);
        await _sut.Handle(new ResolveVotingCommand(session.Id), _ct); // -> Night round 2

        await _sut.Handle(new ResolveNightPhaseCommand(session.Id), _ct); // -> Day round 2
        VoteOutUnanimously(session, mafias[1].Id);
        await _sut.Handle(new ResolveVotingCommand(session.Id), _ct);

        Assert.Equal(GamePhase.Ended, session.Phase);
        Assert.Equal(Domain.GameSession.Enums.WinningTeam.Town, session.WinningTeam);
    }

    [Fact]
    public async Task RequestRematch_WhenEnded_Should_ResetPhaseToNight_AndReviveAllPlayers()
    {
        var session = MakeSession(6);
        var mafias = session.Players.Where(p => p.Role == Role.SimpleMafia).ToList();
        _repo.Seed(session);

        await _sut.Handle(new ResolveNightPhaseCommand(session.Id), _ct);
        VoteOutUnanimously(session, mafias[0].Id);
        await _sut.Handle(new ResolveVotingCommand(session.Id), _ct);
        await _sut.Handle(new ResolveNightPhaseCommand(session.Id), _ct);
        VoteOutUnanimously(session, mafias[1].Id);
        await _sut.Handle(new ResolveVotingCommand(session.Id), _ct);
        Assert.Equal(GamePhase.Ended, session.Phase);

        await _sut.Handle(new RequestRematchCommand(session.Id), _ct);

        Assert.Equal(GamePhase.Night, session.Phase);
        Assert.Equal(1, session.Round);
        Assert.All(session.Players, p => Assert.True(p.IsAlive));
    }

    private static void VoteOutUnanimously(GameSessionAgg session, long targetId)
    {
        foreach (var voter in session.Players.Where(p => p.IsAlive && p.Id != targetId))
            session.CastVote(voter.Id, targetId);
    }
    /// <summary>
    /// مافیایی که اجازه‌ی ثبتِ کشتن دارد. نقش‌ها تصادفی تخصیص می‌یابند و با بیش
    /// از یک مافیای زنده دامین فقط از رئیس می‌پذیرد — برداشتنِ «اولین مافیا»
    /// تست را وابسته به شانس می‌کرد.
    /// </summary>
    private static GamePlayer KillDecider(GameSessionAgg session)
        => session.Players.First(p => p.IsAlive && p.Role is Role.SimpleMafia or Role.GodFather && p.IsMafiaLeader);

}
