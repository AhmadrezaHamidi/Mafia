// نشان بازی بالای صفحه‌ی ورود.
//
// اگر فایل public/mafia-logo.png وجود داشته باشد از آن استفاده می‌شود؛
// وگرنه تیتر متنی. این‌طور می‌شود لوگوی واقعی را بدون دست‌زدن به کد
// جایگزین کرد — فقط فایل را در public/ بگذار.

import { logoUrl, useLogoStatus } from "../lib/assets";

export function LogoMark() {
  const logo = useLogoStatus();

  if (logo === "ok") {
    return (
      <img
        src={logoUrl}
        alt="مافیا"
        width={220}
        height={220}
        className="mx-auto block h-auto w-[min(58vw,220px)]"
        style={{ filter: "drop-shadow(0 6px 24px rgba(200,16,46,0.35))" }}
      />
    );
  }

  // «probing» هم همین را می‌گیرد: تیتر متنی جای درستی را اشغال می‌کند،
  // پس اگر بعداً عکس بیاید صفحه نمی‌پرد.
  return <h1 className="text-5xl">مافیا</h1>;
}
