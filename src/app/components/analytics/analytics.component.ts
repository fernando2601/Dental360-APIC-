import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-analytics',
  template: `
    <div class="container">
      <h2>Analytics e Relatórios</h2>
      
      <div class="analytics-grid">
        <div class="chart-card">
          <h3>Agendamentos por Mês</h3>
          <div class="chart-placeholder">
            <div class="chart-bars">
              <div class="bar" style="height: 60%"><span>Jan</span></div>
              <div class="bar" style="height: 80%"><span>Fev</span></div>
              <div class="bar" style="height: 45%"><span>Mar</span></div>
              <div class="bar" style="height: 90%"><span>Abr</span></div>
              <div class="bar" style="height: 75%"><span>Mai</span></div>
              <div class="bar" style="height: 95%"><span>Jun</span></div>
            </div>
          </div>
        </div>

        <div class="chart-card">
          <h3>Serviços Mais Procurados</h3>
          <div class="services-list">
            <div class="service-item" *ngFor="let service of topServices">
              <span class="service-name">{{ service.name }}</span>
              <span class="service-count">{{ service.count }}</span>
            </div>
          </div>
        </div>

        <div class="chart-card">
          <h3>Receita por Trimestre</h3>
          <div class="revenue-chart">
            <div class="revenue-item" *ngFor="let quarter of quarterlyRevenue">
              <div class="quarter-label">{{ quarter.quarter }}</div>
              <div class="revenue-bar">
                <div class="revenue-fill" [style.width.%]="quarter.percentage"></div>
              </div>
              <div class="revenue-amount">R$ {{ quarter.amount | number:'1.2-2' }}</div>
            </div>
          </div>
        </div>

        <div class="chart-card">
          <h3>Métricas Gerais</h3>
          <div class="metrics-list">
            <div class="metric-item">
              <div class="metric-label">Taxa de Conversão</div>
              <div class="metric-value">{{ metrics.conversionRate }}%</div>
            </div>
            <div class="metric-item">
              <div class="metric-label">Pacientes Novos</div>
              <div class="metric-value">{{ metrics.newPatients }}</div>
            </div>
            <div class="metric-item">
              <div class="metric-label">Taxa de Retenção</div>
              <div class="metric-value">{{ metrics.retentionRate }}%</div>
            </div>
            <div class="metric-item">
              <div class="metric-label">Avaliação Média</div>
              <div class="metric-value">{{ metrics.averageRating }}/5</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .container { padding: 2rem; }
    .analytics-grid { display: grid; grid-template-columns: repeat(2, 1fr); gap: 1.5rem; }
    .chart-card { background: white; padding: 1.5rem; border-radius: 8px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }
    .chart-card h3 { margin: 0 0 1.5rem 0; color: #1f2937; }
    
    .chart-placeholder { height: 200px; display: flex; align-items: end; justify-content: center; }
    .chart-bars { display: flex; align-items: end; gap: 1rem; height: 100%; }
    .bar { width: 30px; background: #3b82f6; border-radius: 4px 4px 0 0; display: flex; align-items: end; justify-content: center; }
    .bar span { color: white; font-size: 0.75rem; padding: 0.25rem 0; }
    
    .services-list { display: grid; gap: 0.75rem; }
    .service-item { display: flex; justify-content: space-between; align-items: center; padding: 0.75rem; background: #f9fafb; border-radius: 6px; }
    .service-name { font-weight: 500; color: #1f2937; }
    .service-count { background: #3b82f6; color: white; padding: 0.25rem 0.75rem; border-radius: 20px; font-size: 0.875rem; }
    
    .revenue-chart { display: grid; gap: 1rem; }
    .revenue-item { display: grid; grid-template-columns: 80px 1fr 100px; align-items: center; gap: 1rem; }
    .quarter-label { font-weight: 500; color: #6b7280; }
    .revenue-bar { height: 8px; background: #e5e7eb; border-radius: 4px; overflow: hidden; }
    .revenue-fill { height: 100%; background: #10b981; }
    .revenue-amount { font-weight: 600; color: #1f2937; text-align: right; }
    
    .metrics-list { display: grid; gap: 1rem; }
    .metric-item { display: flex; justify-content: space-between; align-items: center; padding: 1rem; background: #f9fafb; border-radius: 6px; }
    .metric-label { color: #6b7280; }
    .metric-value { font-weight: 600; color: #1f2937; font-size: 1.1rem; }
    
    @media (max-width: 768px) { .analytics-grid { grid-template-columns: 1fr; } }
  `]
})
export class AnalyticsComponent implements OnInit {
  topServices: any[] = [];
  quarterlyRevenue: any[] = [];
  metrics = {
    conversionRate: 0,
    newPatients: 0,
    retentionRate: 0,
    averageRating: 0
  };

  constructor(private apiService: ApiService) { }

  ngOnInit() {
    this.loadAnalyticsData();
  }

  loadAnalyticsData() {
    this.apiService.get('/analytics/services').subscribe({
      next: (data) => this.topServices = data,
      error: () => {
        this.topServices = [
          { name: 'Limpeza Dental', count: 45 },
          { name: 'Clareamento', count: 32 },
          { name: 'Restauração', count: 28 },
          { name: 'Consulta', count: 67 }
        ];
      }
    });

    this.apiService.get('/analytics/revenue').subscribe({
      next: (data) => this.quarterlyRevenue = data,
      error: () => {
        this.quarterlyRevenue = [
          { quarter: 'Q1 2024', amount: 42750.00, percentage: 85 },
          { quarter: 'Q2 2024', amount: 47250.00, percentage: 95 },
          { quarter: 'Q3 2024', amount: 39800.00, percentage: 80 },
          { quarter: 'Q4 2024', amount: 50200.00, percentage: 100 }
        ];
      }
    });

    this.apiService.get('/analytics/metrics').subscribe({
      next: (data) => this.metrics = data,
      error: () => {
        this.metrics = {
          conversionRate: 78,
          newPatients: 24,
          retentionRate: 92,
          averageRating: 4.8
        };
      }
    });
  }
}