import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-packages',
  template: `
    <div class="container-fluid p-4">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <h2><i class="fas fa-box-open me-2"></i>Pacotes de Serviços</h2>
        <button class="btn btn-primary">
          <i class="fas fa-plus me-2"></i>Novo Pacote
        </button>
      </div>
      
      <div class="row mb-4">
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-box-open text-primary fa-2x mb-2"></i>
              <h6>Pacotes Ativos</h6>
              <h4 class="text-primary">{{stats.active}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-star text-warning fa-2x mb-2"></i>
              <h6>Mais Vendidos</h6>
              <h4 class="text-warning">{{stats.popular}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-percentage text-success fa-2x mb-2"></i>
              <h6>Com Desconto</h6>
              <h4 class="text-success">{{stats.discounted}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-dollar-sign text-info fa-2x mb-2"></i>
              <h6>Receita Total</h6>
              <h4 class="text-info">{{formatCurrency(stats.totalRevenue)}}</h4>
            </div>
          </div>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-white">
          <h5 class="mb-0">Pacotes Disponíveis</h5>
        </div>
        <div class="card-body">
          <div *ngIf="loading" class="text-center py-4">
            <div class="spinner-border" role="status">
              <span class="visually-hidden">Carregando...</span>
            </div>
          </div>
          <div *ngIf="!loading && packages.length > 0" class="row">
            <div *ngFor="let package of packages" class="col-md-4 mb-3">
              <div class="card h-100 border-0 shadow-sm">
                <div class="card-body">
                  <div class="d-flex justify-content-between align-items-start mb-2">
                    <h6 class="card-title">{{package.name}}</h6>
                    <span class="badge bg-success" *ngIf="package.discount > 0">
                      -{{package.discount}}%
                    </span>
                  </div>
                  <p class="card-text small text-muted">{{package.description}}</p>
                  <div class="mb-3">
                    <h6>Serviços Inclusos:</h6>
                    <ul class="list-unstyled small">
                      <li *ngFor="let service of package.services">
                        <i class="fas fa-check text-success me-1"></i>{{service.name}}
                      </li>
                    </ul>
                  </div>
                  <div class="d-flex justify-content-between align-items-center">
                    <div>
                      <span class="text-muted text-decoration-line-through" *ngIf="package.discount > 0">
                        {{formatCurrency(package.originalPrice)}}
                      </span>
                      <strong class="text-primary d-block">{{formatCurrency(package.finalPrice)}}</strong>
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
export class PackagesComponent implements OnInit {
  stats = { active: 0, popular: 0, discounted: 0, totalRevenue: 0 };
  packages: any[] = [];
  loading = true;

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadPackages();
    this.loadStats();
  }

  loadPackages() {
    this.apiService.get('/api/packages').subscribe({
      next: (data: any) => {
        this.packages = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar pacotes:', error);
        this.loading = false;
      }
    });
  }

  loadStats() {
    this.apiService.get('/api/packages/stats').subscribe({
      next: (data: any) => {
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
}