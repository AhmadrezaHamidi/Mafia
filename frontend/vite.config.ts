import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

// روی سرور زیر مسیر /Mafia سرو می‌شود، در dev از ریشه.
// با VITE_BASE=/Mafia/ موقع build کنترلش می‌کنیم.
const base = process.env.VITE_BASE ?? '/'

export default defineConfig({
  base,
  plugins: [react(), tailwindcss()],
  server: {
    port: 5180,
    proxy: {
      // بک‌اند Kestrel روی 5057 بالا می‌آید (launchSettings پروفایل http)
      '/api': { target: 'http://localhost:5057', changeOrigin: true },
    },
  },
})
