import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const token = authService.getToken();

  // 1️⃣ Nenhum token → volta pro login
  if (!token) {
    router.navigate(['/login']);
    return false;
  }

  // 2️⃣ Token expirado → logout + volta pro login
  if (!authService.isAuthenticated()) {
    authService.logout();
    router.navigate(['/login']);
    return false;
  }

  return true;
};

//Verifica se o usuário tem um token válido (AuthService.isAuthenticated()).
//Caso não tenha, redireciona automaticamente para /login.