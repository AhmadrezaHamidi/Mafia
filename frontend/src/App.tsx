import { BrowserRouter, Routes, Route } from "react-router-dom";
import { Landing } from "./screens/Landing";
import { Room } from "./screens/Room";

// روی سرور زیر /Mafia سرو می‌شود؛ basename از همان base ویت گرفته می‌شود
// تا مسیرها در dev (ریشه) و production (زیرمسیر) هر دو درست کار کنند.
const basename = import.meta.env.BASE_URL.replace(/\/$/, "");

function App() {
  return (
    <BrowserRouter basename={basename || undefined}>
      <Routes>
        <Route path="/" element={<Landing />} />
        <Route path="/room/:code" element={<Room />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
