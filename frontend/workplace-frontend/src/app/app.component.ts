import { Component } from '@angular/core';
import { RouterOutlet, Router, NavigationEnd } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from './components/navbar/navbar.component';
import { AuthService } from './services/auth.service';
import { Observable } from 'rxjs';
import { filter, map, startWith } from 'rxjs/operators';
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, NavbarComponent],
  templateUrl: './app.component.html',
  styles: [`
    .app-container {
      padding: 1rem;
      font-family: 'Segoe UI', sans-serif;
    }
  `]
})
export class AppComponent {
  isAuthenticated$: Observable<boolean>;
  currentUrl: string = '';
  
  constructor(private authService: AuthService, private router: Router) {
    // Monitora o status do usuário
    this.isAuthenticated$ = this.authService.currentUser$.pipe(
      map(user => !!user)
    );
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      map((event: any) => event.urlAfterRedirects),
      startWith(this.router.url)
    ).subscribe(url => this.currentUrl = url);
  }

  get showNavbar(): boolean {
    return this.currentUrl !== '/login' && !!this.authService.getUser();
  }
   ngOnInit(): void {
    if (!this.authService.isAuthenticated()) {
      this.authService.logout();
    }
  }
}
//Componente raiz que utiliza RouterOutlet para renderizar componentes baseados na rota atual.