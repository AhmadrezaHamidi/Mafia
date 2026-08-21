using FluentValidation;
using Ahmad.Mafia.Application.Contract.Room.Commands;

namespace Ahmad.Mafia.Application.Validators.Room;

public sealed class QuickJoinCommandValidator : AbstractValidator<QuickJoinCommand>
{
    public QuickJoinCommandValidator()
    {
        RuleFor(x => x.Nickname)
            .MinimumLength(2).WithMessage("اسم باید حداقل ۲ حرف باشد.")
            .MaximumLength(20).WithMessage("اسم نباید بیشتر از ۲۰ حرف باشد.");
    }
}
