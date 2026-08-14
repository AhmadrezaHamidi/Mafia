using Ahmad.Mafia.Application.Contract.GameSession.Commands;
using Ahmad.Mafia.Domain.GameSession.Enums;
using Ahmad.Mafia.Domain.Repositories;
using AhmadBase.Application;

namespace Ahmad.Mafia.ServiceHost;

/// <summary>
/// هر چند ثانیه یک‌بار GameSession هایی که تایمر فازشون تموم شده رو پیدا می‌کنه
/// و ResolveNightPhase/ResolveVoting رو براشون صدا می‌زنه — حتی اگه هیچ بازیکنی اکشنی نزده باشه.
/// </summary>
public sealed class PhaseTimerBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<PhaseTimerBackgroundService> logger) : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(2);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(PollInterval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await ResolveDueSessionsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "خطا در resolve کردن فاز بازی‌های سررسیدشده.");
            }
        }
    }

    private async Task ResolveDueSessionsAsync(CancellationToken token)
    {
        using var scope = scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IGameSessionRepository>();
        var bus = scope.ServiceProvider.GetRequiredService<ICommandBus>();

        var dueSessions = await repository.GetSessionsPastDeadlineAsync(token);

        foreach (var session in dueSessions)
        {
            if (session.Phase == GamePhase.Night)
                await bus.Dispatch<long>(new ResolveNightPhaseCommand(session.Id), token);
            else if (session.Phase == GamePhase.Day)
                await bus.Dispatch<long>(new ResolveVotingCommand(session.Id), token);
        }
    }
}
