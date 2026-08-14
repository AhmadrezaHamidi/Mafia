# API Endpoints — بازی مافیا (v1)

*نوشته‌ی لیلا (BA)* — REST برای عملیات‌های مدیریتی، WebSocket برای state بازی real-time.
Base URL: `/api/v1` · Auth: `Bearer <token>` (توکن مهمان — نیازی به OTP کامل نیست، فقط nickname + session)

---

## 🔑 Session — ورود مهمان

| Method | Route | Auth | توضیح |
|---|---|---|---|
| `POST` | `/Session/Guest` | ❌ | ساخت session مهمان با nickname → دریافت token |

---

## 🚪 Room — روم و Lobby

| Method | Route | Auth | نقش مجاز | توضیح |
|---|---|---|---|---|
| `POST` | `/Room` | ✅ | هرکس | ساخت روم جدید → RoomCode + خودش Host می‌شود |
| `POST` | `/Room/{code}/Join` | ✅ | هرکس | ورود به روم با RoomCode |
| `POST` | `/Room/{code}/Leave` | ✅ | عضو روم | خروج از Lobby (قبل از شروع بازی) |
| `GET` | `/Room/{code}` | ✅ | عضو روم | اطلاعات روم + لیست بازیکنان Lobby |
| `PUT` | `/Room/{code}/Start` | ✅ Host | Host | شروع بازی → تخصیص نقش + ساخت GameSession |
| `DELETE` | `/Room/{code}` | ✅ Host | Host | بستن روم (فقط قبل از شروع) |

---

## 🎮 Game — وضعیت و اکشن‌های بازی

| Method | Route | Auth | نقش مجاز | توضیح |
|---|---|---|---|---|
| `GET` | `/Game/{sessionId}/State` | ✅ | عضو بازی | وضعیت فعلی **فیلترشده برای همان بازیکن** (نقش خودش، لیست زنده‌ها، فاز، تایمر) |
| `POST` | `/Game/{sessionId}/Night/Action` | ✅ مافیا | فقط نقش‌های دارای قابلیت شب | ثبت/تغییر هدف اکشن شب — idempotent تا پایان فاز |
| `POST` | `/Game/{sessionId}/Day/Vote` | ✅ | بازیکن زنده | ثبت رأی روز (علنی) |
| `DELETE` | `/Game/{sessionId}/Day/Vote` | ✅ | بازیکن زنده | پس‌گرفتن رأی قبل از پایان فاز |
| `GET` | `/Game/{sessionId}/Result` | ✅ | عضو بازی | نتیجه‌ی نهایی + افشای همه‌ی نقش‌ها (فقط بعد از پایان بازی) |
| `POST` | `/Game/{sessionId}/Rematch` | ✅ Host | Host | ساخت GameSession جدید با همون اعضا |

---

## 📡 WebSocket — همگام‌سازی زنده

اتصال: `wss://.../ws/game/{sessionId}?token=...`

**رویدادهای Server → Client:**

| Event | Payload | چه‌کسانی دریافت می‌کنن |
|---|---|---|
| `phase.changed` | `{ phase, round, deadlineUtc }` | همه |
| `player.died` | `{ playerId, cause }` | همه (بدون افشای نقش) |
| `vote.cast` | `{ voterId, targetId }` | همه (رأی روز علنیه) |
| `night.action.ack` | `{ accepted: true }` | فقط بازیکنی که اکشن زده |
| `player.connection.changed` | `{ playerId, state }` | همه |
| `game.ended` | `{ winningTeam, reveal: [{playerId, role}] }` | همه |

**رویدادهای Client → Server:** فقط از طریق REST بالا (WebSocket صرفاً read/broadcast است، نه ورودی — برای جلوگیری از race condition در ثبت اکشن‌ها).

---

## قوانین دسترسی مهم (طبق تحلیل Domain Rules)

1. `GET /Game/{sessionId}/State` هیچ‌وقت نقش بقیه‌ی بازیکنان رو برنمی‌گردونه — فقط نقش خود caller.
2. `POST /Night/Action` برای غیرمافیا همیشه `403` می‌ده (در v1 فقط مافیا قابلیت شب داره).
3. `Result` قبل از پایان بازی → `409 Conflict` (جلوگیری از لو رفتن نقش‌ها زودتر از موقع).
