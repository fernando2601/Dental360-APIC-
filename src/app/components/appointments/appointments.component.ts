import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-appointments',
  templateUrl: './appointments.component.html'
})
export class AppointmentsComponent implements OnInit {
  appointments: any[] = [];
  filteredAppointments: any[] = [];
  patients: any[] = [];
  services: any[] = [];
  staffMembers: any[] = [];
  
  selectedDate = new Date().toISOString().split('T')[0];
  statusFilter = '';
  searchTerm = '';
  loading = true;
  saving = false;
  isEditing = false;
  showModal = false;
  
  appointmentForm: FormGroup;
  selectedAppointment: any = null;

  constructor(
    private apiService: ApiService,
    private fb: FormBuilder
  ) {
    this.appointmentForm = this.fb.group({
      patient_id: ['', Validators.required],
      service_id: ['', Validators.required],
      staff_id: ['', Validators.required],
      appointment_date: ['', Validators.required],
      notes: ['']
    });
  }

  ngOnInit() {
    this.loadAllData();
  }

  loadAllData() {
    this.loading = true;
    Promise.all([
      this.loadAppointments(),
      this.loadPatients(),
      this.loadServices(),
      this.loadStaff()
    ]).then(() => {
      this.loading = false;
      this.filterAppointments();
    }).catch(error => {
      console.error('Error loading data:', error);
      this.loading = false;
    });
  }

  loadAppointments(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.apiService.get('/appointments').subscribe({
        next: (appointments: any[]) => {
          this.appointments = appointments;
          resolve();
        },
        error: (error) => reject(error)
      });
    });
  }

  loadPatients(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.apiService.get('/patients').subscribe({
        next: (patients: any[]) => {
          this.patients = patients;
          resolve();
        },
        error: (error) => reject(error)
      });
    });
  }

  loadServices(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.apiService.get('/services').subscribe({
        next: (services: any[]) => {
          this.services = services;
          resolve();
        },
        error: (error) => reject(error)
      });
    });
  }

  loadStaff(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.apiService.get('/staff').subscribe({
        next: (staff: any[]) => {
          this.staffMembers = staff;
          resolve();
        },
        error: (error) => reject(error)
      });
    });
  }

  filterAppointments() {
    let filtered = this.appointments;
    
    if (this.selectedDate) {
      filtered = filtered.filter(appointment => 
        appointment.appointment_date.startsWith(this.selectedDate)
      );
    }
    
    if (this.statusFilter) {
      filtered = filtered.filter(appointment => 
        appointment.status === this.statusFilter
      );
    }
    
    if (this.searchTerm) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(appointment =>
        appointment.patient_name?.toLowerCase().includes(term) ||
        appointment.service_name?.toLowerCase().includes(term)
      );
    }
    
    this.filteredAppointments = filtered.sort((a, b) => 
      new Date(a.appointment_date).getTime() - new Date(b.appointment_date).getTime()
    );
  }

  openAddAppointmentModal() {
    this.isEditing = false;
    this.selectedAppointment = null;
    this.appointmentForm.reset();
    this.showModal = true;
  }

  editAppointment(appointment: any) {
    this.isEditing = true;
    this.selectedAppointment = appointment;
    this.appointmentForm.patchValue({
      patient_id: appointment.patient_id,
      service_id: appointment.service_id,
      staff_id: appointment.staff_id,
      appointment_date: appointment.appointment_date.substring(0, 16),
      notes: appointment.notes
    });
    this.showModal = true;
  }

  confirmAppointment(appointment: any) {
    this.updateAppointmentStatus(appointment.id, 'confirmed');
  }

  completeAppointment(appointment: any) {
    this.updateAppointmentStatus(appointment.id, 'completed');
  }

  cancelAppointment(appointment: any) {
    if (confirm('Tem certeza que deseja cancelar este agendamento?')) {
      this.updateAppointmentStatus(appointment.id, 'cancelled');
    }
  }

  updateAppointmentStatus(appointmentId: number, status: string) {
    this.apiService.put(`/appointments/${appointmentId}`, { status }).subscribe({
      next: () => {
        this.loadAllData();
      },
      error: (error) => {
        console.error('Error updating appointment status:', error);
      }
    });
  }

  saveAppointment() {
    if (!this.appointmentForm.valid) return;

    this.saving = true;
    const appointmentData = this.appointmentForm.value;

    const request = this.isEditing 
      ? this.apiService.put(`/appointments/${this.selectedAppointment?.id}`, appointmentData)
      : this.apiService.post('/appointments', appointmentData);

    request.subscribe({
      next: () => {
        this.saving = false;
        this.closeModal();
        this.loadAllData();
      },
      error: (error) => {
        console.error('Error saving appointment:', error);
        this.saving = false;
      }
    });
  }

  closeModal() {
    this.showModal = false;
  }

  formatDateTime(dateString: string): string {
    return new Date(dateString).toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }

  getStatusClass(status: string): string {
    const statusClasses: { [key: string]: string } = {
      'scheduled': 'bg-yellow-100 text-yellow-800',
      'confirmed': 'bg-green-100 text-green-800',
      'completed': 'bg-blue-100 text-blue-800',
      'cancelled': 'bg-red-100 text-red-800'
    };
    return statusClasses[status] || 'bg-gray-100 text-gray-800';
  }

  getStatusLabel(status: string): string {
    const statusLabels: { [key: string]: string } = {
      'scheduled': 'Agendado',
      'confirmed': 'Confirmado',
      'completed': 'Concluído',
      'cancelled': 'Cancelado'
    };
    return statusLabels[status] || status;
  }
}