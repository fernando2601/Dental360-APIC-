import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

// Components will be created next
// import { PatientsListComponent } from './components/patients-list.component';
// import { PatientDetailsComponent } from './components/patient-details.component';
// import { PatientFormComponent } from './components/patient-form.component';

const routes: Routes = [
  { path: '', redirectTo: '/patients/list', pathMatch: 'full' },
  // { path: 'list', component: PatientsListComponent },
  // { path: 'new', component: PatientFormComponent },
  // { path: ':id', component: PatientDetailsComponent },
  // { path: 'edit/:id', component: PatientFormComponent }
];

@NgModule({
  declarations: [
    // PatientsListComponent,
    // PatientDetailsComponent,
    // PatientFormComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule.forChild(routes)
  ]
})
export class PatientsModule { }