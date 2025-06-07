import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../services/api.service';

declare var bootstrap: any;

interface Patient {
  id?: number;
  name: string;
  email?: string;
  phone?: string;
  birth_date?: string;
  cpf?: string;
  address?: string;
  emergency_contact?: string;
  medical_history?: string;
  allergies?: string;
  created_at?: string;
  updated_at?: string;
}

@Component({
  selector: 'app-patients',
  templateUrl: './patients.component.html'
})
export class PatientsComponent implements OnInit {
  patients: Patient[] = [];
  filteredPatients: Patient[] = [];
  appointments: any[] = [];
  searchTerm = '';
  loading = true;
  saving = false;
  isEditing = false;
  
  patientForm: FormGroup;
  patientModal: any;

  constructor(
    private apiService: ApiService,
    private fb: FormBuilder
  ) {
    this.patientForm = this.fb.group({
      name: ['', Validators.required],
      email: [''],
      phone: [''],
      birth_date: [''],
      cpf: [''],
      address: [''],
      emergency_contact: [''],
      medical_history: [''],
      allergies: ['']
    });
  }

  ngOnInit() {
    this.loadPatients();
    this.loadAppointments();
  }

  loadPatients() {
    this.loading = true;
    this.apiService.get<Patient[]>('/patients').subscribe({
      next: (patients) => {
        this.patients = patients;
        this.filteredPatients = patients;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading patients:', error);
        this.loading = false;
      }
    });
  }

  loadAppointments() {
    this.apiService.get<any[]>('/appointments').subscribe({
      next: (appointments) => {
        this.appointments = appointments;
      },
      error: (error) => {
        console.error('Error loading appointments:', error);
      }
    });
  }

  filterPatients() {
    if (!this.searchTerm) {
      this.filteredPatients = this.patients;
      return;
    }

    const term = this.searchTerm.toLowerCase();
    this.filteredPatients = this.patients.filter(patient => 
      patient.name.toLowerCase().includes(term) ||
      patient.email?.toLowerCase().includes(term) ||
      patient.cpf?.includes(term) ||
      patient.phone?.includes(term)
    );
  }

  openAddPatientModal() {
    this.isEditing = false;
    this.patientForm.reset();
    this.openModal();
  }

  editPatient(patient: Patient) {
    this.isEditing = true;
    this.patientForm.patchValue(patient);
    this.openModal();
  }

  viewPatient(patient: Patient) {
    console.log('Viewing patient:', patient);
  }

  scheduleAppointment(patient: Patient) {
    console.log('Scheduling appointment for:', patient);
  }

  savePatient() {
    if (!this.patientForm.valid) return;

    this.saving = true;
    const patientData = this.patientForm.value;

    const request = this.isEditing 
      ? this.apiService.put<Patient>(`/patients/${patientData.id}`, patientData)
      : this.apiService.post<Patient>('/patients', patientData);

    request.subscribe({
      next: (patient) => {
        this.saving = false;
        this.closeModal();
        this.loadPatients();
      },
      error: (error) => {
        console.error('Error saving patient:', error);
        this.saving = false;
      }
    });
  }

  openModal() {
    this.patientModal = new bootstrap.Modal(document.getElementById('patientModal'));
    this.patientModal.show();
  }

  closeModal() {
    if (this.patientModal) {
      this.patientModal.hide();
    }
  }

  exportPatients() {
    console.log('Exporting patients...');
  }

  // Utility methods
  getInitials(name: string): string {
    return name.split(' ').map(n => n[0]).join('').toUpperCase().substring(0, 2);
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('pt-BR');
  }

  formatCPF(cpf: string): string {
    return cpf.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
  }

  getAge(birthDate: string): number {
    const today = new Date();
    const birth = new Date(birthDate);
    let age = today.getFullYear() - birth.getFullYear();
    const monthDiff = today.getMonth() - birth.getMonth();
    
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birth.getDate())) {
      age--;
    }
    
    return age;
  }

  getNewPatientsThisMonth(): number {
    const currentMonth = new Date().getMonth();
    const currentYear = new Date().getFullYear();
    
    return this.patients.filter(patient => {
      if (!patient.created_at) return false;
      const createdDate = new Date(patient.created_at);
      return createdDate.getMonth() === currentMonth && 
             createdDate.getFullYear() === currentYear;
    }).length;
  }

  getPatientsWithAppointments(): number {
    const patientIds = new Set(this.appointments.map(app => app.patient_id));
    return patientIds.size;
  }

  getPatientsWithMedicalHistory(): number {
    return this.patients.filter(patient => 
      patient.medical_history && patient.medical_history.trim() !== ''
    ).length;
  }
}