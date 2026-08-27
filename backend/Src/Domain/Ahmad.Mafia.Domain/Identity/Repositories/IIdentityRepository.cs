using Ahmad.Mafia.Domain.Identity.Aggregates;

namespace Ahmad.Mafia.Domain.Identity.Repositories;

public interface IIdentityRepository
{
    Task<PlayerAccount?> GetAccountByMobileAsync(string mobile, CancellationToken token = default);
    Task<PlayerAccount?> GetAccountByIdAsync(long id, CancellationToken token = default);
    Task AddAccountAsync(PlayerAccount account, CancellationToken token = default);
    Task UpdateAccountAsync(PlayerAccount account, CancellationToken token = default);

    /// <summary>تازه‌ترین کدِ صادرشده برای این شماره — چه مصرف‌شده چه نه.</summary>
    Task<OtpChallenge?> GetLatestChallengeAsync(string mobile, CancellationToken token = default);
    Task AddChallengeAsync(OtpChallenge challenge, CancellationToken token = default);
    Task UpdateChallengeAsync(OtpChallenge challenge, CancellationToken token = default);

    Task<long> GetNextAccountIdAsync();
    Task<long> GetNextChallengeIdAsync();
}
