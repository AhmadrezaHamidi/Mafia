import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

export default defineConfig({
  plugins: [react(), tailwindcss()],
  server: {
    port: 5180,
    proxy: {
      // بک‌اند Kestrel روی 5057 بالا می‌آید (launchSettings پروفایل http)
      '/api': { target: 'http://localhost:5057', changeOrigin: true },
    },
  },
})
