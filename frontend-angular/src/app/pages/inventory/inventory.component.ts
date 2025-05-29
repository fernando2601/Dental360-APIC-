import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

interface InventoryItem {
  id?: number;
  name: string;
  category: string;
  description?: string;
  quantity: number;
  unitPrice: number;
  supplier?: string;
  expirationDate?: string;
  minimumStock: number;
}

@Component({
  selector: 'app-inventory',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="max-w-7xl mx-auto">
      <div class="flex justify-between items-center mb-6">
        <h2 class="text-2xl font-bold text-gray-900">Inventário</h2>
        <button
          (click)="showNewItemForm = !showNewItemForm"
          class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md text-sm font-medium"
        >
          {{ showNewItemForm ? 'Cancelar' : 'Novo Item' }}
        </button>
      </div>

      <!-- Formulário Novo Item -->
      <div *ngIf="showNewItemForm" class="bg-white shadow rounded-lg p-6 mb-6">
        <h3 class="text-lg font-medium text-gray-900 mb-4">Cadastro de Item</h3>
        <form [formGroup]="itemForm" (ngSubmit)="onSubmit()">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700">Nome do Item</label>
              <input
                type="text"
                formControlName="name"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
                placeholder="Nome do produto"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Categoria</label>
              <select
                formControlName="category"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="">Selecione uma categoria</option>
                <option value="Injetáveis">Injetáveis</option>
                <option value="Materiais Dentários">Materiais Dentários</option>
                <option value="Instrumentos">Instrumentos</option>
                <option value="Medicamentos">Medicamentos</option>
                <option value="Higiene">Higiene</option>
                <option value="Equipamentos">Equipamentos</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Quantidade</label>
              <input
                type="number"
                formControlName="quantity"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
                placeholder="Quantidade em estoque"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Preço Unitário (R$)</label>
              <input
                type="number"
                step="0.01"
                formControlName="unitPrice"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
                placeholder="0.00"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Fornecedor</label>
              <input
                type="text"
                formControlName="supplier"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
                placeholder="Nome do fornecedor"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Data de Validade</label>
              <input
                type="date"
                formControlName="expirationDate"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Estoque Mínimo</label>
              <input
                type="number"
                formControlName="minimumStock"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
                placeholder="Quantidade mínima"
              />
            </div>
          </div>
          <div class="mt-4">
            <label class="block text-sm font-medium text-gray-700">Descrição</label>
            <textarea
              formControlName="description"
              rows="3"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
              placeholder="Descrição do produto"
            ></textarea>
          </div>
          <div class="mt-6 flex justify-end space-x-3">
            <button
              type="button"
              (click)="showNewItemForm = false"
              class="bg-gray-300 hover:bg-gray-400 text-gray-800 px-4 py-2 rounded-md text-sm font-medium"
            >
              Cancelar
            </button>
            <button
              type="submit"
              [disabled]="!itemForm.valid || isLoading"
              class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md text-sm font-medium disabled:opacity-50"
            >
              {{ isLoading ? 'Salvando...' : 'Salvar Item' }}
            </button>
          </div>
        </form>
      </div>

      <!-- Lista de Itens -->
      <div class="bg-white shadow rounded-lg">
        <div class="px-6 py-4 border-b border-gray-200">
          <h3 class="text-lg font-medium text-gray-900">Itens do Inventário</h3>
        </div>
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Item
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Categoria
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Quantidade
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Preço Unit.
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Fornecedor
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Status
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Ações
                </th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr *ngFor="let item of inventoryItems">
                <td class="px-6 py-4 whitespace-nowrap">
                  <div>
                    <div class="text-sm font-medium text-gray-900">{{ item.name }}</div>
                    <div class="text-sm text-gray-500">{{ item.description }}</div>
                  </div>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {{ item.category }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {{ item.quantity }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  R$ {{ item.unitPrice.toFixed(2) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {{ item.supplier || '-' }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span [class]="getStockStatusClass(item)" class="inline-flex px-2 py-1 text-xs font-semibold rounded-full">
                    {{ getStockStatus(item) }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <button
                    (click)="editItem(item)"
                    class="text-blue-600 hover:text-blue-900 mr-3"
                  >
                    Editar
                  </button>
                  <button
                    (click)="deleteItem(item.id!)"
                    class="text-red-600 hover:text-red-900"
                  >
                    Excluir
                  </button>
                </td>
              </tr>
              <tr *ngIf="inventoryItems.length === 0">
                <td colspan="7" class="px-6 py-4 text-center text-sm text-gray-500">
                  Nenhum item cadastrado
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `
})
export class InventoryComponent implements OnInit {
  inventoryItems: InventoryItem[] = [];
  itemForm: FormGroup;
  showNewItemForm = false;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient
  ) {
    this.itemForm = this.fb.group({
      name: ['', [Validators.required]],
      category: ['', [Validators.required]],
      description: [''],
      quantity: [0, [Validators.required, Validators.min(0)]],
      unitPrice: [0, [Validators.required, Validators.min(0)]],
      supplier: [''],
      expirationDate: [''],
      minimumStock: [0, [Validators.required, Validators.min(0)]]
    });
  }

  ngOnInit(): void {
    this.loadInventoryItems();
  }

  private loadInventoryItems(): void {
    this.http.get<InventoryItem[]>('https://localhost:5001/api/inventory').subscribe({
      next: (data) => {
        this.inventoryItems = data;
      },
      error: (error) => {
        console.error('Erro ao carregar inventário:', error);
        this.inventoryItems = [];
      }
    });
  }

  onSubmit(): void {
    if (this.itemForm.valid) {
      this.isLoading = true;
      const itemData = this.itemForm.value;

      this.http.post<InventoryItem>('https://localhost:5001/api/inventory', itemData).subscribe({
        next: (response) => {
          this.inventoryItems.push(response);
          this.itemForm.reset();
          this.showNewItemForm = false;
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Erro ao criar item:', error);
          this.isLoading = false;
        }
      });
    }
  }

  editItem(item: InventoryItem): void {
    this.itemForm.patchValue(item);
    this.showNewItemForm = true;
  }

  deleteItem(id: number): void {
    if (confirm('Tem certeza que deseja excluir este item?')) {
      this.http.delete(`https://localhost:5001/api/inventory/${id}`).subscribe({
        next: () => {
          this.inventoryItems = this.inventoryItems.filter(i => i.id !== id);
        },
        error: (error) => {
          console.error('Erro ao excluir item:', error);
        }
      });
    }
  }

  getStockStatus(item: InventoryItem): string {
    if (item.quantity <= 0) return 'Sem Estoque';
    if (item.quantity <= item.minimumStock) return 'Estoque Baixo';
    return 'Em Estoque';
  }

  getStockStatusClass(item: InventoryItem): string {
    if (item.quantity <= 0) return 'bg-red-100 text-red-800';
    if (item.quantity <= item.minimumStock) return 'bg-yellow-100 text-yellow-800';
    return 'bg-green-100 text-green-800';
  }
}