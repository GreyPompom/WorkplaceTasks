import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { TaskService } from '../../../services/task.service';
import { AuthService } from '../../../services/auth.service';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Task } from '../../../models/task/task.model';

@Component({
  selector: 'app-task-form',
  standalone: true,
  templateUrl: './task-form.component.html',
  styleUrls: ['./task-form.component.css'],
  imports: [CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatSnackBarModule
  ],
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
    public router: Router, //verificar se pode ser private
    private route: ActivatedRoute,
    private snackBar: MatSnackBar
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
    this.loading = true; //enquanto verifica a tarefa

    this.taskService.getById(this.taskId).subscribe({
      next: (task) => {
        this.form.patchValue(task);
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        this.snackBar.open('Erro ao carregar tarefa.', 'Fechar', { duration: 3000 });
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
        this.snackBar.open(
          this.isEditMode ? 'Tarefa atualizada com sucesso!' : 'Tarefa criada com sucesso!',
          'Fechar',
          { duration: 3000 }
        );
        this.router.navigate(['/tasks']);
      },
      error: (err) => {
        this.loading = false;
        const message = err?.error?.message || 'Erro ao salvar tarefa.';
        this.snackBar.open(message, 'Fechar', { duration: 3000 });
      },
    });
  }

  
  cancel(): void {
    this.router.navigate(['/tasks']);
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
