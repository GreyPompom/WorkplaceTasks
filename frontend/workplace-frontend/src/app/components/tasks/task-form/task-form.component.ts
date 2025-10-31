import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TaskService } from '../../../services/task.service';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-task-form',
  standalone: true,
  templateUrl: './task-form.component.html',
  styleUrls: ['./task-form.component.css'],
  imports: [CommonModule, ReactiveFormsModule]
})
export class TaskFormComponent implements OnInit {
  form!: FormGroup;
  isEditMode = false;
  taskId: string | null = null;
  loading = false;
  error = '';

  constructor(
    private fb: FormBuilder,
    private taskService: TaskService,
    private authService: AuthService,
    public  router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(250)]],
      description: ['', [Validators.maxLength(2000)]],
      status: ['Pending', Validators.required]
    });

    this.taskId = this.route.snapshot.paramMap.get('id');
    this.isEditMode = !!this.taskId;

    if (this.isEditMode) {
      this.loadTask();
    }
  }

  loadTask(): void {
    if (!this.taskId) return;
    this.loading = true;

    this.taskService.getById(this.taskId).subscribe({
      next: (task) => {
        this.form.patchValue(task);
        this.loading = false;
      },
      error: (err) => {
        this.error = err?.error?.message || 'Erro ao carregar tarefa.';
        this.loading = false;
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.loading = true;
    const taskData = this.form.value;

    const request$ = this.isEditMode
      ? this.taskService.update(this.taskId!, taskData)
      : this.taskService.create(taskData);

    request$.subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/tasks']);
      },
      error: (err) => {
        this.error = err?.error?.message || 'Erro ao salvar tarefa.';
        this.loading = false;
      }
    });
  }

  get title() {
    return this.form.get('title');
  }

  get description() {
    return this.form.get('description');
  }

  get status() {
    return this.form.get('status');
  }
}
