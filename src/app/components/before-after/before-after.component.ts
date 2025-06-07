import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-before-after',
  template: `
    <div class="container-fluid p-4">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <h2><i class="fas fa-images me-2"></i>Antes e Depois</h2>
        <button class="btn btn-primary">
          <i class="fas fa-plus me-2"></i>Adicionar Caso
        </button>
      </div>
      
      <div class="row mb-4">
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-images text-primary fa-2x mb-2"></i>
              <h6>Total de Casos</h6>
              <h4 class="text-primary">{{stats.totalCases}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-tooth text-success fa-2x mb-2"></i>
              <h6>Odontologia</h6>
              <h4 class="text-success">{{stats.dental}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-user-md text-info fa-2x mb-2"></i>
              <h6>Harmonização</h6>
              <h4 class="text-info">{{stats.harmonization}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-spa text-warning fa-2x mb-2"></i>
              <h6>Estética</h6>
              <h4 class="text-warning">{{stats.aesthetic}}</h4>
            </div>
          </div>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-white d-flex justify-content-between align-items-center">
          <h5 class="mb-0">Galeria de Transformações</h5>
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
          <div *ngIf="!loading && cases.length > 0" class="row">
            <div *ngFor="let case of cases" class="col-md-6 col-lg-4 mb-4">
              <div class="card h-100 border-0 shadow-sm">
                <div class="card-body">
                  <div class="d-flex justify-content-between align-items-start mb-3">
                    <h6 class="card-title">{{case.title}}</h6>
                    <span class="badge" [ngClass]="getCategoryClass(case.category)">
                      {{case.category}}
                    </span>
                  </div>
                  
                  <div class="row mb-3">
                    <div class="col-6">
                      <div class="text-center">
                        <h6 class="small text-muted mb-2">ANTES</h6>
                        <div class="before-image-container">
                          <img [src]="case.beforeImage" class="img-fluid rounded" 
                               alt="Antes" style="height: 120px; object-fit: cover; width: 100%;">
                        </div>
                      </div>
                    </div>
                    <div class="col-6">
                      <div class="text-center">
                        <h6 class="small text-muted mb-2">DEPOIS</h6>
                        <div class="after-image-container">
                          <img [src]="case.afterImage" class="img-fluid rounded" 
                               alt="Depois" style="height: 120px; object-fit: cover; width: 100%;">
                        </div>
                      </div>
                    </div>
                  </div>
                  
                  <p class="card-text small text-muted">{{case.description}}</p>
                  
                  <div class="d-flex justify-content-between align-items-center mt-3">
                    <small class="text-muted">
                      <i class="fas fa-calendar me-1"></i>{{formatDate(case.date)}}
                    </small>
                    <div class="btn-group" role="group">
                      <button class="btn btn-sm btn-outline-primary" title="Visualizar">
                        <i class="fas fa-eye"></i>
                      </button>
                      <button class="btn btn-sm btn-outline-secondary" title="Editar">
                        <i class="fas fa-edit"></i>
                      </button>
                      <button class="btn btn-sm btn-outline-danger" title="Excluir">
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
export class BeforeAfterComponent implements OnInit {
  stats = { totalCases: 0, dental: 0, harmonization: 0, aesthetic: 0 };
  cases: any[] = [];
  loading = true;

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadCases();
    this.loadStats();
  }

  loadCases() {
    this.apiService.get('/api/before-after').subscribe({
      next: (data: any) => {
        this.cases = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar casos:', error);
        this.loading = false;
      }
    });
  }

  loadStats() {
    this.apiService.get('/api/before-after/stats').subscribe({
      next: (data: any) => {
        this.stats = data;
      },
      error: (error) => {
        console.error('Erro ao carregar estatísticas:', error);
      }
    });
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('pt-BR');
  }

  getCategoryClass(category: string): string {
    const categoryClasses: any = {
      'Odontologia': 'bg-success',
      'Harmonização': 'bg-info',
      'Estética': 'bg-warning'
    };
    return categoryClasses[category] || 'bg-secondary';
  }
}