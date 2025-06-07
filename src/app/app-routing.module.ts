import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LoginComponent } from './components/auth/login.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { ClientsComponent } from './components/clients/clients.component';
import { AppointmentsComponent } from './components/appointments/appointments.component';
import { ServicesComponent } from './components/services/services.component';
import { StaffComponent } from './components/staff/staff.component';
import { FinancialComponent } from './components/financial/financial.component';
import { InventoryComponent } from './components/inventory/inventory.component';
import { PackagesComponent } from './components/packages/packages.component';
import { BeforeAfterComponent } from './components/before-after/before-after.component';
import { LearningComponent } from './components/learning/learning.component';
import { ClinicInfoComponent } from './components/clinic-info/clinic-info.component';
import { SubscriptionsComponent } from './components/subscriptions/subscriptions.component';
import { AnalyticsComponent } from './components/analytics/analytics.component';
import { AgendaComponent } from './components/agenda/agenda.component';
import { PatientsComponent } from './components/patients/patients.component';
import { WhatsAppComponent } from './components/whatsapp/whatsapp.component';

import { AuthGuard } from './guards/auth.guard';

const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
  { path: 'clients', component: ClientsComponent, canActivate: [AuthGuard] },
  { path: 'patients', component: PatientsComponent, canActivate: [AuthGuard] },
  { path: 'appointments', component: AppointmentsComponent, canActivate: [AuthGuard] },
  { path: 'agenda', component: AgendaComponent, canActivate: [AuthGuard] },
  { path: 'services', component: ServicesComponent, canActivate: [AuthGuard] },
  { path: 'packages', component: PackagesComponent, canActivate: [AuthGuard] },
  { path: 'staff', component: StaffComponent, canActivate: [AuthGuard] },
  { path: 'financial', component: FinancialComponent, canActivate: [AuthGuard] },
  { path: 'inventory', component: InventoryComponent, canActivate: [AuthGuard] },
  { path: 'before-after', component: BeforeAfterComponent, canActivate: [AuthGuard] },
  { path: 'learning', component: LearningComponent, canActivate: [AuthGuard] },
  { path: 'clinic-info', component: ClinicInfoComponent, canActivate: [AuthGuard] },
  { path: 'subscriptions', component: SubscriptionsComponent, canActivate: [AuthGuard] },
  { path: 'analytics', component: AnalyticsComponent, canActivate: [AuthGuard] },
  { path: 'whatsapp', component: WhatsAppComponent, canActivate: [AuthGuard] },
  { path: '**', redirectTo: '/dashboard' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }