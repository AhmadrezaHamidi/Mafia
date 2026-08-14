using Ahmad.Mafia.Application.Query.Handlers;
using Ahmad.Mafia.Application.Query.Queries;
using Ahmad.Mafia.Application.Tests.Fakes;
using Ahmad.Mafia.Domain.GameSession.Args;
using Ahmad.Mafia.Domain.GameSession.Enums;
using Ahmad.Mafia.Domain.GameSession.Exceptions;
using GameSessionAgg = Ahmad.Mafia.Domain.GameSession.Aggregates.GameSession;

namespace Ahmad.Mafia.Application.Tests.Tests;

public class GameSessionQueryHandlersTests
{
    private readonly FakeGameSessionRepository _repo = new();
    private readonly GameSessionQueryHandlers _sut;
    private readonly CancellationToken _ct = CancellationToken.None;

    public GameSessionQueryHandlersTests() => _sut = new GameSessionQueryHandlers(_repo);

    private static GameSessionAgg MakeSession(int playerCount = 6)
    {
        var players = Enumerable.Range(1, playerCount)
            .Select(i => new GamePlayerSeed(i, $"بازیکن{i}"))
            .ToList();
        return GameSessionAgg.Create(new CreateGameSessionArg(1, RoomId: 10, players));
    }

    /// <summary>
    /// مهم‌ترین قانون امنیتی دامنه: خروجی State نباید نقش بقیه‌ی بازیکنان را افشا کند —
    /// فقط MyRole باید نقش caller باشد، DTO اصلاً فیلدی برای نقش سایرین ندارد.
    /// </summary>
    [Fact]
    public async Task GetGameState_Should_OnlyRevealRequestingPlayersOwnRole()
    {
        var session = MakeSession();
        _repo.Seed(session);
        var me = session.Players.First();

        var response = await _sut.HandleAsync(new GetGameStateQuery(session.Id, me.Id), _ct);

        Assert.Equal(me.Role.ToString(), response.MyRole);
        // DTO نوع GamePlayerView اصلاً فیلد Role ندارد — یعنی از نظر type-system هم نقش بقیه قابل افشا نیست.
        Assert.DoesNotContain("Role", typeof(GamePlayerView).GetProperties().Select(p => p.Name));
    }

    [Fact]
    public async Task GetGameResult_WhenGameNotEnded_Should_Throw_GameNotEndedException()
    {
        var session = MakeSession();
        _repo.Seed(session);

        await Assert.ThrowsAsync<GameNotEndedException>(
            () => _sut.HandleAsync(new GetGameResultQuery(session.Id), _ct));
    }

    [Fact]
    public async Task GetGameState_Night_Should_ExposeVotes_AsNull()
    {
        var session = MakeSession();
        _repo.Seed(session);
        var me = session.Players.First();

        var response = await _sut.HandleAsync(new GetGameStateQuery(session.Id, me.Id), _ct);

        Assert.Null(response.Votes);
    }

    [Fact]
    public async Task GetGameState_Day_Should_ExposeVotes_Publicly()
    {
        var session = MakeSession();
        session.ResolveNightPhase(); // -> Day
        session.CastVote(1, 2);
        _repo.Seed(session);

        var response = await _sut.HandleAsync(new GetGameStateQuery(session.Id, 3), _ct);

        Assert.NotNull(response.Votes);
        Assert.Equal(2, response.Votes![1]);
    }

    /// <summary>بعد از پایان بازی، برخلاف GetGameState، همه‌ی نقش‌ها باید رونمایی بشن</summary>
    [Fact]
    public async Task GetGameResult_WhenGameEnded_Should_RevealAllRoles()
    {
        var session = MakeSession(6);
        var mafias = session.Players.Where(p => p.Role == Role.SimpleMafia).ToList();

        session.ResolveNightPhase();
        VoteOutUnanimously(session, mafias[0].Id);
        session.ResolveVoting();
        session.ResolveNightPhase();
        VoteOutUnanimously(session, mafias[1].Id);
        session.ResolveVoting();
        Assert.Equal(GamePhase.Ended, session.Phase);
        _repo.Seed(session);

        var response = await _sut.HandleAsync(new GetGameResultQuery(session.Id), _ct);

        Assert.Equal("Town", response.WinningTeam);
        Assert.Equal(6, response.Reveal.Count);
        Assert.Equal(2, response.Reveal.Count(r => r.Role == "SimpleMafia"));
        Assert.Contains(response.Reveal, r => !r.IsAlive);
    }

    private static void VoteOutUnanimously(GameSessionAgg session, long targetId)
    {
        foreach (var voter in session.Players.Where(p => p.IsAlive && p.Id != targetId))
            session.CastVote(voter.Id, targetId);
    }
}
