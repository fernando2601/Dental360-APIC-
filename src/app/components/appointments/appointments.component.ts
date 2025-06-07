import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../services/api.service';

declare var bootstrap: any;

interface Appointment {
  id?: number;
  patient_id: number;
  service_id: number;
  staff_id: number;
  appointment_date: string;
  duration_minutes?: number;
  status: string;
  notes?: string;
  patient_name?: string;
  service_name?: string;
  staff_name?: string;
}

interface Patient {
  id: number;
  name: string;
  email?: string;
  phone?: string;
}

interface Service {
  id: number;
  name: string;
  price: number;
  duration_minutes: number;
  category: string;
}

interface Staff {
  id: number;
  full_name: string;
  specialization: string;
  position: string;
}

@Component({
  selector: 'app-appointments',
  templateUrl: './appointments.component.html'
})
export class AppointmentsComponent implements OnInit {
  appointments: Appointment[] = [];
  filteredAppointments: Appointment[] = [];
  patients: Patient[] = [];
  services: Service[] = [];
  staffMembers: Staff[] = [];
  
  selectedDate = new Date().toISOString().split('T')[0];
  statusFilter = '';
  loading = true;
  saving = false;
  isEditing = false;
  
  appointmentForm: FormGroup;
  appointmentModal: any;

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
      this.filterByDate();
    }).catch(error => {
      console.error('Error loading data:', error);
      this.loading = false;
    });
  }

  loadAppointments(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.apiService.get<Appointment[]>('/appointments').subscribe({
        next: (appointments) => {
          this.appointments = appointments;
          resolve();
        },
        error: (error) => reject(error)
      });
    });
  }

  loadPatients(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.apiService.get<Patient[]>('/patients').subscribe({
        next: (patients) => {
          this.patients = patients;
          resolve();
        },
        error: (error) => reject(error)
      });
    });
  }

  loadServices(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.apiService.get<Service[]>('/services').subscribe({
        next: (services) => {
          this.services = services;
          resolve();
        },
        error: (error) => reject(error)
      });
    });
  }

  loadStaff(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.apiService.get<Staff[]>('/staff').subscribe({
        next: (staff) => {
          this.staffMembers = staff;
          resolve();
        },
        error: (error) => reject(error)
      });
    });
  }

  filterByDate() {
    const selectedDateStr = this.selectedDate;
    this.filteredAppointments = this.appointments.filter(appointment => 
      appointment.appointment_date.startsWith(selectedDateStr)
    );
    this.filterAppointments();
  }

  filterAppointments() {
    let filtered = this.filteredAppointments;
    
    if (this.statusFilter) {
      filtered = filtered.filter(appointment => 
        appointment.status === this.statusFilter
      );
    }
    
    this.filteredAppointments = filtered.sort((a, b) => 
      new Date(a.appointment_date).getTime() - new Date(b.appointment_date).getTime()
    );
  }

  getAppointmentsByStatus(status: string): Appointment[] {
    return this.appointments.filter(appointment => appointment.status === status);
  }

  getTodayAppointments(): Appointment[] {
    const today = new Date().toISOString().split('T')[0];
    return this.appointments.filter(appointment => 
      appointment.appointment_date.startsWith(today)
    );
  }

  openNewAppointmentModal() {
    this.isEditing = false;
    this.appointmentForm.reset();
    this.openModal();
  }

  editAppointment(appointment: Appointment) {
    this.isEditing = true;
    this.appointmentForm.patchValue({
      patient_id: appointment.patient_id,
      service_id: appointment.service_id,
      staff_id: appointment.staff_id,
      appointment_date: appointment.appointment_date.substring(0, 16),
      notes: appointment.notes
    });
    this.openModal();
  }

  viewAppointment(appointment: Appointment) {
    console.log('Viewing appointment:', appointment);
  }

  confirmAppointment(appointment: Appointment) {
    this.updateAppointmentStatus(appointment.id!, 'confirmed');
  }

  completeAppointment(appointment: Appointment) {
    this.updateAppointmentStatus(appointment.id!, 'completed');
  }

  cancelAppointment(appointment: Appointment) {
    this.updateAppointmentStatus(appointment.id!, 'cancelled');
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
      ? this.apiService.put<Appointment>(`/appointments/${appointmentData.id}`, appointmentData)
      : this.apiService.post<Appointment>('/appointments', appointmentData);

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

  openModal() {
    this.appointmentModal = new bootstrap.Modal(document.getElementById('appointmentModal'));
    this.appointmentModal.show();
  }

  closeModal() {
    if (this.appointmentModal) {
      this.appointmentModal.hide();
    }
  }

  viewTodaySchedule() {
    this.selectedDate = new Date().toISOString().split('T')[0];
    this.filterByDate();
  }

  viewWeekSchedule() {
    console.log('Viewing week schedule...');
  }

  exportSchedule() {
    console.log('Exporting schedule...');
  }

  isStaffAvailable(staff: Staff): boolean {
    const todayAppointments = this.getTodayAppointments();
    const currentHour = new Date().getHours();
    
    const staffAppointments = todayAppointments.filter(appointment => 
      appointment.staff_id === staff.id &&
      new Date(appointment.appointment_date).getHours() === currentHour
    );
    
    return staffAppointments.length === 0;
  }

  formatTime(dateString: string): string {
    return new Date(dateString).toLocaleTimeString('pt-BR', {
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

  getStatusBadgeClass(status: string): string {
    const statusClasses: { [key: string]: string } = {
      'scheduled': 'bg-warning',
      'confirmed': 'bg-success',
      'completed': 'bg-primary',
      'cancelled': 'bg-danger'
    };
    return statusClasses[status] || 'bg-secondary';
  }
}