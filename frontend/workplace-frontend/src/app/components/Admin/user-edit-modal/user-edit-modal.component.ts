import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, FormGroup, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { UserUpdateDto } from '../../../dtos/user/user-update.dto';
import { UserResponseDto } from '../../../dtos/user/user-response.dto';

@Component({
  selector: 'app-user-edit-modal',
  standalone: true,
  templateUrl: './user-edit-modal.component.html',
  styleUrls: ['./user-edit-modal.component.css'],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule
  ]
})
export class UserEditModalComponent {
  form!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<UserEditModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { user: UserResponseDto }
  ) {

  }

  ngOnInit(): void {
    this.form = this.fb.group({
      name: [this.data.user.name, Validators.required],
      role: [this.data.user.role, Validators.required]
    });
  }
  save(): void {
    if (this.form.invalid) return;
    const dto: UserUpdateDto = this.form.value as UserUpdateDto;
    this.dialogRef.close(dto);
  }

  cancel(): void {
    this.dialogRef.close(null);
  }
}
