// آدرس و در دسترس بودنِ لوگوی بازی.
//
// چرا مسیر را از BASE_URL می‌سازیم: اپ روی سرور زیر /Mafia سرو می‌شود، پس
// مسیر مطلقِ "/mafia-logo.png" آنجا ۴۰۴ می‌دهد.
//
// چرا اول probe می‌کنیم و بعد <img> را رندر: اگر فایل نباشد، مرورگر تا قبل
// از رسیدن onError آیکنِ «تصویر شکسته» را نشان می‌دهد — یک فلشِ زشت در هر
// بار لود. با probe، تا وقتی جواب نیامده هیچ <img>ای در DOM نیست.

import { useEffect, useState } from "react";

const base = import.meta.env.BASE_URL.replace(/\/$/, "");

/** لوگوی بازی؛ اگر فایل نباشد کامپوننت‌ها به نسخه‌ی وکتوری برمی‌گردند */
export const logoUrl = `${base}/mafia-logo.png`;

type Status = "probing" | "ok" | "missing";

// نتیجه بین همه‌ی کامپوننت‌ها مشترک است تا فقط یک درخواست برود
let cached: Status = "probing";
let started = false;
const listeners = new Set<(s: Status) => void>();

function probe() {
  const img = new Image();
  const settle = (s: Status) => () => {
    cached = s;
    listeners.forEach((fn) => fn(s));
  };
  img.onload = settle("ok");
  img.onerror = settle("missing");
  img.src = logoUrl;
}

/** «probing» یعنی هنوز نمی‌دانیم — در این حالت هیچ‌کدام را رندر نکن */
export function useLogoStatus(): Status {
  const [status, setStatus] = useState(cached);

  useEffect(() => {
    if (cached !== "probing") {
      setStatus(cached);
      return;
    }
    listeners.add(setStatus);
    if (!started) {
      started = true;
      probe();
    }
    return () => {
      listeners.delete(setStatus);
    };
  }, []);

  return status;
}
