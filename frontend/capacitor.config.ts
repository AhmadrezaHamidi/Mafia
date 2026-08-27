import type { CapacitorConfig } from "@capacitor/cli";

// اپ موبایل همان build وب است که داخل بسته‌ی نیتیو سرو می‌شود — کد React
// دوباره نوشته نمی‌شود.
//
// دو تفاوت مهم با نسخه‌ی وب:
//  ۱) صفحه از capacitor://localhost بالا می‌آید نه از /Mafia، پس build موبایل
//     باید با base نسبی گرفته شود (اسکریپت build:mobile).
//  ۲) هیچ پروکسی‌ای در کار نیست، پس VITE_API_BASE باید آدرس کامل سرور باشد.
//
// سرور روی HTTPS است، پس نه allowMixedContent لازم است نه استثنای cleartext.
const config: CapacitorConfig = {
  appId: "ir.ahmadhamidi.mafia",
  appName: "مافیا",
  webDir: "dist",

  plugins: {
    SplashScreen: {
      launchShowDuration: 1200,
      backgroundColor: "#0A0709",
      androidSpinnerStyle: "small",
      spinnerColor: "#9C2B32",
    },
  },
};

export default config;
