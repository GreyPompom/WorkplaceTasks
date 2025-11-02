import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';

localStorage.removeItem('token');
localStorage.removeItem('user');

bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));
