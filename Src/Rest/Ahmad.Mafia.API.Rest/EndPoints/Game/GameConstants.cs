namespace Ahmad.Mafia.Rest.EndPoints.Game;

public static class GameConstants
{
    public static class Routes
    {
        public const string BaseRoute = "api/v{version:apiVersion}/Game";

        public const string GetState = "/{id}/State";
        public const string GetResult = "/{id}/Result";
        public const string SubmitNightAction = "/{id}/Night/Action";
        public const string CastVote = "/{id}/Day/Vote";
        public const string RetractVote = "/{id}/Day/Vote";
        public const string Rematch = "/{id}/Rematch";
    }

    public static class Names
    {
        public const string GetState = "GetGameState";
        public const string GetResult = "GetGameResult";
        public const string SubmitNightAction = "SubmitNightAction";
        public const string CastVote = "CastVote";
        public const string RetractVote = "RetractVote";
        public const string Rematch = "RequestRematch";
    }

    public static class Docs
    {
        public static class GetState
        {
            public const string Summary = "دریافت state بازی";
            public const string Description = "state فیلترشده برای بازیکن درخواست‌دهنده — نقش بقیه‌ی بازیکنان هرگز افشا نمی‌شود.";
        }
        public static class GetResult
        {
            public const string Summary = "نتیجه‌ی نهایی بازی";
            public const string Description = "فقط بعد از پایان بازی در دسترسه؛ همه‌ی نقش‌ها رونمایی می‌شوند.";
        }
        public static class SubmitNightAction
        {
            public const string Summary = "ثبت اکشن شب";
            public const string Description = "فقط مافیای زنده می‌تواند تا پایان فاز شب هدف را ثبت یا تغییر دهد.";
        }
        public static class CastVote
        {
            public const string Summary = "ثبت رأی روز";
            public const string Description = "رأی روز علنی‌ست و برای همه‌ی بازیکنان نمایش داده می‌شود.";
        }
        public static class RetractVote
        {
            public const string Summary = "پس‌گرفتن رأی";
            public const string Description = "بازیکن می‌تواند قبل از پایان فاز روز رأی‌اش را پس بگیرد.";
        }
        public static class Rematch
        {
            public const string Summary = "بازی دوباره";
            public const string Description = "بعد از پایان بازی، یک راند جدید با همان اعضا و نقش‌های تازه شروع می‌کند.";
        }
    }
}
