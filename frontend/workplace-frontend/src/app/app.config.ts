import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptorsFromDi  } from '@angular/common/http';
import { jwtInterceptor } from './interceptors/jwt.interceptor'; // caso tenha criado
import { provideAnimations } from '@angular/platform-browser/animations';
import { HTTP_INTERCEPTORS } from '@angular/common/http';

export const appConfig: ApplicationConfig = {
   providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()), // habilita interceptors via DI
    provideAnimations(),
    { provide: HTTP_INTERCEPTORS, useClass: jwtInterceptor, multi: true } // <-- registra o interceptor
  ]
};
