import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { DashboardComponent } from './components/dashboard/dashboard.component';
import { PatientsComponent } from './components/patients/patients.component';
import { AppointmentsComponent } from './components/appointments/appointments.component';
import { ServicesComponent } from './components/services/services.component';
import { StaffComponent } from './components/staff/staff.component';
import { FinancialComponent } from './components/financial/financial.component';

const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'patients', component: PatientsComponent },
  { path: 'appointments', component: AppointmentsComponent },
  { path: 'services', component: ServicesComponent },
  { path: 'staff', component: StaffComponent },
  { path: 'financial', component: FinancialComponent },
  { path: '**', redirectTo: '/dashboard' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }