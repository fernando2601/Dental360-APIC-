import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppointmentsComponent } from './components/appointments/appointments.component';

const routes: Routes = [
  { path: '', redirectTo: '/appointments', pathMatch: 'full' },
  { path: 'appointments', component: AppointmentsComponent },
  { path: 'reports', component: AppointmentsComponent },
  { path: 'dashboard', component: AppointmentsComponent },
  { path: 'clients', component: AppointmentsComponent },
  { path: 'services', component: AppointmentsComponent },
  { path: 'staff', component: AppointmentsComponent },
  { path: 'inventory', component: AppointmentsComponent },
  { path: 'financial', component: AppointmentsComponent },
  { path: '**', redirectTo: '/appointments' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }