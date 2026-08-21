import { useGameStore } from "../store/gameStore";
import { Table } from "../components/Table";
import { RoleCard } from "../components/RoleCard";
import { MicToggle } from "../components/MicToggle";
import { ChatPanel } from "../components/ChatPanel";
import { StatusBar } from "../components/StatusBar";

export function Night() {
  const players = useGameStore((s) => s.players);
  const round = useGameStore((s) => s.round);
  const scenario = useGameStore((s) => s.scenario);
  const timeLeftSec = useGameStore((s) => s.timeLeftSec);
  const nightTarget = useGameStore((s) => s.nightTarget);
  const nightSaveTarget = useGameStore((s) => s.nightSaveTarget);
  const nightInvestigateTarget = useGameStore((s) => s.nightInvestigateTarget);
  const lastInvestigation = useGameStore((s) => s.lastInvestigation);
  const submitNightAction = useGameStore((s) => s.submitNightAction);
  const lastDeath = useGameStore((s) => s.lastDeath);
  const me = useGameStore((s) => s.me());

  // تیم مافیا هم مافیای ساده رو شامل می‌شه هم پدرخوانده (سناریوی شب‌های مافیا).
  const isMafiaTeam = me?.role === "SimpleMafia" || me?.role === "GodFather";
  // اگه بیش از یک نفر از تیم مافیا زنده باشه، فقط رئیس می‌تونه اکشن رو ثبت کنه —
  // سرور isMafiaLeader رو فقط برای خودِ بازیکنِ تیمِ مافیا پر می‌کنه (سند ۰۶: قانون فیلتر خروجی).
  // پدرخوانده همیشه رئیسه، پس این فلگ براش خودکار true میاد.
  const isMafiaLeader = me?.isMafiaLeader === true;
  const canDecideKill = isMafiaTeam && isMafiaLeader;

  const isDoctor = me?.role === "Doctor";
  const isDetective = me?.role === "Detective";

  const deadPlayer = lastDeath ? players.find((p) => p.id === lastDeath.playerId) : null;
  const minutes = String(Math.floor(timeLeftSec / 60)).padStart(2, "0");
  const seconds = String(timeLeftSec % 60).padStart(2, "0");

  // فقط یکی از این سه حالت هر لحظه فعاله — بر اساس نقش خودم.
  const tableProps = canDecideKill
    ? {
        selectable: !!me?.alive,
        selectedId: nightTarget,
        onSelect: (id: string) => submitNightAction(id, "Kill"),
      }
    : isDoctor
      ? {
          selectable: !!me?.alive,
          selectedId: nightSaveTarget,
          onSelect: (id: string) => submitNightAction(id, "Save"),
          allowSelf: true,
        }
      : isDetective
        ? {
            selectable: !!me?.alive,
            selectedId: nightInvestigateTarget,
            onSelect: (id: string) => submitNightAction(id, "Investigate"),
          }
        : { selectable: false };

  const investigatedPlayer = lastInvestigation
    ? players.find((p) => p.id === lastInvestigation.targetId)
    : null;

  function statusText() {
    if (canDecideKill) return "قربانی امشب رو انتخاب کن";
    if (isMafiaTeam) return "منتظر تصمیم رئیس مافیا — نظرت رو توی چت بگو";
    if (isDoctor) return "یک نفر رو برای نجات امشب انتخاب کن (می‌تونی خودت رو هم انتخاب کنی)";
    if (isDetective) return "یک نفر رو برای استعلام هویت انتخاب کن";
    return "مافیا دارن قربانی امشب رو انتخاب می‌کنن";
  }

  function roleHint() {
    if (canDecideKill) {
      return me?.role === "GodFather"
        ? "نقش تو پدرخوانده‌ست — همیشه رئیس ثابت مافیایی. روی یکی از هم‌بازی‌ها بزن تا هدف امشب رو انتخاب کنی."
        : "نقش تو مافیای ساده‌ست و رئیسی — روی یکی از هم‌بازی‌ها بزن تا هدف امشب رو انتخاب کنی.";
    }
    if (isMafiaTeam) return "نقش تو مافیای ساده‌ست، ولی تصمیم نهایی با رئیسه — توی چت خصوصی نظرت رو بگو.";
    if (isDoctor) return "نقش تو دکتره — هر شب یک نفر رو (حتی خودت رو) می‌تونی نجات بدی.";
    if (isDetective) return "نقش تو کارآگاهه — هر شب هویت واقعی یک نفر رو استعلام می‌گیری.";
    return "نقش تو شهروند ساده‌ست. فقط منتظر بمون تا صبح بشه.";
  }

  return (
    <div className="mx-auto flex min-h-screen max-w-md flex-col p-6 pb-10 screen-enter">
      <StatusBar />
      <div className="mb-1 text-center">
        <p
          className="mb-1 font-mono text-[0.68rem] uppercase tracking-[0.12em]"
          style={{ color: "var(--blood-bright)" }}
        >
          راند {round} · شب
        </p>
        <h2 className="text-2xl font-extrabold">شهر خوابیده…</h2>
        <p className="mt-1 text-sm" style={{ color: "var(--parchment-dim)" }}>
          {statusText()}
        </p>
      </div>

      {deadPlayer && (
        <div
          className="mb-3 rounded-lg border px-3 py-2 text-center text-sm"
          style={{
            background: "rgba(156,43,50,0.14)",
            borderColor: "rgba(156,43,50,0.4)",
            color: "var(--blood-bright)",
          }}
        >
          {lastDeath?.cause === "night" ? "دیشب" : "امروز با رأی شهر"} «{deadPlayer.name}» حذف شد
        </div>
      )}

      {isDetective && lastInvestigation && (
        <div
          className="mb-3 rounded-lg border px-3 py-2 text-center text-sm"
          style={{
            background: "rgba(212,165,74,0.12)",
            borderColor: "rgba(212,165,74,0.4)",
            color: "var(--lamp)",
          }}
        >
          🔍 نتیجه‌ی آخرین استعلام: «{investigatedPlayer?.name ?? "؟"}»{" "}
          {lastInvestigation.isMafia ? "عضو مافیاست" : "بی‌گناهه"}
        </div>
      )}

      <Table
        players={players}
        center={
          <>
            <div className="font-mono text-3xl tabular-nums" style={{ color: "var(--lamp)" }}>
              {minutes}:{seconds}
            </div>
            <div className="text-sm" style={{ color: "var(--parchment-dim)" }}>
              تا پایان فاز شب
            </div>
          </>
        }
        {...tableProps}
      />

      {me?.alive && me.role && (
        <RoleCard role={me.role} hint={roleHint()} scenario={scenario} />
      )}
      {(canDecideKill || isDoctor || isDetective) && (
        <p className="mt-2 mb-2 text-center text-xs" style={{ color: "var(--muted)" }}>
          تا وقتی تایمر تموم نشه می‌تونی نظرت رو عوض کنی
        </p>
      )}

      <div className="mt-3 flex flex-col gap-3">
        <MicToggle />
        <ChatPanel />
      </div>
    </div>
  );
}
