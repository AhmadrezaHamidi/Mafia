import { BrowserRouter, Routes, Route, Navigate, useLocation } from "react-router-dom";
import { Landing } from "./screens/Landing";
import { Room } from "./screens/Room";
import { Login } from "./screens/Login";
import { AppBackground } from "./components/AppBackground";

// روی سرور زیر /Mafia سرو می‌شود؛ basename از همان base ویت گرفته می‌شود
// تا مسیرها در dev (ریشه) و production (زیرمسیر) هر دو درست کار کنند.
const basename = import.meta.env.BASE_URL.replace(/\/$/, "");

/** پس‌زمینه یک‌بار برای کل اپ رندر می‌شود، ولی شدتش به صفحه بستگی دارد:
 *  ورود محتوای کمی دارد پس تصویر پررنگ‌تر است؛ داخل بازی محو می‌شود تا
 *  با اطلاعات بازی رقابت نکند. */
function Background() {
  const { pathname } = useLocation();
  return <AppBackground intensity={pathname === "/" || pathname === "/login" ? "full" : "dim"} />;
}

function App() {
  return (
    <BrowserRouter basename={basename || undefined}>
      <Background />
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/" element={<Landing />} />
        <Route path="/room/:code" element={<Room />} />
        {/* هر مسیر ناشناخته به خانه — قبلاً صفحه‌ی کاملاً سفید می‌داد */}
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
