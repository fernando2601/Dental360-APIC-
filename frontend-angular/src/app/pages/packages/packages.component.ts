import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

interface PackageItem {
  id?: number;
  name: string;
  description: string;
  category: string;
  originalPrice: number;
  discountPercentage: number;
  finalPrice: number;
  durationDays: number;
  validityDays: number;
  isActive: boolean;
  services: Service[];
  imageUrl: string;
  terms: string;
  maxUsages: number;
  isPopular: boolean;
  totalSavings: number;
  totalPurchases: number;
  totalRevenue: number;
  averageRating: number;
  reviewCount: number;
  formattedOriginalPrice: string;
  formattedFinalPrice: string;
  formattedSavings: string;
  formattedDiscount: string;
  createdAt?: string;
  updatedAt?: string;
}

interface Service {
  id: number;
  name: string;
  category: string;
  price: number;
  duration: number;
  isActive: boolean;
}

interface PackageStats {
  totalPackages: number;
  activePackages: number;
  inactivePackages: number;
  totalRevenue: number;
  averagePackagePrice: number;
  averageDiscount: number;
  categoryBreakdown: CategoryStats[];
  mostPopular: PopularPackage[];
}

interface CategoryStats {
  category: string;
  packageCount: number;
  totalRevenue: number;
  averagePrice: number;
}

interface PopularPackage {
  id: number;
  name: string;
  purchaseCount: number;
  revenue: number;
  rating: number;
}

interface ClinicInfo {
  id: number;
  name: string;
  description: string;
  address: string;
  phone: string;
  email: string;
  website: string;
  logoUrl: string;
  openingHours: string;
  specialties: string;
  socialMedia: any;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

@Component({
  selector: 'app-packages',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  template: `
    <div class="max-w-7xl mx-auto">
      <!-- Header seguindo o design DentalSpa da imagem -->
      <div class="bg-gradient-to-r from-purple-600 to-purple-700 text-white p-6 rounded-lg mb-6">
        <div class="flex justify-between items-center">
          <div>
            <h1 class="text-2xl font-bold">Pacotes</h1>
            <p class="text-purple-100">Gerencie pacotes de serviços com descontos.</p>
          </div>
          <div class="flex space-x-3">
            <button
              (click)="showNewPackageForm = !showNewPackageForm"
              class="bg-white text-purple-600 hover:bg-purple-50 px-4 py-2 rounded-md font-medium transition-colors"
            >
              + Novo Pacote
            </button>
            <button
              (click)="activeTab = 'clinic-info'"
              class="bg-purple-500 hover:bg-purple-400 text-white px-4 py-2 rounded-md font-medium transition-colors"
            >
              🏥 Dados da Clínica
            </button>
          </div>
        </div>
      </div>

      <!-- Navegação das abas -->
      <div class="bg-white shadow rounded-lg mb-6">
        <div class="border-b border-gray-200">
          <nav class="-mb-px flex space-x-8 px-6">
            <button
              (click)="activeTab = 'active'; filterByStatus('active')"
              [class]="activeTab === 'active' ? 'border-purple-500 text-purple-600' : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'"
              class="py-4 px-1 border-b-2 font-medium text-sm"
            >
              📦 Pacotes Ativos
            </button>
            <button
              (click)="activeTab = 'inactive'; filterByStatus('inactive')"
              [class]="activeTab === 'inactive' ? 'border-purple-500 text-purple-600' : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'"
              class="py-4 px-1 border-b-2 font-medium text-sm"
            >
              📋 Pacotes Inativos
            </button>
            <button
              (click)="activeTab = 'stats'"
              [class]="activeTab === 'stats' ? 'border-purple-500 text-purple-600' : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'"
              class="py-4 px-1 border-b-2 font-medium text-sm"
            >
              📊 Estatísticas
            </button>
            <button
              (click)="activeTab = 'clinic-info'"
              [class]="activeTab === 'clinic-info' ? 'border-purple-500 text-purple-600' : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'"
              class="py-4 px-1 border-b-2 font-medium text-sm"
            >
              🏥 Dados da Clínica
            </button>
          </nav>
        </div>

        <!-- Filtros e busca -->
        <div class="p-4" *ngIf="activeTab === 'active' || activeTab === 'inactive'">
          <div class="flex items-center space-x-4">
            <div class="flex-1 relative">
              <input
                type="text"
                [(ngModel)]="searchTerm"
                (input)="onSearch()"
                placeholder="Buscar pacotes..."
                class="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-md focus:ring-purple-500 focus:border-purple-500"
              />
              <div class="absolute inset-y-0 left-0 pl-3 flex items-center">
                <span class="text-gray-400">🔍</span>
              </div>
            </div>
            <select
              [(ngModel)]="selectedCategory"
              (change)="onCategoryChange()"
              class="border border-gray-300 rounded-md px-3 py-2 focus:ring-purple-500 focus:border-purple-500"
            >
              <option value="">Todas Categorias</option>
              <option value="Estética">Estética</option>
              <option value="Harmonização">Harmonização</option>
              <option value="Prevenção">Prevenção</option>
              <option value="Ortodontia">Ortodontia</option>
              <option value="Implantodontia">Implantodontia</option>
            </select>
          </div>
        </div>
      </div>

      <!-- Formulário Novo Pacote -->
      <div *ngIf="showNewPackageForm" class="bg-white shadow rounded-lg p-6 mb-6">
        <h3 class="text-lg font-medium text-gray-900 mb-4">{{ editingPackage ? 'Editar Pacote' : 'Novo Pacote' }}</h3>
        <form [formGroup]="packageForm" (ngSubmit)="onSubmit()">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700">Nome do Pacote</label>
              <input
                type="text"
                formControlName="name"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                placeholder="Ex: Pacote Clareamento Premium"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Categoria</label>
              <select
                formControlName="category"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
              >
                <option value="">Selecione a categoria</option>
                <option value="Estética">Estética</option>
                <option value="Harmonização">Harmonização</option>
                <option value="Prevenção">Prevenção</option>
                <option value="Ortodontia">Ortodontia</option>
                <option value="Implantodontia">Implantodontia</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Preço Original (R$)</label>
              <input
                type="number"
                step="0.01"
                formControlName="originalPrice"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                placeholder="1000.00"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Desconto (%)</label>
              <input
                type="number"
                formControlName="discountPercentage"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                placeholder="15"
                min="0"
                max="100"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Duração (dias)</label>
              <input
                type="number"
                formControlName="durationDays"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                placeholder="120"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Validade (dias)</label>
              <input
                type="number"
                formControlName="validityDays"
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
                placeholder="Descrição detalhada do pacote"
              ></textarea>
            </div>
            <div class="md:col-span-2">
              <label class="block text-sm font-medium text-gray-700">Termos e Condições</label>
              <textarea
                formControlName="terms"
                rows="2"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                placeholder="Válido por X dias. Não cumulativo com outras promoções."
              ></textarea>
            </div>
            <div class="flex items-center space-x-4">
              <label class="flex items-center">
                <input
                  type="checkbox"
                  formControlName="isActive"
                  class="rounded border-gray-300 text-purple-600 shadow-sm focus:border-purple-300 focus:ring focus:ring-purple-200 focus:ring-opacity-50"
                />
                <span class="ml-2 text-sm text-gray-700">Pacote ativo</span>
              </label>
              <label class="flex items-center">
                <input
                  type="checkbox"
                  formControlName="isPopular"
                  class="rounded border-gray-300 text-purple-600 shadow-sm focus:border-purple-300 focus:ring focus:ring-purple-200 focus:ring-opacity-50"
                />
                <span class="ml-2 text-sm text-gray-700">Pacote popular</span>
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
              [disabled]="!packageForm.valid || isLoading"
              class="bg-purple-600 hover:bg-purple-700 text-white px-4 py-2 rounded-md text-sm font-medium disabled:opacity-50"
            >
              {{ isLoading ? 'Salvando...' : (editingPackage ? 'Atualizar' : 'Criar Pacote') }}
            </button>
          </div>
        </form>
      </div>

      <!-- Conteúdo das abas -->
      <div [ngSwitch]="activeTab">
        
        <!-- Pacotes Ativos/Inativos -->
        <div *ngSwitchCase="'active'" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <div *ngFor="let packageItem of filteredPackages" class="bg-white shadow rounded-lg overflow-hidden hover:shadow-xl transition-all duration-300">
            <!-- Header do card com desconto -->
            <div class="relative bg-gradient-to-r from-purple-500 to-purple-600 text-white p-4">
              <div class="flex justify-between items-start">
                <div class="flex-1">
                  <h3 class="font-bold text-lg mb-1">{{ packageItem.name }}</h3>
                  <span class="text-purple-100 text-sm">{{ packageItem.category }}</span>
                </div>
                <div class="bg-white bg-opacity-20 rounded-full px-3 py-1">
                  <span class="text-sm font-bold">{{ packageItem.discountPercentage }}% OFF</span>
                </div>
              </div>
              <div *ngIf="packageItem.isPopular" class="absolute top-2 left-2">
                <span class="bg-yellow-400 text-yellow-900 px-2 py-1 rounded-full text-xs font-bold">⭐ Popular</span>
              </div>
            </div>

            <!-- Conteúdo do card -->
            <div class="p-4">
              <p class="text-gray-600 text-sm mb-4 line-clamp-2">{{ packageItem.description }}</p>
              
              <!-- Preços -->
              <div class="mb-4">
                <div class="flex items-center space-x-2 mb-1">
                  <span class="text-gray-400 line-through text-sm">{{ packageItem.formattedOriginalPrice }}</span>
                  <span class="text-2xl font-bold text-purple-600">{{ packageItem.formattedFinalPrice }}</span>
                </div>
                <p class="text-green-600 text-sm font-medium">Economia: {{ packageItem.formattedSavings }}</p>
              </div>

              <!-- Informações adicionais -->
              <div class="space-y-2 text-sm text-gray-600 mb-4">
                <div class="flex justify-between">
                  <span>Duração:</span>
                  <span>{{ packageItem.durationDays }} minutos</span>
                </div>
                <div class="flex justify-between">
                  <span>Validade:</span>
                  <span>{{ packageItem.validityDays }} dias</span>
                </div>
                <div class="flex justify-between">
                  <span>Serviços:</span>
                  <span>{{ packageItem.services.length }} inclusos</span>
                </div>
              </div>

              <!-- Serviços inclusos -->
              <div class="mb-4">
                <h4 class="text-sm font-medium text-gray-900 mb-2">Serviços Incluídos:</h4>
                <div class="space-y-1">
                  <div *ngFor="let service of packageItem.services.slice(0, 3)" class="flex items-center text-sm">
                    <span class="text-green-500 mr-2">✓</span>
                    <span class="text-gray-700">{{ service.name }}</span>
                  </div>
                  <div *ngIf="packageItem.services.length > 3" class="text-sm text-purple-600">
                    +{{ packageItem.services.length - 3 }} mais serviços
                  </div>
                </div>
              </div>

              <!-- Ações -->
              <div class="flex space-x-2">
                <button
                  (click)="editPackage(packageItem)"
                  class="flex-1 bg-purple-100 hover:bg-purple-200 text-purple-700 px-3 py-2 rounded text-sm font-medium"
                >
                  Editar
                </button>
                <button
                  (click)="togglePackageStatus(packageItem)"
                  [class]="packageItem.isActive ? 'bg-red-100 hover:bg-red-200 text-red-700' : 'bg-green-100 hover:bg-green-200 text-green-700'"
                  class="flex-1 px-3 py-2 rounded text-sm font-medium"
                >
                  {{ packageItem.isActive ? 'Desativar' : 'Ativar' }}
                </button>
              </div>
            </div>
          </div>

          <!-- Card vazio -->
          <div *ngIf="filteredPackages.length === 0" class="col-span-full text-center py-12">
            <div class="text-gray-400 text-lg mb-2">📦</div>
            <p class="text-gray-500">Nenhum pacote encontrado</p>
          </div>
        </div>

        <div *ngSwitchCase="'inactive'">
          <!-- Mesmo layout para pacotes inativos -->
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <div *ngFor="let packageItem of filteredPackages" class="bg-white shadow rounded-lg overflow-hidden opacity-75">
              <!-- Conteúdo similar ao ativo, mas com estilo diferente -->
              <div class="relative bg-gradient-to-r from-gray-400 to-gray-500 text-white p-4">
                <div class="flex justify-between items-start">
                  <div class="flex-1">
                    <h3 class="font-bold text-lg mb-1">{{ packageItem.name }}</h3>
                    <span class="text-gray-200 text-sm">{{ packageItem.category }}</span>
                  </div>
                  <div class="bg-white bg-opacity-20 rounded-full px-3 py-1">
                    <span class="text-sm font-bold">Inativo</span>
                  </div>
                </div>
              </div>
              <div class="p-4">
                <p class="text-gray-600 text-sm mb-4">{{ packageItem.description }}</p>
                <div class="flex space-x-2">
                  <button
                    (click)="editPackage(packageItem)"
                    class="flex-1 bg-gray-100 hover:bg-gray-200 text-gray-700 px-3 py-2 rounded text-sm font-medium"
                  >
                    Editar
                  </button>
                  <button
                    (click)="togglePackageStatus(packageItem)"
                    class="flex-1 bg-green-100 hover:bg-green-200 text-green-700 px-3 py-2 rounded text-sm font-medium"
                  >
                    Ativar
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Aba Estatísticas -->
        <div *ngSwitchCase="'stats'">
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-6">
            <div class="bg-white shadow rounded-lg p-6">
              <div class="flex items-center">
                <div class="flex-shrink-0">
                  <div class="w-8 h-8 bg-purple-500 rounded-md flex items-center justify-center">
                    <span class="text-white text-sm">📦</span>
                  </div>
                </div>
                <div class="ml-5">
                  <p class="text-sm font-medium text-gray-500">Total de Pacotes</p>
                  <p class="text-2xl font-bold text-gray-900">{{ stats?.totalPackages || 0 }}</p>
                </div>
              </div>
            </div>

            <div class="bg-white shadow rounded-lg p-6">
              <div class="flex items-center">
                <div class="flex-shrink-0">
                  <div class="w-8 h-8 bg-green-500 rounded-md flex items-center justify-center">
                    <span class="text-white text-sm">✅</span>
                  </div>
                </div>
                <div class="ml-5">
                  <p class="text-sm font-medium text-gray-500">Pacotes Ativos</p>
                  <p class="text-2xl font-bold text-gray-900">{{ stats?.activePackages || 0 }}</p>
                </div>
              </div>
            </div>

            <div class="bg-white shadow rounded-lg p-6">
              <div class="flex items-center">
                <div class="flex-shrink-0">
                  <div class="w-8 h-8 bg-yellow-500 rounded-md flex items-center justify-center">
                    <span class="text-white text-sm">💰</span>
                  </div>
                </div>
                <div class="ml-5">
                  <p class="text-sm font-medium text-gray-500">Receita Total</p>
                  <p class="text-2xl font-bold text-gray-900">R$ {{ (stats?.totalRevenue || 0).toFixed(2) }}</p>
                </div>
              </div>
            </div>

            <div class="bg-white shadow rounded-lg p-6">
              <div class="flex items-center">
                <div class="flex-shrink-0">
                  <div class="w-8 h-8 bg-blue-500 rounded-md flex items-center justify-center">
                    <span class="text-white text-sm">📊</span>
                  </div>
                </div>
                <div class="ml-5">
                  <p class="text-sm font-medium text-gray-500">Preço Médio</p>
                  <p class="text-2xl font-bold text-gray-900">R$ {{ (stats?.averagePackagePrice || 0).toFixed(2) }}</p>
                </div>
              </div>
            </div>
          </div>

          <div class="bg-white shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 mb-4">Análise Detalhada</h3>
            <p class="text-gray-600">Relatórios detalhados em desenvolvimento...</p>
          </div>
        </div>

        <!-- Aba Dados da Clínica -->
        <div *ngSwitchCase="'clinic-info'">
          <div class="bg-white shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 mb-6">Informações da Clínica</h3>
            
            <form [formGroup]="clinicForm" (ngSubmit)="onSubmitClinic()">
              <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div>
                  <label class="block text-sm font-medium text-gray-700">Nome da Clínica</label>
                  <input
                    type="text"
                    formControlName="name"
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                  />
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700">Email</label>
                  <input
                    type="email"
                    formControlName="email"
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                  />
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700">Telefone</label>
                  <input
                    type="tel"
                    formControlName="phone"
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                  />
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700">Website</label>
                  <input
                    type="url"
                    formControlName="website"
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                  />
                </div>
                <div class="md:col-span-2">
                  <label class="block text-sm font-medium text-gray-700">Endereço</label>
                  <textarea
                    formControlName="address"
                    rows="2"
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                  ></textarea>
                </div>
                <div class="md:col-span-2">
                  <label class="block text-sm font-medium text-gray-700">Descrição</label>
                  <textarea
                    formControlName="description"
                    rows="3"
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                  ></textarea>
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700">Horário de Funcionamento</label>
                  <input
                    type="text"
                    formControlName="openingHours"
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                    placeholder="Segunda a Sexta: 8h às 18h"
                  />
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700">Especialidades</label>
                  <input
                    type="text"
                    formControlName="specialties"
                    class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-purple-500 focus:border-purple-500"
                    placeholder="Odontologia Geral, Estética..."
                  />
                </div>
              </div>
              
              <div class="mt-6 flex justify-end">
                <button
                  type="submit"
                  [disabled]="!clinicForm.valid || isLoadingClinic"
                  class="bg-purple-600 hover:bg-purple-700 text-white px-6 py-2 rounded-md font-medium disabled:opacity-50"
                >
                  {{ isLoadingClinic ? 'Salvando...' : 'Salvar Informações' }}
                </button>
              </div>
            </form>
          </div>
        </div>

      </div>
    </div>
  `
})
export class PackagesComponent implements OnInit {
  packages: PackageItem[] = [];
  filteredPackages: PackageItem[] = [];
  services: Service[] = [];
  packageForm: FormGroup;
  clinicForm: FormGroup;
  showNewPackageForm = false;
  isLoading = false;
  isLoadingClinic = false;
  activeTab = 'active';
  searchTerm = '';
  selectedCategory = '';
  editingPackage: PackageItem | null = null;
  stats: PackageStats | null = null;
  clinicInfo: ClinicInfo | null = null;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient
  ) {
    this.packageForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', [Validators.required, Validators.maxLength(1000)]],
      category: ['', [Validators.required]],
      originalPrice: [0, [Validators.required, Validators.min(0.01)]],
      discountPercentage: [0, [Validators.min(0), Validators.max(100)]],
      durationDays: [120, [Validators.required, Validators.min(1)]],
      validityDays: [30, [Validators.required, Validators.min(1)]],
      terms: [''],
      isActive: [true],
      isPopular: [false],
      serviceIds: [[]]
    });

    this.clinicForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(200)]],
      description: [''],
      address: ['', [Validators.required, Validators.maxLength(500)]],
      phone: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      website: [''],
      openingHours: [''],
      specialties: [''],
      socialMedia: ['']
    });
  }

  ngOnInit(): void {
    this.loadPackages();
    this.loadServices();
    this.loadStats();
    this.loadClinicInfo();
  }

  private loadPackages(): void {
    this.http.get<PackageItem[]>('http://localhost:5000/api/packages').subscribe({
      next: (data) => {
        this.packages = data.map(pkg => ({
          ...pkg,
          formattedOriginalPrice: `R$ ${pkg.originalPrice.toFixed(2)}`,
          formattedFinalPrice: `R$ ${pkg.finalPrice.toFixed(2)}`,
          formattedSavings: `R$ ${(pkg.originalPrice - pkg.finalPrice).toFixed(2)}`,
          formattedDiscount: `${pkg.discountPercentage}% OFF`,
          totalSavings: pkg.originalPrice - pkg.finalPrice
        }));
        this.filterByStatus('active');
      },
      error: (error) => {
        console.error('Erro ao carregar pacotes:', error);
        this.packages = [
          {
            id: 1,
            name: 'Pacote Clareamento Premium',
            description: 'Inclui clareamento a laser e limpeza dental profissional',
            category: 'Estética',
            originalPrice: 1000,
            discountPercentage: 15,
            finalPrice: 850,
            durationDays: 120,
            validityDays: 30,
            isActive: true,
            services: [
              { id: 1, name: 'Limpeza Dental', category: 'Odontológico', price: 200, duration: 60, isActive: true },
              { id: 2, name: 'Clareamento a Laser', category: 'Estética', price: 800, duration: 90, isActive: true }
            ],
            imageUrl: '',
            terms: 'Válido por 30 dias. Não cumulativo com outras promoções.',
            maxUsages: 1,
            isPopular: true,
            totalSavings: 150,
            totalPurchases: 24,
            totalRevenue: 20400,
            averageRating: 4.8,
            reviewCount: 18,
            formattedOriginalPrice: 'R$ 1.000,00',
            formattedFinalPrice: 'R$ 850,00',
            formattedSavings: 'R$ 150,00',
            formattedDiscount: '15% OFF'
          },
          {
            id: 2,
            name: 'Pacote Harmonização Facial',
            description: 'Inclui aplicação de botox e preenchimento labial',
            category: 'Harmonização',
            originalPrice: 2450,
            discountPercentage: 10,
            finalPrice: 2200,
            durationDays: 90,
            validityDays: 60,
            isActive: true,
            services: [
              { id: 3, name: 'Aplicação de Botox', category: 'Harmonização', price: 1500, duration: 45, isActive: true },
              { id: 4, name: 'Preenchimento Labial', category: 'Harmonização', price: 950, duration: 30, isActive: true }
            ],
            imageUrl: '',
            terms: 'Válido por 60 dias. Retoque incluso após 15 dias.',
            maxUsages: 1,
            isPopular: true,
            totalSavings: 250,
            totalPurchases: 12,
            totalRevenue: 26400,
            averageRating: 4.9,
            reviewCount: 10,
            formattedOriginalPrice: 'R$ 2.450,00',
            formattedFinalPrice: 'R$ 2.200,00',
            formattedSavings: 'R$ 250,00',
            formattedDiscount: '10% OFF'
          }
        ];
        this.filterByStatus('active');
      }
    });
  }

  private loadServices(): void {
    this.http.get<Service[]>('http://localhost:5000/api/services').subscribe({
      next: (data) => {
        this.services = data;
      },
      error: (error) => {
        console.error('Erro ao carregar serviços:', error);
      }
    });
  }

  private loadStats(): void {
    this.http.get<PackageStats>('http://localhost:5000/api/packages/stats').subscribe({
      next: (data) => {
        this.stats = data;
      },
      error: (error) => {
        console.error('Erro ao carregar estatísticas:', error);
        this.stats = {
          totalPackages: 4,
          activePackages: 4,
          inactivePackages: 0,
          totalRevenue: 46800,
          averagePackagePrice: 1525,
          averageDiscount: 12.5,
          categoryBreakdown: [],
          mostPopular: []
        };
      }
    });
  }

  private loadClinicInfo(): void {
    this.http.get<ClinicInfo>('http://localhost:5000/api/clinic-info').subscribe({
      next: (data) => {
        this.clinicInfo = data;
        this.clinicForm.patchValue(data);
      },
      error: (error) => {
        console.error('Erro ao carregar informações da clínica:', error);
        const defaultInfo = {
          id: 1,
          name: 'Clínica DentalSpa',
          description: 'Clínica odontológica especializada em tratamentos estéticos e harmonização facial.',
          address: 'Rua das Flores, 456 - Jardim Botânico, São Paulo - SP',
          phone: '(11) 3456-7890',
          email: 'contato@dentalspa.com.br',
          website: 'https://www.dentalspa.com.br',
          logoUrl: '',
          openingHours: 'Segunda a Sexta: 8h às 18h | Sábado: 8h às 12h',
          specialties: 'Odontologia Geral, Estética Dental, Ortodontia, Harmonização Facial',
          socialMedia: '{}',
          isActive: true,
          createdAt: new Date().toISOString(),
          updatedAt: new Date().toISOString()
        };
        this.clinicInfo = defaultInfo;
        this.clinicForm.patchValue(defaultInfo);
      }
    });
  }

  filterByStatus(status: string): void {
    this.activeTab = status;
    const isActive = status === 'active';
    this.filteredPackages = this.packages.filter(pkg => pkg.isActive === isActive);
    this.applyFilters();
  }

  onSearch(): void {
    this.applyFilters();
  }

  onCategoryChange(): void {
    this.applyFilters();
  }

  private applyFilters(): void {
    let filtered = this.packages.filter(pkg => pkg.isActive === (this.activeTab === 'active'));

    if (this.searchTerm.trim()) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(pkg =>
        pkg.name.toLowerCase().includes(term) ||
        pkg.description.toLowerCase().includes(term) ||
        pkg.category.toLowerCase().includes(term)
      );
    }

    if (this.selectedCategory) {
      filtered = filtered.filter(pkg => pkg.category === this.selectedCategory);
    }

    this.filteredPackages = filtered;
  }

  onSubmit(): void {
    if (this.packageForm.valid) {
      this.isLoading = true;
      const packageData = {
        ...this.packageForm.value,
        serviceIds: [1, 2] // Por enquanto fixo, depois implementar seleção de serviços
      };

      const request = this.editingPackage
        ? this.http.put<PackageItem>(`http://localhost:5000/api/packages/${this.editingPackage.id}`, packageData)
        : this.http.post<PackageItem>('http://localhost:5000/api/packages', packageData);

      request.subscribe({
        next: (response) => {
          if (this.editingPackage) {
            const index = this.packages.findIndex(p => p.id === this.editingPackage!.id);
            if (index !== -1) {
              this.packages[index] = response;
            }
          } else {
            this.packages.push(response);
          }
          this.filterByStatus(this.activeTab);
          this.cancelForm();
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Erro ao salvar pacote:', error);
          this.isLoading = false;
        }
      });
    }
  }

  onSubmitClinic(): void {
    if (this.clinicForm.valid) {
      this.isLoadingClinic = true;
      
      this.http.put<ClinicInfo>('http://localhost:5000/api/clinic-info', this.clinicForm.value).subscribe({
        next: (response) => {
          this.clinicInfo = response;
          this.isLoadingClinic = false;
        },
        error: (error) => {
          console.error('Erro ao salvar informações da clínica:', error);
          this.isLoadingClinic = false;
        }
      });
    }
  }

  editPackage(packageItem: PackageItem): void {
    this.editingPackage = packageItem;
    this.packageForm.patchValue(packageItem);
    this.showNewPackageForm = true;
  }

  togglePackageStatus(packageItem: PackageItem): void {
    const updatedPackage = { ...packageItem, isActive: !packageItem.isActive };
    
    this.http.put<PackageItem>(`http://localhost:5000/api/packages/${packageItem.id}`, updatedPackage).subscribe({
      next: (response) => {
        const index = this.packages.findIndex(p => p.id === packageItem.id);
        if (index !== -1) {
          this.packages[index] = response;
        }
        this.filterByStatus(this.activeTab);
      },
      error: (error) => {
        console.error('Erro ao alterar status do pacote:', error);
      }
    });
  }

  cancelForm(): void {
    this.showNewPackageForm = false;
    this.editingPackage = null;
    this.packageForm.reset({
      name: '',
      description: '',
      category: '',
      originalPrice: 0,
      discountPercentage: 0,
      durationDays: 120,
      validityDays: 30,
      terms: '',
      isActive: true,
      isPopular: false,
      serviceIds: []
    });
  }
}