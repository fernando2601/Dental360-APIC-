import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-agenda',
  template: `
    <div class="container-fluid p-4">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <h2><i class="fas fa-calendar me-2"></i>Agenda</h2>
        <div class="d-flex gap-2">
          <button class="btn btn-outline-primary">
            <i class="fas fa-calendar-day me-2"></i>Hoje
          </button>
          <button class="btn btn-outline-primary">
            <i class="fas fa-calendar-week me-2"></i>Semana
          </button>
          <button class="btn btn-outline-primary">
            <i class="fas fa-calendar-alt me-2"></i>Mês
          </button>
          <button class="btn btn-primary">
            <i class="fas fa-plus me-2"></i>Novo Agendamento
          </button>
        </div>
      </div>

      <div class="row">
        <div class="col-md-8">
          <div class="card border-0 shadow-sm">
            <div class="card-header bg-white d-flex justify-content-between align-items-center">
              <h5 class="mb-0">{{currentDate | date:'MMMM yyyy'}}</h5>
              <div class="btn-group" role="group">
                <button class="btn btn-sm btn-outline-secondary" (click)="previousMonth()">
                  <i class="fas fa-chevron-left"></i>
                </button>
                <button class="btn btn-sm btn-outline-secondary" (click)="nextMonth()">
                  <i class="fas fa-chevron-right"></i>
                </button>
              </div>
            </div>
            <div class="card-body p-0">
              <div class="calendar-grid">
                <div class="calendar-header">
                  <div class="calendar-day-header">Dom</div>
                  <div class="calendar-day-header">Seg</div>
                  <div class="calendar-day-header">Ter</div>
                  <div class="calendar-day-header">Qua</div>
                  <div class="calendar-day-header">Qui</div>
                  <div class="calendar-day-header">Sex</div>
                  <div class="calendar-day-header">Sáb</div>
                </div>
                <div class="calendar-body">
                  <div *ngFor="let week of calendarWeeks" class="calendar-week">
                    <div *ngFor="let day of week" 
                         class="calendar-day"
                         [class.other-month]="!day.currentMonth"
                         [class.today]="day.isToday"
                         [class.selected]="day.isSelected"
                         (click)="selectDay(day)">
                      <div class="day-number">{{day.date}}</div>
                      <div class="appointments-count" *ngIf="day.appointmentsCount > 0">
                        <small class="badge bg-primary">{{day.appointmentsCount}}</small>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="col-md-4">
          <div class="card border-0 shadow-sm mb-4">
            <div class="card-header bg-white">
              <h6 class="mb-0">Agendamentos de Hoje</h6>
            </div>
            <div class="card-body">
              <div *ngIf="todayAppointments.length === 0" class="text-center py-3 text-muted">
                <i class="fas fa-calendar-times fa-2x mb-2"></i>
                <p class="mb-0">Nenhum agendamento para hoje</p>
              </div>
              <div *ngFor="let appointment of todayAppointments" class="appointment-item mb-3 p-2 border rounded">
                <div class="d-flex justify-content-between align-items-start">
                  <div>
                    <h6 class="mb-1">{{appointment.clientName}}</h6>
                    <small class="text-muted">{{appointment.serviceName}}</small>
                    <br>
                    <small class="text-primary">
                      <i class="fas fa-clock me-1"></i>{{appointment.time}}
                    </small>
                  </div>
                  <span class="badge" [ngClass]="getStatusClass(appointment.status)">
                    {{appointment.status}}
                  </span>
                </div>
              </div>
            </div>
          </div>

          <div class="card border-0 shadow-sm">
            <div class="card-header bg-white">
              <h6 class="mb-0">Horários Disponíveis</h6>
            </div>
            <div class="card-body">
              <div class="row">
                <div *ngFor="let slot of availableSlots" class="col-6 mb-2">
                  <button class="btn btn-sm btn-outline-primary w-100">
                    {{slot}}
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <style>
      .calendar-grid {
        font-size: 14px;
      }
      .calendar-header {
        display: grid;
        grid-template-columns: repeat(7, 1fr);
        background-color: #f8f9fa;
      }
      .calendar-day-header {
        text-align: center;
        padding: 10px;
        font-weight: 600;
        border: 1px solid #dee2e6;
      }
      .calendar-body {
        display: grid;
        grid-template-rows: repeat(6, 1fr);
      }
      .calendar-week {
        display: grid;
        grid-template-columns: repeat(7, 1fr);
      }
      .calendar-day {
        min-height: 80px;
        border: 1px solid #dee2e6;
        padding: 5px;
        cursor: pointer;
        position: relative;
      }
      .calendar-day:hover {
        background-color: #f8f9fa;
      }
      .calendar-day.other-month {
        color: #ccc;
        background-color: #fafafa;
      }
      .calendar-day.today {
        background-color: #e3f2fd;
        border-color: #2196f3;
      }
      .calendar-day.selected {
        background-color: #bbdefb;
      }
      .day-number {
        font-weight: 600;
      }
      .appointments-count {
        position: absolute;
        top: 2px;
        right: 2px;
      }
      .appointment-item {
        transition: all 0.2s;
      }
      .appointment-item:hover {
        background-color: #f8f9fa;
      }
    </style>
  `
})
export class AgendaComponent implements OnInit {
  currentDate = new Date();
  calendarWeeks: any[][] = [];
  todayAppointments: any[] = [];
  availableSlots: string[] = [];
  selectedDay: any = null;

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.generateCalendar();
    this.loadTodayAppointments();
    this.loadAvailableSlots();
  }

  generateCalendar() {
    const year = this.currentDate.getFullYear();
    const month = this.currentDate.getMonth();
    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);
    const startDate = new Date(firstDay);
    startDate.setDate(startDate.getDate() - firstDay.getDay());

    this.calendarWeeks = [];
    let currentWeek = [];

    for (let i = 0; i < 42; i++) {
      const date = new Date(startDate);
      date.setDate(startDate.getDate() + i);
      
      const dayObj = {
        date: date.getDate(),
        fullDate: new Date(date),
        currentMonth: date.getMonth() === month,
        isToday: this.isToday(date),
        isSelected: false,
        appointmentsCount: this.getAppointmentsCount(date)
      };

      currentWeek.push(dayObj);

      if (currentWeek.length === 7) {
        this.calendarWeeks.push(currentWeek);
        currentWeek = [];
      }
    }
  }

  isToday(date: Date): boolean {
    const today = new Date();
    return date.toDateString() === today.toDateString();
  }

  getAppointmentsCount(date: Date): number {
    // This would normally come from the API
    return Math.floor(Math.random() * 5);
  }

  selectDay(day: any) {
    if (this.selectedDay) {
      this.selectedDay.isSelected = false;
    }
    day.isSelected = true;
    this.selectedDay = day;
    this.loadDayAppointments(day.fullDate);
  }

  previousMonth() {
    this.currentDate.setMonth(this.currentDate.getMonth() - 1);
    this.generateCalendar();
  }

  nextMonth() {
    this.currentDate.setMonth(this.currentDate.getMonth() + 1);
    this.generateCalendar();
  }

  loadTodayAppointments() {
    this.apiService.get('/api/agenda/today').subscribe({
      next: (data: any) => {
        this.todayAppointments = data;
      },
      error: (error) => {
        console.error('Erro ao carregar agendamentos:', error);
      }
    });
  }

  loadDayAppointments(date: Date) {
    const dateStr = date.toISOString().split('T')[0];
    this.apiService.get(`/api/agenda/day/${dateStr}`).subscribe({
      next: (data: any) => {
        // Update selected day appointments
      },
      error: (error) => {
        console.error('Erro ao carregar agendamentos do dia:', error);
      }
    });
  }

  loadAvailableSlots() {
    this.availableSlots = [
      '08:00', '08:30', '09:00', '09:30', '10:00', '10:30',
      '11:00', '11:30', '14:00', '14:30', '15:00', '15:30',
      '16:00', '16:30', '17:00', '17:30'
    ];
  }

  getStatusClass(status: string): string {
    const statusClasses: any = {
      'Confirmado': 'bg-success',
      'Pendente': 'bg-warning',
      'Cancelado': 'bg-danger',
      'Concluído': 'bg-info'
    };
    return statusClasses[status] || 'bg-secondary';
  }
}