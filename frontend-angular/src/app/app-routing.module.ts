import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard, AdminGuard, ManagerGuard } from './core/guards/auth.guard';

const routes: Routes = [
  // Redirect root to dashboard
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },

  // Auth routes (no guard required)
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.module').then(m => m.AuthModule)
  },

  // Protected routes
  {
    path: 'dashboard',
    loadChildren: () => import('./features/dashboard/dashboard.module').then(m => m.DashboardModule),
    canActivate: [AuthGuard]
  },

  {
    path: 'agenda',
    loadChildren: () => import('./features/agenda/agenda.module').then(m => m.AgendaModule),
    canActivate: [AuthGuard]
  },

  {
    path: 'patients',
    loadChildren: () => import('./features/patients/patients.module').then(m => m.PatientsModule),
    canActivate: [AuthGuard]
  },

  {
    path: 'inventory',
    loadChildren: () => import('./features/inventory/inventory.module').then(m => m.InventoryModule),
    canActivate: [AuthGuard]
  },

  {
    path: 'finance',
    loadChildren: () => import('./features/finance/finance.module').then(m => m.FinanceModule),
    canActivate: [AuthGuard],
    data: { roles: ['admin', 'manager'] }
  },

  // Admin only routes
  {
    path: 'admin',
    loadChildren: () => import('./features/admin/admin.module').then(m => m.AdminModule),
    canActivate: [AdminGuard]
  },

  // Error pages
  { path: 'access-denied', loadChildren: () => import('./shared/components/access-denied/access-denied.module').then(m => m.AccessDeniedModule) },
  { path: 'not-found', loadChildren: () => import('./shared/components/not-found/not-found.module').then(m => m.NotFoundModule) },

  // Wildcard route - must be last
  { path: '**', redirectTo: '/not-found' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {
    enableTracing: false, // Set to true for debugging
    scrollPositionRestoration: 'top',
    anchorScrolling: 'enabled',
    onSameUrlNavigation: 'reload'
  })],
  exports: [RouterModule]
})
export class AppRoutingModule { }