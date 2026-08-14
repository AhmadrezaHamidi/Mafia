namespace Ahmad.Mafia.Rest.EndPoints.Room;

public static class RoomConstants
{
    public static class Routes
    {
        public const string BaseRoute = "api/v{version:apiVersion}/Room";

        public const string CreateRoom = "/";
        public const string JoinRoom = "/Join";
        public const string GetRoom = "/{code}";
        public const string StartRoom = "/{id}/Start";
        public const string LeaveRoom = "/{id}/Members/{playerId}";
    }

    public static class Names
    {
        public const string CreateRoom = "CreateRoom";
        public const string JoinRoom = "JoinRoom";
        public const string GetRoom = "GetRoom";
        public const string StartRoom = "StartRoom";
        public const string LeaveRoom = "LeaveRoom";
    }

    public static class Docs
    {
        public static class CreateRoom
        {
            public const string Summary = "ساخت روم جدید";
            public const string Description = "یک روم با ظرفیت مشخص می‌سازد و سازنده را به‌عنوان Host و اولین عضو ثبت می‌کند.";
        }
        public static class JoinRoom
        {
            public const string Summary = "ورود به روم با کد";
            public const string Description = "با کد ۶ کاراکتری روم، یک بازیکن جدید به Lobby اضافه می‌کند.";
        }
        public static class GetRoom
        {
            public const string Summary = "اطلاعات روم";
            public const string Description = "وضعیت فعلی روم و لیست اعضای Lobby را برمی‌گرداند.";
        }
        public static class StartRoom
        {
            public const string Summary = "شروع بازی";
            public const string Description = "فقط Host و فقط وقتی ظرفیت روم پر شده باشد می‌تواند بازی را شروع کند.";
        }
        public static class LeaveRoom
        {
            public const string Summary = "خروج از روم";
            public const string Description = "بازیکن را از Lobby حذف می‌کند؛ اگر Host بود، مالکیت به نفر بعدی منتقل می‌شود.";
        }
    }
}
