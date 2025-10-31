import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';
import { Task } from '../models/task/task.model';
import { TaskCreateDto } from '../dtos/task-create.dto';
import { TaskUpdateDto } from '../dtos/task-update.dto';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private apiUrl = `${environment.apiUrl}/tasks`;

  constructor(private http: HttpClient) {}

 
  getAll(): Observable<Task[]> {
    return this.http.get<Task[]>(this.apiUrl);
  }

getById(id: string): Observable<Task> {
    return this.http.get<Task>(`${this.apiUrl}/${id}`);
  }

  create(task: TaskCreateDto): Observable<Task> {
    return this.http.post<Task>(this.apiUrl, task);
  }

  update(id: string, task: TaskUpdateDto): Observable<Task> {

    return this.http.put<Task>(`${this.apiUrl}/${id}`, task);
  }



  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
