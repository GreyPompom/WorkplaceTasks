import { Injectable } from '@angular/core';
import {
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

@Injectable()
export class jwtInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService, private router: Router) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.authService.getToken(); //pega o token no local storage

    // ve se tem token e coloca no header
    let authToken = req;
    if (token) {
      authToken = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }

   //envia e trata erros
    return next.handle(authToken).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) { //joga pro login se voltar um 401
          this.authService.logout();
          this.router.navigate(['/login']);
        }

        //sem permissao para a ação
        if (error.status === 403) {
          alert('Você não tem permissão para essa ação.');
        }

        return throwError(() => error);
      })
    );
  }
}
