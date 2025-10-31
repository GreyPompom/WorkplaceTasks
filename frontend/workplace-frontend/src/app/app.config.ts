import { ApplicationConfig,importProvidersFrom  } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptorsFromDi  } from '@angular/common/http';
import { jwtInterceptor } from './interceptors/jwt.interceptor'; // caso tenha criado
import { provideAnimations } from '@angular/platform-browser/animations';
import { HTTP_INTERCEPTORS } from '@angular/common/http';


// material imports
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

export const appConfig: ApplicationConfig = {
   providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()), // habilita interceptors via DI
    provideAnimations(),
    { provide: HTTP_INTERCEPTORS, useClass: jwtInterceptor, multi: true }, // <-- registra o interceptor
    importProvidersFrom(
      MatButtonModule,
      MatInputModule,
      MatFormFieldModule,
      MatCardModule,
      MatSnackBarModule,
      MatToolbarModule,
      MatIconModule,
      MatProgressSpinnerModule
    )
  ]
};
