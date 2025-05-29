import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

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
              <input
                type="text"
                formControlName="patientName"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
                placeholder="Nome do paciente"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Dentista</label>
              <select
                formControlName="doctorName"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="">Selecione um dentista</option>
                <option value="Dr. João Silva">Dr. João Silva</option>
                <option value="Dra. Maria Santos">Dra. Maria Santos</option>
                <option value="Dr. Pedro Costa">Dr. Pedro Costa</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Serviço</label>
              <select
                formControlName="service"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="">Selecione um serviço</option>
                <option value="Limpeza Dental">Limpeza Dental</option>
                <option value="Restauração">Restauração</option>
                <option value="Extração">Extração</option>
                <option value="Canal">Canal</option>
                <option value="Ortodontia">Ortodontia</option>
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
  appointmentForm: FormGroup;
  showNewAppointmentForm = false;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient
  ) {
    this.appointmentForm = this.fb.group({
      patientName: ['', [Validators.required]],
      doctorName: ['', [Validators.required]],
      service: ['', [Validators.required]],
      date: ['', [Validators.required]],
      time: ['', [Validators.required]],
      status: ['agendado', [Validators.required]],
      notes: ['']
    });
  }

  ngOnInit(): void {
    this.loadAppointments();
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