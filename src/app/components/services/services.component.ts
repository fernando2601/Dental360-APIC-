import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-services',
  template: `
    <div class="container-fluid p-4">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <h2><i class="fas fa-tooth me-2"></i>Serviços</h2>
        <button class="btn btn-primary">
          <i class="fas fa-plus me-2"></i>Novo Serviço
        </button>
      </div>
      
      <div class="row mb-4">
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-tooth text-primary fa-2x mb-2"></i>
              <h6>Odontologia</h6>
              <h4 class="text-primary">{{stats.dental}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-user-md text-success fa-2x mb-2"></i>
              <h6>Harmonização</h6>
              <h4 class="text-success">{{stats.harmonization}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-spa text-info fa-2x mb-2"></i>
              <h6>Estética</h6>
              <h4 class="text-info">{{stats.aesthetic}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-layer-group text-warning fa-2x mb-2"></i>
              <h6>Total</h6>
              <h4 class="text-warning">{{stats.total}}</h4>
            </div>
          </div>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-white d-flex justify-content-between align-items-center">
          <h5 class="mb-0">Catálogo de Serviços</h5>
          <div class="d-flex gap-2">
            <select class="form-select" style="width: auto;">
              <option value="">Todas as categorias</option>
              <option value="dental">Odontologia</option>
              <option value="harmonization">Harmonização</option>
              <option value="aesthetic">Estética</option>
            </select>
          </div>
        </div>
        <div class="card-body">
          <div *ngIf="loading" class="text-center py-4">
            <div class="spinner-border" role="status">
              <span class="visually-hidden">Carregando...</span>
            </div>
          </div>
          <div *ngIf="!loading && services.length === 0" class="text-center py-4 text-muted">
            <i class="fas fa-tooth fa-3x mb-3"></i>
            <p>Nenhum serviço cadastrado</p>
          </div>
          <div *ngIf="!loading && services.length > 0" class="row">
            <div *ngFor="let service of services" class="col-md-4 mb-3">
              <div class="card h-100 border-0 shadow-sm">
                <div class="card-body">
                  <div class="d-flex justify-content-between align-items-start mb-2">
                    <h6 class="card-title">{{service.name}}</h6>
                    <span class="badge" [ngClass]="getCategoryClass(service.category)">
                      {{service.category}}
                    </span>
                  </div>
                  <p class="card-text small text-muted">{{service.description}}</p>
                  <div class="d-flex justify-content-between align-items-center">
                    <div>
                      <strong class="text-primary">{{formatCurrency(service.price)}}</strong>
                      <br>
                      <small class="text-muted">{{service.duration}} min</small>
                    </div>
                    <div class="btn-group" role="group">
                      <button class="btn btn-sm btn-outline-primary">
                        <i class="fas fa-edit"></i>
                      </button>
                      <button class="btn btn-sm btn-outline-danger">
                        <i class="fas fa-trash"></i>
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class ServicesComponent implements OnInit {
  stats = { dental: 0, harmonization: 0, aesthetic: 0, total: 0 };
  services: any[] = [];
  loading = true;

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadServices();
    this.loadStats();
  }

  loadServices() {
    this.apiService.get('/api/services').subscribe({
      next: (data) => {
        this.services = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar serviços:', error);
        this.loading = false;
      }
    });
  }

  loadStats() {
    this.apiService.get('/api/services/stats').subscribe({
      next: (data) => {
        this.stats = data;
      },
      error: (error) => {
        console.error('Erro ao carregar estatísticas:', error);
      }
    });
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }

  getCategoryClass(category: string): string {
    const categoryClasses: any = {
      'Odontologia': 'bg-primary',
      'Harmonização': 'bg-success',
      'Estética': 'bg-info'
    };
    return categoryClasses[category] || 'bg-secondary';
  }
}