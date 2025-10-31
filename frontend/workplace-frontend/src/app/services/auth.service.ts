import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from '../environments/environment';
import { AuthResponse } from '../models/auth/auth-response.model';
import { LoginRequestDto } from '../dtos/auth/login-request.dto';
import { User } from '../models/auth/user.mode';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `${environment.apiUrl}/auth`;
  private jwtHelper = new JwtHelperService();
  private currentUserSubject = new BehaviorSubject<AuthResponse | null>(this.getStoredUser());
  currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {}

  login(credentials: LoginRequestDto): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/token`, credentials).pipe(
      tap((response) => {
        localStorage.setItem('token', response.token);
        localStorage.setItem('user', JSON.stringify(response.user));
        this.currentUserSubject.next(response);
      })
    );
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  isAuthenticated(): boolean {
    const token = localStorage.getItem('token');
    return token != null && !this.jwtHelper.isTokenExpired(token);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  getUser(): User | null {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  }

  getUserRole(): 'Admin' | 'Manager' | 'Member' | null {
    return this.getUser()?.role ?? null;
  }

  getUserId(): string | null {
    return this.getUser()?.id ?? null;
  }

  private getStoredUser(): AuthResponse | null {
    const token = localStorage.getItem('token');
    const user = localStorage.getItem('user');
    if (!token || !user) return null;
    return { token, user: JSON.parse(user) };
  }

  hasRole(roles: string[]): boolean {
    const userRole = this.getUserRole();
    return userRole ? roles.includes(userRole) : false;
  } 

  
}
