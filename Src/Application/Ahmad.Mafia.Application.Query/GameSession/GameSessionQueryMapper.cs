using Ahmad.Mafia.Domain.GameSession.Enums;
using GameSessionAgg = Ahmad.Mafia.Domain.GameSession.Aggregates.GameSession;

namespace Ahmad.Mafia.Application.Query.Mappers;

internal static class GameSessionQueryMapper
{
    internal static GetGameStateQueryResponse ToStateResponse(this GameSessionAgg session, long requestingPlayerId)
    {
        var me = session.Players.FirstOrDefault(p => p.Id == requestingPlayerId);
        var timeLeft = Math.Max(0, (int)Math.Ceiling((session.PhaseDeadlineUtc - DateTime.UtcNow).TotalSeconds));

        var isMafia = me?.Role == Role.SimpleMafia;
        long? myNightTarget = session.Phase == GamePhase.Night && isMafia ? session.NightTargetPlayerId : null;
        IReadOnlyDictionary<long, long>? votes = session.Phase == GamePhase.Day ? session.Votes : null;

        return new GetGameStateQueryResponse(
            GameSessionId: session.Id,
            Phase: session.Phase.ToString(),
            Round: session.Round,
            TimeLeftSeconds: timeLeft,
            MyRole: me?.Role.ToString(),
            IAmAlive: me?.IsAlive ?? false,
            MyNightTarget: myNightTarget,
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
