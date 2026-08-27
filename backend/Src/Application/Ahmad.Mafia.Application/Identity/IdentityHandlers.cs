using System.Security.Cryptography;
using System.Text;
using Ahmad.Mafia.Application.Contract.Identity.Commands;
using Ahmad.Mafia.Application.Contract.Identity.Services;
using Ahmad.Mafia.Domain.Identity.Aggregates;
using Ahmad.Mafia.Domain.Identity.Args;
using Ahmad.Mafia.Domain.Identity.Exceptions;
using Ahmad.Mafia.Domain.Identity.Repositories;
using Ahmad.Mafia.Domain.Identity.ValueObjects;
using Ahmad.Mafia.Persistence.EF;

namespace Ahmad.Mafia.Application.Handlers;

public sealed class IdentityHandlers(
    IIdentityRepository repository,
    IJwtService jwtService,
    IOtpSender otpSender,
    MafiaDbContext context) :
    ICommandHandler<RequestOtpCommand, RequestOtpResult>,
    ICommandHandler<VerifyOtpCommand, VerifyOtpResult>
{
    public async Task<RequestOtpResult> Handle(RequestOtpCommand command, CancellationToken token)
    {
        var mobile = MobileNumber.Normalize(command.Mobile);
        var now = DateTime.UtcNow;

        // اگر کد قبلی هنوز در بازه‌ی cooldown است، کد تازه نمی‌دهیم. این تنها
        // چیزی است که جلوی استفاده از endpoint به‌عنوان بمب‌افکن پیامک را می‌گیرد.
        var latest = await repository.GetLatestChallengeAsync(mobile, token);
        if (latest is not null)
        {
            var wait = latest.SecondsUntilResendAllowed(now);
            if (wait > 0) throw new OtpResendTooSoonException(wait);
        }

        var code = GenerateCode();
        var salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
        var id = await repository.GetNextChallengeIdAsync();

        var challenge = OtpChallenge.Issue(new IssueOtpArg(id, mobile, HashCode(salt, code), salt, now));
        await repository.AddChallengeAsync(challenge, token);
        await context.CommitAsync(token);

        var delivered = await otpSender.SendAsync(mobile, code, token);
        var account = await repository.GetAccountByMobileAsync(mobile, token);

        return new RequestOtpResult(
            Mobile: MobileNumber.ToLocalFormat(mobile),
            ExpiresInSeconds: challenge.SecondsUntilExpiry(now),
            ResendAfterSeconds: OtpChallenge.ResendCooldownSeconds,
            IsRegistered: account is not null,
            // پیامک واقعی که رفته باشد، کد نباید از API بیرون بزند
            DemoCode: delivered ? null : code);
    }

    public async Task<VerifyOtpResult> Handle(VerifyOtpCommand command, CancellationToken token)
    {
        var mobile = MobileNumber.Normalize(command.Mobile);
        var now = DateTime.UtcNow;

        var challenge = await repository.GetLatestChallengeAsync(mobile, token)
            ?? throw new OtpNotFoundException();

        try
        {
            challenge.Verify(HashCode(challenge.Salt, (command.Code ?? string.Empty).Trim()), now);
        }
        finally
        {
            // شمارنده‌ی تلاش ناموفق هم باید بماند، وگرنه سقف تلاش بی‌اثر می‌شود.
            await repository.UpdateChallengeAsync(challenge, token);
            await context.CommitAsync(token);
        }

        var account = await repository.GetAccountByMobileAsync(mobile, token);
        var isNew = account is null;

        if (account is null)
        {
            var accountId = await repository.GetNextAccountIdAsync();
            account = PlayerAccount.Register(new RegisterPlayerArg(accountId, mobile, command.DisplayName ?? string.Empty));
            await repository.AddAccountAsync(account, token);
        }
        else
        {
            account.RecordLogin();
            await repository.UpdateAccountAsync(account, token);
        }

        await context.CommitAsync(token);

        return new VerifyOtpResult(
            PlayerId: account.Id,
            Mobile: MobileNumber.ToLocalFormat(account.Mobile),
            DisplayName: account.DisplayName,
            IsNewAccount: isNew,
            Token: jwtService.GenerateToken(account.Id, account.Mobile, account.DisplayName));
    }

    /// <summary>کد ۶ رقمی با مولد امن — Random معمولی قابل پیش‌بینی است.</summary>
    private static string GenerateCode()
        => RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");

    private static string HashCode(string salt, string code)
        => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(salt + ':' + code)));
}
