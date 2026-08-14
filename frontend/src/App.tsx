import { BrowserRouter, Routes, Route } from "react-router-dom";
import { Landing } from "./screens/Landing";
import { Room } from "./screens/Room";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Landing />} />
        <Route path="/room/:code" element={<Room />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
