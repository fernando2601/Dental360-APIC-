import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-dashboard',
  template: `
    <div class="dashboard">
      <h1 class="page-title">Dashboard - DentalSpa</h1>
      
      <div class="stats-grid">
        <div class="stat-card patients">
          <div class="stat-icon">👥</div>
          <div class="stat-content">
            <div class="stat-number">{{ stats.totalPatients }}</div>
            <div class="stat-label">Total de Pacientes</div>
          </div>
        </div>
        
        <div class="stat-card appointments">
          <div class="stat-icon">📅</div>
          <div class="stat-content">
            <div class="stat-number">{{ stats.todayAppointments }}</div>
            <div class="stat-label">Consultas Hoje</div>
          </div>
        </div>
        
        <div class="stat-card revenue">
          <div class="stat-icon">💰</div>
          <div class="stat-content">
            <div class="stat-number">R$ {{ stats.monthlyRevenue | number:'1.2-2' }}</div>
            <div class="stat-label">Receita Mensal</div>
          </div>
        </div>
        
        <div class="stat-card pending">
          <div class="stat-icon">⏰</div>
          <div class="stat-content">
            <div class="stat-number">{{ stats.pendingAppointments }}</div>
            <div class="stat-label">Pendentes</div>
          </div>
        </div>
      </div>

      <div class="dashboard-grid">
        <div class="card recent-appointments">
          <h3>Próximas Consultas</h3>
          <div class="appointment-list">
            <div class="appointment-item" *ngFor="let appointment of recentAppointments">
              <div class="appointment-time">{{ appointment.appointment_date | date:'HH:mm' }}</div>
              <div class="appointment-info">
                <div class="patient-name">{{ appointment.patient_name }}</div>
                <div class="service-name">{{ appointment.service_name }}</div>
              </div>
              <div class="appointment-status" [class]="appointment.status">{{ getStatusLabel(appointment.status) }}</div>
            </div>
          </div>
        </div>

        <div class="card quick-actions">
          <h3>Ações Rápidas</h3>
          <div class="action-buttons">
            <button class="action-btn primary" routerLink="/appointments">
              <span class="icon">📅</span>
              Novo Agendamento
            </button>
            <button class="action-btn secondary" routerLink="/patients">
              <span class="icon">👤</span>
              Adicionar Paciente
            </button>
            <button class="action-btn secondary" routerLink="/agenda">
              <span class="icon">🗓️</span>
              Ver Agenda
            </button>
            <button class="action-btn secondary" routerLink="/financial">
              <span class="icon">💰</span>
              Financeiro
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dashboard { padding: 2rem; }
    .page-title { font-size: 2rem; font-weight: 700; margin-bottom: 2rem; color: #1f2937; }
    
    .stats-grid { 
      display: grid; 
      grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); 
      gap: 1.5rem; 
      margin-bottom: 3rem; 
    }
    
    .stat-card {
      background: white;
      padding: 1.5rem;
      border-radius: 12px;
      box-shadow: 0 1px 3px rgba(0,0,0,0.1);
      display: flex;
      align-items: center;
      gap: 1rem;
      border-left: 4px solid;
    }
    
    .stat-card.patients { border-left-color: #3b82f6; }
    .stat-card.appointments { border-left-color: #10b981; }
    .stat-card.revenue { border-left-color: #f59e0b; }
    .stat-card.pending { border-left-color: #ef4444; }
    
    .stat-icon { font-size: 2.5rem; }
    .stat-number { font-size: 2rem; font-weight: 700; color: #1f2937; }
    .stat-label { color: #6b7280; font-weight: 500; }
    
    .dashboard-grid {
      display: grid;
      grid-template-columns: 2fr 1fr;
      gap: 2rem;
    }
    
    .card {
      background: white;
      border-radius: 12px;
      padding: 1.5rem;
      box-shadow: 0 1px 3px rgba(0,0,0,0.1);
    }
    
    .card h3 { margin: 0 0 1rem 0; font-weight: 600; color: #1f2937; }
    
    .appointment-item {
      display: flex;
      align-items: center;
      gap: 1rem;
      padding: 1rem;
      border-bottom: 1px solid #f3f4f6;
    }
    
    .appointment-time {
      font-weight: 600;
      color: #3b82f6;
      min-width: 60px;
    }
    
    .appointment-info { flex: 1; }
    .patient-name { font-weight: 600; color: #1f2937; }
    .service-name { color: #6b7280; font-size: 0.875rem; }
    
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
    
    .action-buttons {
      display: grid;
      gap: 0.75rem;
    }
    
    .action-btn {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding: 1rem;
      border: none;
      border-radius: 8px;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.2s;
      text-decoration: none;
    }
    
    .action-btn.primary {
      background: #3b82f6;
      color: white;
    }
    
    .action-btn.secondary {
      background: #f9fafb;
      color: #374151;
      border: 1px solid #e5e7eb;
    }
    
    .action-btn:hover {
      transform: translateY(-1px);
      box-shadow: 0 4px 6px rgba(0,0,0,0.1);
    }
    
    @media (max-width: 768px) {
      .dashboard-grid { grid-template-columns: 1fr; }
      .stats-grid { grid-template-columns: 1fr; }
    }
  `]
})
export class DashboardComponent implements OnInit {
  stats = {
    totalPatients: 0,
    todayAppointments: 0,
    monthlyRevenue: 0,
    pendingAppointments: 0
  };
  
  recentAppointments: any[] = [];

  constructor(private apiService: ApiService) { }

  ngOnInit() {
    this.loadDashboardData();
  }

  loadDashboardData() {
    this.apiService.get('/dashboard/stats').subscribe({
      next: (data) => this.stats = data,
      error: () => {
        // Dados de demonstração para desenvolvimento
        this.stats = {
          totalPatients: 156,
          todayAppointments: 8,
          monthlyRevenue: 15750.00,
          pendingAppointments: 3
        };
      }
    });

    this.apiService.get('/appointments/today').subscribe({
      next: (data) => this.recentAppointments = data,
      error: () => {
        // Dados de demonstração
        this.recentAppointments = [
          { appointment_date: new Date(), patient_name: 'Maria Silva', service_name: 'Limpeza', status: 'confirmed' },
          { appointment_date: new Date(), patient_name: 'João Santos', service_name: 'Consulta', status: 'scheduled' }
        ];
      }
    });
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