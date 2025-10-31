import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    return true;
  }

  // Se não estiver autenticado, redireciona para o login
  router.navigate(['/login']);
  return false;
};

//Verifica se o usuário tem um token válido (AuthService.isAuthenticated()).
//Caso não tenha, redireciona automaticamente para /login.