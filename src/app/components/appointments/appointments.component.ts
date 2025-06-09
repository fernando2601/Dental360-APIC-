import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-appointments',
  template: `
    <div class="container">
      <div class="header">
        <h2>Gestão de Agendamentos</h2>
        <button class="btn btn-primary" (click)="openAddModal()">+ Novo Agendamento</button>
      </div>

      <div class="filters">
        <select [(ngModel)]="statusFilter" (change)="filterAppointments()" class="filter-select">
          <option value="">Todos os Status</option>
          <option value="scheduled">Agendado</option>
          <option value="confirmed">Confirmado</option>
          <option value="completed">Concluído</option>
          <option value="cancelled">Cancelado</option>
        </select>
        
        <input type="date" [(ngModel)]="dateFilter" (change)="filterAppointments()" class="filter-date">
      </div>

      <div class="appointments-list" *ngIf="!loading">
        <div class="appointment-card" *ngFor="let appointment of filteredAppointments">
          <div class="appointment-header">
            <div class="appointment-date">
              {{ appointment.appointment_date | date:'dd/MM/yyyy HH:mm' }}
            </div>
            <div class="appointment-status" [class]="appointment.status">
              {{ getStatusLabel(appointment.status) }}
            </div>
          </div>
          
          <div class="appointment-details">
            <div class="detail-row">
              <strong>Paciente:</strong> {{ appointment.patient_name }}
            </div>
            <div class="detail-row">
              <strong>Serviço:</strong> {{ appointment.service_name }}
            </div>
            <div class="detail-row">
              <strong>Profissional:</strong> {{ appointment.staff_name }}
            </div>
            <div class="detail-row" *ngIf="appointment.notes">
              <strong>Observações:</strong> {{ appointment.notes }}
            </div>
          </div>
          
          <div class="appointment-actions">
            <button class="btn btn-sm btn-outline" (click)="editAppointment(appointment)">Editar</button>
            <button class="btn btn-sm btn-success" *ngIf="appointment.status === 'scheduled'" 
                    (click)="confirmAppointment(appointment.id)">Confirmar</button>
            <button class="btn btn-sm btn-info" *ngIf="appointment.status === 'confirmed'" 
                    (click)="completeAppointment(appointment.id)">Concluir</button>
            <button class="btn btn-sm btn-danger" (click)="cancelAppointment(appointment.id)">Cancelar</button>
          </div>
        </div>
      </div>

      <div class="loading" *ngIf="loading">Carregando agendamentos...</div>

      <!-- Modal -->
      <div class="modal" *ngIf="showModal" (click)="closeModal()">
        <div class="modal-content" (click)="$event.stopPropagation()">
          <div class="modal-header">
            <h3>{{ isEditing ? 'Editar' : 'Novo' }} Agendamento</h3>
            <button class="close-btn" (click)="closeModal()">×</button>
          </div>
          
          <form [formGroup]="appointmentForm" (ngSubmit)="saveAppointment()">
            <div class="form-grid">
              <div class="form-group">
                <label>Paciente *</label>
                <select formControlName="patient_id" class="form-control">
                  <option value="">Selecione um paciente</option>
                  <option *ngFor="let patient of patients" [value]="patient.id">{{ patient.name }}</option>
                </select>
              </div>
              
              <div class="form-group">
                <label>Serviço *</label>
                <select formControlName="service_id" class="form-control">
                  <option value="">Selecione um serviço</option>
                  <option *ngFor="let service of services" [value]="service.id">{{ service.name }}</option>
                </select>
              </div>
              
              <div class="form-group">
                <label>Profissional *</label>
                <select formControlName="staff_id" class="form-control">
                  <option value="">Selecione um profissional</option>
                  <option *ngFor="let staff of staffMembers" [value]="staff.id">{{ staff.name }}</option>
                </select>
              </div>
              
              <div class="form-group">
                <label>Data *</label>
                <input type="date" formControlName="appointment_date" class="form-control">
              </div>
              
              <div class="form-group">
                <label>Horário *</label>
                <input type="time" formControlName="appointment_time" class="form-control">
              </div>
              
              <div class="form-group">
                <label>Status</label>
                <select formControlName="status" class="form-control">
                  <option value="scheduled">Agendado</option>
                  <option value="confirmed">Confirmado</option>
                  <option value="completed">Concluído</option>
                  <option value="cancelled">Cancelado</option>
                </select>
              </div>
            </div>
            
            <div class="form-group">
              <label>Observações</label>
              <textarea formControlName="notes" class="form-control" rows="3"></textarea>
            </div>
            
            <div class="modal-actions">
              <button type="button" class="btn btn-outline" (click)="closeModal()">Cancelar</button>
              <button type="submit" class="btn btn-primary" [disabled]="!appointmentForm.valid">Salvar</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .container { padding: 2rem; }
    .header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 2rem; }
    
    .filters {
      display: flex;
      gap: 1rem;
      margin-bottom: 2rem;
    }
    
    .filter-select, .filter-date {
      padding: 0.5rem;
      border: 1px solid #d1d5db;
      border-radius: 4px;
    }
    
    .appointments-list {
      display: grid;
      gap: 1rem;
    }
    
    .appointment-card {
      background: white;
      border-radius: 8px;
      padding: 1.5rem;
      box-shadow: 0 1px 3px rgba(0,0,0,0.1);
    }
    
    .appointment-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1rem;
    }
    
    .appointment-date {
      font-weight: 600;
      color: #1f2937;
      font-size: 1.1rem;
    }
    
    .appointment-status {
      padding: 0.25rem 0.75rem;
      border-radius: 20px;
      font-size: 0.75rem;
      font-weight: 600;
      text-transform: uppercase;
    }
    
    .appointment-status.scheduled { background: #dbeafe; color: #1d4ed8; }
    .appointment-status.confirmed { background: #d1fae5; color: #059669; }
    .appointment-status.completed { background: #f3f4f6; color: #6b7280; }
    .appointment-status.cancelled { background: #fee2e2; color: #dc2626; }
    
    .appointment-details {
      margin-bottom: 1rem;
    }
    
    .detail-row {
      margin-bottom: 0.5rem;
      color: #6b7280;
    }
    
    .appointment-actions {
      display: flex;
      gap: 0.5rem;
      flex-wrap: wrap;
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
    .btn-success { background: #10b981; color: white; }
    .btn-info { background: #06b6d4; color: white; }
    .btn-danger { background: #ef4444; color: white; }
    .btn-sm { padding: 0.5rem 1rem; font-size: 0.875rem; }
    
    .btn:hover { opacity: 0.9; }
    .btn:disabled { opacity: 0.5; cursor: not-allowed; }
    
    .loading { text-align: center; padding: 3rem; color: #6b7280; }
  `]
})
export class AppointmentsComponent implements OnInit {
  appointments: any[] = [];
  filteredAppointments: any[] = [];
  patients: any[] = [];
  services: any[] = [];
  staffMembers: any[] = [];
  appointmentForm: FormGroup;
  selectedAppointment: any = null;
  showModal = false;
  isEditing = false;
  loading = false;
  statusFilter = '';
  dateFilter = '';

  constructor(
    private apiService: ApiService,
    private fb: FormBuilder
  ) {
    this.appointmentForm = this.fb.group({
      patient_id: ['', Validators.required],
      service_id: ['', Validators.required],
      staff_id: ['', Validators.required],
      appointment_date: ['', Validators.required],
      appointment_time: ['', Validators.required],
      status: ['scheduled', Validators.required],
      notes: ['']
    });
  }

  ngOnInit() {
    this.loadAppointments();
    this.loadPatients();
    this.loadServices();
    this.loadStaff();
  }

  loadAppointments() {
    this.loading = true;
    this.apiService.get('/appointments').subscribe({
      next: (data) => {
        this.appointments = data;
        this.filteredAppointments = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar agendamentos:', error);
        this.loading = false;
        // Dados de demonstração
        this.appointments = [
          {
            id: 1,
            patient_name: 'Maria Silva',
            service_name: 'Limpeza Dental',
            staff_name: 'Dr. João',
            appointment_date: '2024-06-10T10:00:00',
            status: 'confirmed',
            notes: 'Primeira consulta'
          },
          {
            id: 2,
            patient_name: 'João Santos',
            service_name: 'Consulta',
            staff_name: 'Dra. Ana',
            appointment_date: '2024-06-10T14:00:00',
            status: 'scheduled',
            notes: ''
          }
        ];
        this.filteredAppointments = this.appointments;
      }
    });
  }

  loadPatients() {
    this.apiService.get('/patients').subscribe({
      next: (data) => this.patients = data,
      error: () => {
        this.patients = [
          { id: 1, name: 'Maria Silva' },
          { id: 2, name: 'João Santos' }
        ];
      }
    });
  }

  loadServices() {
    this.apiService.get('/services').subscribe({
      next: (data) => this.services = data,
      error: () => {
        this.services = [
          { id: 1, name: 'Limpeza Dental' },
          { id: 2, name: 'Consulta' },
          { id: 3, name: 'Clareamento' }
        ];
      }
    });
  }

  loadStaff() {
    this.apiService.get('/staff').subscribe({
      next: (data) => this.staffMembers = data,
      error: () => {
        this.staffMembers = [
          { id: 1, name: 'Dr. João Silva' },
          { id: 2, name: 'Dra. Ana Costa' }
        ];
      }
    });
  }

  filterAppointments() {
    let filtered = this.appointments;

    if (this.statusFilter) {
      filtered = filtered.filter(apt => apt.status === this.statusFilter);
    }

    if (this.dateFilter) {
      filtered = filtered.filter(apt => {
        const aptDate = new Date(apt.appointment_date).toISOString().split('T')[0];
        return aptDate === this.dateFilter;
      });
    }

    this.filteredAppointments = filtered;
  }

  openAddModal() {
    this.isEditing = false;
    this.selectedAppointment = null;
    this.appointmentForm.reset();
    this.appointmentForm.patchValue({ status: 'scheduled' });
    this.showModal = true;
  }

  editAppointment(appointment: any) {
    this.isEditing = true;
    this.selectedAppointment = appointment;
    
    const appointmentDate = new Date(appointment.appointment_date);
    this.appointmentForm.patchValue({
      ...appointment,
      appointment_date: appointmentDate.toISOString().split('T')[0],
      appointment_time: appointmentDate.toTimeString().split(' ')[0].substr(0, 5)
    });
    this.showModal = true;
  }

  confirmAppointment(id: number) {
    this.updateAppointmentStatus(id, 'confirmed');
  }

  completeAppointment(id: number) {
    this.updateAppointmentStatus(id, 'completed');
  }

  cancelAppointment(id: number) {
    if (confirm('Tem certeza que deseja cancelar este agendamento?')) {
      this.updateAppointmentStatus(id, 'cancelled');
    }
  }

  updateAppointmentStatus(id: number, status: string) {
    this.apiService.put(`/appointments/${id}`, { status }).subscribe({
      next: () => this.loadAppointments(),
      error: (error) => console.error('Erro ao atualizar status:', error)
    });
  }

  saveAppointment() {
    if (this.appointmentForm.valid) {
      const formData = this.appointmentForm.value;
      const appointmentData = {
        ...formData,
        appointment_date: `${formData.appointment_date}T${formData.appointment_time}:00`
      };
      
      if (this.isEditing) {
        this.apiService.put(`/appointments/${this.selectedAppointment.id}`, appointmentData).subscribe({
          next: () => {
            this.closeModal();
            this.loadAppointments();
          },
          error: (error) => console.error('Erro ao atualizar agendamento:', error)
        });
      } else {
        this.apiService.post('/appointments', appointmentData).subscribe({
          next: () => {
            this.closeModal();
            this.loadAppointments();
          },
          error: (error) => console.error('Erro ao criar agendamento:', error)
        });
      }
    }
  }

  closeModal() {
    this.showModal = false;
    this.selectedAppointment = null;
    this.appointmentForm.reset();
  }

  getStatusLabel(status: string): string {
    const labels: any = {
      'scheduled': 'Agendado',
      'confirmed': 'Confirmado',
      'completed': 'Concluído',
      'cancelled': 'Cancelado'
    };
    return labels[status] || status;
  }
}