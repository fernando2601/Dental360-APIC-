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
  { path: 'dashboard', component: DashboardComponent },
  { path: 'clients', component: ClientsComponent },
  { path: 'patients', component: PatientsComponent },
  { path: 'appointments', component: AppointmentsComponent },
  { path: 'agenda', component: AgendaComponent },
  { path: 'services', component: ServicesComponent },
  { path: 'packages', component: PackagesComponent },
  { path: 'staff', component: StaffComponent },
  { path: 'financial', component: FinancialComponent },
  { path: 'inventory', component: InventoryComponent },
  { path: 'before-after', component: BeforeAfterComponent },
  { path: 'learning', component: LearningComponent },
  { path: 'clinic-info', component: ClinicInfoComponent },
  { path: 'subscriptions', component: SubscriptionsComponent },
  { path: 'analytics', component: AnalyticsComponent },
  { path: 'whatsapp', component: WhatsAppComponent },
  { path: '**', redirectTo: '/dashboard' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }