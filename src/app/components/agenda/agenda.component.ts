import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-agenda',
  template: `
    <div class="container">
      <h2>Agenda da Clínica</h2>
      <div class="calendar-container">
        <div class="calendar-header">
          <button class="btn btn-outline" (click)="previousWeek()">← Semana Anterior</button>
          <h3>{{ currentWeekStart | date:'dd/MM/yyyy' }} - {{ currentWeekEnd | date:'dd/MM/yyyy' }}</h3>
          <button class="btn btn-outline" (click)="nextWeek()">Próxima Semana →</button>
        </div>
        
        <div class="calendar-grid">
          <div class="time-column">
            <div class="time-slot" *ngFor="let hour of workingHours">{{ hour }}:00</div>
          </div>
          
          <div class="day-column" *ngFor="let day of weekDays">
            <div class="day-header">{{ day.name }} - {{ day.date | date:'dd/MM' }}</div>
            <div class="appointment-slot" 
                 *ngFor="let hour of workingHours"
                 [class.has-appointment]="hasAppointment(day.date, hour)"
                 (click)="openNewAppointment(day.date, hour)">
              <div class="appointment" *ngFor="let apt of getAppointmentsForSlot(day.date, hour)">
                <div class="appointment-time">{{ apt.time }}</div>
                <div class="appointment-patient">{{ apt.patient_name }}</div>
                <div class="appointment-service">{{ apt.service_name }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <div class="appointments-summary">
        <h3>Resumo da Semana</h3>
        <div class="stats-grid">
          <div class="stat-card">
            <div class="stat-number">{{ weekAppointments.length }}</div>
            <div class="stat-label">Total de Consultas</div>
          </div>
          <div class="stat-card">
            <div class="stat-number">{{ getConfirmedCount() }}</div>
            <div class="stat-label">Confirmadas</div>
          </div>
          <div class="stat-card">
            <div class="stat-number">{{ getPendingCount() }}</div>
            <div class="stat-label">Pendentes</div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .container { padding: 2rem; }
    .calendar-container { background: white; border-radius: 8px; padding: 1.5rem; margin-bottom: 2rem; }
    .calendar-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 2rem; }
    .calendar-grid { display: grid; grid-template-columns: 80px repeat(7, 1fr); gap: 1px; background: #e5e7eb; }
    .time-column, .day-column { background: white; }
    .time-slot, .appointment-slot { height: 60px; padding: 0.5rem; border-bottom: 1px solid #f3f4f6; }
    .day-header { background: #f9fafb; padding: 1rem; font-weight: 600; text-align: center; }
    .appointment-slot { cursor: pointer; transition: background 0.2s; }
    .appointment-slot:hover { background: #f3f4f6; }
    .appointment-slot.has-appointment { background: #dbeafe; }
    .appointment { background: #3b82f6; color: white; padding: 0.25rem; border-radius: 4px; margin-bottom: 0.25rem; font-size: 0.75rem; }
    .stats-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 1rem; }
    .stat-card { text-align: center; padding: 1rem; background: #f9fafb; border-radius: 8px; }
    .stat-number { font-size: 2rem; font-weight: bold; color: #3b82f6; }
    .btn { padding: 0.5rem 1rem; border: 1px solid #d1d5db; background: white; border-radius: 4px; cursor: pointer; }
    .btn:hover { background: #f9fafb; }
  `]
})
export class AgendaComponent implements OnInit {
  currentWeekStart: Date = new Date();
  currentWeekEnd: Date = new Date();
  weekDays: any[] = [];
  workingHours = [8, 9, 10, 11, 14, 15, 16, 17, 18];
  weekAppointments: any[] = [];

  constructor(private apiService: ApiService) { }

  ngOnInit() {
    this.setCurrentWeek();
    this.loadWeekAppointments();
  }

  setCurrentWeek() {
    const today = new Date();
    const dayOfWeek = today.getDay();
    const startOfWeek = new Date(today);
    startOfWeek.setDate(today.getDate() - dayOfWeek + 1);
    
    this.currentWeekStart = startOfWeek;
    this.currentWeekEnd = new Date(startOfWeek);
    this.currentWeekEnd.setDate(startOfWeek.getDate() + 6);
    
    this.weekDays = [];
    for (let i = 0; i < 7; i++) {
      const date = new Date(startOfWeek);
      date.setDate(startOfWeek.getDate() + i);
      this.weekDays.push({
        name: this.getDayName(i),
        date: date
      });
    }
  }

  getDayName(index: number): string {
    const days = ['Segunda', 'Terça', 'Quarta', 'Quinta', 'Sexta', 'Sábado', 'Domingo'];
    return days[index];
  }

  loadWeekAppointments() {
    this.apiService.get('/appointments').subscribe({
      next: (appointments: any[]) => {
        this.weekAppointments = appointments.filter(apt => {
          const aptDate = new Date(apt.appointment_date);
          return aptDate >= this.currentWeekStart && aptDate <= this.currentWeekEnd;
        });
      },
      error: (error) => console.error('Erro ao carregar agendamentos:', error)
    });
  }

  previousWeek() {
    this.currentWeekStart.setDate(this.currentWeekStart.getDate() - 7);
    this.currentWeekEnd.setDate(this.currentWeekEnd.getDate() - 7);
    this.setCurrentWeek();
    this.loadWeekAppointments();
  }

  nextWeek() {
    this.currentWeekStart.setDate(this.currentWeekStart.getDate() + 7);
    this.currentWeekEnd.setDate(this.currentWeekEnd.getDate() + 7);
    this.setCurrentWeek();
    this.loadWeekAppointments();
  }

  hasAppointment(date: Date, hour: number): boolean {
    return this.getAppointmentsForSlot(date, hour).length > 0;
  }

  getAppointmentsForSlot(date: Date, hour: number): any[] {
    return this.weekAppointments.filter(apt => {
      const aptDate = new Date(apt.appointment_date);
      return aptDate.toDateString() === date.toDateString() && aptDate.getHours() === hour;
    });
  }

  openNewAppointment(date: Date, hour: number) {
    console.log('Novo agendamento para:', date, 'às', hour);
  }

  getConfirmedCount(): number {
    return this.weekAppointments.filter(apt => apt.status === 'confirmed').length;
  }

  getPendingCount(): number {
    return this.weekAppointments.filter(apt => apt.status === 'scheduled').length;
  }
}