import { ApplicationConfig } from '@angular/core';
import { provideRouter, withRouterConfig, withViewTransitions } from '@angular/router';
import { routes } from './app.routes';
import { provideClientHydration, withHttpTransferCacheOptions } from '@angular/platform-browser';
import { provideHttpClient, withInterceptors, withFetch } from '@angular/common/http';
import { AuthInterceptor } from './interceptors/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(
      routes,
      withViewTransitions(),
      withRouterConfig({
        onSameUrlNavigation: 'reload'
      })
    ),
    provideClientHydration(
      withHttpTransferCacheOptions({
        includeHeaders: ['X-Total-Count', 'Content-Type']
      })
    ),
    provideHttpClient(
      withFetch(),
      withInterceptors([AuthInterceptor])
    )
  ]
};
