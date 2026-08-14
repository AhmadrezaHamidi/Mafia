using FluentValidation;
using Ahmad.Mafia.Application.Contract.Room.Commands;

namespace Ahmad.Mafia.Application.Validators.Room;

public sealed class JoinRoomCommandValidator : AbstractValidator<JoinRoomCommand>
{
    public JoinRoomCommandValidator()
    {
        RuleFor(x => x.RoomCode)
            .NotEmpty().WithMessage("کد روم را وارد کن.")
            .Length(6).WithMessage("کد روم باید ۶ کاراکتر باشد.");

        RuleFor(x => x.Nickname)
            .MinimumLength(2).WithMessage("اسم باید حداقل ۲ حرف باشد.")
            .MaximumLength(20).WithMessage("اسم نباید بیشتر از ۲۰ حرف باشد.");
    }
}
