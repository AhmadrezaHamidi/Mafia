namespace Ahmad.Mafia.Rest.EndPoints.Identity;

public static class AuthConstants
{
    public static class Routes
    {
        public const string BaseRoute = "api/v{version:apiVersion}/Auth";

        public const string RequestOtp = "/Otp/Request";
        public const string VerifyOtp = "/Otp/Verify";
    }

    public static class Names
    {
        public const string RequestOtp = "RequestOtp";
        public const string VerifyOtp = "VerifyOtp";
    }

    public static class Docs
    {
        public static class RequestOtp
        {
            public const string Summary = "ارسال کد یک‌بارمصرف به موبایل";
            public const string Description =
                "کد ۶ رقمی برای شماره صادر می‌کند. تا ۶۰ ثانیه بعد کد تازه نمی‌دهد. " +
                "در نسخه‌ی آزمایشی که درگاه پیامکی وصل نیست، کد در فیلد demoCode برمی‌گردد.";
        }

        public static class VerifyOtp
        {
            public const string Summary = "تأیید کد و ورود";
            public const string Description =
                "کد را می‌سنجد و توکن برمی‌گرداند. اگر شماره تازه باشد حساب ساخته می‌شود و " +
                "displayName الزامی است؛ برای حساب موجود نادیده گرفته می‌شود.";
        }
    }
}
