import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TaskService } from '../../../services/task.service';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-task-list',
  standalone: true,
  templateUrl: './task-list.component.html',
  styleUrls: ['./task-list.component.css'],
  imports: [CommonModule]
})
export class TaskListComponent implements OnInit {
  tasks: any[] = [];
  loading = true;
  userRole = '';
  userId = '';

  constructor(
    private taskService: TaskService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.userRole = this.authService.getUserRole() ?? '';
    this.userId = this.authService.getUserId() ?? '';
    this.loadTasks();
  }

  loadTasks(): void {
    this.taskService.getAll().subscribe({
      next: (tasks) => {
        this.tasks = tasks;
        this.loading = false;
      },
      error: () => (this.loading = false)
    });
  }

  canEdit(task: any): boolean {
    return this.userRole === 'Admin' || (this.userRole === 'Member' && task.createdById === this.userId);
  }

  canDelete(task: any): boolean {
    return this.userRole === 'Admin' || task.createdById === this.userId;
  }

  deleteTask(id: string): void {
    this.taskService.delete(id).subscribe(() => this.loadTasks());
  }
}
