import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  template: `
    <main class="app-container">
      <router-outlet></router-outlet>
    </main>
  `,
  styles: [`
    .app-container {
      padding: 1rem;
      font-family: 'Segoe UI', sans-serif;
    }
  `]
})
export class AppComponent {}
//Componente raiz que utiliza RouterOutlet para renderizar componentes baseados na rota atual.