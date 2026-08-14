# Backlog v1 — برای وارد کردن به Jira

*اولویت‌بندی PO با روش MoSCoW — این لیست دقیقاً همون چیزیه که بعد از بالا اومدن Jira به‌صورت Epic/Task ثبت می‌شه.*

## Epic 1 — Room & Lobby (Must)
- [ ] ساخت روم + تولید RoomCode
- [ ] ورود مهمان با nickname (Session/Guest)
- [ ] ورود/خروج به روم با RoomCode
- [ ] لیست بازیکنان Lobby (real-time)
- [ ] انتقال مالکیت Host هنگام قطعی اتصال Host

## Epic 2 — Role Assignment Engine (Must)
- [ ] مدل داده‌ی Role قابل‌ترکیب (composable roles، نه هاردکد سناریو)
- [ ] پیاده‌سازی نقش‌های v1: شهروند ساده، مافیای ساده
- [ ] تخصیص تصادفی نقش با رعایت نسبت مافیا:شهر
- [ ] فیلتر state خروجی بر اساس نقش caller (بازیکن نباید نقش بقیه رو ببینه)

## Epic 3 — Night Phase (Must)
- [ ] فاز شب با تایمر (۴۵ ثانیه پیش‌فرض)
- [ ] ثبت/تغییر اکشن مافیا تا پایان فاز (idempotent)
- [ ] قفل خودکار اکشن‌ها بعد از timeout
- [ ] غیرفعال‌سازی کشتن شب از راند دوم به بعد (طبق قانون سناریو)

## Epic 4 — Day Phase & Voting (Must)
- [ ] فاز بحث با تایمر
- [ ] رأی‌گیری علنی + broadcast لحظه‌ای هر رأی
- [ ] محاسبه‌ی اکثریت + دور دوم در صورت تساوی
- [ ] حذف بازیکن رأی‌آورده

## Epic 5 — Win Condition & Game End (Must)
- [ ] چک شرط برد بعد از هر حذف (شب یا روز)
- [ ] افشای نقش‌ها فقط بعد از پایان بازی
- [ ] صفحه‌ی نتیجه + آمار بازی
- [ ] Rematch با همون اعضا

## Epic 6 — Realtime Sync (Must)
- [ ] WebSocket gateway (`/ws/game/{sessionId}`)
- [ ] رویدادهای phase.changed / player.died / vote.cast / game.ended
- [ ] Reconnect resync کامل state

## Epic 7 — Resilience (Should)
- [ ] مدیریت قطعی اتصال بدون حذف خودکار بازیکن
- [ ] Resync کامل بعد از reconnect
- [ ] جلوگیری از race condition بین REST action و broadcast

## Epic 8 — Roadmap v2 (Won't — این نسخه)
- [ ] افزودن نقش‌های کارآگاه/دکتر/پدرخوانده (سناریوی «شب‌های مافیا»)
- [ ] پشتیبانی از چند سناریوی هم‌زمان (config-driven roles)
- [ ] پنل ادمین/گاد برای override دستی فاز

## Epic 9 — Lobby ظرفیت‌محور (Must)
- [ ] فیلد `Capacity` روی Room (Host موقع ساخت انتخاب می‌کنه)
- [ ] غیرفعال بودن Start تا `Members.Count == Capacity`
- [ ] وضعیت `WaitingForPlayers` → `ReadyToStart`

## Epic 10 — چت متنی کانال‌بندی‌شده (Must)
- [ ] `ChatThread` per نوع (Lobby/DayPublic/NightMafia/DeadChat)
- [ ] اعمال قانون دسترسی یکسان با VoiceChannel (سند ۰۷)
- [ ] انتقال خودکار بازیکن حذف‌شده به DeadChat

## Epic 11 — چت صوتی (Must)
- [ ] انتخاب SFU آماده (LiveKit/mediasoup) به‌جای WebRTC از صفر
- [ ] `VoiceChannel` aggregate + Command های Join/ToggleMute
- [ ] Force-switch خودکار کانال بعد از حذف یا تغییر فاز (سرورمحور، نه client)
- [ ] Push-to-talk (فاز بعدی این Epic)

## Epic 12 — کنترل‌های میزبان و Spectator (Should)
- [ ] Kick بازیکن از Lobby یا حین بازی (Host)
- [ ] Mute اجباری یک بازیکن (Host)
- [ ] روم خصوصی با رمز
- [ ] Spectator UI (لیست زنده‌ها + رأی‌های زنده برای بازیکنان حذف‌شده)

---

**نکته‌ی PO:** Epic های 1 تا 6 و 9 تا 11 = MVP قابل‌بازی با چت/صدا. Epic 7 و 12 قبل از عرضه‌ی عمومی لازمن ولی می‌تونن موازی با تست MVP پیش برن. Epic 8 عمداً خارج از v1 نگه داشته شده تا rule-engine چندسناریویی درست طراحی بشه، نه عجولانه. جزئیات تحقیق: سند‌های ۰۷ و ۰۸.
