// پس‌زمینه‌ی سراسری — روی همه‌ی صفحات، نه فقط ورود.
//
// اگر public/mafia-logo.png موجود باشد از آن استفاده می‌شود؛ وگرنه پیکره‌ی
// وکتوری. هر دو داخل همان لایه و زیر همان scrim می‌نشینند، پس با گذاشتن
// فایل، پس‌زمینه عوض می‌شود بدون اینکه خوانایی متن فرق کند.

import { NoirPoster } from "./NoirPoster";
import { logoUrl, useLogoStatus } from "../lib/assets";

interface Props {
  /** شدت حضور پس‌زمینه؛ صفحه‌ی ورود پررنگ‌تر، داخل بازی محوتر */
  intensity?: "full" | "dim";
}

export function AppBackground({ intensity = "dim" }: Props) {
  const logo = useLogoStatus();

  return (
    <div aria-hidden="true" className={`app-bg app-bg--${intensity}`}>
      {logo === "ok" && <img src={logoUrl} alt="" className="app-bg__layer app-bg__img" />}
      {logo === "missing" && <NoirPoster />}
      {/* لایه‌ی تیره‌کننده: بدون این، متن روی نواحی روشن خوانا نمی‌ماند */}
      <div className="app-bg__scrim" />
    </div>
  );
}
