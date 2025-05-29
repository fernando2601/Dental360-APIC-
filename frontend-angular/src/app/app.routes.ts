import { Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/login',
    pathMatch: 'full'
  },
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./pages/dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'agenda',
    loadComponent: () => import('./pages/agenda/agenda.component').then(m => m.AgendaComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'pacientes',
    loadComponent: () => import('./pages/patients/patients.component').then(m => m.PatientsComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'inventario',
    loadComponent: () => import('./pages/inventory/inventory.component').then(m => m.InventoryComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'financeiro',
    loadComponent: () => import('./pages/finance/finance.component').then(m => m.FinanceComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'servicos',
    loadComponent: () => import('./pages/services/services.component').then(m => m.ServicesComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'equipe',
    loadComponent: () => import('./pages/staff/staff.component').then(m => m.StaffComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'aprendizado',
    loadComponent: () => import('./pages/learning/learning.component').then(m => m.LearningComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'pacotes',
    loadComponent: () => import('./pages/packages/packages.component').then(m => m.PackagesComponent),
    canActivate: [AuthGuard]
  },
  {
    path: '**',
    redirectTo: '/dashboard'
  }
];