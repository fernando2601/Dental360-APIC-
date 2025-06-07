import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

// Components
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { ClientsComponent } from './components/clients/clients.component';
import { AppointmentsComponent } from './components/appointments/appointments.component';
import { ServicesComponent } from './components/services/services.component';
import { StaffComponent } from './components/staff/staff.component';
import { FinancialComponent } from './components/financial/financial.component';
import { InventoryComponent } from './components/inventory/inventory.component';
import { LoginComponent } from './components/auth/login.component';
import { PackagesComponent } from './components/packages/packages.component';
import { BeforeAfterComponent } from './components/before-after/before-after.component';
import { LearningComponent } from './components/learning/learning.component';
import { ClinicInfoComponent } from './components/clinic-info/clinic-info.component';
import { SubscriptionsComponent } from './components/subscriptions/subscriptions.component';
import { AnalyticsComponent } from './components/analytics/analytics.component';
import { AgendaComponent } from './components/agenda/agenda.component';
import { PatientsComponent } from './components/patients/patients.component';
import { WhatsAppComponent } from './components/whatsapp/whatsapp.component';

// Services
import { ApiService } from './services/api.service';
import { AuthService } from './services/auth.service';
import { ClientService } from './services/client.service';
import { AppointmentService } from './services/appointment.service';
import { ServiceService } from './services/service.service';
import { StaffService } from './services/staff.service';
import { FinancialService } from './services/financial.service';
import { InventoryService } from './services/inventory.service';

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    ClientsComponent,
    AppointmentsComponent,
    ServicesComponent,
    StaffComponent,
    FinancialComponent,
    InventoryComponent,
    LoginComponent,
    PackagesComponent,
    BeforeAfterComponent,
    LearningComponent,
    ClinicInfoComponent,
    SubscriptionsComponent,
    AnalyticsComponent,
    AgendaComponent,
    PatientsComponent,
    WhatsAppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule
  ],
  providers: [
    ApiService,
    AuthService,
    ClientService,
    AppointmentService,
    ServiceService,
    StaffService,
    FinancialService,
    InventoryService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }