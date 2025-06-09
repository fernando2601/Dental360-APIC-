import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-inventory',
  template: `
    <div class="container">
      <h2>Controle de Estoque</h2>
      <div class="inventory-grid">
        <div class="inventory-card" *ngFor="let item of inventoryItems">
          <div class="item-header">
            <h4>{{ item.name }}</h4>
            <div class="item-category">{{ item.category }}</div>
          </div>
          <div class="item-details">
            <div class="quantity" [class.low-stock]="item.quantity < item.min_quantity">
              <strong>Quantidade:</strong> {{ item.quantity }} {{ item.unit }}
            </div>
            <div class="supplier">
              <strong>Fornecedor:</strong> {{ item.supplier }}
            </div>
            <div class="price">
              <strong>Preço:</strong> R$ {{ item.price | number:'1.2-2' }}
            </div>
          </div>
          <div class="item-actions" *ngIf="item.quantity < item.min_quantity">
            <button class="btn btn-warning btn-sm">Reabastecer</button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .container { padding: 2rem; }
    .inventory-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); gap: 1.5rem; }
    .inventory-card { background: white; padding: 1.5rem; border-radius: 8px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }
    .item-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1rem; }
    .item-category { background: #e5e7eb; color: #374151; padding: 0.25rem 0.75rem; border-radius: 20px; font-size: 0.75rem; }
    .item-details div { margin-bottom: 0.5rem; color: #6b7280; }
    .quantity.low-stock { color: #ef4444; font-weight: 600; }
    .btn { padding: 0.5rem 1rem; border: none; border-radius: 4px; font-weight: 500; cursor: pointer; font-size: 0.875rem; }
    .btn-warning { background: #f59e0b; color: white; }
    .btn-sm { padding: 0.375rem 0.75rem; }
  `]
})
export class InventoryComponent implements OnInit {
  inventoryItems: any[] = [];

  constructor(private apiService: ApiService) { }

  ngOnInit() {
    this.apiService.get('/inventory').subscribe({
      next: (data) => this.inventoryItems = data,
      error: () => {
        this.inventoryItems = [
          { id: 1, name: 'Anestésico Lidocaína', category: 'Medicamentos', quantity: 25, min_quantity: 10, unit: 'tubetes', supplier: 'DentSupply', price: 3.50 },
          { id: 2, name: 'Resina Composta', category: 'Materiais', quantity: 5, min_quantity: 8, unit: 'seringas', supplier: '3M', price: 45.00 },
          { id: 3, name: 'Luvas Descartáveis', category: 'EPIs', quantity: 150, min_quantity: 50, unit: 'pares', supplier: 'MedProtect', price: 0.75 }
        ];
      }
    });
  }
}