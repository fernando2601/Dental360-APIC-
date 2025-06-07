import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-patients',
  template: `
    <div class="container-fluid p-4">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <h2><i class="fas fa-user-injured me-2"></i>Pacientes</h2>
        <button class="btn btn-primary">
          <i class="fas fa-plus me-2"></i>Novo Paciente
        </button>
      </div>
      
      <div class="row mb-4">
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-user-injured text-primary fa-2x mb-2"></i>
              <h6>Total Pacientes</h6>
              <h4 class="text-primary">{{stats.totalPatients}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-user-plus text-success fa-2x mb-2"></i>
              <h6>Novos Este Mês</h6>
              <h4 class="text-success">{{stats.newThisMonth}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-calendar-check text-info fa-2x mb-2"></i>
              <h6>Em Tratamento</h6>
              <h4 class="text-info">{{stats.inTreatment}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-star text-warning fa-2x mb-2"></i>
              <h6>Satisfação Média</h6>
              <h4 class="text-warning">{{stats.avgSatisfaction}}/5</h4>
            </div>
          </div>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-white d-flex justify-content-between align-items-center">
          <h5 class="mb-0">Lista de Pacientes</h5>
          <div class="d-flex gap-2">
            <div class="input-group" style="width: 300px;">
              <span class="input-group-text">
                <i class="fas fa-search"></i>
              </span>
              <input type="text" class="form-control" placeholder="Buscar paciente..." [(ngModel)]="searchTerm" (input)="filterPatients()">
            </div>
            <select class="form-select" style="width: auto;" [(ngModel)]="statusFilter" (change)="filterPatients()">
              <option value="">Todos os status</option>
              <option value="active">Ativo</option>
              <option value="treatment">Em Tratamento</option>
              <option value="completed">Tratamento Concluído</option>
              <option value="inactive">Inativo</option>
            </select>
          </div>
        </div>
        <div class="card-body">
          <div *ngIf="loading" class="text-center py-4">
            <div class="spinner-border" role="status">
              <span class="visually-hidden">Carregando...</span>
            </div>
          </div>
          <div *ngIf="!loading && filteredPatients.length > 0" class="table-responsive">
            <table class="table table-hover">
              <thead>
                <tr>
                  <th>Paciente</th>
                  <th>Idade</th>
                  <th>Telefone</th>
                  <th>Último Atendimento</th>
                  <th>Próxima Consulta</th>
                  <th>Status</th>
                  <th>Ações</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let patient of filteredPatients">
                  <td>
                    <div class="d-flex align-items-center">
                      <div class="avatar-sm bg-light rounded-circle d-flex align-items-center justify-content-center me-3">
                        <i class="fas fa-user text-muted"></i>
                      </div>
                      <div>
                        <strong>{{patient.name}}</strong>
                        <br>
                        <small class="text-muted">{{patient.email}}</small>
                      </div>
                    </div>
                  </td>
                  <td>{{patient.age}} anos</td>
                  <td>{{patient.phone}}</td>
                  <td>
                    <span *ngIf="patient.lastAppointment">{{formatDate(patient.lastAppointment)}}</span>
                    <span *ngIf="!patient.lastAppointment" class="text-muted">Nunca</span>
                  </td>
                  <td>
                    <span *ngIf="patient.nextAppointment">{{formatDate(patient.nextAppointment)}}</span>
                    <span *ngIf="!patient.nextAppointment" class="text-muted">Não agendado</span>
                  </td>
                  <td>
                    <span class="badge" [ngClass]="getStatusClass(patient.status)">
                      {{patient.status}}
                    </span>
                  </td>
                  <td>
                    <div class="btn-group" role="group">
                      <button class="btn btn-sm btn-outline-primary" title="Ver Prontuário">
                        <i class="fas fa-file-medical"></i>
                      </button>
                      <button class="btn btn-sm btn-outline-success" title="Agendar Consulta">
                        <i class="fas fa-calendar-plus"></i>
                      </button>
                      <button class="btn btn-sm btn-outline-info" title="Histórico">
                        <i class="fas fa-history"></i>
                      </button>
                      <button class="btn btn-sm btn-outline-secondary" title="Editar">
                        <i class="fas fa-edit"></i>
                      </button>
                    </div>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          <div *ngIf="!loading && filteredPatients.length === 0" class="text-center py-4 text-muted">
            <i class="fas fa-user-injured fa-3x mb-3"></i>
            <p>Nenhum paciente encontrado</p>
          </div>
        </div>
      </div>
    </div>
  `
})
export class PatientsComponent implements OnInit {
  stats = { 
    totalPatients: 0, 
    newThisMonth: 0, 
    inTreatment: 0, 
    avgSatisfaction: 0 
  };
  patients: any[] = [];
  filteredPatients: any[] = [];
  loading = true;
  searchTerm = '';
  statusFilter = '';

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadPatients();
    this.loadStats();
  }

  loadPatients() {
    this.apiService.get('/api/patients').subscribe({
      next: (data: any) => {
        this.patients = data;
        this.filteredPatients = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar pacientes:', error);
        this.loading = false;
      }
    });
  }

  loadStats() {
    this.apiService.get('/api/patients/stats').subscribe({
      next: (data: any) => {
        this.stats = data;
      },
      error: (error) => {
        console.error('Erro ao carregar estatísticas:', error);
      }
    });
  }

  filterPatients() {
    this.filteredPatients = this.patients.filter(patient => {
      const matchesSearch = !this.searchTerm || 
        patient.name.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        patient.email.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        patient.phone.includes(this.searchTerm);
      
      const matchesStatus = !this.statusFilter || patient.status === this.statusFilter;
      
      return matchesSearch && matchesStatus;
    });
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('pt-BR');
  }

  getStatusClass(status: string): string {
    const statusClasses: any = {
      'Ativo': 'bg-success',
      'Em Tratamento': 'bg-info',
      'Tratamento Concluído': 'bg-primary',
      'Inativo': 'bg-secondary'
    };
    return statusClasses[status] || 'bg-secondary';
  }
}