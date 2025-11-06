import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbar } from '@angular/material/toolbar';
import { TaskService } from '../../../services/task.service';
import { AuthService } from '../../../services/auth.service';
import { Task } from '../../../models/task/task.model';
import { RoleDirective } from '../../../directives/role.directive';
import { MatFormField, MatLabel } from "@angular/material/form-field";
import { MatSelect, MatOption } from "@angular/material/select";


@Component({
  selector: 'app-task-list',
  standalone: true,
  templateUrl: './task-list.component.html',
  styleUrls: ['./task-list.component.css'],
  imports: [CommonModule,
    MatCardModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatIconModule, MatToolbar,
    RoleDirective,
    MatPaginatorModule, MatFormField, MatLabel, MatSelect, MatOption]
})
export class TaskListComponent implements OnInit {
  tasks: Task[] = [];
  loading = true;
  userRole: 'Admin' | 'Manager' | 'Member' | null = null;
  error: string | null = null;
  userId: string | null = null;
  selectedStatus: string = '';
  totalCount = 0;
  pageSize = 5;
  pageNumber = 1;

  constructor(private taskService: TaskService, private authService: AuthService, private router: Router) {}

  ngOnInit(): void {
    this.userRole = this.authService.getUserRole();
    this.userId = this.authService.getUserId();
    console.log('User Role:', this.userRole);
    console.log('User ID:', this.userId);
     this.loadTasks();
  }

 loadTasks(): void {
    this.loading = true;
    this.taskService.getPaged(this.selectedStatus, this.pageNumber, this.pageSize).subscribe({
      next: (res) => {
        this.tasks = res.items;
        this.totalCount = res.totalCount;
        this.loading = false;
      },
      error: (err) => {
        this.error = err?.error?.message || 'Erro ao carregar tarefas.';
        this.loading = false;
      }
    });
  }

  onPageChange(event: PageEvent): void {
    this.pageNumber = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadTasks();
  }

  onFilterChange(): void {
    this.pageNumber = 1;
    this.loadTasks();
  }

  canEdit(task: Task): boolean {
    return this.userRole === 'Admin' || this.userRole === 'Manager' || task.createdById === this.userId;
  }

  canDelete(task: Task): boolean {
    return this.userRole === 'Admin' || task.createdById === this.userId;
  }

  goToEdit(id: string) {
    this.router.navigate(['/tasks/edit', id]);
  }

  goToCreate() {
    this.router.navigate(['/tasks/create']);
  }

  deleteTask(id: string) {
    if (confirm('Tem certeza que deseja excluir esta tarefa?')) {
      this.taskService.delete(id).subscribe(() => {
        this.tasks = this.tasks.filter((t) => t.id !== id);
      });
    }
  }
  //Traduz status
  getStatusLabel(status: string): string {
    switch (status) {
      case 'Pending': return 'Pendente';
      case 'InProgress': return 'Em andamento';
      case 'Done': return 'Concluída';
      default: return status;
    }
  }
  //define class do card com base no status
   getStatusClass(status: string): string {
    switch (status) {
      case 'Pending': return 'status-pending';
      case 'InProgress': return 'status-progress';
      case 'Done': return 'status-done';
      default: return '';
    }
  }
}
