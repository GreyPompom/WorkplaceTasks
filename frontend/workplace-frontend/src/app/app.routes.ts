import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { TaskListComponent } from './components/tasks/task-list/task-list.component';
import { TaskFormComponent } from './components/tasks/task-form/task-form.component';
import { authGuard  } from './guards/auth.guard';
import { roleGuard } from './guards/role.guard';
import { UserListComponent } from './components/Admin/user-list/user-list.component';

export const routes: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: 'login', component: LoginComponent },

    // autenticados e liberado para todas roles
    {
        path: 'tasks',
        canActivate: [authGuard, roleGuard(['Admin', 'Manager', 'Member'])] ,
        children: [
        { path: '', component: TaskListComponent },
        { path: 'create', component: TaskFormComponent },
        { path: ':id/edit', component: TaskFormComponent }
        ]
    },
    // apenas para Admin
     {
    path: 'admin',
    canActivate: [authGuard, authGuard],
    children: [
      { path: 'users', component: UserListComponent }
    ]
  },
    { path: '**', redirectTo: 'login' }
];
