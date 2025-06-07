import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-inventory',
  template: `
    <div class="container-fluid p-4">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <h2><i class="fas fa-boxes me-2"></i>Estoque</h2>
        <button class="btn btn-primary">
          <i class="fas fa-plus me-2"></i>Adicionar Item
        </button>
      </div>
      
      <div class="row mb-4">
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-cubes text-primary fa-2x mb-2"></i>
              <h6>Total de Itens</h6>
              <h4 class="text-primary">{{stats.totalItems}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-exclamation-triangle text-warning fa-2x mb-2"></i>
              <h6>Estoque Baixo</h6>
              <h4 class="text-warning">{{stats.lowStock}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-times-circle text-danger fa-2x mb-2"></i>
              <h6>Sem Estoque</h6>
              <h4 class="text-danger">{{stats.outOfStock}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-dollar-sign text-success fa-2x mb-2"></i>
              <h6>Valor Total</h6>
              <h4 class="text-success">{{formatCurrency(stats.totalValue)}}</h4>
            </div>
          </div>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-white d-flex justify-content-between align-items-center">
          <h5 class="mb-0">Itens do Estoque</h5>
          <div class="d-flex gap-2">
            <select class="form-select" style="width: auto;">
              <option value="">Todas as categorias</option>
              <option value="materials">Materiais</option>
              <option value="equipment">Equipamentos</option>
              <option value="medicines">Medicamentos</option>
              <option value="consumables">Consumíveis</option>
            </select>
            <div class="input-group" style="width: 250px;">
              <span class="input-group-text">
                <i class="fas fa-search"></i>
              </span>
              <input type="text" class="form-control" placeholder="Buscar item...">
            </div>
          </div>
        </div>
        <div class="card-body">
          <div *ngIf="loading" class="text-center py-4">
            <div class="spinner-border" role="status">
              <span class="visually-hidden">Carregando...</span>
            </div>
          </div>
          <div *ngIf="!loading && items.length === 0" class="text-center py-4 text-muted">
            <i class="fas fa-boxes fa-3x mb-3"></i>
            <p>Nenhum item no estoque</p>
          </div>
          <div *ngIf="!loading && items.length > 0" class="table-responsive">
            <table class="table table-hover">
              <thead>
                <tr>
                  <th>Item</th>
                  <th>Categoria</th>
                  <th>Quantidade</th>
                  <th>Estoque Mín.</th>
                  <th>Valor Unit.</th>
                  <th>Status</th>
                  <th>Ações</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let item of items">
                  <td>
                    <div>
                      <strong>{{item.name}}</strong>
                      <br>
                      <small class="text-muted">{{item.description}}</small>
                    </div>
                  </td>
                  <td>
                    <span class="badge" [ngClass]="getCategoryClass(item.category)">
                      {{item.category}}
                    </span>
                  </td>
                  <td>
                    <span [ngClass]="getQuantityClass(item.quantity, item.minimumStock)">
                      {{item.quantity}} {{item.unit}}
                    </span>
                  </td>
                  <td>{{item.minimumStock}} {{item.unit}}</td>
                  <td>{{formatCurrency(item.unitPrice)}}</td>
                  <td>
                    <span class="badge" [ngClass]="getStatusClass(item.quantity, item.minimumStock)">
                      {{getStatus(item.quantity, item.minimumStock)}}
                    </span>
                  </td>
                  <td>
                    <button class="btn btn-sm btn-outline-primary me-1" title="Editar">
                      <i class="fas fa-edit"></i>
                    </button>
                    <button class="btn btn-sm btn-outline-success me-1" title="Adicionar Estoque">
                      <i class="fas fa-plus"></i>
                    </button>
                    <button class="btn btn-sm btn-outline-danger" title="Remover">
                      <i class="fas fa-trash"></i>
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  `
})
export class InventoryComponent implements OnInit {
  stats = { 
    totalItems: 0, 
    lowStock: 0, 
    outOfStock: 0, 
    totalValue: 0 
  };
  items: any[] = [];
  loading = true;

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadInventory();
    this.loadStats();
  }

  loadInventory() {
    this.apiService.get('/api/inventory').subscribe({
      next: (data: any) => {
        this.items = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar estoque:', error);
        this.loading = false;
      }
    });
  }

  loadStats() {
    this.apiService.get('/api/inventory/stats').subscribe({
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

  getCategoryClass(category: string): string {
    const categoryClasses: any = {
      'Materiais': 'bg-primary',
      'Equipamentos': 'bg-success',
      'Medicamentos': 'bg-info',
      'Consumíveis': 'bg-warning'
    };
    return categoryClasses[category] || 'bg-secondary';
  }

  getQuantityClass(quantity: number, minimumStock: number): string {
    if (quantity === 0) return 'text-danger fw-bold';
    if (quantity <= minimumStock) return 'text-warning fw-bold';
    return 'text-success fw-bold';
  }

  getStatusClass(quantity: number, minimumStock: number): string {
    if (quantity === 0) return 'bg-danger';
    if (quantity <= minimumStock) return 'bg-warning';
    return 'bg-success';
  }

  getStatus(quantity: number, minimumStock: number): string {
    if (quantity === 0) return 'Sem Estoque';
    if (quantity <= minimumStock) return 'Estoque Baixo';
    return 'Normal';
  }
}