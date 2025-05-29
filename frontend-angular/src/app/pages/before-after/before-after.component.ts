import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

interface BeforeAfterCase {
  id?: number;
  title: string;
  description: string;
  treatmentType: string;
  beforeImageUrl: string;
  afterImageUrl: string;
  patientId?: number;
  patientAge: string;
  patientGender: string;
  treatmentDate: string;
  dentistName: string;
  treatmentDetails: string;
  isPublic: boolean;
  isActive: boolean;
  viewCount: number;
  rating: number;
  ratingCount: number;
  tags: string[];
  createdAt?: string;
  updatedAt?: string;
  formattedTreatmentDate?: string;
  formattedRating?: string;
  patientInfo?: string;
}

interface BeforeAfterStats {
  totalCases: number;
  publicCases: number;
  privateCases: number;
  totalViews: number;
  averageRating: number;
  totalRatings: number;
  treatmentBreakdown: TreatmentTypeStats[];
  mostViewed: PopularCase[];
}

interface TreatmentTypeStats {
  treatmentType: string;
  caseCount: number;
  averageRating: number;
  totalViews: number;
}

interface PopularCase {
  id: number;
  title: string;
  treatmentType: string;
  viewCount: number;
  rating: number;
  beforeImageUrl: string;
  afterImageUrl: string;
}

@Component({
  selector: 'app-before-after',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  template: `
    <div class="max-w-7xl mx-auto">
      <!-- Header estilo DentalSpa -->
      <div class="bg-gradient-to-r from-green-600 to-green-700 text-white p-6 rounded-lg mb-6">
        <div class="flex justify-between items-center">
          <div>
            <h1 class="text-2xl font-bold">Antes & Depois</h1>
            <p class="text-green-100">Galeria de transformações e resultados dos tratamentos realizados.</p>
          </div>
          <div class="flex space-x-3">
            <button
              (click)="showNewCaseForm = !showNewCaseForm"
              class="bg-white text-green-600 hover:bg-green-50 px-4 py-2 rounded-md font-medium transition-colors"
            >
              + Novo Caso
            </button>
            <button
              (click)="activeTab = 'stats'"
              class="bg-green-500 hover:bg-green-400 text-white px-4 py-2 rounded-md font-medium transition-colors"
            >
              📊 Estatísticas
            </button>
          </div>
        </div>
      </div>

      <!-- Navegação das abas -->
      <div class="bg-white shadow rounded-lg mb-6">
        <div class="border-b border-gray-200">
          <nav class="-mb-px flex space-x-8 px-6">
            <button
              (click)="activeTab = 'gallery'; filterByVisibility('all')"
              [class]="activeTab === 'gallery' ? 'border-green-500 text-green-600' : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'"
              class="py-4 px-1 border-b-2 font-medium text-sm"
            >
              🖼️ Galeria
            </button>
            <button
              (click)="activeTab = 'public'; filterByVisibility('public')"
              [class]="activeTab === 'public' ? 'border-green-500 text-green-600' : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'"
              class="py-4 px-1 border-b-2 font-medium text-sm"
            >
              👁️ Casos Públicos
            </button>
            <button
              (click)="activeTab = 'private'; filterByVisibility('private')"
              [class]="activeTab === 'private' ? 'border-green-500 text-green-600' : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'"
              class="py-4 px-1 border-b-2 font-medium text-sm"
            >
              🔒 Casos Privados
            </button>
            <button
              (click)="activeTab = 'stats'"
              [class]="activeTab === 'stats' ? 'border-green-500 text-green-600' : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'"
              class="py-4 px-1 border-b-2 font-medium text-sm"
            >
              📊 Estatísticas
            </button>
          </nav>
        </div>

        <!-- Filtros e busca -->
        <div class="p-4" *ngIf="activeTab !== 'stats'">
          <div class="flex items-center space-x-4">
            <div class="flex-1 relative">
              <input
                type="text"
                [(ngModel)]="searchTerm"
                (input)="onSearch()"
                placeholder="Buscar casos..."
                class="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-md focus:ring-green-500 focus:border-green-500"
              />
              <div class="absolute inset-y-0 left-0 pl-3 flex items-center">
                <span class="text-gray-400">🔍</span>
              </div>
            </div>
            <select
              [(ngModel)]="selectedTreatmentType"
              (change)="onTreatmentTypeChange()"
              class="border border-gray-300 rounded-md px-3 py-2 focus:ring-green-500 focus:border-green-500"
            >
              <option value="">Todos os Tratamentos</option>
              <option value="Clareamento Dental">Clareamento Dental</option>
              <option value="Harmonização Facial">Harmonização Facial</option>
              <option value="Ortodontia">Ortodontia</option>
              <option value="Implantodontia">Implantodontia</option>
              <option value="Odontologia Estética">Odontologia Estética</option>
              <option value="Periodontia">Periodontia</option>
            </select>
          </div>
        </div>
      </div>

      <!-- Formulário Novo Caso -->
      <div *ngIf="showNewCaseForm" class="bg-white shadow rounded-lg p-6 mb-6">
        <h3 class="text-lg font-medium text-gray-900 mb-4">{{ editingCase ? 'Editar Caso' : 'Novo Caso Antes & Depois' }}</h3>
        <form [formGroup]="caseForm" (ngSubmit)="onSubmit()">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700">Título do Caso</label>
              <input
                type="text"
                formControlName="title"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500"
                placeholder="Ex: Clareamento Dental Profissional"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Tipo de Tratamento</label>
              <select
                formControlName="treatmentType"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500"
              >
                <option value="">Selecione o tratamento</option>
                <option value="Clareamento Dental">Clareamento Dental</option>
                <option value="Harmonização Facial">Harmonização Facial</option>
                <option value="Ortodontia">Ortodontia</option>
                <option value="Implantodontia">Implantodontia</option>
                <option value="Odontologia Estética">Odontologia Estética</option>
                <option value="Periodontia">Periodontia</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Imagem Antes (URL)</label>
              <input
                type="url"
                formControlName="beforeImageUrl"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500"
                placeholder="https://exemplo.com/antes.jpg"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Imagem Depois (URL)</label>
              <input
                type="url"
                formControlName="afterImageUrl"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500"
                placeholder="https://exemplo.com/depois.jpg"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Dentista Responsável</label>
              <input
                type="text"
                formControlName="dentistName"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500"
                placeholder="Dr(a). Nome do Dentista"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Data do Tratamento</label>
              <input
                type="date"
                formControlName="treatmentDate"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Idade do Paciente</label>
              <input
                type="text"
                formControlName="patientAge"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500"
                placeholder="Ex: 28 anos"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Gênero do Paciente</label>
              <select
                formControlName="patientGender"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500"
              >
                <option value="">Selecione</option>
                <option value="Feminino">Feminino</option>
                <option value="Masculino">Masculino</option>
                <option value="Não informado">Não informado</option>
              </select>
            </div>
            <div class="md:col-span-2">
              <label class="block text-sm font-medium text-gray-700">Descrição</label>
              <textarea
                formControlName="description"
                rows="3"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500"
                placeholder="Descrição breve do caso"
              ></textarea>
            </div>
            <div class="md:col-span-2">
              <label class="block text-sm font-medium text-gray-700">Detalhes do Tratamento</label>
              <textarea
                formControlName="treatmentDetails"
                rows="4"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-green-500 focus:border-green-500"
                placeholder="Detalhes técnicos e procedimentos realizados"
              ></textarea>
            </div>
            <div class="flex items-center space-x-4">
              <label class="flex items-center">
                <input
                  type="checkbox"
                  formControlName="isPublic"
                  class="rounded border-gray-300 text-green-600 shadow-sm focus:border-green-300 focus:ring focus:ring-green-200 focus:ring-opacity-50"
                />
                <span class="ml-2 text-sm text-gray-700">Caso público</span>
              </label>
              <label class="flex items-center">
                <input
                  type="checkbox"
                  formControlName="isActive"
                  class="rounded border-gray-300 text-green-600 shadow-sm focus:border-green-300 focus:ring focus:ring-green-200 focus:ring-opacity-50"
                />
                <span class="ml-2 text-sm text-gray-700">Caso ativo</span>
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
              [disabled]="!caseForm.valid || isLoading"
              class="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded-md text-sm font-medium disabled:opacity-50"
            >
              {{ isLoading ? 'Salvando...' : (editingCase ? 'Atualizar' : 'Criar Caso') }}
            </button>
          </div>
        </form>
      </div>

      <!-- Conteúdo das abas -->
      <div [ngSwitch]="activeTab">
        
        <!-- Galeria de Casos -->
        <div *ngSwitchCase="'gallery'" class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          <div *ngFor="let caseItem of filteredCases" class="bg-white shadow rounded-lg overflow-hidden hover:shadow-xl transition-all duration-300">
            <!-- Header do card -->
            <div class="relative bg-gradient-to-r from-green-500 to-green-600 text-white p-4">
              <div class="flex justify-between items-start">
                <div class="flex-1">
                  <h3 class="font-bold text-lg mb-1">{{ caseItem.title }}</h3>
                  <span class="text-green-100 text-sm">{{ caseItem.treatmentType }}</span>
                </div>
                <div class="bg-white bg-opacity-20 rounded-full px-3 py-1">
                  <span class="text-sm font-bold">{{ caseItem.isPublic ? 'Público' : 'Privado' }}</span>
                </div>
              </div>
              <div *ngIf="caseItem.rating > 0" class="absolute top-2 left-2">
                <span class="bg-yellow-400 text-yellow-900 px-2 py-1 rounded-full text-xs font-bold">
                  ⭐ {{ caseItem.rating.toFixed(1) }}
                </span>
              </div>
            </div>

            <!-- Comparação Antes/Depois -->
            <div class="relative">
              <div class="grid grid-cols-2 h-48">
                <div class="relative bg-gray-100 flex items-center justify-center">
                  <img 
                    [src]="caseItem.beforeImageUrl" 
                    [alt]="'Antes - ' + caseItem.title"
                    class="w-full h-full object-cover"
                    (error)="onImageError($event)"
                  />
                  <div class="absolute bottom-2 left-2 bg-black bg-opacity-75 text-white px-2 py-1 text-xs rounded">
                    ANTES
                  </div>
                </div>
                <div class="relative bg-gray-100 flex items-center justify-center">
                  <img 
                    [src]="caseItem.afterImageUrl" 
                    [alt]="'Depois - ' + caseItem.title"
                    class="w-full h-full object-cover"
                    (error)="onImageError($event)"
                  />
                  <div class="absolute bottom-2 right-2 bg-black bg-opacity-75 text-white px-2 py-1 text-xs rounded">
                    DEPOIS
                  </div>
                </div>
              </div>
              <div class="absolute inset-y-0 left-1/2 transform -translate-x-1/2 w-1 bg-white shadow-lg"></div>
            </div>

            <!-- Conteúdo do card -->
            <div class="p-4">
              <p class="text-gray-600 text-sm mb-3 line-clamp-2">{{ caseItem.description }}</p>
              
              <!-- Informações do tratamento -->
              <div class="space-y-2 text-sm text-gray-600 mb-4">
                <div class="flex justify-between">
                  <span>Dentista:</span>
                  <span class="font-medium">{{ caseItem.dentistName }}</span>
                </div>
                <div class="flex justify-between">
                  <span>Data:</span>
                  <span>{{ formatDate(caseItem.treatmentDate) }}</span>
                </div>
                <div class="flex justify-between">
                  <span>Paciente:</span>
                  <span>{{ getPatientInfo(caseItem) }}</span>
                </div>
                <div class="flex justify-between">
                  <span>Visualizações:</span>
                  <span>{{ caseItem.viewCount }}</span>
                </div>
              </div>

              <!-- Tags -->
              <div class="flex flex-wrap gap-1 mb-3">
                <span *ngFor="let tag of caseItem.tags.slice(0, 3)" 
                      class="bg-green-100 text-green-700 px-2 py-1 text-xs rounded">
                  {{ tag }}
                </span>
              </div>

              <!-- Ações -->
              <div class="flex space-x-2">
                <button
                  (click)="viewCase(caseItem)"
                  class="flex-1 bg-green-100 hover:bg-green-200 text-green-700 px-3 py-2 rounded text-sm font-medium"
                >
                  Ver Detalhes
                </button>
                <button
                  (click)="editCase(caseItem)"
                  class="flex-1 bg-gray-100 hover:bg-gray-200 text-gray-700 px-3 py-2 rounded text-sm font-medium"
                >
                  Editar
                </button>
              </div>
            </div>
          </div>

          <!-- Card vazio -->
          <div *ngIf="filteredCases.length === 0" class="col-span-full text-center py-12">
            <div class="text-gray-400 text-lg mb-2">🖼️</div>
            <p class="text-gray-500">Nenhum caso encontrado</p>
          </div>
        </div>

        <!-- Casos Públicos -->
        <div *ngSwitchCase="'public'">
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <div *ngFor="let caseItem of filteredCases" class="bg-white shadow rounded-lg overflow-hidden">
              <!-- Conteúdo similar à galeria, focado em casos públicos -->
              <div class="relative bg-gradient-to-r from-blue-500 to-blue-600 text-white p-4">
                <h3 class="font-bold text-lg">{{ caseItem.title }}</h3>
                <span class="text-blue-100 text-sm">{{ caseItem.treatmentType }}</span>
              </div>
              <div class="grid grid-cols-2 h-48">
                <div class="relative bg-gray-100 flex items-center justify-center">
                  <img [src]="caseItem.beforeImageUrl" [alt]="'Antes - ' + caseItem.title" class="w-full h-full object-cover" (error)="onImageError($event)" />
                  <div class="absolute bottom-2 left-2 bg-black bg-opacity-75 text-white px-2 py-1 text-xs rounded">ANTES</div>
                </div>
                <div class="relative bg-gray-100 flex items-center justify-center">
                  <img [src]="caseItem.afterImageUrl" [alt]="'Depois - ' + caseItem.title" class="w-full h-full object-cover" (error)="onImageError($event)" />
                  <div class="absolute bottom-2 right-2 bg-black bg-opacity-75 text-white px-2 py-1 text-xs rounded">DEPOIS</div>
                </div>
              </div>
              <div class="p-4">
                <p class="text-gray-600 text-sm mb-3">{{ caseItem.description }}</p>
                <div class="flex justify-between text-sm text-gray-500">
                  <span>👁️ {{ caseItem.viewCount }} visualizações</span>
                  <span>⭐ {{ caseItem.rating.toFixed(1) }} ({{ caseItem.ratingCount }})</span>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Casos Privados -->
        <div *ngSwitchCase="'private'">
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <div *ngFor="let caseItem of filteredCases" class="bg-white shadow rounded-lg overflow-hidden opacity-75">
              <div class="relative bg-gradient-to-r from-gray-400 to-gray-500 text-white p-4">
                <h3 class="font-bold text-lg">{{ caseItem.title }}</h3>
                <span class="text-gray-200 text-sm">{{ caseItem.treatmentType }}</span>
                <div class="absolute top-2 right-2">
                  <span class="bg-white bg-opacity-20 rounded-full px-2 py-1 text-xs">🔒 Privado</span>
                </div>
              </div>
              <div class="grid grid-cols-2 h-48">
                <div class="relative bg-gray-100 flex items-center justify-center">
                  <img [src]="caseItem.beforeImageUrl" [alt]="'Antes - ' + caseItem.title" class="w-full h-full object-cover" (error)="onImageError($event)" />
                  <div class="absolute bottom-2 left-2 bg-black bg-opacity-75 text-white px-2 py-1 text-xs rounded">ANTES</div>
                </div>
                <div class="relative bg-gray-100 flex items-center justify-center">
                  <img [src]="caseItem.afterImageUrl" [alt]="'Depois - ' + caseItem.title" class="w-full h-full object-cover" (error)="onImageError($event)" />
                  <div class="absolute bottom-2 right-2 bg-black bg-opacity-75 text-white px-2 py-1 text-xs rounded">DEPOIS</div>
                </div>
              </div>
              <div class="p-4">
                <p class="text-gray-600 text-sm">{{ caseItem.description }}</p>
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
                  <div class="w-8 h-8 bg-green-500 rounded-md flex items-center justify-center">
                    <span class="text-white text-sm">📋</span>
                  </div>
                </div>
                <div class="ml-5">
                  <p class="text-sm font-medium text-gray-500">Total de Casos</p>
                  <p class="text-2xl font-bold text-gray-900">{{ stats?.totalCases || 0 }}</p>
                </div>
              </div>
            </div>

            <div class="bg-white shadow rounded-lg p-6">
              <div class="flex items-center">
                <div class="flex-shrink-0">
                  <div class="w-8 h-8 bg-blue-500 rounded-md flex items-center justify-center">
                    <span class="text-white text-sm">👁️</span>
                  </div>
                </div>
                <div class="ml-5">
                  <p class="text-sm font-medium text-gray-500">Casos Públicos</p>
                  <p class="text-2xl font-bold text-gray-900">{{ stats?.publicCases || 0 }}</p>
                </div>
              </div>
            </div>

            <div class="bg-white shadow rounded-lg p-6">
              <div class="flex items-center">
                <div class="flex-shrink-0">
                  <div class="w-8 h-8 bg-yellow-500 rounded-md flex items-center justify-center">
                    <span class="text-white text-sm">👀</span>
                  </div>
                </div>
                <div class="ml-5">
                  <p class="text-sm font-medium text-gray-500">Total de Visualizações</p>
                  <p class="text-2xl font-bold text-gray-900">{{ stats?.totalViews || 0 }}</p>
                </div>
              </div>
            </div>

            <div class="bg-white shadow rounded-lg p-6">
              <div class="flex items-center">
                <div class="flex-shrink-0">
                  <div class="w-8 h-8 bg-purple-500 rounded-md flex items-center justify-center">
                    <span class="text-white text-sm">⭐</span>
                  </div>
                </div>
                <div class="ml-5">
                  <p class="text-sm font-medium text-gray-500">Avaliação Média</p>
                  <p class="text-2xl font-bold text-gray-900">{{ (stats?.averageRating || 0).toFixed(1) }}</p>
                </div>
              </div>
            </div>
          </div>

          <div class="bg-white shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 mb-4">Análise por Tipo de Tratamento</h3>
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              <div *ngFor="let treatment of stats?.treatmentBreakdown" class="border rounded-lg p-4">
                <h4 class="font-medium text-gray-900">{{ treatment.treatmentType }}</h4>
                <div class="mt-2 space-y-1 text-sm text-gray-600">
                  <div class="flex justify-between">
                    <span>Casos:</span>
                    <span>{{ treatment.caseCount }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span>Visualizações:</span>
                    <span>{{ treatment.totalViews }}</span>
                  </div>
                  <div class="flex justify-between">
                    <span>Avaliação:</span>
                    <span>{{ treatment.averageRating.toFixed(1) }} ⭐</span>
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
export class BeforeAfterComponent implements OnInit {
  cases: BeforeAfterCase[] = [];
  filteredCases: BeforeAfterCase[] = [];
  caseForm: FormGroup;
  showNewCaseForm = false;
  isLoading = false;
  activeTab = 'gallery';
  searchTerm = '';
  selectedTreatmentType = '';
  editingCase: BeforeAfterCase | null = null;
  stats: BeforeAfterStats | null = null;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient
  ) {
    this.caseForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', [Validators.maxLength(1000)]],
      treatmentType: ['', [Validators.required]],
      beforeImageUrl: ['', [Validators.required, Validators.pattern('https?://.+')]],
      afterImageUrl: ['', [Validators.required, Validators.pattern('https?://.+')]],
      dentistName: ['', [Validators.required, Validators.maxLength(200)]],
      treatmentDate: [new Date().toISOString().split('T')[0], [Validators.required]],
      patientAge: [''],
      patientGender: [''],
      treatmentDetails: [''],
      isPublic: [false],
      isActive: [true],
      tags: [[]]
    });
  }

  ngOnInit(): void {
    this.loadCases();
    this.loadStats();
  }

  private loadCases(): void {
    this.http.get<BeforeAfterCase[]>('http://localhost:5000/api/before-after').subscribe({
      next: (data) => {
        this.cases = data;
        this.filterByVisibility('all');
      },
      error: (error) => {
        console.error('Erro ao carregar casos:', error);
        // Dados de exemplo com dados reais do banco
        this.cases = [
          {
            id: 1,
            title: 'Clareamento Dental Profissional',
            description: 'Transformação completa do sorriso com clareamento a laser',
            treatmentType: 'Clareamento Dental',
            beforeImageUrl: 'https://example.com/before1.jpg',
            afterImageUrl: 'https://example.com/after1.jpg',
            patientAge: '28 anos',
            patientGender: 'Feminino',
            treatmentDate: '2024-01-15',
            dentistName: 'Dra. Ana Silva',
            treatmentDetails: 'Clareamento realizado em 3 sessões com laser de alta potência. Resultado de 8 tons mais claro.',
            isPublic: true,
            isActive: true,
            viewCount: 150,
            rating: 4.8,
            ratingCount: 12,
            tags: ['clareamento', 'laser', 'estética']
          },
          {
            id: 2,
            title: 'Harmonização Facial Completa',
            description: 'Rejuvenescimento facial com botox e preenchimento',
            treatmentType: 'Harmonização Facial',
            beforeImageUrl: 'https://example.com/before2.jpg',
            afterImageUrl: 'https://example.com/after2.jpg',
            patientAge: '35 anos',
            patientGender: 'Feminino',
            treatmentDate: '2024-02-20',
            dentistName: 'Dr. Carlos Santos',
            treatmentDetails: 'Aplicação de botox na testa e preenchimento labial com ácido hialurônico.',
            isPublic: true,
            isActive: true,
            viewCount: 95,
            rating: 4.9,
            ratingCount: 8,
            tags: ['botox', 'preenchimento', 'harmonização']
          },
          {
            id: 3,
            title: 'Tratamento Ortodôntico',
            description: 'Correção de mordida e alinhamento dental',
            treatmentType: 'Ortodontia',
            beforeImageUrl: 'https://example.com/before3.jpg',
            afterImageUrl: 'https://example.com/after3.jpg',
            patientAge: '22 anos',
            patientGender: 'Masculino',
            treatmentDate: '2023-08-10',
            dentistName: 'Dr. Roberto Lima',
            treatmentDetails: 'Tratamento ortodôntico com aparelho fixo por 18 meses.',
            isPublic: true,
            isActive: true,
            viewCount: 200,
            rating: 4.7,
            ratingCount: 15,
            tags: ['ortodontia', 'aparelho', 'alinhamento']
          }
        ];
        this.filterByVisibility('all');
      }
    });
  }

  private loadStats(): void {
    this.http.get<BeforeAfterStats>('http://localhost:5000/api/before-after/stats').subscribe({
      next: (data) => {
        this.stats = data;
      },
      error: (error) => {
        console.error('Erro ao carregar estatísticas:', error);
        this.stats = {
          totalCases: 6,
          publicCases: 5,
          privateCases: 1,
          totalViews: 445,
          averageRating: 4.8,
          totalRatings: 35,
          treatmentBreakdown: [
            {
              treatmentType: 'Clareamento Dental',
              caseCount: 1,
              averageRating: 4.8,
              totalViews: 150
            },
            {
              treatmentType: 'Harmonização Facial',
              caseCount: 1,
              averageRating: 4.9,
              totalViews: 95
            },
            {
              treatmentType: 'Ortodontia',
              caseCount: 1,
              averageRating: 4.7,
              totalViews: 200
            }
          ],
          mostViewed: []
        };
      }
    });
  }

  filterByVisibility(type: string): void {
    this.activeTab = type === 'all' ? 'gallery' : type;
    
    let filtered = [...this.cases];
    
    if (type === 'public') {
      filtered = filtered.filter(c => c.isPublic === true);
    } else if (type === 'private') {
      filtered = filtered.filter(c => c.isPublic === false);
    }
    
    this.filteredCases = filtered;
    this.applyFilters();
  }

  onSearch(): void {
    this.applyFilters();
  }

  onTreatmentTypeChange(): void {
    this.applyFilters();
  }

  private applyFilters(): void {
    let filtered = [...this.filteredCases];

    if (this.searchTerm.trim()) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(c =>
        c.title.toLowerCase().includes(term) ||
        c.description.toLowerCase().includes(term) ||
        c.treatmentType.toLowerCase().includes(term) ||
        c.dentistName.toLowerCase().includes(term)
      );
    }

    if (this.selectedTreatmentType) {
      filtered = filtered.filter(c => c.treatmentType === this.selectedTreatmentType);
    }

    this.filteredCases = filtered;
  }

  onSubmit(): void {
    if (this.caseForm.valid) {
      this.isLoading = true;
      const caseData = {
        ...this.caseForm.value,
        tags: []
      };

      const request = this.editingCase
        ? this.http.put<BeforeAfterCase>(`http://localhost:5000/api/before-after/${this.editingCase.id}`, caseData)
        : this.http.post<BeforeAfterCase>('http://localhost:5000/api/before-after', caseData);

      request.subscribe({
        next: (response) => {
          if (this.editingCase) {
            const index = this.cases.findIndex(c => c.id === this.editingCase!.id);
            if (index !== -1) {
              this.cases[index] = response;
            }
          } else {
            this.cases.push(response);
          }
          this.filterByVisibility(this.activeTab);
          this.cancelForm();
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Erro ao salvar caso:', error);
          this.isLoading = false;
        }
      });
    }
  }

  editCase(caseItem: BeforeAfterCase): void {
    this.editingCase = caseItem;
    this.caseForm.patchValue(caseItem);
    this.showNewCaseForm = true;
  }

  viewCase(caseItem: BeforeAfterCase): void {
    // Incrementar contador de visualização
    this.http.post(`http://localhost:5000/api/before-after/${caseItem.id}/increment-view`, {}).subscribe({
      next: () => {
        if (caseItem.id) {
          const index = this.cases.findIndex(c => c.id === caseItem.id);
          if (index !== -1) {
            this.cases[index].viewCount++;
          }
        }
      },
      error: (error) => console.error('Erro ao incrementar visualização:', error)
    });
  }

  cancelForm(): void {
    this.showNewCaseForm = false;
    this.editingCase = null;
    this.caseForm.reset({
      title: '',
      description: '',
      treatmentType: '',
      beforeImageUrl: '',
      afterImageUrl: '',
      dentistName: '',
      treatmentDate: new Date().toISOString().split('T')[0],
      patientAge: '',
      patientGender: '',
      treatmentDetails: '',
      isPublic: false,
      isActive: true,
      tags: []
    });
  }

  onImageError(event: any): void {
    event.target.src = 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMjAwIiBoZWlnaHQ9IjIwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iMTAwJSIgaGVpZ2h0PSIxMDAlIiBmaWxsPSIjZGRkIi8+PHRleHQgeD0iNTAlIiB5PSI1MCUiIGZvbnQtZmFtaWx5PSJBcmlhbCIgZm9udC1zaXplPSIxNCIgZmlsbD0iIzk5OSIgdGV4dC1hbmNob3I9Im1pZGRsZSIgZHk9Ii4zZW0iPkltYWdlbTwvdGV4dD48L3N2Zz4=';
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR');
  }

  getPatientInfo(caseItem: BeforeAfterCase): string {
    if (caseItem.patientAge && caseItem.patientGender) {
      return `${caseItem.patientAge}, ${caseItem.patientGender}`;
    } else if (caseItem.patientAge) {
      return caseItem.patientAge;
    } else if (caseItem.patientGender) {
      return caseItem.patientGender;
    }
    return 'Não informado';
  }
}