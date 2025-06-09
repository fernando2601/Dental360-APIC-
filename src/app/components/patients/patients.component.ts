import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-patients',
  templateUrl: './patients.component.html'
})
export class PatientsComponent implements OnInit {
  patients: any[] = [];
  filteredPatients: any[] = [];
  searchTerm = '';
  loading = true;
  saving = false;
  isEditing = false;
  showModal = false;
  
  patientForm: FormGroup;
  selectedPatient: any = null;

  constructor(
    private apiService: ApiService,
    private fb: FormBuilder
  ) {
    this.patientForm = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.email]],
      phone: [''],
      birth_date: [''],
      cpf: [''],
      address: [''],
      city: [''],
      state: [''],
      zip_code: [''],
      emergency_contact: [''],
      emergency_phone: [''],
      medical_history: [''],
      allergies: [''],
      medications: [''],
      insurance: ['']
    });
  }

  ngOnInit() {
    this.loadPatients();
  }

  loadPatients() {
    this.loading = true;
    this.apiService.get('/patients').subscribe({
      next: (patients: any[]) => {
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

  filterPatients() {
    if (!this.searchTerm) {
      this.filteredPatients = this.patients;
    } else {
      const term = this.searchTerm.toLowerCase();
      this.filteredPatients = this.patients.filter(patient =>
        patient.name.toLowerCase().includes(term) ||
        patient.email?.toLowerCase().includes(term) ||
        patient.phone?.includes(term) ||
        patient.cpf?.includes(term)
      );
    }
  }

  openAddPatientModal() {
    this.isEditing = false;
    this.selectedPatient = null;
    this.patientForm.reset();
    this.showModal = true;
  }

  editPatient(patient: any) {
    this.isEditing = true;
    this.selectedPatient = patient;
    this.patientForm.patchValue(patient);
    this.showModal = true;
  }

  deletePatient(patient: any) {
    if (confirm(`Tem certeza que deseja excluir o paciente ${patient.name}?`)) {
      this.apiService.delete(`/patients/${patient.id}`).subscribe({
        next: () => {
          this.loadPatients();
        },
        error: (error) => {
          console.error('Error deleting patient:', error);
        }
      });
    }
  }

  savePatient() {
    if (!this.patientForm.valid) return;

    this.saving = true;
    const patientData = this.patientForm.value;

    const request = this.isEditing 
      ? this.apiService.put(`/patients/${this.selectedPatient?.id}`, patientData)
      : this.apiService.post('/patients', patientData);

    request.subscribe({
      next: () => {
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

  closeModal() {
    this.showModal = false;
  }

  calculateAge(birthDate: string): number {
    if (!birthDate) return 0;
    const today = new Date();
    const birth = new Date(birthDate);
    let age = today.getFullYear() - birth.getFullYear();
    const monthDiff = today.getMonth() - birth.getMonth();
    
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birth.getDate())) {
      age--;
    }
    
    return age;
  }

  formatDate(dateString: string): string {
    if (!dateString) return '';
    return new Date(dateString).toLocaleDateString('pt-BR');
  }
}