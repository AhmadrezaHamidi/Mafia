// گالری نقش‌ها — محتوای static، مستقل از بک‌اند.
// پنج سناریو داریم: مافیای روسی، شب‌های مافیا، انتخابات شهر، محافظ سایه، شکار روانی.

import type { Scenario } from "../types";

export interface RoleInfo {
  key: string;
  name: string;
  team: "شهر" | "مافیا" | "مستقل";
  icon: string;
  scenario: Scenario;
  summary: string;
  nightAbility: string;
  winCondition: string;
  /** رنگ تِم کارت — کلید متغیرهای CSS موجود در theme */
  accent: "blood" | "town" | "lamp" | "neutral";
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
  {
    key: "SimpleMafia-mayor",
    name: "مافیای ساده",
    team: "مافیا",
    icon: "🔪",
    scenario: "MayorElection",
    summary: "عضو گروه مافیاست. باید مراقب باشه شهردار رو زودتر از بقیه شناسایی و حذف کنه چون رأیش دو نفره.",
    nightAbility: "شب اول: انتخاب یک قربانی به‌صورت گروهی؛ فقط رئیس مافیا تصمیم نهایی رو ثبت می‌کنه.",
    winCondition: "وقتی تعداد مافیای زنده با تعداد شهروند زنده برابر یا بیشتر بشه.",
    accent: "blood",
  },
  {
    key: "Mayor",
    name: "شهردار",
    team: "شهر",
    icon: "👑",
    scenario: "MayorElection",
    summary: "یه شهروند عادیه با یه امتیاز بزرگ — روزها رأیش دو نفر حساب می‌شه، پس نظرش وزن بیشتری داره.",
    nightAbility: "قابلیت شب نداره؛ فقط روزها رأیش دو برابره.",
    winCondition: "وقتی همه‌ی مافیاها حذف بشن.",
    accent: "lamp",
  },
  {
    key: "SimpleMafia-guard",
    name: "مافیای ساده",
    team: "مافیا",
    icon: "🔪",
    scenario: "ShadowGuard",
    summary: "عضو گروه مافیاست. اینجا کشتن فقط شب اول نیست — هر شب می‌تونه دست به کار بشه، پس بادیگارد باید هوشیار بمونه.",
    nightAbility: "هر شب: انتخاب یک قربانی به‌صورت گروهی؛ فقط رئیس مافیا تصمیم نهایی رو ثبت می‌کنه.",
    winCondition: "وقتی تعداد مافیای زنده با تعداد شهروند زنده برابر یا بیشتر بشه.",
    accent: "blood",
  },
  {
    key: "Bodyguard",
    name: "بادیگارد",
    team: "شهر",
    icon: "🛡️",
    scenario: "ShadowGuard",
    summary: "هر شب از یک نفر (حتی خودش) محافظت می‌کنه. اگه مافیا دقیقاً همون نفر رو هدف بگیره، به‌جای هدف، خودِ بادیگارد کشته می‌شه.",
    nightAbility: "هر شب: انتخاب یک نفر برای محافظت. محافظت با نجاتِ دکتر فرق داره — اینجا یکی واقعاً جونش رو فدا می‌کنه.",
    winCondition: "وقتی همه‌ی مافیاها حذف بشن.",
    accent: "town",
  },
  {
    key: "SimpleMafia-hunt",
    name: "مافیای ساده",
    team: "مافیا",
    icon: "🔪",
    scenario: "SerialHunt",
    summary: "عضو گروه مافیاست. تنها نیستن که شب‌ها شکار می‌کنن — یه قاتل زنجیره‌ای مستقل هم همون شب فعاله.",
    nightAbility: "هر شب: انتخاب یک قربانی به‌صورت گروهی؛ فقط رئیس مافیا تصمیم نهایی رو ثبت می‌کنه.",
    winCondition: "وقتی تعداد مافیای زنده با تعداد شهروند زنده برابر یا بیشتر بشه (و قاتل زنجیره‌ای حذف شده باشه).",
    accent: "blood",
  },
  {
    key: "SerialKiller",
    name: "قاتل زنجیره‌ای",
    team: "مستقل",
    icon: "🔪🎭",
    scenario: "SerialHunt",
    summary: "نه عضو مافیاست نه شهروند — کاملاً تنهاست. هر شب مستقل از بقیه یک نفر رو می‌کشه، بدون نیاز به تأیید کسی.",
    nightAbility: "هر شب: انتخاب یک قربانی، کاملاً مستقل از مافیا و بدون نیاز به رئیس.",
    winCondition: "وقتی تنها بازمانده‌ی زنده‌ی بازی باشه (همه‌ی مافیا و شهروندها حذف شده باشن).",
    accent: "neutral",
  },
];

export const rolesByScenario = (scenario: Scenario) => roles.filter((r) => r.scenario === scenario);

export const scenarios: { key: Scenario; name: string; description: string }[] = [
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
  {
    key: "MayorElection",
    name: "انتخابات شهر",
    description: "یه شهروند «شهردار» می‌شه که رأیش روز دو برابر حساب می‌شه — پیداکردنش برای مافیا حیاتیه.",
  },
  {
    key: "ShadowGuard",
    name: "محافظ سایه",
    description: "یه بادیگارد هر شب از یک نفر محافظت می‌کنه؛ اگه اشتباه حدس بزنه، به‌جای هدف خودش می‌میره.",
  },
  {
    key: "SerialHunt",
    name: "شکار روانی",
    description: "علاوه بر مافیا، یه قاتل زنجیره‌ای مستقل هم شب‌ها شکار می‌کنه — هدفش تنها موندنه، نه بردن تیمی.",
  },
];

/** توضیح مکانیک رئیس مافیا — روی گالری جدا نمایش داده می‌شه چون یه نقش مجزا نیست، یه نقشِ درونِ تیمه. */
export const mafiaLeaderNote =
  "وقتی بیش از یک نفر از تیم مافیا زنده باشه، یکی «رئیس» می‌شه (در «شب‌های مافیا» همیشه پدرخوانده، وگرنه تصادفی). همه‌ی مافیا توی چت خصوصی شب می‌تونن نظر بدن، ولی فقط رئیس می‌تونه قربانی نهایی رو ثبت کنه. اگه رئیس حذف بشه، رئیس بعدی خودکار تعیین می‌شه. قاتل زنجیره‌ای (سناریوی «شکار روانی») از این قانون مستثناست — چون اصلاً عضو تیم مافیا نیست.";

const scenarioSuffix: Partial<Record<Scenario, string>> = {
  MafiaNights: "-nights",
  MayorElection: "-mayor",
  ShadowGuard: "-guard",
  SerialHunt: "-hunt",
};

export function roleByKey(key: string | null | undefined, scenario?: string): RoleInfo | undefined {
  if (!key) return undefined;
  // «مافیای ساده» توی چند سناریو حضور داره و هرکدوم توضیح مخصوص خودش رو داره —
  // بدون scenario نمی‌شه فهمید کدوم توضیح درسته، پس کلیدش رو صریح چک می‌کنیم.
  if (key === "SimpleMafia" && scenario) {
    const suffix = scenarioSuffix[scenario as Scenario];
    if (suffix) {
      const variant = roles.find((r) => r.key === `SimpleMafia${suffix}`);
      if (variant) return variant;
    }
  }
  return roles.find((r) => r.key === key);
}
