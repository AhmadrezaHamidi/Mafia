// سطح صدای میکروفون → یک CSS variable روی خودِ عنصر DOM.
//
// چرا از state ری‌اکت استفاده نمی‌کنیم: هاله باید ~۶۰ بار در ثانیه به‌روز شود.
// اگر این را از setState عبور بدهیم، کل درخت در هر فریم رندر می‌شود و روی
// موبایل بازی کند می‌شود. به‌جایش مستقیم `--level` را روی همان یک عنصر
// می‌نویسیم و بقیه‌ی کامپوننت اصلاً رندر نمی‌شود.

import { useEffect, useRef } from "react";

interface Options {
  /** فقط وقتی نوبت این بازیکن است میکروفون باز می‌شود */
  active: boolean;
}

export function useVoiceLevel({ active }: Options) {
  const ref = useRef<HTMLElement | null>(null);

  useEffect(() => {
    if (!active) {
      ref.current?.style.setProperty("--level", "0");
      return;
    }

    // کاربری که کاهش حرکت خواسته، هاله‌ی نفس‌کش نمی‌بیند — فقط یک حالت ثابت
    const reduced =
      typeof matchMedia === "function" &&
      matchMedia("(prefers-reduced-motion: reduce)").matches;
    if (reduced) {
      ref.current?.style.setProperty("--level", "0.55");
      return;
    }

    let raf = 0;
    let ctx: AudioContext | null = null;
    let stream: MediaStream | null = null;
    let cancelled = false;

    (async () => {
      try {
        stream = await navigator.mediaDevices.getUserMedia({
          audio: { echoCancellation: true, noiseSuppression: true },
        });
        if (cancelled) {
          stream.getTracks().forEach((t) => t.stop());
          return;
        }

        ctx = new AudioContext();
        const source = ctx.createMediaStreamSource(stream);
        const analyser = ctx.createAnalyser();
        analyser.fftSize = 512;
        analyser.smoothingTimeConstant = 0.75; // نرمی؛ بدون این، هاله تیک‌تیکی می‌شود
        source.connect(analyser);

        const buf = new Uint8Array(analyser.frequencyBinCount);
        let smoothed = 0;

        const tick = () => {
          analyser.getByteFrequencyData(buf);

          // RMS به‌جای میانگین ساده — به بلندی ادراکی نزدیک‌تر است
          let sum = 0;
          for (let i = 0; i < buf.length; i++) sum += buf[i] * buf[i];
          const rms = Math.sqrt(sum / buf.length) / 255;

          // کف نویز را می‌بُریم تا هاله موقع سکوت نلرزد
          const gated = rms < 0.04 ? 0 : Math.min(1, (rms - 0.04) * 3.2);

          // بالا رفتن سریع، پایین آمدن آرام — حس طبیعی‌تری از صدا می‌دهد
          smoothed = gated > smoothed ? gated : smoothed * 0.86 + gated * 0.14;

          ref.current?.style.setProperty("--level", smoothed.toFixed(3));
          raf = requestAnimationFrame(tick);
        };
        tick();
      } catch {
        // دسترسی میکروفون رد شد یا مرورگر پشتیبانی نمی‌کند —
        // بازی باید بدون صدا هم کار کند، پس فقط بی‌صدا رد می‌شویم
        ref.current?.style.setProperty("--level", "0");
      }
    })();

    return () => {
      cancelled = true;
      cancelAnimationFrame(raf);
      stream?.getTracks().forEach((t) => t.stop());
      void ctx?.close();
    };
  }, [active]);

  return ref;
}
