import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-services',
  template: `
    <div class="container">
      <div class="header">
        <h2>Catálogo de Serviços</h2>
        <button class="btn btn-primary" (click)="openAddModal()">+ Novo Serviço</button>
      </div>

      <div class="services-grid" *ngIf="!loading">
        <div class="service-card" *ngFor="let service of services">
          <div class="service-header">
            <h4>{{ service.name }}</h4>
            <div class="service-price">R$ {{ service.price | number:'1.2-2' }}</div>
          </div>
          
          <div class="service-details">
            <p class="service-description">{{ service.description }}</p>
            <div class="service-info">
              <div class="info-item">
                <strong>Duração:</strong> {{ service.duration }} min
              </div>
              <div class="info-item">
                <strong>Categoria:</strong> {{ service.category }}
              </div>
            </div>
          </div>
          
          <div class="service-actions">
            <button class="btn btn-sm btn-outline" (click)="editService(service)">Editar</button>
            <button class="btn btn-sm btn-danger" (click)="deleteService(service.id)">Excluir</button>
          </div>
        </div>
      </div>

      <div class="loading" *ngIf="loading">Carregando serviços...</div>

      <!-- Modal -->
      <div class="modal" *ngIf="showModal" (click)="closeModal()">
        <div class="modal-content" (click)="$event.stopPropagation()">
          <div class="modal-header">
            <h3>{{ isEditing ? 'Editar' : 'Novo' }} Serviço</h3>
            <button class="close-btn" (click)="closeModal()">×</button>
          </div>
          
          <form [formGroup]="serviceForm" (ngSubmit)="saveService()">
            <div class="form-body">
              <div class="form-group">
                <label>Nome *</label>
                <input type="text" formControlName="name" class="form-control">
              </div>
              
              <div class="form-group">
                <label>Descrição</label>
                <textarea formControlName="description" class="form-control" rows="3"></textarea>
              </div>
              
              <div class="form-grid">
                <div class="form-group">
                  <label>Preço (R$) *</label>
                  <input type="number" formControlName="price" class="form-control" step="0.01">
                </div>
                
                <div class="form-group">
                  <label>Duração (min) *</label>
                  <input type="number" formControlName="duration" class="form-control">
                </div>
              </div>
              
              <div class="form-group">
                <label>Categoria *</label>
                <select formControlName="category" class="form-control">
                  <option value="">Selecione uma categoria</option>
                  <option value="Preventivo">Preventivo</option>
                  <option value="Restaurador">Restaurador</option>
                  <option value="Estético">Estético</option>
                  <option value="Cirúrgico">Cirúrgico</option>
                  <option value="Ortodôntico">Ortodôntico</option>
                  <option value="Endodôntico">Endodôntico</option>
                </select>
              </div>
            </div>
            
            <div class="modal-actions">
              <button type="button" class="btn btn-outline" (click)="closeModal()">Cancelar</button>
              <button type="submit" class="btn btn-primary" [disabled]="!serviceForm.valid">Salvar</button>
            </div>
          </form>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .container { padding: 2rem; }
    .header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 2rem; }
    
    .services-grid { 
      display: grid; 
      grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); 
      gap: 1.5rem; 
    }
    
    .service-card {
      background: white;
      border-radius: 8px;
      padding: 1.5rem;
      box-shadow: 0 1px 3px rgba(0,0,0,0.1);
      border-left: 4px solid #3b82f6;
    }
    
    .service-header {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      margin-bottom: 1rem;
    }
    
    .service-header h4 { 
      margin: 0; 
      color: #1f2937; 
      flex: 1;
    }
    
    .service-price {
      background: #dbeafe;
      color: #1d4ed8;
      padding: 0.5rem 1rem;
      border-radius: 20px;
      font-weight: 600;
      font-size: 0.875rem;
    }
    
    .service-description {
      color: #6b7280;
      margin-bottom: 1rem;
      line-height: 1.5;
    }
    
    .service-info {
      margin-bottom: 1rem;
    }
    
    .info-item {
      margin-bottom: 0.5rem;
      color: #6b7280;
      font-size: 0.875rem;
    }
    
    .service-actions {
      display: flex;
      gap: 0.5rem;
    }
    
    .modal {
      position: fixed;
      top: 0;
      left: 0;
      width: 100%;
      height: 100%;
      background: rgba(0,0,0,0.5);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1000;
    }
    
    .modal-content {
      background: white;
      border-radius: 8px;
      width: 90%;
      max-width: 500px;
      max-height: 90vh;
      overflow-y: auto;
    }
    
    .modal-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 1.5rem;
      border-bottom: 1px solid #e5e7eb;
    }
    
    .close-btn {
      background: none;
      border: none;
      font-size: 1.5rem;
      cursor: pointer;
    }
    
    .form-body {
      padding: 1.5rem;
    }
    
    .form-grid {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 1rem;
    }
    
    .form-group {
      display: flex;
      flex-direction: column;
      margin-bottom: 1rem;
    }
    
    .form-group label {
      margin-bottom: 0.5rem;
      font-weight: 500;
      color: #374151;
    }
    
    .form-control {
      padding: 0.75rem;
      border: 1px solid #d1d5db;
      border-radius: 4px;
    }
    
    .modal-actions {
      display: flex;
      justify-content: flex-end;
      gap: 1rem;
      padding: 1.5rem;
      border-top: 1px solid #e5e7eb;
    }
    
    .btn {
      padding: 0.75rem 1.5rem;
      border: none;
      border-radius: 4px;
      font-weight: 500;
      cursor: pointer;
      transition: all 0.2s;
    }
    
    .btn-primary { background: #3b82f6; color: white; }
    .btn-outline { background: transparent; border: 1px solid #d1d5db; color: #374151; }
    .btn-danger { background: #ef4444; color: white; }
    .btn-sm { padding: 0.5rem 1rem; font-size: 0.875rem; }
    
    .btn:hover { opacity: 0.9; }
    .btn:disabled { opacity: 0.5; cursor: not-allowed; }
    
    .loading { text-align: center; padding: 3rem; color: #6b7280; }
  `]
})
export class ServicesComponent implements OnInit {
  services: any[] = [];
  serviceForm: FormGroup;
  selectedService: any = null;
  showModal = false;
  isEditing = false;
  loading = false;

  constructor(
    private apiService: ApiService,
    private fb: FormBuilder
  ) {
    this.serviceForm = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      price: ['', [Validators.required, Validators.min(0)]],
      duration: ['', [Validators.required, Validators.min(1)]],
      category: ['', Validators.required]
    });
  }

  ngOnInit() {
    this.loadServices();
  }

  loadServices() {
    this.loading = true;
    this.apiService.get('/services').subscribe({
      next: (data) => {
        this.services = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar serviços:', error);
        this.loading = false;
        // Dados de demonstração
        this.services = [
          {
            id: 1,
            name: 'Limpeza Dental',
            description: 'Profilaxia completa com remoção de tártaro e polimento',
            price: 80.00,
            duration: 60,
            category: 'Preventivo'
          },
          {
            id: 2,
            name: 'Clareamento Dental',
            description: 'Clareamento profissional com gel de peróxido',
            price: 450.00,
            duration: 90,
            category: 'Estético'
          },
          {
            id: 3,
            name: 'Restauração',
            description: 'Restauração em resina composta',
            price: 120.00,
            duration: 45,
            category: 'Restaurador'
          }
        ];
      }
    });
  }

  openAddModal() {
    this.isEditing = false;
    this.selectedService = null;
    this.serviceForm.reset();
    this.showModal = true;
  }

  editService(service: any) {
    this.isEditing = true;
    this.selectedService = service;
    this.serviceForm.patchValue(service);
    this.showModal = true;
  }

  deleteService(id: number) {
    if (confirm('Tem certeza que deseja excluir este serviço?')) {
      this.apiService.delete(`/services/${id}`).subscribe({
        next: () => {
          this.loadServices();
        },
        error: (error) => {
          console.error('Erro ao excluir serviço:', error);
        }
      });
    }
  }

  saveService() {
    if (this.serviceForm.valid) {
      const serviceData = this.serviceForm.value;
      
      if (this.isEditing) {
        this.apiService.put(`/services/${this.selectedService.id}`, serviceData).subscribe({
          next: () => {
            this.closeModal();
            this.loadServices();
          },
          error: (error) => {
            console.error('Erro ao atualizar serviço:', error);
          }
        });
      } else {
        this.apiService.post('/services', serviceData).subscribe({
          next: () => {
            this.closeModal();
            this.loadServices();
          },
          error: (error) => {
            console.error('Erro ao criar serviço:', error);
          }
        });
      }
    }
  }

  closeModal() {
    this.showModal = false;
    this.selectedService = null;
    this.serviceForm.reset();
  }
}