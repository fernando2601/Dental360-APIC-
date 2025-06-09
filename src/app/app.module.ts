import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

// Components
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { ClientsComponent } from './components/clients/clients.component';
import { PatientsComponent } from './components/patients/patients.component';
import { AppointmentsComponent } from './components/appointments/appointments.component';
import { AgendaComponent } from './components/agenda/agenda.component';
import { ServicesComponent } from './components/services/services.component';
import { PackagesComponent } from './components/packages/packages.component';
import { StaffComponent } from './components/staff/staff.component';
import { FinancialComponent } from './components/financial/financial.component';
import { SubscriptionsComponent } from './components/subscriptions/subscriptions.component';
import { InventoryComponent } from './components/inventory/inventory.component';
import { BeforeAfterComponent } from './components/before-after/before-after.component';
import { LearningComponent } from './components/learning/learning.component';
import { ClinicInfoComponent } from './components/clinic-info/clinic-info.component';
import { AnalyticsComponent } from './components/analytics/analytics.component';
import { WhatsappComponent } from './components/whatsapp/whatsapp.component';
import { LoginComponent } from './components/auth/login.component';

// Services
import { ApiService } from './services/api.service';
import { AuthService } from './services/auth.service';
import { ClientService } from './services/client.service';
import { AppointmentService } from './services/appointment.service';
import { ServiceService } from './services/service.service';
import { StaffService } from './services/staff.service';
import { FinancialService } from './services/financial.service';
import { InventoryService } from './services/inventory.service';

// Guards
import { AuthGuard } from './guards/auth.guard';

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    ClientsComponent,
    PatientsComponent,
    AppointmentsComponent,
    AgendaComponent,
    ServicesComponent,
    PackagesComponent,
    StaffComponent,
    FinancialComponent,
    SubscriptionsComponent,
    InventoryComponent,
    BeforeAfterComponent,
    LearningComponent,
    ClinicInfoComponent,
    AnalyticsComponent,
    WhatsappComponent,
    LoginComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule
  ],
  providers: [
    ApiService,
    AuthService,
    ClientService,
    AppointmentService,
    ServiceService,
    StaffService,
    FinancialService,
    InventoryService,
    AuthGuard
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }