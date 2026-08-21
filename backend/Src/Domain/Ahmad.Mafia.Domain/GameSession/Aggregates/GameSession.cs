using AhmadBase.Doamin;
using Ahmad.Mafia.Domain.GameSession.Args;
using Ahmad.Mafia.Domain.GameSession.Entities;
using Ahmad.Mafia.Domain.GameSession.Enums;
using Ahmad.Mafia.Domain.GameSession.Events;
using Ahmad.Mafia.Domain.GameSession.Exceptions;
using Ahmad.Mafia.Domain.Room.Enums;

namespace Ahmad.Mafia.Domain.GameSession.Aggregates;

public sealed class GameSession : AggregateRoot<long>
{
    private readonly List<GamePlayer> _players = [];
    private readonly Dictionary<long, long> _votes = [];

    public long RoomId { get; private set; }
    public ScenarioType Scenario { get; private set; }
    public GamePhase Phase { get; private set; }
    public int Round { get; private set; }
    public DateTime PhaseDeadlineUtc { get; private set; }
    public WinningTeam WinningTeam { get; private set; }
    /// <summary>هدفِ کشتنِ تیم مافیا (نامِ قدیمی حفظ شده تا سازگار با بقیه‌ی کد بمونه).</summary>
    public long? NightTargetPlayerId { get; private set; }
    /// <summary>فقط سناریوی «شب‌های مافیا» — هدفِ نجاتِ دکتر.</summary>
    public long? NightSaveTargetPlayerId { get; private set; }
    /// <summary>فقط سناریوی «شب‌های مافیا» — هدفِ استعلامِ کارآگاه.</summary>
    public long? NightInvestigateTargetPlayerId { get; private set; }
    public long? NightInvestigatorPlayerId { get; private set; }
    /// <summary>فقط سناریوی «محافظ سایه» — کسی که بادیگارد امشب ازش محافظت می‌کنه.</summary>
    public long? NightGuardTargetPlayerId { get; private set; }
    public long? NightGuardianPlayerId { get; private set; }
    /// <summary>فقط سناریوی «شکار روانی» — هدفِ کشتنِ مستقلِ قاتل زنجیره‌ای، جدا از هدفِ مافیا.</summary>
    public long? NightSerialKillerTargetPlayerId { get; private set; }
    public int NightDurationSeconds { get; private set; }
    public int DayDurationSeconds { get; private set; }

    public IReadOnlyCollection<GamePlayer> Players => _players.AsReadOnly();
    public IReadOnlyDictionary<long, long> Votes => _votes;

    private GameSession() { }

    private GameSession(CreateGameSessionArg arg) : base(arg.Id)
    {
        RoomId = arg.RoomId;
        Scenario = arg.Scenario;
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
        var roles = AssignRoles(arg.Scenario, arg.Players.Count);

        for (var i = 0; i < arg.Players.Count; i++)
        {
            var seed = arg.Players[i];
            session._players.Add(new GamePlayer(seed.PlayerId, session.Id, seed.Nickname, roles[i]));
        }

        session.AssignMafiaLeader();
        session.RaiseDomainEvent(new GameSessionStartedEvent(session.Id, arg.RoomId, arg.Players.Count));
        return session;
    }

    // ── Night ─────────────────────────────────────────────

    public void SubmitNightAction(long actorId, long targetId, NightActionType actionType = NightActionType.Kill)
    {
        GuardPhaseIs(GamePhase.Night);

        var actor = GetAlivePlayerOrThrow(actorId);
        GetAlivePlayerOrThrow(targetId);

        switch (actionType)
        {
            case NightActionType.Kill:
                if (actor.Role == Role.SerialKiller)
                {
                    // قاتل زنجیره‌ای مستقله — نه عضو تیم مافیاست نه نیاز به تأیید رئیس داره.
                    NightSerialKillerTargetPlayerId = targetId;
                }
                else
                {
                    GuardCanKill(actor);
                    NightTargetPlayerId = targetId;
                }
                break;

            case NightActionType.Save:
                if (actor.Role != Role.Doctor)
                    throw new ActionNotAllowedForRoleException();
                NightSaveTargetPlayerId = targetId;
                break;

            case NightActionType.Investigate:
                if (actor.Role != Role.Detective)
                    throw new ActionNotAllowedForRoleException();
                NightInvestigateTargetPlayerId = targetId;
                NightInvestigatorPlayerId = actorId;
                break;

            case NightActionType.Guard:
                if (actor.Role != Role.Bodyguard)
                    throw new ActionNotAllowedForRoleException();
                NightGuardTargetPlayerId = targetId;
                NightGuardianPlayerId = actorId;
                break;
        }

        RaiseDomainEvent(new NightActionSubmittedEvent(Id, actorId, targetId));
    }

    /// <summary>
    /// «کشتن» تصمیم تیمیه — مافیای ساده و پدرخوانده هر دو تیم مافیان. وقتی بیش از
    /// یک نفر از تیم زنده باشه، فقط رئیس (پدرخوانده اگه زنده باشه، وگرنه رئیسِ
    /// تصادفی‌شده‌ی مافیای ساده) اجازه‌ی ثبت داره — بقیه فقط توی چت نظر می‌دن.
    /// </summary>
    private void GuardCanKill(GamePlayer actor)
    {
        if (actor.Role is not (Role.SimpleMafia or Role.GodFather))
            throw new ActionNotAllowedForRoleException();

        var aliveMafiaTeamCount = _players.Count(p => p.IsAlive && p.Role is Role.SimpleMafia or Role.GodFather);
        if (aliveMafiaTeamCount > 1 && !actor.IsMafiaLeader)
            throw new MafiaLeaderRequiredException();
    }

    /// <summary>
    /// در «مافیای روسی» فقط شب اول کشتار داره (Round == 1)، در بقیه‌ی سناریوها هر شب.
    /// نجاتِ دکتر روی هدفِ کشتنِ مافیا جلوی مرگ رو می‌گیره. بادیگارد اگه دقیقاً همون
    /// کسی که مافیا هدف گرفته رو نگهبانی کرده باشه، به‌جای هدف خودِ بادیگارد کشته
    /// می‌شه. نتیجه‌ی تحقیقِ کارآگاه فقط برای خودش ذخیره می‌شه — پدرخوانده جلوی
    /// کارآگاه «بی‌گناه» دیده می‌شه. قاتل زنجیره‌ای کاملاً مستقل و هم‌زمان با مافیا
    /// یک قربانی جدا می‌گیره؛ هیچ‌کدوم از نجات/محافظت رو ما به ازاش شامل نمی‌شه.
    /// </summary>
    public void ResolveNightPhase()
    {
        GuardPhaseIs(GamePhase.Night);

        var killAllowedThisRound = Scenario == ScenarioType.MafiaNights || Round == 1
            || Scenario is ScenarioType.MayorElection or ScenarioType.ShadowGuard or ScenarioType.SerialHunt;

        long? eliminatedId = null;
        if (killAllowedThisRound && NightTargetPlayerId is { } targetId)
        {
            var wasSaved = NightSaveTargetPlayerId is { } savedId && savedId == targetId;
            if (!wasSaved)
            {
                var guardedSameTarget = NightGuardTargetPlayerId is { } guardedId && guardedId == targetId;
                if (guardedSameTarget && NightGuardianPlayerId is { } guardianId)
                {
                    var guardian = _players.FirstOrDefault(p => p.Id == guardianId && p.IsAlive);
                    if (guardian is not null)
                    {
                        guardian.Eliminate();
                        eliminatedId = guardian.Id;
                        AssignMafiaLeader();
                    }
                    // اگه بادیگارد خودش قبلاً از یه راه دیگه حذف شده باشه، هدف اصلی هم امشب امن می‌مونه.
                }
                else
                {
                    var target = _players.First(p => p.Id == targetId);
                    target.Eliminate();
                    eliminatedId = targetId;
                    AssignMafiaLeader();
                }
            }
        }

        // قاتل زنجیره‌ای — کاملاً مستقل از منطق بالا، همون شب می‌تونه یه نفر دیگه رو هم بکشه.
        long? secondEliminatedId = null;
        if (NightSerialKillerTargetPlayerId is { } skTargetId)
        {
            var skTarget = _players.FirstOrDefault(p => p.Id == skTargetId && p.IsAlive);
            if (skTarget is not null && skTargetId != eliminatedId)
            {
                skTarget.Eliminate();
                secondEliminatedId = skTargetId;
            }
        }

        if (NightInvestigatorPlayerId is { } investigatorId && NightInvestigateTargetPlayerId is { } investigatedId)
        {
            var investigator = _players.First(p => p.Id == investigatorId);
            var suspect = _players.First(p => p.Id == investigatedId);
            var readsAsMafia = suspect.Role == Role.SimpleMafia; // پدرخوانده عمداً بی‌گناه دیده می‌شه
            investigator.SetInvestigationResult(investigatedId, readsAsMafia);
        }

        NightTargetPlayerId = null;
        NightSaveTargetPlayerId = null;
        NightInvestigateTargetPlayerId = null;
        NightInvestigatorPlayerId = null;
        NightGuardTargetPlayerId = null;
        NightGuardianPlayerId = null;
        NightSerialKillerTargetPlayerId = null;
        RaiseDomainEvent(new NightPhaseResolvedEvent(Id, Round, eliminatedId, secondEliminatedId));

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
            // سناریوی «انتخابات شهر»: رأی شهردار دو نفر حساب می‌شه؛ بقیه‌ی سناریوها همه یک رأی دارن.
            var tally = _votes
                .Select(kv => new { VoterId = kv.Key, TargetId = kv.Value })
                .GroupBy(v => v.TargetId)
                .Select(g => new
                {
                    TargetId = g.Key,
                    Count = g.Sum(v => _players.FirstOrDefault(p => p.Id == v.VoterId)?.Role == Role.Mayor ? 2 : 1),
                })
                .OrderByDescending(x => x.Count)
                .ToList();

            var topCount = tally[0].Count;
            var topVoted = tally.Where(x => x.Count == topCount).ToList();

            if (topVoted.Count == 1)
            {
                eliminatedId = topVoted[0].TargetId;
                _players.First(p => p.Id == eliminatedId).Eliminate();
                AssignMafiaLeader();
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

        var roles = AssignRoles(Scenario, _players.Count);
        for (var i = 0; i < _players.Count; i++)
        {
            _players[i].ResetForRematch();
            _players[i].AssignRole(roles[i]);
        }
        AssignMafiaLeader();

        _votes.Clear();
        NightTargetPlayerId = null;
        NightSaveTargetPlayerId = null;
        NightInvestigateTargetPlayerId = null;
        NightInvestigatorPlayerId = null;
        NightGuardTargetPlayerId = null;
        NightGuardianPlayerId = null;
        NightSerialKillerTargetPlayerId = null;
        WinningTeam = WinningTeam.None;
        Round = 1;
        Phase = GamePhase.Night;
        PhaseDeadlineUtc = DateTime.UtcNow.AddSeconds(NightDurationSeconds);
    }

    // ── Internals ─────────────────────────────────────────

    private static IReadOnlyList<Role> AssignRoles(ScenarioType scenario, int playerCount) => scenario switch
    {
        ScenarioType.MafiaNights => AssignMafiaNightsRoles(playerCount),
        ScenarioType.MayorElection => AssignMayorRoles(playerCount),
        ScenarioType.ShadowGuard => AssignBodyguardRoles(playerCount),
        ScenarioType.SerialHunt => AssignSerialKillerRoles(playerCount),
        _ => AssignRussianMafiaRoles(playerCount),
    };

    /// <summary>سناریوی «مافیای روسی» — فقط دو نقش: مافیای ساده و شهروند ساده.</summary>
    private static IReadOnlyList<Role> AssignRussianMafiaRoles(int playerCount)
    {
        var mafiaCount = Math.Max(1, (int)Math.Round(playerCount / 4.0, MidpointRounding.AwayFromZero));
        var roles = new List<Role>(playerCount);
        for (var i = 0; i < playerCount; i++)
            roles.Add(i < mafiaCount ? Role.SimpleMafia : Role.SimpleCitizen);

        Shuffle(roles);
        return roles;
    }

    /// <summary>
    /// سناریوی «شب‌های مافیا» — یک پدرخوانده (رئیس ثابت تیم مافیا)، چند مافیای ساده،
    /// یک دکتر، یک کارآگاه و بقیه شهروند ساده.
    /// </summary>
    private static IReadOnlyList<Role> AssignMafiaNightsRoles(int playerCount)
    {
        var mafiaTeamCount = Math.Max(2, (int)Math.Round(playerCount / 4.0, MidpointRounding.AwayFromZero));
        var roles = new List<Role> { Role.GodFather };
        for (var i = 1; i < mafiaTeamCount; i++)
            roles.Add(Role.SimpleMafia);

        roles.Add(Role.Doctor);
        roles.Add(Role.Detective);

        while (roles.Count < playerCount)
            roles.Add(Role.SimpleCitizen);

        Shuffle(roles);
        return roles;
    }

    /// <summary>سناریوی «انتخابات شهر» — مثل مافیای روسی به‌علاوه‌ی یک شهروند «شهردار» که رأیش روز دو برابر حساب می‌شه.</summary>
    private static IReadOnlyList<Role> AssignMayorRoles(int playerCount)
    {
        var mafiaCount = Math.Max(1, (int)Math.Round(playerCount / 4.0, MidpointRounding.AwayFromZero));
        var roles = new List<Role>(playerCount);
        for (var i = 0; i < mafiaCount; i++) roles.Add(Role.SimpleMafia);
        roles.Add(Role.Mayor);
        while (roles.Count < playerCount) roles.Add(Role.SimpleCitizen);

        Shuffle(roles);
        return roles;
    }

    /// <summary>سناریوی «محافظ سایه» — مثل مافیای روسی به‌علاوه‌ی یک بادیگارد که هر شب فعاله (نه فقط شب اول).</summary>
    private static IReadOnlyList<Role> AssignBodyguardRoles(int playerCount)
    {
        var mafiaCount = Math.Max(1, (int)Math.Round(playerCount / 4.0, MidpointRounding.AwayFromZero));
        var roles = new List<Role>(playerCount);
        for (var i = 0; i < mafiaCount; i++) roles.Add(Role.SimpleMafia);
        roles.Add(Role.Bodyguard);
        while (roles.Count < playerCount) roles.Add(Role.SimpleCitizen);

        Shuffle(roles);
        return roles;
    }

    /// <summary>
    /// سناریوی «شکار روانی» — مافیای معمولی + یک قاتل زنجیره‌ای مستقل که همون شب‌هایی
    /// که مافیا فعاله، جدا از اون‌ها دست به کشتن می‌زنه.
    /// </summary>
    private static IReadOnlyList<Role> AssignSerialKillerRoles(int playerCount)
    {
        var mafiaCount = Math.Max(1, (int)Math.Round(playerCount / 5.0, MidpointRounding.AwayFromZero));
        var roles = new List<Role>(playerCount);
        for (var i = 0; i < mafiaCount; i++) roles.Add(Role.SimpleMafia);
        roles.Add(Role.SerialKiller);
        while (roles.Count < playerCount) roles.Add(Role.SimpleCitizen);

        Shuffle(roles);
        return roles;
    }

    private static void Shuffle(List<Role> roles)
    {
        var rng = Random.Shared;
        for (var i = roles.Count - 1; i > 0; i--)
        {
            var j = rng.Next(i + 1);
            (roles[i], roles[j]) = (roles[j], roles[i]);
        }
    }

    /// <summary>
    /// یک رئیس مافیا بین مافیاهای زنده تعیین می‌کند — فقط او اکشن شب را نهایی می‌کند.
    /// بعد از هر حذف دوباره صدا زده می‌شود تا اگر رئیس حذف شده بود، جانشین تعیین شود
    /// (وگرنه بقیه‌ی مافیا برای همیشه قفل می‌مانند چون هیچ‌کس اجازه‌ی تصمیم ندارد).
    /// </summary>
    private void AssignMafiaLeader()
    {
        var aliveMafiaTeam = _players.Where(p => p.IsAlive && p.Role is Role.SimpleMafia or Role.GodFather).ToList();
        if (aliveMafiaTeam.Count == 0) return;
        if (aliveMafiaTeam.Any(p => p.IsMafiaLeader)) return;

        // پدرخوانده اگه زنده باشه همیشه رئیس ثابته؛ وگرنه یکی از مافیای ساده تصادفی رئیس می‌شه.
        var godFather = aliveMafiaTeam.FirstOrDefault(p => p.Role == Role.GodFather);
        var leader = godFather ?? aliveMafiaTeam[Random.Shared.Next(aliveMafiaTeam.Count)];
        leader.SetMafiaLeader(true);
    }

    private bool TryEndGame()
    {
        var alive = _players.Where(p => p.IsAlive).ToList();
        var mafiaAlive = alive.Count(p => p.Role is Role.SimpleMafia or Role.GodFather);
        var killerAlive = alive.Count(p => p.Role == Role.SerialKiller);
        // قاتل زنجیره‌ای عضو هیچ تیمی نیست — از شمارشِ شهر کنار گذاشته می‌شه تا شرط‌های
        // برد مافیا/شهر رو اشتباه محاسبه نکنه.
        var townAlive = alive.Count - mafiaAlive - killerAlive;

        // تنها کسی که زنده مونده قاتل زنجیره‌ایه — یعنی «تنها موندن» محقق شده و برده.
        if (killerAlive > 0 && alive.Count == killerAlive)
        {
            EndGame(Enums.WinningTeam.SerialKiller);
            return true;
        }

        if (mafiaAlive == 0 && killerAlive == 0)
        {
            EndGame(Enums.WinningTeam.Town);
            return true;
        }
        if (mafiaAlive > 0 && mafiaAlive >= townAlive && killerAlive == 0)
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
