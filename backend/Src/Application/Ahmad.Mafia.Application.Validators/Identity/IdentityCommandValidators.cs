using Ahmad.Mafia.Application.Contract.Identity.Commands;
using FluentValidation;

namespace Ahmad.Mafia.Application.Validators.Identity;

public sealed class RequestOtpCommandValidator : AbstractValidator<RequestOtpCommand>
{
    public RequestOtpCommandValidator()
    {
        // شکل دقیق شماره را دامین می‌سنجد (چون ارقام فارسی و پیشوندها را هم
        // نرمال می‌کند)؛ اینجا فقط جلوی ورودی خالی گرفته می‌شود.
        RuleFor(x => x.Mobile).NotEmpty().WithMessage("شماره موبایل را وارد کن.");
    }
}

public sealed class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
{
    public VerifyOtpCommandValidator()
    {
        RuleFor(x => x.Mobile).NotEmpty().WithMessage("شماره موبایل را وارد کن.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("کد را وارد کن.")
            .Matches(@"^\d{6}$").WithMessage("کد باید ۶ رقم باشد.");
    }
}
