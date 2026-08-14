using FluentValidation;
using Ahmad.Mafia.Application.Contract.Room.Commands;

namespace Ahmad.Mafia.Application.Validators.Room;

public sealed class CreateRoomCommandValidator : AbstractValidator<CreateRoomCommand>
{
    public CreateRoomCommandValidator()
    {
        RuleFor(x => x.HostNickname)
            .MinimumLength(2).WithMessage("اسم باید حداقل ۲ حرف باشد.")
            .MaximumLength(20).WithMessage("اسم نباید بیشتر از ۲۰ حرف باشد.");

        RuleFor(x => x.Capacity)
            .InclusiveBetween(6, 15).WithMessage("ظرفیت روم باید بین ۶ تا ۱۵ نفر باشد.");
    }
}
