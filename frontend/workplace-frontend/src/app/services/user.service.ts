import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { UserCreateDto } from '../dtos/user/user-create.dto';
import { UserUpdateDto } from '../dtos/user/user-update.dto';
import { UserResponseDto } from '../dtos/user/user-response.dto';

@Injectable({ providedIn: 'root' })
export class UserService {
  private apiUrl = `${environment.apiUrl}/users`;

  constructor(private http: HttpClient) {}

  getPaged(page: number, pageSize: number, role?: string, search?: string):
    Observable<{ data: UserResponseDto[]; totalCount: number }> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (role) params = params.set('role', role);
    if (search) params = params.set('search', search);
    return this.http.get<{ data: UserResponseDto[]; totalCount: number }>(`${this.apiUrl}/paged`, { params });
  }

  create(dto: UserCreateDto): Observable<UserResponseDto> {
    return this.http.post<UserResponseDto>(this.apiUrl, dto);
  }

  update(id: string, dto: UserUpdateDto): Observable<UserResponseDto> {
    return this.http.put<UserResponseDto>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
