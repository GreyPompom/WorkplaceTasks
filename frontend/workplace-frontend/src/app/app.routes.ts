import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { TaskListComponent } from './components/tasks/task-list/task-list.component';
import { TaskFormComponent } from './components/tasks/task-form/task-form.component';
import { authGuard  } from './guards/auth.guard';
import { roleGuard } from './guards/role.guard';

export const routes: Routes = [
{ path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },

  // Área protegida
  { path: 'tasks', component: TaskListComponent, canActivate: [authGuard] },
  { path: 'tasks/new', component: TaskFormComponent, canActivate: [authGuard, roleGuard(['Admin', 'Manager', 'Member'])] },
  { path: 'tasks/edit/:id', component: TaskFormComponent, canActivate: [authGuard, roleGuard(['Admin', 'Manager', 'Member'])] },

  { path: '**', redirectTo: 'login' }
];
