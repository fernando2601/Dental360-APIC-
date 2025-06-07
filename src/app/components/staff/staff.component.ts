import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-staff',
  template: `
    <div class="container-fluid p-4">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <h2><i class="fas fa-user-md me-2"></i>Equipe</h2>
        <button class="btn btn-primary">
          <i class="fas fa-plus me-2"></i>Adicionar Membro
        </button>
      </div>
      
      <div class="row mb-4">
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-user-md text-primary fa-2x mb-2"></i>
              <h6>Dentistas</h6>
              <h4 class="text-primary">{{stats.dentists}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-user-nurse text-success fa-2x mb-2"></i>
              <h6>Auxiliares</h6>
              <h4 class="text-success">{{stats.assistants}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-user-tie text-info fa-2x mb-2"></i>
              <h6>Administração</h6>
              <h4 class="text-info">{{stats.admin}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-users text-warning fa-2x mb-2"></i>
              <h6>Total</h6>
              <h4 class="text-warning">{{stats.total}}</h4>
            </div>
          </div>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-white d-flex justify-content-between align-items-center">
          <h5 class="mb-0">Membros da Equipe</h5>
          <div class="d-flex gap-2">
            <select class="form-select" style="width: auto;">
              <option value="">Todos os cargos</option>
              <option value="dentist">Dentista</option>
              <option value="assistant">Auxiliar</option>
              <option value="admin">Administração</option>
            </select>
          </div>
        </div>
        <div class="card-body">
          <div *ngIf="loading" class="text-center py-4">
            <div class="spinner-border" role="status">
              <span class="visually-hidden">Carregando...</span>
            </div>
          </div>
          <div *ngIf="!loading && staff.length === 0" class="text-center py-4 text-muted">
            <i class="fas fa-users fa-3x mb-3"></i>
            <p>Nenhum membro da equipe cadastrado</p>
          </div>
          <div *ngIf="!loading && staff.length > 0" class="row">
            <div *ngFor="let member of staff" class="col-md-4 mb-3">
              <div class="card h-100 border-0 shadow-sm">
                <div class="card-body text-center">
                  <div class="mb-3">
                    <i class="fas fa-user-circle fa-3x text-secondary"></i>
                  </div>
                  <h6 class="card-title">{{member.name}}</h6>
                  <span class="badge mb-2" [ngClass]="getRoleClass(member.role)">
                    {{member.role}}
                  </span>
                  <p class="card-text small text-muted">{{member.specialization}}</p>
                  <div class="d-flex justify-content-center gap-2">
                    <button class="btn btn-sm btn-outline-primary">
                      <i class="fas fa-edit"></i>
                    </button>
                    <button class="btn btn-sm btn-outline-info">
                      <i class="fas fa-calendar"></i>
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
  `
})
export class StaffComponent implements OnInit {
  stats = { dentists: 0, assistants: 0, admin: 0, total: 0 };
  staff: any[] = [];
  loading = true;

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadStaff();
    this.loadStats();
  }

  loadStaff() {
    this.apiService.get('/api/staff').subscribe({
      next: (data: any) => {
        this.staff = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar equipe:', error);
        this.loading = false;
      }
    });
  }

  loadStats() {
    this.apiService.get('/api/staff/stats').subscribe({
      next: (data: any) => {
        this.stats = data;
      },
      error: (error) => {
        console.error('Erro ao carregar estatísticas:', error);
      }
    });
  }

  getRoleClass(role: string): string {
    const roleClasses: any = {
      'Dentista': 'bg-primary',
      'Auxiliar': 'bg-success',
      'Administração': 'bg-info'
    };
    return roleClasses[role] || 'bg-secondary';
  }
}