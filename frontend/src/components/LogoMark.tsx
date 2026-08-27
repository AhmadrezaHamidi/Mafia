// نشان بازی بالای صفحه‌ی ورود.
//
// اگر فایل public/mafia-logo.png وجود داشته باشد از آن استفاده می‌شود؛
// وگرنه به تیتر متنی برمی‌گردد. این‌طور می‌شود لوگوی واقعی را بدون دست‌زدن
// به کد جایگزین کرد — فقط فایل را در public/ بگذار.

import { useState } from "react";

export function LogoMark() {
  const [hasImage, setHasImage] = useState(true);

  if (!hasImage) {
    return <h1 className="text-5xl">مافیا</h1>;
  }

  return (
    <img
      src="/mafia-logo.png"
      alt="مافیا"
      width={220}
      height={220}
      onError={() => setHasImage(false)}
      className="mx-auto block h-auto w-[min(58vw,220px)]"
      style={{ filter: "drop-shadow(0 6px 24px rgba(200,16,46,0.35))" }}
    />
  );
}
