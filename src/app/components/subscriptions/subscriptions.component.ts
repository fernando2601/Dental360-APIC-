import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-subscriptions',
  template: `
    <div class="container-fluid p-4">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <h2><i class="fas fa-credit-card me-2"></i>Assinaturas</h2>
        <button class="btn btn-primary">
          <i class="fas fa-plus me-2"></i>Nova Assinatura
        </button>
      </div>
      
      <div class="row mb-4">
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-users text-primary fa-2x mb-2"></i>
              <h6>Assinantes Ativos</h6>
              <h4 class="text-primary">{{stats.activeSubscriptions}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-dollar-sign text-success fa-2x mb-2"></i>
              <h6>Receita Mensal</h6>
              <h4 class="text-success">{{formatCurrency(stats.monthlyRevenue)}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-clock text-warning fa-2x mb-2"></i>
              <h6>Vencendo Hoje</h6>
              <h4 class="text-warning">{{stats.expiringToday}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-times-circle text-danger fa-2x mb-2"></i>
              <h6>Canceladas</h6>
              <h4 class="text-danger">{{stats.cancelledThisMonth}}</h4>
            </div>
          </div>
        </div>
      </div>

      <div class="row">
        <div class="col-md-8">
          <div class="card border-0 shadow-sm">
            <div class="card-header bg-white d-flex justify-content-between align-items-center">
              <h5 class="mb-0">Assinaturas Ativas</h5>
              <div class="d-flex gap-2">
                <select class="form-select" style="width: auto;">
                  <option value="">Todos os planos</option>
                  <option value="basic">Básico</option>
                  <option value="premium">Premium</option>
                  <option value="vip">VIP</option>
                </select>
              </div>
            </div>
            <div class="card-body">
              <div *ngIf="loading" class="text-center py-4">
                <div class="spinner-border" role="status">
                  <span class="visually-hidden">Carregando...</span>
                </div>
              </div>
              <div *ngIf="!loading && subscriptions.length > 0" class="table-responsive">
                <table class="table table-hover">
                  <thead>
                    <tr>
                      <th>Cliente</th>
                      <th>Plano</th>
                      <th>Valor</th>
                      <th>Próximo Vencimento</th>
                      <th>Status</th>
                      <th>Ações</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr *ngFor="let subscription of subscriptions">
                      <td>
                        <div>
                          <strong>{{subscription.clientName}}</strong>
                          <br>
                          <small class="text-muted">{{subscription.clientEmail}}</small>
                        </div>
                      </td>
                      <td>
                        <span class="badge" [ngClass]="getPlanClass(subscription.planType)">
                          {{subscription.planName}}
                        </span>
                      </td>
                      <td>{{formatCurrency(subscription.amount)}}</td>
                      <td>
                        <span [ngClass]="getExpirationClass(subscription.nextDueDate)">
                          {{formatDate(subscription.nextDueDate)}}
                        </span>
                      </td>
                      <td>
                        <span class="badge" [ngClass]="getStatusClass(subscription.status)">
                          {{subscription.status}}
                        </span>
                      </td>
                      <td>
                        <div class="btn-group" role="group">
                          <button class="btn btn-sm btn-outline-primary" title="Ver Detalhes">
                            <i class="fas fa-eye"></i>
                          </button>
                          <button class="btn btn-sm btn-outline-warning" title="Renovar">
                            <i class="fas fa-redo"></i>
                          </button>
                          <button class="btn btn-sm btn-outline-danger" title="Cancelar">
                            <i class="fas fa-times"></i>
                          </button>
                        </div>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>

        <div class="col-md-4">
          <div class="card border-0 shadow-sm mb-4">
            <div class="card-header bg-white">
              <h6 class="mb-0">Planos Disponíveis</h6>
            </div>
            <div class="card-body">
              <div *ngFor="let plan of availablePlans" class="mb-3 p-3 border rounded">
                <div class="d-flex justify-content-between align-items-center mb-2">
                  <h6 class="mb-0">{{plan.name}}</h6>
                  <strong class="text-primary">{{formatCurrency(plan.price)}}/mês</strong>
                </div>
                <ul class="list-unstyled small mb-2">
                  <li *ngFor="let feature of plan.features">
                    <i class="fas fa-check text-success me-1"></i>{{feature}}
                  </li>
                </ul>
                <small class="text-muted">{{plan.subscribersCount}} assinantes</small>
              </div>
            </div>
          </div>

          <div class="card border-0 shadow-sm">
            <div class="card-header bg-white">
              <h6 class="mb-0">Vencimentos Próximos</h6>
            </div>
            <div class="card-body">
              <div *ngFor="let expiring of expiringSubscriptions" class="d-flex justify-content-between align-items-center mb-2">
                <div>
                  <strong class="small">{{expiring.clientName}}</strong>
                  <br>
                  <small class="text-muted">{{expiring.planName}}</small>
                </div>
                <div class="text-end">
                  <small class="text-danger">{{formatDate(expiring.dueDate)}}</small>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class SubscriptionsComponent implements OnInit {
  stats = { 
    activeSubscriptions: 0, 
    monthlyRevenue: 0, 
    expiringToday: 0, 
    cancelledThisMonth: 0 
  };
  subscriptions: any[] = [];
  availablePlans: any[] = [];
  expiringSubscriptions: any[] = [];
  loading = true;

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadSubscriptionsData();
  }

  loadSubscriptionsData() {
    this.loadSubscriptions();
    this.loadStats();
    this.loadAvailablePlans();
    this.loadExpiringSubscriptions();
  }

  loadSubscriptions() {
    this.apiService.get('/api/subscriptions').subscribe({
      next: (data: any) => {
        this.subscriptions = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar assinaturas:', error);
        this.loading = false;
      }
    });
  }

  loadStats() {
    this.apiService.get('/api/subscriptions/stats').subscribe({
      next: (data: any) => {
        this.stats = data;
      },
      error: (error) => {
        console.error('Erro ao carregar estatísticas:', error);
      }
    });
  }

  loadAvailablePlans() {
    this.apiService.get('/api/subscriptions/plans').subscribe({
      next: (data: any) => {
        this.availablePlans = data;
      },
      error: (error) => {
        console.error('Erro ao carregar planos:', error);
      }
    });
  }

  loadExpiringSubscriptions() {
    this.apiService.get('/api/subscriptions/expiring').subscribe({
      next: (data: any) => {
        this.expiringSubscriptions = data;
      },
      error: (error) => {
        console.error('Erro ao carregar vencimentos:', error);
      }
    });
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('pt-BR');
  }

  getPlanClass(planType: string): string {
    const planClasses: any = {
      'basic': 'bg-secondary',
      'premium': 'bg-primary',
      'vip': 'bg-warning'
    };
    return planClasses[planType] || 'bg-secondary';
  }

  getStatusClass(status: string): string {
    const statusClasses: any = {
      'Ativa': 'bg-success',
      'Vencida': 'bg-danger',
      'Cancelada': 'bg-secondary',
      'Suspensa': 'bg-warning'
    };
    return statusClasses[status] || 'bg-secondary';
  }

  getExpirationClass(dueDate: string): string {
    const today = new Date();
    const due = new Date(dueDate);
    const diffDays = Math.ceil((due.getTime() - today.getTime()) / (1000 * 3600 * 24));
    
    if (diffDays <= 0) return 'text-danger fw-bold';
    if (diffDays <= 7) return 'text-warning fw-bold';
    return 'text-muted';
  }
}