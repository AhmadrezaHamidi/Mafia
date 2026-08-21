// گالری نقش‌ها — محتوای static، مستقل از بک‌اند.
// دو سناریو داریم: «مافیای روسی» (v1، دو نقش ساده) و «شب‌های مافیا» (v2، پنج نقش).

export interface RoleInfo {
  key: string;
  name: string;
  team: "شهر" | "مافیا";
  icon: string;
  scenario: "RussianMafia" | "MafiaNights";
  summary: string;
  nightAbility: string;
  winCondition: string;
  /** رنگ تِم کارت — کلید متغیرهای CSS موجود در theme */
  accent: "blood" | "town" | "lamp";
}

export const roles: RoleInfo[] = [
  {
    key: "SimpleCitizen",
    name: "شهروند ساده",
    team: "شهر",
    icon: "👤",
    scenario: "RussianMafia",
    summary: "یک اهالی عادی شهر که هیچ قابلیت ویژه‌ای نداره — فقط با بحث و رأی‌گیری روز می‌تونه مافیا رو پیدا کنه.",
    nightAbility: "قابلیتی نداره؛ شب‌ها فقط منتظر صبح می‌مونه.",
    winCondition: "وقتی همه‌ی مافیاها حذف بشن.",
    accent: "town",
  },
  {
    key: "SimpleMafia",
    name: "مافیای ساده",
    team: "مافیا",
    icon: "🔪",
    scenario: "RussianMafia",
    summary: "عضو گروه مافیاست و هویتش از بقیه پنهانه. با بقیه‌ی مافیا یک قربانی انتخاب می‌کنه.",
    nightAbility: "شب اول: انتخاب یک قربانی (به‌صورت گروهی در چت خصوصی مافیا؛ فقط «رئیس مافیا» تصمیم نهایی رو ثبت می‌کنه). از شب دوم به بعد کشتنی در کار نیست.",
    winCondition: "وقتی تعداد مافیای زنده با تعداد شهروند زنده برابر یا بیشتر بشه.",
    accent: "blood",
  },
  {
    key: "GodFather",
    name: "پدرخوانده",
    team: "مافیا",
    icon: "🎩",
    scenario: "MafiaNights",
    summary: "رئیسِ ثابتِ تیم مافیاست — همیشه اون تصمیم نهاییِ کشتن رو می‌گیره، حتی اگه بقیه‌ی مافیا هم زنده باشن.",
    nightAbility: "هر شب: انتخاب نهاییِ قربانی. جلوی کارآگاه هم «بی‌گناه» دیده می‌شه، پس شناسایی‌اش سخت‌تره.",
    winCondition: "وقتی تعداد تیم مافیا با تعداد شهروند زنده برابر یا بیشتر بشه.",
    accent: "blood",
  },
  {
    key: "SimpleMafia-nights",
    name: "مافیای ساده",
    team: "مافیا",
    icon: "🔪",
    scenario: "MafiaNights",
    summary: "عضو تیم مافیاست، ولی تصمیم نهایی با پدرخوانده‌ست. توی چت خصوصی شب نظر می‌ده.",
    nightAbility: "هر شب می‌تونه توی چت مافیا نظر بده، ولی فقط پدرخوانده قربانی رو نهایی می‌کنه.",
    winCondition: "وقتی تعداد تیم مافیا با تعداد شهروند زنده برابر یا بیشتر بشه.",
    accent: "blood",
  },
  {
    key: "Doctor",
    name: "دکتر",
    team: "شهر",
    icon: "💉",
    scenario: "MafiaNights",
    summary: "هر شب می‌تونه یک نفر رو (حتی خودش رو) از کشته‌شدن نجات بده.",
    nightAbility: "هر شب: انتخاب یک نفر برای نجات. اگه همون کسی باشه که مافیا هدف گرفته، اون شب کسی نمی‌میره.",
    winCondition: "وقتی همه‌ی تیم مافیا حذف بشن.",
    accent: "town",
  },
  {
    key: "Detective",
    name: "کارآگاه",
    team: "شهر",
    icon: "🔍",
    scenario: "MafiaNights",
    summary: "هر شب می‌تونه هویت واقعی یک نفر رو استعلام بگیره — نتیجه فقط برای خودش نمایش داده می‌شه.",
    nightAbility: "هر شب: استعلام یک نفر. نتیجه «مافیاست» یا «بی‌گناهه» رو صبح می‌بینه (پدرخوانده همیشه بی‌گناه نشون داده می‌شه).",
    winCondition: "وقتی همه‌ی تیم مافیا حذف بشن.",
    accent: "lamp",
  },
];

export const rolesByScenario = (scenario: "RussianMafia" | "MafiaNights") =>
  roles.filter((r) => r.scenario === scenario);

export const scenarios: { key: "RussianMafia" | "MafiaNights"; name: string; description: string }[] = [
  {
    key: "RussianMafia",
    name: "مافیای روسی",
    description: "سناریوی کلاسیک و ساده — فقط مافیا و شهروند، کشتن فقط شب اول.",
  },
  {
    key: "MafiaNights",
    name: "شب‌های مافیا",
    description: "سناریوی کامل‌تر با پدرخوانده، دکتر و کارآگاه — هر شب یه اتفاق تازه.",
  },
];

/** توضیح مکانیک رئیس مافیا — روی گالری جدا نمایش داده می‌شه چون یه نقش مجزا نیست، یه نقشِ درونِ تیمه. */
export const mafiaLeaderNote =
  "وقتی بیش از یک نفر از تیم مافیا زنده باشه، یکی «رئیس» می‌شه (در «شب‌های مافیا» همیشه پدرخوانده، وگرنه تصادفی). همه‌ی مافیا توی چت خصوصی شب می‌تونن نظر بدن، ولی فقط رئیس می‌تونه قربانی نهایی رو ثبت کنه. اگه رئیس حذف بشه، رئیس بعدی خودکار تعیین می‌شه.";

export function roleByKey(key: string | null | undefined, scenario?: string): RoleInfo | undefined {
  if (!key) return undefined;
  // «مافیای ساده» چون هم توی روسی هم توی شب‌های مافیا وجود داره، دو رکورد جدا داره —
  // بدون scenario نمی‌شه فهمید کدوم توضیح درسته، پس کلیدش رو صریح چک می‌کنیم.
  if (scenario === "MafiaNights" && key === "SimpleMafia") {
    return roles.find((r) => r.key === "SimpleMafia-nights");
  }
  return roles.find((r) => r.key === key);
}
