import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { PatientsService, Patient } from '../../services/patients.service';
import { ServicesService, Service } from '../../services/services.service';
import { StaffService, Staff } from '../../services/staff.service';

interface Appointment {
  id?: number;
  patientName: string;
  doctorName: string;
  service: string;
  date: string;
  time: string;
  status: 'agendado' | 'confirmado' | 'em_andamento' | 'concluido' | 'cancelado';
  notes?: string;
}

@Component({
  selector: 'app-agenda',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="max-w-7xl mx-auto">
      <div class="flex justify-between items-center mb-6">
        <h2 class="text-2xl font-bold text-gray-900">Agenda</h2>
        <button
          (click)="showNewAppointmentForm = !showNewAppointmentForm"
          class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md text-sm font-medium"
        >
          {{ showNewAppointmentForm ? 'Cancelar' : 'Novo Agendamento' }}
        </button>
      </div>

      <!-- Novo Agendamento Form -->
      <div *ngIf="showNewAppointmentForm" class="bg-white shadow rounded-lg p-6 mb-6">
        <h3 class="text-lg font-medium text-gray-900 mb-4">Novo Agendamento</h3>
        <form [formGroup]="appointmentForm" (ngSubmit)="onSubmit()">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700">Paciente</label>
              <div class="flex items-center gap-2">
                <select
                  formControlName="patientId"
                  class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
                >
                  <option value="">Pesquise/Selecione</option>
                  <option *ngFor="let patient of patients" [value]="patient.id">
                    {{ patient.fullName }}
                  </option>
                </select>
                <button
                  type="button"
                  (click)="showAddPatientModal = true"
                  class="mt-1 flex-shrink-0 bg-purple-600 hover:bg-purple-700 text-white px-3 py-2 rounded-md text-sm font-medium"
                  title="Adicionar novo paciente"
                >
                  +
                </button>
              </div>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Profissional</label>
              <select
                formControlName="staffId"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="">Selecione um profissional</option>
                <option *ngFor="let member of staff" [value]="member.id">
                  {{ member.specialization }}
                </option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Serviço</label>
              <select
                formControlName="serviceId"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="">Selecione um serviço</option>
                <option *ngFor="let service of services" [value]="service.id">
                  {{ service.name }} ({{ service.duration }}min)
                </option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Status</label>
              <select
                formControlName="status"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="agendado">Agendado</option>
                <option value="confirmado">Confirmado</option>
                <option value="em_andamento">Em Andamento</option>
                <option value="concluido">Concluído</option>
                <option value="cancelado">Cancelado</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Data</label>
              <input
                type="date"
                formControlName="date"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Horário</label>
              <input
                type="time"
                formControlName="time"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
          </div>
          <div class="mt-4">
            <label class="block text-sm font-medium text-gray-700">Observações</label>
            <textarea
              formControlName="notes"
              rows="3"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
              placeholder="Observações sobre o agendamento"
            ></textarea>
          </div>
          <div class="mt-6 flex justify-end space-x-3">
            <button
              type="button"
              (click)="showNewAppointmentForm = false"
              class="bg-gray-300 hover:bg-gray-400 text-gray-800 px-4 py-2 rounded-md text-sm font-medium"
            >
              Cancelar
            </button>
            <button
              type="submit"
              [disabled]="!appointmentForm.valid || isLoading"
              class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md text-sm font-medium disabled:opacity-50"
            >
              {{ isLoading ? 'Salvando...' : 'Salvar Agendamento' }}
            </button>
          </div>
        </form>
      </div>

      <!-- Modal Adicionar Paciente -->
      <div *ngIf="showAddPatientModal" class="fixed inset-0 bg-gray-600 bg-opacity-50 overflow-y-auto h-full w-full z-50">
        <div class="relative top-10 mx-auto p-6 border w-full max-w-2xl shadow-lg rounded-md bg-white">
          <div class="flex justify-between items-center mb-6">
            <h3 class="text-xl font-medium text-gray-900">Novo paciente</h3>
            <button 
              (click)="showAddPatientModal = false"
              class="text-gray-400 hover:text-gray-600"
            >
              ✕
            </button>
          </div>
          
          <form [formGroup]="patientForm" (ngSubmit)="onAddPatient()">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <!-- Coluna Esquerda -->
              <div class="space-y-4">
                <!-- Foto -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">Foto</label>
                  <div class="flex items-center space-x-4">
                    <div class="w-16 h-16 bg-gray-200 rounded-full flex items-center justify-center">
                      <svg class="w-8 h-8 text-gray-400" fill="currentColor" viewBox="0 0 20 20">
                        <path fill-rule="evenodd" d="M10 9a3 3 0 100-6 3 3 0 000 6zm-7 9a7 7 0 1114 0H3z" clip-rule="evenodd" />
                      </svg>
                    </div>
                    <div>
                      <p class="text-xs text-gray-500 mb-1">Selecione um arquivo JPG ou PNG do seu dispositivo</p>
                      <button 
                        type="button"
                        class="bg-purple-100 text-purple-700 px-4 py-1 rounded text-sm hover:bg-purple-200"
                      >
                        Escolher foto
                      </button>
                    </div>
                  </div>
                </div>

                <!-- Telefone -->
                <div>
                  <label class="block text-sm font-medium text-gray-700">Telefone</label>
                  <div class="flex">
                    <select class="mt-1 block w-20 border-gray-300 rounded-l-md shadow-sm focus:ring-purple-500 focus:border-purple-500">
                      <option>🇧🇷 +55</option>
                    </select>
                    <input
                      type="tel"
                      formControlName="phone"
                      class="mt-1 block w-full border-gray-300 rounded-r-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                      placeholder="(99)9999-9999"
                    />
                  </div>
                </div>

                <!-- CPF -->
                <div>
                  <label class="block text-sm font-medium text-gray-700">CPF</label>
                  <input
                    type="text"
                    formControlName="cpf"
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                    placeholder="000.000.000-00"
                  />
                </div>

                <!-- Sexo -->
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-2">Sexo</label>
                  <div class="flex space-x-4">
                    <label class="flex items-center">
                      <input
                        type="radio"
                        formControlName="gender"
                        value="Feminino"
                        class="form-radio text-purple-600"
                      />
                      <span class="ml-2 text-sm text-gray-700">Feminino</span>
                    </label>
                    <label class="flex items-center">
                      <input
                        type="radio"
                        formControlName="gender"
                        value="Masculino"
                        class="form-radio text-purple-600"
                      />
                      <span class="ml-2 text-sm text-gray-700">Masculino</span>
                    </label>
                  </div>
                </div>
              </div>

              <!-- Coluna Direita -->
              <div class="space-y-4">
                <!-- Nome -->
                <div>
                  <label class="block text-sm font-medium text-gray-700">Nome*</label>
                  <input
                    type="text"
                    formControlName="fullName"
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                    placeholder="Nome Sobrenome"
                  />
                </div>

                <!-- Email -->
                <div>
                  <label class="block text-sm font-medium text-gray-700">E-mail</label>
                  <input
                    type="email"
                    formControlName="email"
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                    placeholder="E-mail"
                  />
                </div>

                <!-- Data de Nascimento -->
                <div>
                  <label class="block text-sm font-medium text-gray-700">Data de nascimento</label>
                  <input
                    type="date"
                    formControlName="birthday"
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                  />
                </div>

                <!-- RG -->
                <div>
                  <label class="block text-sm font-medium text-gray-700">RG</label>
                  <input
                    type="text"
                    formControlName="rg"
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                    placeholder="Digite"
                  />
                </div>

                <!-- Etiquetas -->
                <div>
                  <label class="block text-sm font-medium text-gray-700">Etiquetas 
                    <span class="text-gray-400">ⓘ</span>
                  </label>
                  <div class="flex items-center">
                    <select
                      formControlName="tags"
                      class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                    >
                      <option value="">Pesquise/Selecione</option>
                      <option value="VIP">VIP</option>
                      <option value="Regular">Regular</option>
                      <option value="Novo">Novo</option>
                    </select>
                    <button
                      type="button"
                      class="ml-2 text-purple-600 hover:text-purple-800 text-sm"
                    >
                      + Adicionar
                    </button>
                  </div>
                </div>
              </div>
            </div>

            <!-- Botão Salvar -->
            <div class="mt-8 flex justify-center">
              <button
                type="submit"
                [disabled]="!patientForm.valid || isAddingPatient"
                class="bg-purple-600 hover:bg-purple-700 text-white px-8 py-2 rounded-md text-sm font-medium disabled:opacity-50"
              >
                {{ isAddingPatient ? 'Salvando...' : 'Salvar' }}
              </button>
            </div>
          </form>
        </div>
      </div>

      <!-- Lista de Agendamentos -->
      <div class="bg-white shadow rounded-lg">
        <div class="px-6 py-4 border-b border-gray-200">
          <h3 class="text-lg font-medium text-gray-900">Agendamentos</h3>
        </div>
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Paciente
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Dentista
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Serviço
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Data/Hora
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Status
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Ações
                </th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr *ngFor="let appointment of appointments">
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                  {{ appointment.patientName }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {{ appointment.doctorName }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {{ appointment.service }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {{ formatDateTime(appointment.date, appointment.time) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span [class]="getStatusClass(appointment.status)" class="inline-flex px-2 py-1 text-xs font-semibold rounded-full">
                    {{ getStatusLabel(appointment.status) }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <button
                    (click)="editAppointment(appointment)"
                    class="text-blue-600 hover:text-blue-900 mr-3"
                  >
                    Editar
                  </button>
                  <button
                    (click)="deleteAppointment(appointment.id!)"
                    class="text-red-600 hover:text-red-900"
                  >
                    Excluir
                  </button>
                </td>
              </tr>
              <tr *ngIf="appointments.length === 0">
                <td colspan="6" class="px-6 py-4 text-center text-sm text-gray-500">
                  Nenhum agendamento encontrado
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `
})
export class AgendaComponent implements OnInit {
  appointments: Appointment[] = [];
  patients: Patient[] = [];
  services: Service[] = [];
  staff: Staff[] = [];
  appointmentForm: FormGroup;
  patientForm: FormGroup;
  showNewAppointmentForm = false;
  showAddPatientModal = false;
  isLoading = false;
  isAddingPatient = false;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private patientsService: PatientsService,
    private servicesService: ServicesService,
    private staffService: StaffService
  ) {
    this.appointmentForm = this.fb.group({
      patientId: ['', [Validators.required]],
      staffId: ['', [Validators.required]],
      serviceId: ['', [Validators.required]],
      date: ['', [Validators.required]],
      time: ['', [Validators.required]],
      status: ['agendado', [Validators.required]],
      notes: ['']
    });

    this.patientForm = this.fb.group({
      fullName: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.required]],
      cpf: [''],
      rg: [''],
      birthday: [''],
      gender: [''],
      tags: [''],
      address: [''],
      notes: ['']
    });
  }

  ngOnInit(): void {
    this.loadAppointments();
    this.loadPatients();
    this.loadServices();
    this.loadStaff();
  }

  private loadAppointments(): void {
    // Conectará com backend .NET em https://localhost:5001/api/agenda
    this.http.get<Appointment[]>('https://localhost:5001/api/agenda').subscribe({
      next: (data) => {
        this.appointments = data;
      },
      error: (error) => {
        console.error('Erro ao carregar agendamentos:', error);
        this.appointments = [];
      }
    });
  }

  private loadPatients(): void {
    this.patientsService.getPatients().subscribe({
      next: (data) => {
        this.patients = data;
      },
      error: (error) => {
        console.error('Erro ao carregar pacientes:', error);
        this.patients = [];
      }
    });
  }

  private loadServices(): void {
    this.servicesService.getServices().subscribe({
      next: (data) => {
        this.services = data;
      },
      error: (error) => {
        console.error('Erro ao carregar serviços:', error);
        this.services = [];
      }
    });
  }

  private loadStaff(): void {
    this.staffService.getStaffMembers().subscribe({
      next: (data) => {
        this.staff = data;
      },
      error: (error) => {
        console.error('Erro ao carregar equipe:', error);
        this.staff = [];
      }
    });
  }

  onAddPatient(): void {
    if (this.patientForm.valid) {
      this.isAddingPatient = true;
      const patientData = this.patientForm.value;

      this.patientsService.createPatient(patientData).subscribe({
        next: (newPatient) => {
          this.patients.push(newPatient);
          this.appointmentForm.patchValue({ patientId: newPatient.id });
          this.patientForm.reset();
          this.showAddPatientModal = false;
          this.isAddingPatient = false;
        },
        error: (error) => {
          console.error('Erro ao criar paciente:', error);
          this.isAddingPatient = false;
        }
      });
    }
  }

  onSubmit(): void {
    if (this.appointmentForm.valid) {
      this.isLoading = true;
      const appointmentData = this.appointmentForm.value;

      this.http.post<Appointment>('https://localhost:5001/api/agenda', appointmentData).subscribe({
        next: (response) => {
          this.appointments.push(response);
          this.appointmentForm.reset();
          this.showNewAppointmentForm = false;
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Erro ao criar agendamento:', error);
          this.isLoading = false;
        }
      });
    }
  }

  editAppointment(appointment: Appointment): void {
    this.appointmentForm.patchValue(appointment);
    this.showNewAppointmentForm = true;
  }

  deleteAppointment(id: number): void {
    if (confirm('Tem certeza que deseja excluir este agendamento?')) {
      this.http.delete(`https://localhost:5001/api/agenda/${id}`).subscribe({
        next: () => {
          this.appointments = this.appointments.filter(a => a.id !== id);
        },
        error: (error) => {
          console.error('Erro ao excluir agendamento:', error);
        }
      });
    }
  }

  formatDateTime(date: string, time: string): string {
    const dateObj = new Date(date + 'T' + time);
    return dateObj.toLocaleString('pt-BR');
  }

  getStatusClass(status: string): string {
    const classes = {
      'agendado': 'bg-yellow-100 text-yellow-800',
      'confirmado': 'bg-blue-100 text-blue-800',
      'em_andamento': 'bg-purple-100 text-purple-800',
      'concluido': 'bg-green-100 text-green-800',
      'cancelado': 'bg-red-100 text-red-800'
    };
    return classes[status as keyof typeof classes] || 'bg-gray-100 text-gray-800';
  }

  getStatusLabel(status: string): string {
    const labels = {
      'agendado': 'Agendado',
      'confirmado': 'Confirmado',
      'em_andamento': 'Em Andamento',
      'concluido': 'Concluído',
      'cancelado': 'Cancelado'
    };
    return labels[status as keyof typeof labels] || status;
  }
}