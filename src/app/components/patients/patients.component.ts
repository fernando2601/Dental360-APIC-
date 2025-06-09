import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-patients',
  template: `
    <div class="container">
      <div class="header">
        <h2>Gestão de Pacientes</h2>
        <button class="btn btn-primary" (click)="openAddModal()">+ Novo Paciente</button>
      </div>

      <div class="search-bar">
        <input type="text" 
               placeholder="Buscar pacientes..." 
               [(ngModel)]="searchTerm" 
               (input)="filterPatients()"
               class="search-input">
      </div>

      <div class="patients-grid" *ngIf="!loading">
        <div class="patient-card" *ngFor="let patient of filteredPatients">
          <div class="patient-info">
            <h4>{{ patient.name }}</h4>
            <p><strong>Email:</strong> {{ patient.email }}</p>
            <p><strong>Telefone:</strong> {{ patient.phone }}</p>
            <p><strong>CPF:</strong> {{ patient.cpf }}</p>
            <p><strong>Data Nascimento:</strong> {{ patient.birth_date | date:'dd/MM/yyyy' }}</p>
          </div>
          <div class="patient-actions">
            <button class="btn btn-sm btn-outline" (click)="editPatient(patient)">Editar</button>
            <button class="btn btn-sm btn-danger" (click)="deletePatient(patient.id)">Excluir</button>
          </div>
        </div>
      </div>

      <div class="loading" *ngIf="loading">Carregando pacientes...</div>

      <!-- Modal -->
      <div class="modal" *ngIf="showModal" (click)="closeModal()">
        <div class="modal-content" (click)="$event.stopPropagation()">
          <div class="modal-header">
            <h3>{{ isEditing ? 'Editar' : 'Novo' }} Paciente</h3>
            <button class="close-btn" (click)="closeModal()">×</button>
          </div>
          
          <form [formGroup]="patientForm" (ngSubmit)="savePatient()">
            <div class="form-grid">
              <div class="form-group">
                <label>Nome *</label>
                <input type="text" formControlName="name" class="form-control">
              </div>
              
              <div class="form-group">
                <label>Email *</label>
                <input type="email" formControlName="email" class="form-control">
              </div>
              
              <div class="form-group">
                <label>Telefone *</label>
                <input type="text" formControlName="phone" class="form-control">
              </div>
              
              <div class="form-group">
                <label>CPF</label>
                <input type="text" formControlName="cpf" class="form-control">
              </div>
              
              <div class="form-group">
                <label>Data de Nascimento</label>
                <input type="date" formControlName="birth_date" class="form-control">
              </div>
              
              <div class="form-group">
                <label>Endereço</label>
                <input type="text" formControlName="address" class="form-control">
              </div>
              
              <div class="form-group">
                <label>Cidade</label>
                <input type="text" formControlName="city" class="form-control">
              </div>
              
              <div class="form-group">
                <label>Estado</label>
                <input type="text" formControlName="state" class="form-control">
              </div>
            </div>
            
            <div class="form-group">
              <label>Observações</label>
              <textarea formControlName="notes" class="form-control" rows="3"></textarea>
            </div>
            
            <div class="modal-actions">
              <button type="button" class="btn btn-outline" (click)="closeModal()">Cancelar</button>
              <button type="submit" class="btn btn-primary" [disabled]="!patientForm.valid">Salvar</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .container { padding: 2rem; }
    .header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 2rem; }
    .search-bar { margin-bottom: 2rem; }
    .search-input { 
      width: 100%; 
      max-width: 400px; 
      padding: 0.75rem; 
      border: 1px solid #d1d5db; 
      border-radius: 8px; 
    }
    
    .patients-grid { 
      display: grid; 
      grid-template-columns: repeat(auto-fill, minmax(350px, 1fr)); 
      gap: 1.5rem; 
    }
    
    .patient-card {
      background: white;
      border-radius: 8px;
      padding: 1.5rem;
      box-shadow: 0 1px 3px rgba(0,0,0,0.1);
    }
    
    .patient-info h4 { margin: 0 0 1rem 0; color: #1f2937; }
    .patient-info p { margin: 0.5rem 0; color: #6b7280; }
    
    .patient-actions { 
      margin-top: 1rem; 
      display: flex; 
      gap: 0.5rem; 
    }
    
    .modal {
      position: fixed;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      background: rgba(0,0,0,0.5);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1000;
    }
    
    .modal-content {
      background: white;
      border-radius: 8px;
      width: 90%;
      max-width: 600px;
      max-height: 90vh;
      overflow-y: auto;
    }
    
    .modal-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 1.5rem;
      border-bottom: 1px solid #e5e7eb;
    }
    
    .close-btn {
      background: none;
      border: none;
      font-size: 1.5rem;
      cursor: pointer;
    }
    
    .form-grid {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 1rem;
      padding: 1.5rem;
    }
    
    .form-group {
      display: flex;
      flex-direction: column;
    }
    
    .form-group label {
      margin-bottom: 0.5rem;
      font-weight: 500;
      color: #374151;
    }
    
    .form-control {
      padding: 0.75rem;
      border: 1px solid #d1d5db;
      border-radius: 4px;
    }
    
    .modal-actions {
      display: flex;
      justify-content: flex-end;
      gap: 1rem;
      padding: 1.5rem;
      border-top: 1px solid #e5e7eb;
    }
    
    .btn {
      padding: 0.75rem 1.5rem;
      border: none;
      border-radius: 4px;
      font-weight: 500;
      cursor: pointer;
      transition: all 0.2s;
    }
    
    .btn-primary { background: #3b82f6; color: white; }
    .btn-outline { background: transparent; border: 1px solid #d1d5db; color: #374151; }
    .btn-danger { background: #ef4444; color: white; }
    .btn-sm { padding: 0.5rem 1rem; font-size: 0.875rem; }
    
    .btn:hover { opacity: 0.9; }
    .btn:disabled { opacity: 0.5; cursor: not-allowed; }
    
    .loading { text-align: center; padding: 3rem; color: #6b7280; }
  `]
})
export class PatientsComponent implements OnInit {
  patients: any[] = [];
  filteredPatients: any[] = [];
  patientForm: FormGroup;
  selectedPatient: any = null;
  showModal = false;
  isEditing = false;
  loading = false;
  searchTerm = '';

  constructor(
    private apiService: ApiService,
    private fb: FormBuilder
  ) {
    this.patientForm = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', Validators.required],
      cpf: [''],
      address: [''],
      city: [''],
      state: [''],
      birth_date: [''],
      notes: ['']
    });
  }

  ngOnInit() {
    this.loadPatients();
  }

  loadPatients() {
    this.loading = true;
    this.apiService.get('/patients').subscribe({
      next: (data) => {
        this.patients = data;
        this.filteredPatients = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar pacientes:', error);
        this.loading = false;
        // Dados de demonstração
        this.patients = [
          { id: 1, name: 'Maria Silva', email: 'maria@email.com', phone: '(11) 99999-9999', cpf: '123.456.789-00', birth_date: '1985-05-15' },
          { id: 2, name: 'João Santos', email: 'joao@email.com', phone: '(11) 88888-8888', cpf: '987.654.321-00', birth_date: '1978-12-10' }
        ];
        this.filteredPatients = this.patients;
      }
    });
  }

  filterPatients() {
    if (!this.searchTerm) {
      this.filteredPatients = this.patients;
    } else {
      const term = this.searchTerm.toLowerCase();
      this.filteredPatients = this.patients.filter(patient =>
        patient.name?.toLowerCase().includes(term) ||
        patient.email?.toLowerCase().includes(term) ||
        patient.phone?.includes(term) ||
        patient.cpf?.includes(term)
      );
    }
  }

  openAddModal() {
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

  deletePatient(id: number) {
    if (confirm('Tem certeza que deseja excluir este paciente?')) {
      this.apiService.delete(`/patients/${id}`).subscribe({
        next: () => {
          this.loadPatients();
        },
        error: (error) => {
          console.error('Erro ao excluir paciente:', error);
        }
      });
    }
  }

  savePatient() {
    if (this.patientForm.valid) {
      const patientData = this.patientForm.value;
      
      if (this.isEditing) {
        this.apiService.put(`/patients/${this.selectedPatient.id}`, patientData).subscribe({
          next: () => {
            this.closeModal();
            this.loadPatients();
          },
          error: (error) => {
            console.error('Erro ao atualizar paciente:', error);
          }
        });
      } else {
        this.apiService.post('/patients', patientData).subscribe({
          next: () => {
            this.closeModal();
            this.loadPatients();
          },
          error: (error) => {
            console.error('Erro ao criar paciente:', error);
          }
        });
      }
    }
  }

  closeModal() {
    this.showModal = false;
    this.selectedPatient = null;
    this.patientForm.reset();
  }
}