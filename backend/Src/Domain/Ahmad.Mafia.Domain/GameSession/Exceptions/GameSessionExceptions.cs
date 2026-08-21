using AhmadBase.Doamin;

namespace Ahmad.Mafia.Domain.GameSession.Exceptions;

public sealed class GameSessionNotFoundException : BusinessException
{
    public GameSessionNotFoundException() : base("جلسه‌ی بازی پیدا نشد.") { }
}

public sealed class WrongPhaseForActionException : BusinessException
{
    public WrongPhaseForActionException() : base("این کار در فاز فعلی بازی مجاز نیست.") { }
}

public sealed class ActionNotAllowedForRoleException : BusinessException
{
    public ActionNotAllowedForRoleException() : base("نقش تو اجازه‌ی این کار را نمی‌دهد.") { }
}

public sealed class PlayerAlreadyEliminatedException : BusinessException
{
    public PlayerAlreadyEliminatedException() : base("این بازیکن قبلاً حذف شده است.") { }
}

public sealed class PlayerNotInGameException : BusinessException
{
    public PlayerNotInGameException() : base("این بازیکن در این بازی حضور ندارد.") { }
}

public sealed class GameNotEndedException : BusinessException
{
    public GameNotEndedException() : base("بازی هنوز تمام نشده است.") { }
}

public sealed class NotEnoughPlayersException : BusinessException
{
    public NotEnoughPlayersException() : base("برای شروع بازی حداقل ۶ بازیکن لازم است.") { }
}

public sealed class MafiaLeaderRequiredException : BusinessException
{
    public MafiaLeaderRequiredException() : base("فقط رئیس مافیا می‌تونه تصمیم نهایی شب رو ثبت کنه — بقیه فقط توی چت نظر می‌دن.") { }
}
