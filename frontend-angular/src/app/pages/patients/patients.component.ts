import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

interface Patient {
  id?: number;
  fullName: string;
  email: string;
  phone: string;
  dateOfBirth: string;
  address: string;
  emergencyContact: string;
  medicalHistory?: string;
  allergies?: string;
}

@Component({
  selector: 'app-patients',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="max-w-7xl mx-auto">
      <div class="flex justify-between items-center mb-6">
        <h2 class="text-2xl font-bold text-gray-900">Pacientes</h2>
        <button
          (click)="showNewPatientForm = !showNewPatientForm"
          class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md text-sm font-medium"
        >
          {{ showNewPatientForm ? 'Cancelar' : 'Novo Paciente' }}
        </button>
      </div>

      <!-- Formulário Novo Paciente -->
      <div *ngIf="showNewPatientForm" class="bg-white shadow rounded-lg p-6 mb-6">
        <h3 class="text-lg font-medium text-gray-900 mb-4">Cadastro de Paciente</h3>
        <form [formGroup]="patientForm" (ngSubmit)="onSubmit()">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700">Nome Completo</label>
              <input
                type="text"
                formControlName="fullName"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
                placeholder="Nome completo do paciente"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Email</label>
              <input
                type="email"
                formControlName="email"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
                placeholder="email@exemplo.com"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Telefone</label>
              <input
                type="tel"
                formControlName="phone"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
                placeholder="(11) 99999-9999"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Data de Nascimento</label>
              <input
                type="date"
                formControlName="dateOfBirth"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
            <div class="md:col-span-2">
              <label class="block text-sm font-medium text-gray-700">Endereço</label>
              <input
                type="text"
                formControlName="address"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
                placeholder="Endereço completo"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Contato de Emergência</label>
              <input
                type="text"
                formControlName="emergencyContact"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
                placeholder="Nome e telefone"
              />
            </div>
          </div>
          <div class="mt-4">
            <label class="block text-sm font-medium text-gray-700">Histórico Médico</label>
            <textarea
              formControlName="medicalHistory"
              rows="3"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
              placeholder="Histórico médico e tratamentos anteriores"
            ></textarea>
          </div>
          <div class="mt-4">
            <label class="block text-sm font-medium text-gray-700">Alergias</label>
            <textarea
              formControlName="allergies"
              rows="2"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
              placeholder="Alergias conhecidas"
            ></textarea>
          </div>
          <div class="mt-6 flex justify-end space-x-3">
            <button
              type="button"
              (click)="showNewPatientForm = false"
              class="bg-gray-300 hover:bg-gray-400 text-gray-800 px-4 py-2 rounded-md text-sm font-medium"
            >
              Cancelar
            </button>
            <button
              type="submit"
              [disabled]="!patientForm.valid || isLoading"
              class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md text-sm font-medium disabled:opacity-50"
            >
              {{ isLoading ? 'Salvando...' : 'Salvar Paciente' }}
            </button>
          </div>
        </form>
      </div>

      <!-- Lista de Pacientes -->
      <div class="bg-white shadow rounded-lg">
        <div class="px-6 py-4 border-b border-gray-200">
          <h3 class="text-lg font-medium text-gray-900">Lista de Pacientes</h3>
        </div>
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Nome
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Email
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Telefone
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Data Nascimento
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Ações
                </th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr *ngFor="let patient of patients">
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                  {{ patient.fullName }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {{ patient.email }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {{ patient.phone }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {{ formatDate(patient.dateOfBirth) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <button
                    (click)="editPatient(patient)"
                    class="text-blue-600 hover:text-blue-900 mr-3"
                  >
                    Editar
                  </button>
                  <button
                    (click)="deletePatient(patient.id!)"
                    class="text-red-600 hover:text-red-900"
                  >
                    Excluir
                  </button>
                </td>
              </tr>
              <tr *ngIf="patients.length === 0">
                <td colspan="5" class="px-6 py-4 text-center text-sm text-gray-500">
                  Nenhum paciente cadastrado
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `
})
export class PatientsComponent implements OnInit {
  patients: Patient[] = [];
  patientForm: FormGroup;
  showNewPatientForm = false;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient
  ) {
    this.patientForm = this.fb.group({
      fullName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.required]],
      dateOfBirth: ['', [Validators.required]],
      address: ['', [Validators.required]],
      emergencyContact: ['', [Validators.required]],
      medicalHistory: [''],
      allergies: ['']
    });
  }

  ngOnInit(): void {
    this.loadPatients();
  }

  private loadPatients(): void {
    this.http.get<Patient[]>('https://localhost:5001/api/patients').subscribe({
      next: (data) => {
        this.patients = data;
      },
      error: (error) => {
        console.error('Erro ao carregar pacientes:', error);
        this.patients = [];
      }
    });
  }

  onSubmit(): void {
    if (this.patientForm.valid) {
      this.isLoading = true;
      const patientData = this.patientForm.value;

      this.http.post<Patient>('https://localhost:5001/api/patients', patientData).subscribe({
        next: (response) => {
          this.patients.push(response);
          this.patientForm.reset();
          this.showNewPatientForm = false;
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Erro ao criar paciente:', error);
          this.isLoading = false;
        }
      });
    }
  }

  editPatient(patient: Patient): void {
    this.patientForm.patchValue(patient);
    this.showNewPatientForm = true;
  }

  deletePatient(id: number): void {
    if (confirm('Tem certeza que deseja excluir este paciente?')) {
      this.http.delete(`https://localhost:5001/api/patients/${id}`).subscribe({
        next: () => {
          this.patients = this.patients.filter(p => p.id !== id);
        },
        error: (error) => {
          console.error('Erro ao excluir paciente:', error);
        }
      });
    }
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR');
  }
}