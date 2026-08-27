using Ahmad.Mafia.Domain.Identity.Aggregates;
using Ahmad.Mafia.Domain.Identity.Repositories;
using AhmadBase.Persistence.NHiLoHelper;
using Microsoft.EntityFrameworkCore;

namespace Ahmad.Mafia.Persistence.EF.Repositories;

public sealed class IdentityRepository(
    MafiaDbContext context,
    IHiLoIdGenerator hiLoGenerator) : IIdentityRepository
{
    public async Task<PlayerAccount?> GetAccountByMobileAsync(string mobile, CancellationToken token = default)
        => await context.PlayerAccounts.FirstOrDefaultAsync(x => x.Mobile == mobile, token);

    public async Task<PlayerAccount?> GetAccountByIdAsync(long id, CancellationToken token = default)
        => await context.PlayerAccounts.FirstOrDefaultAsync(x => x.Id == id, token);

    public async Task AddAccountAsync(PlayerAccount account, CancellationToken token = default)
        => await context.PlayerAccounts.AddAsync(account, token);

    public Task UpdateAccountAsync(PlayerAccount account, CancellationToken token = default)
    {
        context.PlayerAccounts.Update(account);
        return Task.CompletedTask;
    }

    public async Task<OtpChallenge?> GetLatestChallengeAsync(string mobile, CancellationToken token = default)
        => await context.OtpChallenges
            .Where(x => x.Mobile == mobile)
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync(token);

    public async Task AddChallengeAsync(OtpChallenge challenge, CancellationToken token = default)
        => await context.OtpChallenges.AddAsync(challenge, token);

    public Task UpdateChallengeAsync(OtpChallenge challenge, CancellationToken token = default)
    {
        context.OtpChallenges.Update(challenge);
        return Task.CompletedTask;
    }

    public Task<long> GetNextAccountIdAsync()
        => Task.FromResult(hiLoGenerator.GetNextId<PlayerAccount>());

    public Task<long> GetNextChallengeIdAsync()
        => Task.FromResult(hiLoGenerator.GetNextId<OtpChallenge>());
}
