import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-appointments',
  template: `
    <div class="container-fluid p-4">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <h2><i class="fas fa-calendar-alt me-2"></i>Agendamentos</h2>
        <button class="btn btn-primary">
          <i class="fas fa-plus me-2"></i>Novo Agendamento
        </button>
      </div>
      
      <div class="row">
        <div class="col-md-4 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-clock text-warning fa-2x mb-2"></i>
              <h5>Pendentes</h5>
              <h3 class="text-warning">{{stats.pending}}</h3>
            </div>
          </div>
        </div>
        <div class="col-md-4 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-check-circle text-success fa-2x mb-2"></i>
              <h5>Confirmados</h5>
              <h3 class="text-success">{{stats.confirmed}}</h3>
            </div>
          </div>
        </div>
        <div class="col-md-4 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-calendar-day text-info fa-2x mb-2"></i>
              <h5>Hoje</h5>
              <h3 class="text-info">{{stats.today}}</h3>
            </div>
          </div>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-white">
          <h5 class="mb-0">Próximos Agendamentos</h5>
        </div>
        <div class="card-body">
          <div *ngIf="loading" class="text-center py-4">
            <div class="spinner-border" role="status">
              <span class="visually-hidden">Carregando...</span>
            </div>
          </div>
          <div *ngIf="!loading && appointments.length === 0" class="text-center py-4 text-muted">
            <i class="fas fa-calendar-times fa-3x mb-3"></i>
            <p>Nenhum agendamento encontrado</p>
          </div>
          <div *ngIf="!loading && appointments.length > 0" class="table-responsive">
            <table class="table table-hover">
              <thead>
                <tr>
                  <th>Cliente</th>
                  <th>Serviço</th>
                  <th>Data/Hora</th>
                  <th>Status</th>
                  <th>Ações</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let appointment of appointments">
                  <td>{{appointment.clientName}}</td>
                  <td>{{appointment.serviceName}}</td>
                  <td>{{formatDateTime(appointment.dateTime)}}</td>
                  <td>
                    <span class="badge" [ngClass]="getStatusClass(appointment.status)">
                      {{appointment.status}}
                    </span>
                  </td>
                  <td>
                    <button class="btn btn-sm btn-outline-primary me-1">
                      <i class="fas fa-edit"></i>
                    </button>
                    <button class="btn btn-sm btn-outline-danger">
                      <i class="fas fa-trash"></i>
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  `
})
export class AppointmentsComponent implements OnInit {
  stats = { pending: 0, confirmed: 0, today: 0 };
  appointments: any[] = [];
  loading = true;

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadAppointments();
    this.loadStats();
  }

  loadAppointments() {
    this.apiService.get('/api/appointments').subscribe({
      next: (data) => {
        this.appointments = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar agendamentos:', error);
        this.loading = false;
      }
    });
  }

  loadStats() {
    this.apiService.get('/api/appointments/stats').subscribe({
      next: (data) => {
        this.stats = data;
      },
      error: (error) => {
        console.error('Erro ao carregar estatísticas:', error);
      }
    });
  }

  formatDateTime(dateTime: string): string {
    return new Date(dateTime).toLocaleString('pt-BR');
  }

  getStatusClass(status: string): string {
    const statusClasses: any = {
      'Pendente': 'bg-warning',
      'Confirmado': 'bg-success',
      'Cancelado': 'bg-danger',
      'Concluído': 'bg-info'
    };
    return statusClasses[status] || 'bg-secondary';
  }
}