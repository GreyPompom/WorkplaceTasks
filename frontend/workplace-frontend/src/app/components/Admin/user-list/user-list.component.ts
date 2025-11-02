import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators, FormGroup } from '@angular/forms';
import { UserService } from '../../../services/user.service';
import { UserResponseDto } from '../../../dtos/user/user-response.dto';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { UserEditModalComponent } from '../user-edit-modal/user-edit-modal.component';
import { MatCard } from "@angular/material/card";
import { UserCreateDialogComponent } from '../user-create-dialog/user-create-dialog.component';

@Component({
  selector: 'app-user-list',
  standalone: true,
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css'],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatPaginatorModule,
    MatTableModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatCard
  ]
})
export class UserListComponent implements OnInit {
  users: UserResponseDto[] = [];
  totalCount = 0;
  page = 1;
  pageSize = 5;
  loading = false;

  filterForm!: FormGroup;
  createForm!: FormGroup;

  displayedColumns = ['name', 'email', 'role', 'actions'];

  constructor(
    private userService: UserService,
    private fb: FormBuilder,
    private dialog: MatDialog,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit(): void {
    this.filterForm = this.fb.group({
      search: [''],
      role: ['']
    });

    this.createForm = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      role: ['Member', Validators.required]
    });

    this.loadUsers();
  }

  /** Mostra um toast bonito */
  showMessage(message: string, isError = false): void {
    this.snackBar.open(message, 'Fechar', {
      duration: 3000,
      horizontalPosition: 'right',
      verticalPosition: 'bottom',
      panelClass: isError ? ['snack-error'] : ['snack-success']
    });
  }

  loadUsers(): void {
    const { role, search } = this.filterForm.value;
    this.loading = true;
    this.userService.getPaged(this.page, this.pageSize, role!, search!).subscribe({
      next: (res) => {
        this.users = res.data;
        this.totalCount = res.totalCount;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.showMessage('Erro ao carregar usuários.', true);
      }
    });
  }

  onPageChange(e: PageEvent): void {
    this.page = e.pageIndex + 1;
    this.pageSize = e.pageSize;
    this.loadUsers();
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(UserCreateDialogComponent, {
      width: '400px'
    });

    dialogRef.afterClosed().subscribe((created) => {
      if (created) {
        this.showMessage('Usuário criado com sucesso!');
        this.loadUsers();
      }
    });
  }

  openEditDialog(user: UserResponseDto): void {
    if (user.role === 'Admin') {
      this.showMessage('Não é permitido editar outro administrador.', true);
      return;
    }

    const dialogRef = this.dialog.open(UserEditModalComponent, {
      width: '400px',
      data: { user }
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.loading = true;
        this.userService.update(user.id, result).subscribe({
          next: () => {
            this.loading = false;
            this.showMessage('Usuário atualizado com sucesso!');
            this.loadUsers();
          },
          error: () => {
            this.loading = false;
            this.showMessage('Erro ao atualizar usuário.', true);
          }
        });
      }
    });
  }

  deleteUser(id: string, role: string): void {
    if (role === 'Admin') {
      this.showMessage('Não é permitido excluir um administrador.', true);
      return;
    }
    if (confirm('Tem certeza que deseja excluir este usuário?')) {
      this.loading = true;
      this.userService.delete(id).subscribe({
        next: () => {
          this.loading = false;
          this.showMessage('Usuário excluído com sucesso!');
          this.loadUsers();
        },
        error: () => {
          this.loading = false;
          this.showMessage('Erro ao excluir usuário.', true);
        }
      });
    }
  }
}
