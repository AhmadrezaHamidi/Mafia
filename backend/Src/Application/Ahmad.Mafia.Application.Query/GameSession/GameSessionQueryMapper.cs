using Ahmad.Mafia.Domain.GameSession.Enums;
using GameSessionAgg = Ahmad.Mafia.Domain.GameSession.Aggregates.GameSession;

namespace Ahmad.Mafia.Application.Query.Mappers;

internal static class GameSessionQueryMapper
{
    internal static GetGameStateQueryResponse ToStateResponse(this GameSessionAgg session, long requestingPlayerId)
    {
        var me = session.Players.FirstOrDefault(p => p.Id == requestingPlayerId);
        var timeLeft = Math.Max(0, (int)Math.Ceiling((session.PhaseDeadlineUtc - DateTime.UtcNow).TotalSeconds));

        var isMafiaTeam = me?.Role is Role.SimpleMafia or Role.GodFather;
        var isDoctor = me?.Role == Role.Doctor;
        var isDetective = me?.Role == Role.Detective;
        var isBodyguard = me?.Role == Role.Bodyguard;
        var isSerialKiller = me?.Role == Role.SerialKiller;
        var atNight = session.Phase == GamePhase.Night;

        long? myNightTarget = atNight && isMafiaTeam
            ? session.NightTargetPlayerId
            : atNight && isSerialKiller
                ? session.NightSerialKillerTargetPlayerId
                : null;
        long? myNightSaveTarget = atNight && isDoctor ? session.NightSaveTargetPlayerId : null;
        long? myNightInvestigateTarget = atNight && isDetective ? session.NightInvestigateTargetPlayerId : null;
        long? myNightGuardTarget = atNight && isBodyguard ? session.NightGuardTargetPlayerId : null;

        InvestigationResultView? myLastInvestigation =
            isDetective && me?.LastInvestigationTargetId is { } targetId && me.LastInvestigationIsMafia is { } isMafia
                ? new InvestigationResultView(targetId, isMafia)
                : null;

        IReadOnlyDictionary<long, long>? votes = session.Phase == GamePhase.Day ? session.Votes : null;

        return new GetGameStateQueryResponse(
            GameSessionId: session.Id,
            Scenario: session.Scenario.ToString(),
            Phase: session.Phase.ToString(),
            Round: session.Round,
            TimeLeftSeconds: timeLeft,
            MyRole: me?.Role.ToString(),
            IAmAlive: me?.IsAlive ?? false,
            MyIsMafiaLeader: isMafiaTeam ? me?.IsMafiaLeader : null,
            MyNightTarget: myNightTarget,
            MyNightSaveTarget: myNightSaveTarget,
            MyNightInvestigateTarget: myNightInvestigateTarget,
            MyLastInvestigation: myLastInvestigation,
            MyNightGuardTarget: myNightGuardTarget,
            Players: session.Players
                .Select(p => new GamePlayerView(p.Id, p.Nickname, p.IsAlive, p.Connection.ToString()))
                .ToList(),
            Votes: votes
        );
    }

    internal static GetGameResultQueryResponse ToResultResponse(this GameSessionAgg session) => new(
        GameSessionId: session.Id,
        WinningTeam: session.WinningTeam.ToString(),
        Reveal: session.Players
            .Select(p => new RevealedPlayerResponse(p.Id, p.Nickname, p.Role.ToString(), p.IsAlive))
            .ToList()
    );
}
