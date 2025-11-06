import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleGuard = (allowedRoles: string[]): CanActivateFn => {
  return () => {
    const authService = inject(AuthService);
    const router = inject(Router);

    const token = authService.getToken();
    const userRole = authService.getUserRole();
    if (!token || !authService.isAuthenticated()) {
      authService.logout();
      router.navigate(['/login']);
      return false;
    }

    if (userRole && allowedRoles.includes(userRole)) {
      return true;
    }

    router.navigate(['/tasks']);
    return false;
  };
};
