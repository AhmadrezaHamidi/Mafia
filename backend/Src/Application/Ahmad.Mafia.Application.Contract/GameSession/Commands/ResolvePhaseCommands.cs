using AhmadBase.Application;

namespace Ahmad.Mafia.Application.Contract.GameSession.Commands;

/// <summary>توسط PhaseTimerBackgroundService وقتی تایمر شب تمام می‌شود صدا زده می‌شود</summary>
public record ResolveNightPhaseCommand(
    long GameSessionId
) : ICommand<long>;

/// <summary>توسط PhaseTimerBackgroundService وقتی تایمر روز تمام می‌شود صدا زده می‌شود</summary>
public record ResolveVotingCommand(
    long GameSessionId
) : ICommand<long>;
