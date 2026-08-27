namespace Ahmad.Mafia.Domain.Identity.Args;

public sealed record RegisterPlayerArg(
    long Id,
    string Mobile,
    string DisplayName
);

public sealed record IssueOtpArg(
    long Id,
    string Mobile,
    string CodeHash,
    string Salt,
    DateTime NowUtc
);
