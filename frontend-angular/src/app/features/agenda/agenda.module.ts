import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

// Components will be created next
// import { AgendaComponent } from './components/agenda.component';
// import { CalendarViewComponent } from './components/calendar-view.component';
// import { AppointmentFormComponent } from './components/appointment-form.component';

const routes: Routes = [
  { path: '', redirectTo: '/agenda/calendar', pathMatch: 'full' },
  // { path: 'calendar', component: AgendaComponent },
  // { path: 'new', component: AppointmentFormComponent },
  // { path: 'edit/:id', component: AppointmentFormComponent }
];

@NgModule({
  declarations: [
    // AgendaComponent,
    // CalendarViewComponent,
    // AppointmentFormComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes)
  ]
})
export class AgendaModule { }