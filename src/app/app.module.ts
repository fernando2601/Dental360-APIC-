import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

// Components
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { PatientsComponent } from './components/patients/patients.component';
import { AppointmentsComponent } from './components/appointments/appointments.component';
import { ServicesComponent } from './components/services/services.component';
import { StaffComponent } from './components/staff/staff.component';
import { FinancialComponent } from './components/financial/financial.component';

// Services
import { ApiService } from './services/api.service';

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    PatientsComponent,
    AppointmentsComponent,
    ServicesComponent,
    StaffComponent,
    FinancialComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [
    ApiService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }