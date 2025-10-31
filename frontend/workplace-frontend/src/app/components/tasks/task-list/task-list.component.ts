import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbar } from '@angular/material/toolbar';
import { TaskService } from '../../../services/task.service';
import { AuthService } from '../../../services/auth.service';
import { Task } from '../../../models/task/task.model';
import { RoleDirective } from '../../../directives/role.directive';


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
    RoleDirective]
})
export class TaskListComponent implements OnInit {
  tasks: Task[] = [];
  loading = true;
  userRole: 'Admin' | 'Manager' | 'Member' | null = null;
  userId: string | null = null;

  constructor(private taskService: TaskService, private auth: AuthService, private router: Router) {}

  ngOnInit(): void {
    this.userRole = this.auth.getUserRole();
    this.userId = this.auth.getUserId();
    console.log('User Role:', this.userRole);
    console.log('User ID:', this.userId);
    this.taskService.getAll().subscribe({
      next: (data) => {
        this.tasks = data;
        this.loading = false;
      },
      error: () => (this.loading = false)
    });
  }

  // loadTasks(): void {
  //   this.taskService.getAll().subscribe({
  //     next: (tasks) => {
  //       this.tasks = tasks;
  //       this.loading = false;
  //     },
  //     error: () => (this.loading = false)
  //   });
  // }

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
    this.router.navigate(['/tasks/new']);
  }

  deleteTask(id: string) {
    if (confirm('Tem certeza que deseja excluir esta tarefa?')) {
      this.taskService.delete(id).subscribe(() => {
        this.tasks = this.tasks.filter((t) => t.id !== id);
      });
    }
  }
}
