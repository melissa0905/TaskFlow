import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5042',
        changeOrigin: true,
      },
      '/hubs': {
        target: 'http://localhost:5042',
        changeOrigin: true,
        ws: true,
      },
      '/ai': {
        target: 'http://127.0.0.1:58147',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/ai/, ''),
      },
    },
  },
  build: {
    target: 'es2023',
  },
})
