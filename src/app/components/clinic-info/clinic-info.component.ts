import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-clinic-info',
  template: `
    <div class="container-fluid p-4">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <h2><i class="fas fa-clinic-medical me-2"></i>Informações da Clínica</h2>
        <button class="btn btn-primary" (click)="toggleEditMode()">
          <i class="fas fa-edit me-2"></i>{{editMode ? 'Cancelar' : 'Editar'}}
        </button>
      </div>

      <div class="row">
        <div class="col-md-8">
          <div class="card border-0 shadow-sm">
            <div class="card-header bg-white">
              <h5 class="mb-0">Dados Principais</h5>
            </div>
            <div class="card-body">
              <form [formGroup]="clinicForm" (ngSubmit)="onSubmit()">
                <div class="row mb-3">
                  <div class="col-md-6">
                    <label class="form-label">Nome da Clínica</label>
                    <input type="text" class="form-control" formControlName="name" [readonly]="!editMode">
                  </div>
                  <div class="col-md-6">
                    <label class="form-label">CNPJ</label>
                    <input type="text" class="form-control" formControlName="cnpj" [readonly]="!editMode">
                  </div>
                </div>

                <div class="row mb-3">
                  <div class="col-md-6">
                    <label class="form-label">Telefone</label>
                    <input type="text" class="form-control" formControlName="phone" [readonly]="!editMode">
                  </div>
                  <div class="col-md-6">
                    <label class="form-label">Email</label>
                    <input type="email" class="form-control" formControlName="email" [readonly]="!editMode">
                  </div>
                </div>

                <div class="mb-3">
                  <label class="form-label">Endereço</label>
                  <input type="text" class="form-control" formControlName="address" [readonly]="!editMode">
                </div>

                <div class="row mb-3">
                  <div class="col-md-6">
                    <label class="form-label">Cidade</label>
                    <input type="text" class="form-control" formControlName="city" [readonly]="!editMode">
                  </div>
                  <div class="col-md-3">
                    <label class="form-label">Estado</label>
                    <input type="text" class="form-control" formControlName="state" [readonly]="!editMode">
                  </div>
                  <div class="col-md-3">
                    <label class="form-label">CEP</label>
                    <input type="text" class="form-control" formControlName="zipCode" [readonly]="!editMode">
                  </div>
                </div>

                <div class="mb-3">
                  <label class="form-label">Descrição</label>
                  <textarea class="form-control" rows="3" formControlName="description" [readonly]="!editMode"></textarea>
                </div>

                <div class="row mb-3">
                  <div class="col-md-6">
                    <label class="form-label">Horário de Funcionamento</label>
                    <input type="text" class="form-control" formControlName="workingHours" [readonly]="!editMode">
                  </div>
                  <div class="col-md-6">
                    <label class="form-label">Website</label>
                    <input type="url" class="form-control" formControlName="website" [readonly]="!editMode">
                  </div>
                </div>

                <div *ngIf="editMode" class="d-flex gap-2">
                  <button type="submit" class="btn btn-success" [disabled]="!clinicForm.valid || saving">
                    <i class="fas fa-save me-2"></i>{{saving ? 'Salvando...' : 'Salvar'}}
                  </button>
                  <button type="button" class="btn btn-secondary" (click)="cancelEdit()">
                    Cancelar
                  </button>
                </div>
              </form>
            </div>
          </div>
        </div>

        <div class="col-md-4">
          <div class="card border-0 shadow-sm mb-4">
            <div class="card-header bg-white">
              <h6 class="mb-0">Redes Sociais</h6>
            </div>
            <div class="card-body">
              <div class="mb-3">
                <label class="form-label">Instagram</label>
                <div class="input-group">
                  <span class="input-group-text"><i class="fab fa-instagram"></i></span>
                  <input type="text" class="form-control" [(ngModel)]="socialMedia.instagram" [readonly]="!editMode">
                </div>
              </div>
              <div class="mb-3">
                <label class="form-label">Facebook</label>
                <div class="input-group">
                  <span class="input-group-text"><i class="fab fa-facebook"></i></span>
                  <input type="text" class="form-control" [(ngModel)]="socialMedia.facebook" [readonly]="!editMode">
                </div>
              </div>
              <div class="mb-3">
                <label class="form-label">WhatsApp</label>
                <div class="input-group">
                  <span class="input-group-text"><i class="fab fa-whatsapp"></i></span>
                  <input type="text" class="form-control" [(ngModel)]="socialMedia.whatsapp" [readonly]="!editMode">
                </div>
              </div>
            </div>
          </div>

          <div class="card border-0 shadow-sm">
            <div class="card-header bg-white">
              <h6 class="mb-0">Estatísticas</h6>
            </div>
            <div class="card-body">
              <div class="row text-center">
                <div class="col-6 mb-3">
                  <h4 class="text-primary">{{stats.yearsInBusiness}}</h4>
                  <small class="text-muted">Anos de Experiência</small>
                </div>
                <div class="col-6 mb-3">
                  <h4 class="text-success">{{stats.totalPatients}}</h4>
                  <small class="text-muted">Pacientes Atendidos</small>
                </div>
                <div class="col-6">
                  <h4 class="text-info">{{stats.totalProcedures}}</h4>
                  <small class="text-muted">Procedimentos</small>
                </div>
                <div class="col-6">
                  <h4 class="text-warning">{{stats.satisfactionRate}}%</h4>
                  <small class="text-muted">Satisfação</small>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class ClinicInfoComponent implements OnInit {
  clinicForm: FormGroup;
  editMode = false;
  saving = false;
  
  socialMedia = {
    instagram: '',
    facebook: '',
    whatsapp: ''
  };

  stats = {
    yearsInBusiness: 0,
    totalPatients: 0,
    totalProcedures: 0,
    satisfactionRate: 0
  };

  constructor(
    private apiService: ApiService,
    private fb: FormBuilder
  ) {
    this.clinicForm = this.fb.group({
      name: ['', Validators.required],
      cnpj: ['', Validators.required],
      phone: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      address: ['', Validators.required],
      city: ['', Validators.required],
      state: ['', Validators.required],
      zipCode: ['', Validators.required],
      description: [''],
      workingHours: [''],
      website: ['']
    });
  }

  ngOnInit() {
    this.loadClinicInfo();
    this.loadStats();
  }

  loadClinicInfo() {
    this.apiService.get('/api/clinic-info').subscribe({
      next: (data: any) => {
        this.clinicForm.patchValue(data);
        this.socialMedia = data.socialMedia || this.socialMedia;
      },
      error: (error) => {
        console.error('Erro ao carregar informações:', error);
      }
    });
  }

  loadStats() {
    this.apiService.get('/api/clinic-info/stats').subscribe({
      next: (data: any) => {
        this.stats = data;
      },
      error: (error) => {
        console.error('Erro ao carregar estatísticas:', error);
      }
    });
  }

  toggleEditMode() {
    this.editMode = !this.editMode;
    if (!this.editMode) {
      this.loadClinicInfo(); // Reset form if canceling
    }
  }

  cancelEdit() {
    this.editMode = false;
    this.loadClinicInfo();
  }

  onSubmit() {
    if (this.clinicForm.valid) {
      this.saving = true;
      const formData = {
        ...this.clinicForm.value,
        socialMedia: this.socialMedia
      };

      this.apiService.put('/api/clinic-info', formData).subscribe({
        next: () => {
          this.saving = false;
          this.editMode = false;
          // Show success message
        },
        error: (error) => {
          console.error('Erro ao salvar:', error);
          this.saving = false;
        }
      });
    }
  }
}