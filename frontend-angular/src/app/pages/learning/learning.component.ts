import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

interface LearningContent {
  id?: number;
  title: string;
  description: string;
  contentType: string;
  category: string;
  difficulty: string;
  contentUrl: string;
  thumbnailUrl: string;
  durationMinutes: number;
  isActive: boolean;
  createdBy: number;
  createdByName?: string;
  createdAt?: string;
  updatedAt?: string;
  tags: string[];
  rating: number;
  totalRatings: number;
  viewCount: number;
  completionCount: number;
  formattedDuration?: string;
  formattedRating?: string;
}

interface LearningStats {
  totalContent: number;
  activeContent: number;
  totalUsers: number;
  activeUsers: number;
  averageCompletionRate: number;
  totalViewTime: number;
  categoryBreakdown: CategoryStats[];
  mostPopular: PopularContent[];
}

interface CategoryStats {
  category: string;
  contentCount: number;
  viewCount: number;
  completionRate: number;
}

interface PopularContent {
  id: number;
  title: string;
  viewCount: number;
  rating: number;
}

@Component({
  selector: 'app-learning',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  template: `
    <div class="max-w-7xl mx-auto">
      <!-- Header com estilo DentalSpa -->
      <div class="bg-gradient-to-r from-blue-600 to-blue-700 text-white p-6 rounded-lg mb-6">
        <div class="flex justify-between items-center">
          <div>
            <h1 class="text-2xl font-bold">Área de Aprendizado</h1>
            <p class="text-blue-100">Centro de conhecimento e desenvolvimento profissional da equipe.</p>
          </div>
          <div class="flex space-x-3">
            <button
              (click)="showNewContentForm = !showNewContentForm"
              class="bg-white text-blue-600 hover:bg-blue-50 px-4 py-2 rounded-md font-medium transition-colors"
            >
              + Novo Conteúdo
            </button>
            <button
              (click)="activeTab = 'stats'"
              class="bg-blue-500 hover:bg-blue-400 text-white px-4 py-2 rounded-md font-medium transition-colors"
            >
              📊 Estatísticas
            </button>
          </div>
        </div>
      </div>

      <!-- Barra de Navegação -->
      <div class="bg-white shadow rounded-lg mb-6">
        <div class="border-b border-gray-200">
          <nav class="-mb-px flex space-x-8 px-6">
            <button
              (click)="activeTab = 'content'"
              [class]="activeTab === 'content' ? 'border-blue-500 text-blue-600' : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'"
              class="py-4 px-1 border-b-2 font-medium text-sm"
            >
              📚 Conteúdo
            </button>
            <button
              (click)="activeTab = 'progress'"
              [class]="activeTab === 'progress' ? 'border-blue-500 text-blue-600' : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'"
              class="py-4 px-1 border-b-2 font-medium text-sm"
            >
              📈 Meu Progresso
            </button>
            <button
              (click)="activeTab = 'stats'"
              [class]="activeTab === 'stats' ? 'border-blue-500 text-blue-600' : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'"
              class="py-4 px-1 border-b-2 font-medium text-sm"
            >
              📊 Estatísticas
            </button>
          </nav>
        </div>

        <!-- Barra de Busca e Filtros -->
        <div class="p-4" *ngIf="activeTab === 'content'">
          <div class="flex items-center space-x-4">
            <div class="flex-1 relative">
              <input
                type="text"
                [(ngModel)]="searchTerm"
                (input)="onSearch()"
                placeholder="Buscar conteúdo..."
                class="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-md focus:ring-blue-500 focus:border-blue-500"
              />
              <div class="absolute inset-y-0 left-0 pl-3 flex items-center">
                <span class="text-gray-400">🔍</span>
              </div>
            </div>
            <select
              [(ngModel)]="selectedCategory"
              (change)="onCategoryChange()"
              class="border border-gray-300 rounded-md px-3 py-2 focus:ring-blue-500 focus:border-blue-500"
            >
              <option value="">Todas Categorias</option>
              <option value="Odontologia Básica">Odontologia Básica</option>
              <option value="Anatomia">Anatomia</option>
              <option value="Biossegurança">Biossegurança</option>
              <option value="Ortodontia">Ortodontia</option>
              <option value="Estética">Estética</option>
              <option value="Emergência">Emergência</option>
            </select>
            <select
              [(ngModel)]="selectedDifficulty"
              (change)="onDifficultyChange()"
              class="border border-gray-300 rounded-md px-3 py-2 focus:ring-blue-500 focus:border-blue-500"
            >
              <option value="">Todas Dificuldades</option>
              <option value="beginner">Iniciante</option>
              <option value="intermediate">Intermediário</option>
              <option value="advanced">Avançado</option>
            </select>
          </div>
        </div>
      </div>

      <!-- Formulário Novo Conteúdo -->
      <div *ngIf="showNewContentForm" class="bg-white shadow rounded-lg p-6 mb-6">
        <h3 class="text-lg font-medium text-gray-900 mb-4">Novo Conteúdo de Aprendizado</h3>
        <form [formGroup]="contentForm" (ngSubmit)="onSubmit()">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700">Título</label>
              <input
                type="text"
                formControlName="title"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
                placeholder="Título do conteúdo"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Tipo de Conteúdo</label>
              <select
                formControlName="contentType"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="">Selecione o tipo</option>
                <option value="video">Vídeo</option>
                <option value="article">Artigo</option>
                <option value="course">Curso</option>
                <option value="quiz">Quiz</option>
                <option value="document">Documento</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Categoria</label>
              <select
                formControlName="category"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="">Selecione a categoria</option>
                <option value="Odontologia Básica">Odontologia Básica</option>
                <option value="Anatomia">Anatomia</option>
                <option value="Biossegurança">Biossegurança</option>
                <option value="Ortodontia">Ortodontia</option>
                <option value="Estética">Estética</option>
                <option value="Emergência">Emergência</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Dificuldade</label>
              <select
                formControlName="difficulty"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="">Selecione a dificuldade</option>
                <option value="beginner">Iniciante</option>
                <option value="intermediate">Intermediário</option>
                <option value="advanced">Avançado</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">URL do Conteúdo</label>
              <input
                type="url"
                formControlName="contentUrl"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
                placeholder="https://exemplo.com/conteudo"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Duração (minutos)</label>
              <input
                type="number"
                formControlName="durationMinutes"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
                placeholder="30"
              />
            </div>
            <div class="md:col-span-2">
              <label class="block text-sm font-medium text-gray-700">Descrição</label>
              <textarea
                formControlName="description"
                rows="3"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
                placeholder="Descrição detalhada do conteúdo"
              ></textarea>
            </div>
            <div>
              <label class="flex items-center">
                <input
                  type="checkbox"
                  formControlName="isActive"
                  class="rounded border-gray-300 text-blue-600 shadow-sm focus:border-blue-300 focus:ring focus:ring-blue-200 focus:ring-opacity-50"
                />
                <span class="ml-2 text-sm text-gray-700">Conteúdo ativo</span>
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
              [disabled]="!contentForm.valid || isLoading"
              class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md text-sm font-medium disabled:opacity-50"
            >
              {{ isLoading ? 'Salvando...' : 'Salvar Conteúdo' }}
            </button>
          </div>
        </form>
      </div>

      <!-- Conteúdo das abas -->
      <div [ngSwitch]="activeTab">
        
        <!-- Aba Conteúdo -->
        <div *ngSwitchCase="'content'">
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            <div *ngFor="let content of filteredContent" class="bg-white shadow rounded-lg overflow-hidden hover:shadow-lg transition-shadow">
              <!-- Thumbnail e badges -->
              <div class="relative">
                <div class="h-48 bg-gradient-to-br from-blue-100 to-blue-200 flex items-center justify-center">
                  <span class="text-blue-600 text-4xl">{{ getContentTypeIcon(content.contentType) }}</span>
                </div>
                <div class="absolute top-2 right-2 flex space-x-1">
                  <span [class]="getDifficultyClass(content.difficulty)" class="px-2 py-1 text-xs font-semibold rounded-full">
                    {{ getDifficultyLabel(content.difficulty) }}
                  </span>
                </div>
                <div class="absolute bottom-2 left-2">
                  <span class="bg-black bg-opacity-75 text-white px-2 py-1 text-xs rounded">
                    {{ content.durationMinutes }} min
                  </span>
                </div>
              </div>

              <!-- Conteúdo do card -->
              <div class="p-4">
                <div class="flex items-center justify-between mb-2">
                  <span class="text-xs text-blue-600 font-medium">{{ content.category }}</span>
                  <div class="flex items-center text-xs text-gray-500">
                    <span class="mr-1">⭐</span>
                    <span>{{ content.rating.toFixed(1) }} ({{ content.totalRatings }})</span>
                  </div>
                </div>
                
                <h3 class="text-lg font-semibold text-gray-900 mb-2 line-clamp-2">{{ content.title }}</h3>
                <p class="text-sm text-gray-600 mb-3 line-clamp-2">{{ content.description }}</p>
                
                <div class="flex items-center justify-between text-xs text-gray-500 mb-3">
                  <span>👁️ {{ content.viewCount }} visualizações</span>
                  <span>✅ {{ content.completionCount }} conclusões</span>
                </div>

                <!-- Tags -->
                <div class="flex flex-wrap gap-1 mb-3">
                  <span *ngFor="let tag of content.tags.slice(0, 3)" 
                        class="bg-gray-100 text-gray-700 px-2 py-1 text-xs rounded">
                    {{ tag }}
                  </span>
                </div>

                <!-- Ações -->
                <div class="flex space-x-2">
                  <button
                    (click)="viewContent(content)"
                    class="flex-1 bg-blue-600 hover:bg-blue-700 text-white px-3 py-2 rounded text-sm font-medium"
                  >
                    Acessar
                  </button>
                  <button
                    (click)="editContent(content)"
                    class="flex-1 bg-gray-100 hover:bg-gray-200 text-gray-700 px-3 py-2 rounded text-sm font-medium"
                  >
                    Editar
                  </button>
                </div>
              </div>
            </div>

            <!-- Card vazio -->
            <div *ngIf="filteredContent.length === 0" class="col-span-full text-center py-12">
              <div class="text-gray-400 text-lg mb-2">📚</div>
              <p class="text-gray-500">Nenhum conteúdo encontrado</p>
            </div>
          </div>
        </div>

        <!-- Aba Progresso -->
        <div *ngSwitchCase="'progress'">
          <div class="bg-white shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 mb-4">Meu Progresso de Aprendizado</h3>
            <p class="text-gray-600">Funcionalidade de progresso em desenvolvimento...</p>
          </div>
        </div>

        <!-- Aba Estatísticas -->
        <div *ngSwitchCase="'stats'">
          <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-6">
            <div class="bg-white shadow rounded-lg p-6">
              <div class="flex items-center">
                <div class="flex-shrink-0">
                  <div class="w-8 h-8 bg-blue-500 rounded-md flex items-center justify-center">
                    <span class="text-white text-sm">📚</span>
                  </div>
                </div>
                <div class="ml-5">
                  <p class="text-sm font-medium text-gray-500">Total de Conteúdo</p>
                  <p class="text-2xl font-bold text-gray-900">{{ stats?.totalContent || 0 }}</p>
                </div>
              </div>
            </div>

            <div class="bg-white shadow rounded-lg p-6">
              <div class="flex items-center">
                <div class="flex-shrink-0">
                  <div class="w-8 h-8 bg-green-500 rounded-md flex items-center justify-center">
                    <span class="text-white text-sm">👥</span>
                  </div>
                </div>
                <div class="ml-5">
                  <p class="text-sm font-medium text-gray-500">Usuários Ativos</p>
                  <p class="text-2xl font-bold text-gray-900">{{ stats?.activeUsers || 0 }}</p>
                </div>
              </div>
            </div>

            <div class="bg-white shadow rounded-lg p-6">
              <div class="flex items-center">
                <div class="flex-shrink-0">
                  <div class="w-8 h-8 bg-yellow-500 rounded-md flex items-center justify-center">
                    <span class="text-white text-sm">📈</span>
                  </div>
                </div>
                <div class="ml-5">
                  <p class="text-sm font-medium text-gray-500">Taxa de Conclusão</p>
                  <p class="text-2xl font-bold text-gray-900">{{ (stats?.averageCompletionRate || 0).toFixed(1) }}%</p>
                </div>
              </div>
            </div>

            <div class="bg-white shadow rounded-lg p-6">
              <div class="flex items-center">
                <div class="flex-shrink-0">
                  <div class="w-8 h-8 bg-purple-500 rounded-md flex items-center justify-center">
                    <span class="text-white text-sm">⏱️</span>
                  </div>
                </div>
                <div class="ml-5">
                  <p class="text-sm font-medium text-gray-500">Tempo Total</p>
                  <p class="text-2xl font-bold text-gray-900">{{ stats?.totalViewTime || 0 }}h</p>
                </div>
              </div>
            </div>
          </div>

          <div class="bg-white shadow rounded-lg p-6">
            <h3 class="text-lg font-medium text-gray-900 mb-4">Estatísticas Detalhadas</h3>
            <p class="text-gray-600">Gráficos e estatísticas detalhadas em desenvolvimento...</p>
          </div>
        </div>

      </div>
    </div>
  `
})
export class LearningComponent implements OnInit {
  content: LearningContent[] = [];
  filteredContent: LearningContent[] = [];
  contentForm: FormGroup;
  showNewContentForm = false;
  isLoading = false;
  activeTab = 'content';
  searchTerm = '';
  selectedCategory = '';
  selectedDifficulty = '';
  editingContent: LearningContent | null = null;
  stats: LearningStats | null = null;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient
  ) {
    this.contentForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(200)]],
      description: ['', [Validators.required, Validators.maxLength(2000)]],
      contentType: ['', [Validators.required]],
      category: ['', [Validators.required]],
      difficulty: ['', [Validators.required]],
      contentUrl: ['', [Validators.required, Validators.pattern('https?://.+')]],
      thumbnailUrl: [''],
      durationMinutes: [30, [Validators.required, Validators.min(1), Validators.max(600)]],
      isActive: [true],
      tags: [[]]
    });
  }

  ngOnInit(): void {
    this.loadContent();
    this.loadStats();
  }

  private loadContent(): void {
    // Carregar conteúdo real do banco de dados
    this.http.get<LearningContent[]>('https://localhost:5001/api/learning').subscribe({
      next: (data) => {
        this.content = data;
        this.applyFilters();
      },
      error: (error) => {
        console.error('Erro ao carregar conteúdo:', error);
        // Dados de exemplo enquanto o backend .NET está sendo configurado
        this.content = [
          {
            id: 1,
            title: 'Técnicas Básicas de Limpeza Dental',
            description: 'Aprenda as técnicas fundamentais para realizar uma limpeza dental eficaz e segura.',
            contentType: 'video',
            category: 'Odontologia Básica',
            difficulty: 'beginner',
            contentUrl: 'https://example.com/video/limpeza-dental',
            thumbnailUrl: 'https://example.com/thumb/limpeza.jpg',
            durationMinutes: 45,
            isActive: true,
            createdBy: 1,
            tags: ['limpeza', 'básico', 'higiene'],
            rating: 4.8,
            totalRatings: 24,
            viewCount: 150,
            completionCount: 120
          },
          {
            id: 2,
            title: 'Anatomia Dental Avançada',
            description: 'Estudo completo da anatomia dental para profissionais que buscam aperfeiçoamento.',
            contentType: 'course',
            category: 'Anatomia',
            difficulty: 'advanced',
            contentUrl: 'https://example.com/course/anatomia-dental',
            thumbnailUrl: 'https://example.com/thumb/anatomia.jpg',
            durationMinutes: 120,
            isActive: true,
            createdBy: 1,
            tags: ['anatomia', 'avançado', 'estudo'],
            rating: 4.5,
            totalRatings: 18,
            viewCount: 85,
            completionCount: 60
          }
        ];
        this.applyFilters();
      }
    });
  }

  private loadStats(): void {
    this.http.get<LearningStats>('https://localhost:5001/api/learning/stats').subscribe({
      next: (data) => {
        this.stats = data;
      },
      error: (error) => {
        console.error('Erro ao carregar estatísticas:', error);
        this.stats = {
          totalContent: 6,
          activeContent: 6,
          totalUsers: 4,
          activeUsers: 3,
          averageCompletionRate: 75.5,
          totalViewTime: 450,
          categoryBreakdown: [],
          mostPopular: []
        };
      }
    });
  }

  onSearch(): void {
    this.applyFilters();
  }

  onCategoryChange(): void {
    this.applyFilters();
  }

  onDifficultyChange(): void {
    this.applyFilters();
  }

  private applyFilters(): void {
    let filtered = [...this.content];

    if (this.searchTerm.trim()) {
      const term = this.searchTerm.toLowerCase();
      filtered = filtered.filter(item =>
        item.title.toLowerCase().includes(term) ||
        item.description.toLowerCase().includes(term) ||
        item.category.toLowerCase().includes(term) ||
        item.tags.some(tag => tag.toLowerCase().includes(term))
      );
    }

    if (this.selectedCategory) {
      filtered = filtered.filter(item => item.category === this.selectedCategory);
    }

    if (this.selectedDifficulty) {
      filtered = filtered.filter(item => item.difficulty === this.selectedDifficulty);
    }

    this.filteredContent = filtered;
  }

  onSubmit(): void {
    if (this.contentForm.valid) {
      this.isLoading = true;
      const contentData = this.contentForm.value;

      const request = this.editingContent
        ? this.http.put<LearningContent>(`https://localhost:5001/api/learning/${this.editingContent.id}`, contentData)
        : this.http.post<LearningContent>('https://localhost:5001/api/learning', contentData);

      request.subscribe({
        next: (response) => {
          if (this.editingContent) {
            const index = this.content.findIndex(c => c.id === this.editingContent!.id);
            if (index !== -1) {
              this.content[index] = response;
            }
          } else {
            this.content.push(response);
          }
          this.applyFilters();
          this.cancelForm();
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Erro ao salvar conteúdo:', error);
          this.isLoading = false;
        }
      });
    }
  }

  editContent(content: LearningContent): void {
    this.editingContent = content;
    this.contentForm.patchValue(content);
    this.showNewContentForm = true;
  }

  viewContent(content: LearningContent): void {
    window.open(content.contentUrl, '_blank');
  }

  cancelForm(): void {
    this.showNewContentForm = false;
    this.editingContent = null;
    this.contentForm.reset({
      title: '',
      description: '',
      contentType: '',
      category: '',
      difficulty: '',
      contentUrl: '',
      thumbnailUrl: '',
      durationMinutes: 30,
      isActive: true,
      tags: []
    });
  }

  getContentTypeIcon(type: string): string {
    const icons = {
      'video': '🎥',
      'article': '📄',
      'course': '🎓',
      'quiz': '❓',
      'document': '📋'
    };
    return icons[type as keyof typeof icons] || '📚';
  }

  getDifficultyClass(difficulty: string): string {
    const classes = {
      'beginner': 'bg-green-100 text-green-800',
      'intermediate': 'bg-yellow-100 text-yellow-800',
      'advanced': 'bg-red-100 text-red-800'
    };
    return classes[difficulty as keyof typeof classes] || 'bg-gray-100 text-gray-800';
  }

  getDifficultyLabel(difficulty: string): string {
    const labels = {
      'beginner': 'Iniciante',
      'intermediate': 'Intermediário',
      'advanced': 'Avançado'
    };
    return labels[difficulty as keyof typeof labels] || difficulty;
  }
}