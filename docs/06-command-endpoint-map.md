# نگاشت Endpoint ↔ Command/Query ↔ Aggregate

*نوشته‌ی لیلا (BA)* — طبق الگوی Ahmad.OnlineShop: یک کلاس Handler ادغام‌شده به‌ازای هر Aggregate (Command + Query در همون کلاس).

## جدول کامل

| Endpoint | CQRS | Aggregate | Event(های) صادرشده |
|---|---|---|---|
| `POST /Session/Guest` | `CreateGuestSessionCommand` | PlayerSession | `GuestSessionCreated` |
| `POST /Room` | `CreateRoomCommand` | Room | `RoomCreated` |
| `POST /Room/{code}/Join` | `JoinRoomCommand` | Room | `PlayerJoinedRoom` |
| `POST /Room/{code}/Leave` | `LeaveRoomCommand` | Room | `PlayerLeftRoom`, `HostTransferred?` |
| `GET /Room/{code}` | `GetRoomQuery` | Room | — |
| `PUT /Room/{code}/Start` | `StartGameCommand` | Room → GameSession | `RoomGameStarted`, `RolesAssigned`, `NightPhaseStarted` |
| `DELETE /Room/{code}` | `CloseRoomCommand` | Room | `RoomClosed` |
| `GET /Game/{id}/State` | `GetGameStateQuery` | GameSession | — (خروجی فیلترشده به نقش caller) |
| `POST /Game/{id}/Night/Action` | `SubmitNightActionCommand` | GameSession | `NightActionSubmitted` |
| `POST /Game/{id}/Day/Vote` | `CastVoteCommand` | GameSession | `VoteCast` |
| `DELETE /Game/{id}/Day/Vote` | `RetractVoteCommand` | GameSession | `VoteRetracted` |
| `GET /Game/{id}/Result` | `GetGameResultQuery` | GameSession | — |
| `POST /Game/{id}/Rematch` | `RequestRematchCommand` | GameSession | `RematchStarted` |

## Command های داخلی (بدون Endpoint — توسط `PhaseTimerHostedService`)

| Command داخلی | Trigger | Event(های) صادرشده |
|---|---|---|
| `ResolveNightPhaseCommand` | پایان `PhaseDeadlineUtc` فاز شب | `NightPhaseResolved`, `PlayerEliminated?`, `DayPhaseStarted` |
| `ResolveVotingCommand` | پایان تایمر روز **یا** همه‌ی زنده‌ها رأی دادن | `VotingResolved`, `PlayerEliminated?`, `GameEnded?`/`NightPhaseStarted` |
| `CheckWinConditionCommand` | بعد از هر `PlayerEliminated` | `GameEnded?` |

> این سه‌تا تنها Command هایی هستن که از WebSocket یا REST نمیان — یک `IHostedService` هر ثانیه چک می‌کنه کدوم GameSession به `PhaseDeadlineUtc` رسیده و Command مربوطه رو صدا می‌زنه. این جداسازی مهمه چون منطق «پایان فاز» نباید وابسته به این باشه که یه بازیکن request بفرسته.

## قانون فیلتر خروجی (مهم‌ترین قانون امنیتی دامنه)

`GetGameStateQuery` هیچ‌وقت مستقیم Entity رو serialize نمی‌کنه؛ یک `GameStateForPlayer` DTO می‌سازه که:
- `Role` فقط برای `callerPlayerId == player.PlayerId` پر می‌شه، برای بقیه `null`
- لیست `AliveOpponents` فقط شامل PlayerId + Nickname است، نه Role
- در `GameEnded`، همه‌چیز باز می‌شه (از `RevealedRoles` استفاده می‌کنه، نه از state زنده)
