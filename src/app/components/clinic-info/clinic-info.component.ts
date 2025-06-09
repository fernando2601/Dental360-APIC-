import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-clinic-info',
  template: `
    <div class="container">
      <h2>Informações da Clínica</h2>
      
      <div class="info-sections">
        <div class="info-card">
          <h3>Dados Básicos</h3>
          <form [formGroup]="clinicForm" (ngSubmit)="saveClinicInfo()">
            <div class="form-grid">
              <div class="form-group">
                <label>Nome da Clínica</label>
                <input type="text" formControlName="name" class="form-control">
              </div>
              
              <div class="form-group">
                <label>CNPJ</label>
                <input type="text" formControlName="cnpj" class="form-control">
              </div>
              
              <div class="form-group">
                <label>Telefone</label>
                <input type="text" formControlName="phone" class="form-control">
              </div>
              
              <div class="form-group">
                <label>Email</label>
                <input type="email" formControlName="email" class="form-control">
              </div>
            </div>
            
            <div class="form-group">
              <label>Endereço Completo</label>
              <textarea formControlName="address" class="form-control" rows="3"></textarea>
            </div>
            
            <div class="form-group">
              <label>Horário de Funcionamento</label>
              <textarea formControlName="hours" class="form-control" rows="3"></textarea>
            </div>
            
            <button type="submit" class="btn btn-primary">Salvar Alterações</button>
          </form>
        </div>

        <div class="info-card">
          <h3>Estatísticas da Clínica</h3>
          <div class="stats-list">
            <div class="stat-item">
              <div class="stat-label">Total de Pacientes</div>
              <div class="stat-value">{{ clinicStats.totalPatients }}</div>
            </div>
            <div class="stat-item">
              <div class="stat-label">Consultas Este Mês</div>
              <div class="stat-value">{{ clinicStats.monthlyAppointments }}</div>
            </div>
            <div class="stat-item">
              <div class="stat-label">Receita Mensal</div>
              <div class="stat-value">R$ {{ clinicStats.monthlyRevenue | number:'1.2-2' }}</div>
            </div>
            <div class="stat-item">
              <div class="stat-label">Taxa de Satisfação</div>
              <div class="stat-value">{{ clinicStats.satisfactionRate }}%</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .container { padding: 2rem; }
    .info-sections { display: grid; grid-template-columns: 2fr 1fr; gap: 2rem; }
    .info-card { background: white; padding: 1.5rem; border-radius: 8px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }
    .info-card h3 { margin: 0 0 1.5rem 0; color: #1f2937; }
    .form-grid { display: grid; grid-template-columns: repeat(2, 1fr); gap: 1rem; margin-bottom: 1rem; }
    .form-group { display: flex; flex-direction: column; margin-bottom: 1rem; }
    .form-group label { margin-bottom: 0.5rem; font-weight: 500; color: #374151; }
    .form-control { padding: 0.75rem; border: 1px solid #d1d5db; border-radius: 4px; }
    .btn { padding: 0.75rem 1.5rem; border: none; border-radius: 4px; font-weight: 500; cursor: pointer; }
    .btn-primary { background: #3b82f6; color: white; }
    .stats-list { display: grid; gap: 1rem; }
    .stat-item { display: flex; justify-content: space-between; align-items: center; padding: 1rem; background: #f9fafb; border-radius: 6px; }
    .stat-label { color: #6b7280; }
    .stat-value { font-weight: 600; color: #1f2937; font-size: 1.1rem; }
    @media (max-width: 768px) { .info-sections { grid-template-columns: 1fr; } }
  `]
})
export class ClinicInfoComponent implements OnInit {
  clinicForm: FormGroup;
  clinicStats = {
    totalPatients: 0,
    monthlyAppointments: 0,
    monthlyRevenue: 0,
    satisfactionRate: 0
  };

  constructor(
    private apiService: ApiService,
    private fb: FormBuilder
  ) {
    this.clinicForm = this.fb.group({
      name: ['DentalSpa Clínica Odontológica', Validators.required],
      cnpj: ['12.345.678/0001-90', Validators.required],
      phone: ['(11) 3456-7890', Validators.required],
      email: ['contato@dentalspa.com', [Validators.required, Validators.email]],
      address: ['Rua das Flores, 123 - Centro\nSão Paulo, SP - CEP: 01234-567'],
      hours: ['Segunda a Sexta: 8h às 18h\nSábado: 8h às 12h\nDomingo: Fechado']
    });
  }

  ngOnInit() {
    this.loadClinicInfo();
    this.loadClinicStats();
  }

  loadClinicInfo() {
    this.apiService.get('/clinic/info').subscribe({
      next: (data) => this.clinicForm.patchValue(data),
      error: () => console.log('Usando dados padrão da clínica')
    });
  }

  loadClinicStats() {
    this.apiService.get('/clinic/stats').subscribe({
      next: (data) => this.clinicStats = data,
      error: () => {
        this.clinicStats = {
          totalPatients: 156,
          monthlyAppointments: 89,
          monthlyRevenue: 15750.00,
          satisfactionRate: 96
        };
      }
    });
  }

  saveClinicInfo() {
    if (this.clinicForm.valid) {
      this.apiService.put('/clinic/info', this.clinicForm.value).subscribe({
        next: () => console.log('Informações da clínica atualizadas'),
        error: (error) => console.error('Erro ao atualizar informações:', error)
      });
    }
  }
}