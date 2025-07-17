import { defineConfig } from 'vite';

export default defineConfig({
  ssr: {
    // Cấu hình cho SSR
    noExternal: ['@angular/**'],
  },
  optimizeDeps: {
    exclude: ['@angular/platform-server']
  }
});
