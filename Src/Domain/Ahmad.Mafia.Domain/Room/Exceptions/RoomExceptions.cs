using AhmadBase.Doamin;

namespace Ahmad.Mafia.Domain.Room.Exceptions;

public sealed class RoomNotFoundException : BusinessException
{
    public RoomNotFoundException() : base("روم مورد نظر پیدا نشد.") { }
}

public sealed class RoomFullException : BusinessException
{
    public RoomFullException() : base("ظرفیت روم تکمیل شده است.") { }
}

public sealed class RoomNotFullException : BusinessException
{
    public RoomNotFullException() : base("بازی تا پر نشدن ظرفیت روم شروع نمی‌شود.") { }
}

public sealed class RoomAlreadyStartedException : BusinessException
{
    public RoomAlreadyStartedException() : base("بازی این روم قبلاً شروع شده است.") { }
}

public sealed class RoomClosedException : BusinessException
{
    public RoomClosedException() : base("این روم بسته شده است.") { }
}

public sealed class PlayerNotInRoomException : BusinessException
{
    public PlayerNotInRoomException() : base("این بازیکن عضو روم نیست.") { }
}

public sealed class OnlyHostCanPerformActionException : BusinessException
{
    public OnlyHostCanPerformActionException() : base("فقط میزبان روم می‌تواند این کار را انجام دهد.") { }
}

public sealed class InvalidNicknameException : BusinessException
{
    public InvalidNicknameException() : base("نام باید حداقل ۲ حرف باشد.") { }
}

public sealed class InvalidCapacityException : BusinessException
{
    public InvalidCapacityException() : base("ظرفیت روم باید بین ۶ تا ۱۵ نفر باشد.") { }
}
