import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

interface Service {
  id?: number;
  name: string;
  category: string;
  description: string;
  price: number;
  durationMinutes: number;
  isActive: boolean;
  createdAt?: string;
  updatedAt?: string;
  formattedPrice?: string;
  formattedDuration?: string;
}

interface ServiceStats {
  totalServices: number;
  activeServices: number;
  inactiveServices: number;
  averagePrice: number;
  averageDuration: number;
  categoryBreakdown: CategoryStats[];
}

interface CategoryStats {
  category: string;
  count: number;
  averagePrice: number;
}

@Component({
  selector: 'app-services',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="max-w-7xl mx-auto">
      <!-- Header com estilo DentalSpa -->
      <div class="bg-gradient-to-r from-purple-600 to-purple-700 text-white p-6 rounded-lg mb-6">
        <div class="flex justify-between items-center">
          <div>
            <h1 class="text-2xl font-bold">Serviços & Fidelização</h1>
            <p class="text-purple-100">Gerenciamento de serviços e programa de fidelidade da clínica.</p>
          </div>
          <button
            (click)="showNewServiceForm = !showNewServiceForm"
            class="bg-white text-purple-600 hover:bg-purple-50 px-4 py-2 rounded-md font-medium transition-colors"
          >
            + Novo Serviço
          </button>
        </div>
      </div>

      <!-- Tabs de navegação -->
      <div class="bg-white shadow rounded-lg mb-6">
        <div class="border-b border-gray-200">
          <nav class="-mb-px flex space-x-8 px-6">
            <button
              (click)="activeTab = 'services'"
              [class]="activeTab === 'services' ? 'border-purple-500 text-purple-600' : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'"
              class="py-4 px-1 border-b-2 font-medium text-sm"
            >
              Serviços
            </button>
            <button
              (click)="activeTab = 'programa'"
              [class]="activeTab === 'programa' ? 'border-purple-500 text-purple-600' : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'"
              class="py-4 px-1 border-b-2 font-medium text-sm"
            >
              Programa de Fidelidade
            </button>
          </nav>
        </div>

        <!-- Filtros por categoria -->
        <div class="p-6 border-b border-gray-200">
          <div class="flex space-x-4">
            <button
              (click)="filterByCategory('Todos')"
              [class]="selectedCategory === 'Todos' ? 'bg-purple-600 text-white' : 'bg-gray-200 text-gray-700 hover:bg-gray-300'"
              class="px-4 py-2 rounded-md text-sm font-medium transition-colors"
            >
              Todos
            </button>
            <button
              (click)="filterByCategory('Odontológico')"
              [class]="selectedCategory === 'Odontológico' ? 'bg-purple-600 text-white' : 'bg-gray-200 text-gray-700 hover:bg-gray-300'"
              class="px-4 py-2 rounded-md text-sm font-medium transition-colors"
            >
              Odontológico
            </button>
            <button
              (click)="filterByCategory('Estética')"
              [class]="selectedCategory === 'Estética' ? 'bg-purple-600 text-white' : 'bg-gray-200 text-gray-700 hover:bg-gray-300'"
              class="px-4 py-2 rounded-md text-sm font-medium transition-colors"
            >
              Estética
            </button>
          </div>
        </div>
      </div>

      <!-- Formulário Novo Serviço -->
      <div *ngIf="showNewServiceForm" class="bg-white shadow rounded-lg p-6 mb-6">
        <h3 class="text-lg font-medium text-gray-900 mb-4">Cadastro de Serviço</h3>
        <form [formGroup]="serviceForm" (ngSubmit)="onSubmit()">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700">Nome do Serviço</label>
              <input
                type="text"
                formControlName="name"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                placeholder="Ex: Limpeza Dental"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Categoria</label>
              <select
                formControlName="category"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
              >
                <option value="">Selecione uma categoria</option>
                <option value="Odontológico">Odontológico</option>
                <option value="Estética">Estética</option>
                <option value="Ortodontia">Ortodontia</option>
                <option value="Cirurgia">Cirurgia</option>
                <option value="Preventivo">Preventivo</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Preço (R$)</label>
              <input
                type="number"
                step="0.01"
                formControlName="price"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                placeholder="0.00"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Duração (minutos)</label>
              <input
                type="number"
                formControlName="durationMinutes"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                placeholder="30"
              />
            </div>
            <div class="md:col-span-2">
              <label class="block text-sm font-medium text-gray-700">Descrição</label>
              <textarea
                formControlName="description"
                rows="3"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                placeholder="Descrição detalhada do serviço"
              ></textarea>
            </div>
            <div>
              <label class="flex items-center">
                <input
                  type="checkbox"
                  formControlName="isActive"
                  class="rounded border-gray-300 text-purple-600 shadow-sm focus:border-purple-300 focus:ring focus:ring-purple-200 focus:ring-opacity-50"
                />
                <span class="ml-2 text-sm text-gray-700">Serviço ativo</span>
              </label>
            </div>
          </div>
          <div class="mt-6 flex justify-end space-x-3">
            <button
              type="button"
              (click)="cancelForm()"
              class="bg-gray-300 hover:bg-gray-400 text-gray-800 px-4 py-2 rounded-md text-sm font-medium"
            >
              Cancelar
            </button>
            <button
              type="submit"
              [disabled]="!serviceForm.valid || isLoading"
              class="bg-purple-600 hover:bg-purple-700 text-white px-4 py-2 rounded-md text-sm font-medium disabled:opacity-50"
            >
              {{ isLoading ? 'Salvando...' : (editingService ? 'Atualizar' : 'Salvar') }}
            </button>
          </div>
        </form>
      </div>

      <!-- Conteúdo das tabs -->
      <div [ngSwitch]="activeTab">
        
        <!-- Tab Serviços -->
        <div *ngSwitchCase="'services'">
          <!-- Grid de Serviços -->
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <div *ngFor="let service of filteredServices" class="bg-white shadow rounded-lg overflow-hidden">
              <!-- Status Badge -->
              <div class="px-4 py-3 border-b border-gray-200 flex justify-between items-center">
                <span [class]="service.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'" 
                      class="inline-flex px-2 py-1 text-xs font-semibold rounded-full">
                  {{ service.isActive ? 'Active' : 'Inactive' }}
                </span>
                <span class="text-sm text-gray-500">{{ service.category }}</span>
              </div>

              <!-- Conteúdo do Card -->
              <div class="p-4">
                <h3 class="text-lg font-semibold text-gray-900 mb-2">{{ service.name }}</h3>
                <p class="text-sm text-gray-600 mb-3">{{ service.description }}</p>
                
                <div class="flex justify-between items-center mb-3">
                  <span class="text-lg font-bold text-purple-600">R$ {{ service.price.toFixed(2) }}</span>
                  <span class="text-sm text-gray-500">⏱️ {{ service.durationMinutes }} minutos</span>
                </div>

                <!-- Ações -->
                <div class="flex space-x-2">
                  <button
                    (click)="editService(service)"
                    class="flex-1 bg-blue-50 text-blue-600 hover:bg-blue-100 px-3 py-2 rounded text-sm font-medium"
                  >
                    ✏️ Edit
                  </button>
                  <button
                    (click)="deleteService(service.id!)"
                    class="flex-1 bg-red-50 text-red-600 hover:bg-red-100 px-3 py-2 rounded text-sm font-medium"
                  >
                    🗑️ Delete
                  </button>
                </div>
              </div>
            </div>

            <!-- Card vazio quando não há serviços -->
            <div *ngIf="filteredServices.length === 0" class="col-span-full text-center py-12">
              <div class="text-gray-400 text-lg mb-2">📋</div>
              <p class="text-gray-500">Nenhum serviço encontrado</p>
            </div>
          </div>
        </div>

        <!-- Tab Programa de Fidelidade -->
        <div *ngSwitchCase="'programa'">
          <div class="bg-white shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 mb-4">Programa de Fidelidade</h3>
            <p class="text-gray-600">Funcionalidade em desenvolvimento...</p>
          </div>
        </div>

      </div>
    </div>
  `
})
export class ServicesComponent implements OnInit {
  services: Service[] = [];
  filteredServices: Service[] = [];
  serviceForm: FormGroup;
  showNewServiceForm = false;
  isLoading = false;
  activeTab = 'services';
  selectedCategory = 'Todos';
  editingService: Service | null = null;
  stats: ServiceStats | null = null;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient
  ) {
    this.serviceForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      category: ['', [Validators.required]],
      description: ['', [Validators.maxLength(1000)]],
      price: [0, [Validators.required, Validators.min(0.01)]],
      durationMinutes: [30, [Validators.required, Validators.min(1), Validators.max(480)]],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.loadServices();
    this.loadStats();
  }

  private loadServices(): void {
    // Conectar com o sistema atual (React/Express) enquanto migra para Angular/.NET
    this.http.get<any[]>('http://localhost:5000/api/services').subscribe({
      next: (data) => {
        // Mapear dados do sistema atual para o formato Angular/.NET
        this.services = data.map(service => ({
          id: service.id,
          name: service.name,
          category: service.category,
          description: service.description,
          price: service.price,
          durationMinutes: service.duration,
          isActive: service.active
        }));
        this.applyFilter();
      },
      error: (error) => {
        console.error('Erro ao carregar serviços:', error);
        this.services = [];
        this.applyFilter();
      }
    });
  }

  private loadStats(): void {
    this.http.get<ServiceStats>('https://localhost:5001/api/service/stats').subscribe({
      next: (data) => {
        this.stats = data;
      },
      error: (error) => {
        console.error('Erro ao carregar estatísticas:', error);
      }
    });
  }

  filterByCategory(category: string): void {
    this.selectedCategory = category;
    this.applyFilter();
  }

  private applyFilter(): void {
    if (this.selectedCategory === 'Todos') {
      this.filteredServices = [...this.services];
    } else {
      this.filteredServices = this.services.filter(s => s.category === this.selectedCategory);
    }
  }

  onSubmit(): void {
    if (this.serviceForm.valid) {
      this.isLoading = true;
      const serviceData = {
        name: this.serviceForm.value.name,
        category: this.serviceForm.value.category,
        description: this.serviceForm.value.description,
        price: this.serviceForm.value.price,
        duration: this.serviceForm.value.durationMinutes,
        active: this.serviceForm.value.isActive
      };

      const request = this.editingService
        ? this.http.put<any>(`http://localhost:5000/api/services/${this.editingService.id}`, serviceData)
        : this.http.post<any>('http://localhost:5000/api/services', serviceData);

      request.subscribe({
        next: (response) => {
          const mappedService = {
            id: response.id,
            name: response.name,
            category: response.category,
            description: response.description,
            price: response.price,
            durationMinutes: response.duration,
            isActive: response.active
          };

          if (this.editingService) {
            const index = this.services.findIndex(s => s.id === this.editingService!.id);
            if (index !== -1) {
              this.services[index] = mappedService;
            }
          } else {
            this.services.push(mappedService);
          }
          this.applyFilter();
          this.cancelForm();
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Erro ao salvar serviço:', error);
          this.isLoading = false;
        }
      });
    }
  }

  editService(service: Service): void {
    this.editingService = service;
    this.serviceForm.patchValue(service);
    this.showNewServiceForm = true;
  }

  deleteService(id: number): void {
    if (confirm('Tem certeza que deseja excluir este serviço?')) {
      this.http.delete(`http://localhost:5000/api/services/${id}`).subscribe({
        next: () => {
          this.services = this.services.filter(s => s.id !== id);
          this.applyFilter();
        },
        error: (error) => {
          console.error('Erro ao excluir serviço:', error);
        }
      });
    }
  }

  cancelForm(): void {
    this.showNewServiceForm = false;
    this.editingService = null;
    this.serviceForm.reset({
      name: '',
      category: '',
      description: '',
      price: 0,
      durationMinutes: 30,
      isActive: true
    });
  }
}