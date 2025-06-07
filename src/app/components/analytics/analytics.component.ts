import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-analytics',
  template: `
    <div class="container-fluid p-4">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <h2><i class="fas fa-chart-bar me-2"></i>Analytics</h2>
        <div class="d-flex gap-2">
          <select class="form-select" style="width: auto;">
            <option value="7">Últimos 7 dias</option>
            <option value="30">Últimos 30 dias</option>
            <option value="90">Últimos 3 meses</option>
            <option value="365">Último ano</option>
          </select>
          <button class="btn btn-primary">
            <i class="fas fa-download me-2"></i>Exportar
          </button>
        </div>
      </div>
      
      <div class="row mb-4">
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-users text-primary fa-2x mb-2"></i>
              <h6>Novos Clientes</h6>
              <h4 class="text-primary">{{metrics.newClients}}</h4>
              <small class="text-success">
                <i class="fas fa-arrow-up"></i> +{{metrics.newClientsGrowth}}%
              </small>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-calendar-check text-success fa-2x mb-2"></i>
              <h6>Agendamentos</h6>
              <h4 class="text-success">{{metrics.appointments}}</h4>
              <small class="text-success">
                <i class="fas fa-arrow-up"></i> +{{metrics.appointmentsGrowth}}%
              </small>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-dollar-sign text-info fa-2x mb-2"></i>
              <h6>Faturamento</h6>
              <h4 class="text-info">{{formatCurrency(metrics.revenue)}}</h4>
              <small class="text-success">
                <i class="fas fa-arrow-up"></i> +{{metrics.revenueGrowth}}%
              </small>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-percentage text-warning fa-2x mb-2"></i>
              <h6>Taxa de Conversão</h6>
              <h4 class="text-warning">{{metrics.conversionRate}}%</h4>
              <small class="text-danger">
                <i class="fas fa-arrow-down"></i> {{metrics.conversionRateChange}}%
              </small>
            </div>
          </div>
        </div>
      </div>

      <div class="row mb-4">
        <div class="col-md-8">
          <div class="card border-0 shadow-sm">
            <div class="card-header bg-white">
              <h5 class="mb-0">Receita por Período</h5>
            </div>
            <div class="card-body">
              <div class="chart-container" style="height: 300px;">
                <canvas id="revenueChart"></canvas>
              </div>
            </div>
          </div>
        </div>
        <div class="col-md-4">
          <div class="card border-0 shadow-sm">
            <div class="card-header bg-white">
              <h5 class="mb-0">Serviços Mais Procurados</h5>
            </div>
            <div class="card-body">
              <div *ngFor="let service of topServices" class="d-flex justify-content-between align-items-center mb-3">
                <div>
                  <h6 class="mb-0">{{service.name}}</h6>
                  <small class="text-muted">{{service.appointments}} agendamentos</small>
                </div>
                <div class="text-end">
                  <strong>{{formatCurrency(service.revenue)}}</strong>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="row">
        <div class="col-md-6">
          <div class="card border-0 shadow-sm">
            <div class="card-header bg-white">
              <h5 class="mb-0">Origem dos Clientes</h5>
            </div>
            <div class="card-body">
              <div *ngFor="let source of clientSources" class="d-flex justify-content-between align-items-center mb-2">
                <span>{{source.name}}</span>
                <div class="d-flex align-items-center">
                  <div class="progress me-2" style="width: 100px; height: 10px;">
                    <div class="progress-bar" [style.width.%]="source.percentage"></div>
                  </div>
                  <span class="small">{{source.percentage}}%</span>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="col-md-6">
          <div class="card border-0 shadow-sm">
            <div class="card-header bg-white">
              <h5 class="mb-0">Performance da Equipe</h5>
            </div>
            <div class="card-body">
              <div *ngFor="let staff of staffPerformance" class="d-flex justify-content-between align-items-center mb-3">
                <div class="d-flex align-items-center">
                  <div class="avatar-sm bg-primary rounded-circle d-flex align-items-center justify-content-center me-3">
                    <i class="fas fa-user text-white"></i>
                  </div>
                  <div>
                    <h6 class="mb-0">{{staff.name}}</h6>
                    <small class="text-muted">{{staff.role}}</small>
                  </div>
                </div>
                <div class="text-end">
                  <strong>{{staff.appointments}}</strong>
                  <br>
                  <small class="text-muted">atendimentos</small>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class AnalyticsComponent implements OnInit {
  metrics = {
    newClients: 0,
    newClientsGrowth: 0,
    appointments: 0,
    appointmentsGrowth: 0,
    revenue: 0,
    revenueGrowth: 0,
    conversionRate: 0,
    conversionRateChange: 0
  };
  
  topServices: any[] = [];
  clientSources: any[] = [];
  staffPerformance: any[] = [];

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadAnalytics();
  }

  loadAnalytics() {
    this.loadMetrics();
    this.loadTopServices();
    this.loadClientSources();
    this.loadStaffPerformance();
  }

  loadMetrics() {
    this.apiService.get('/api/analytics/metrics').subscribe({
      next: (data: any) => {
        this.metrics = data;
      },
      error: (error) => {
        console.error('Erro ao carregar métricas:', error);
      }
    });
  }

  loadTopServices() {
    this.apiService.get('/api/analytics/top-services').subscribe({
      next: (data: any) => {
        this.topServices = data;
      },
      error: (error) => {
        console.error('Erro ao carregar serviços:', error);
      }
    });
  }

  loadClientSources() {
    this.apiService.get('/api/analytics/client-sources').subscribe({
      next: (data: any) => {
        this.clientSources = data;
      },
      error: (error) => {
        console.error('Erro ao carregar origens:', error);
      }
    });
  }

  loadStaffPerformance() {
    this.apiService.get('/api/analytics/staff-performance').subscribe({
      next: (data: any) => {
        this.staffPerformance = data;
      },
      error: (error) => {
        console.error('Erro ao carregar performance:', error);
      }
    });
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }
}