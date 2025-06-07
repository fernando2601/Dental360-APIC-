import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-learning',
  template: `
    <div class="container-fluid p-4">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <h2><i class="fas fa-graduation-cap me-2"></i>Área de Aprendizado</h2>
        <button class="btn btn-primary">
          <i class="fas fa-plus me-2"></i>Novo Conteúdo
        </button>
      </div>
      
      <div class="row mb-4">
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-book text-primary fa-2x mb-2"></i>
              <h6>Total Cursos</h6>
              <h4 class="text-primary">{{stats.totalCourses}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-video text-success fa-2x mb-2"></i>
              <h6>Vídeo Aulas</h6>
              <h4 class="text-success">{{stats.videoLessons}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-file-pdf text-danger fa-2x mb-2"></i>
              <h6>Materiais PDF</h6>
              <h4 class="text-danger">{{stats.pdfMaterials}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-users text-info fa-2x mb-2"></i>
              <h6>Participantes</h6>
              <h4 class="text-info">{{stats.participants}}</h4>
            </div>
          </div>
        </div>
      </div>

      <div class="row">
        <div class="col-md-8">
          <div class="card border-0 shadow-sm">
            <div class="card-header bg-white">
              <h5 class="mb-0">Cursos Disponíveis</h5>
            </div>
            <div class="card-body">
              <div *ngIf="loading" class="text-center py-4">
                <div class="spinner-border" role="status">
                  <span class="visually-hidden">Carregando...</span>
                </div>
              </div>
              <div *ngIf="!loading && courses.length > 0" class="list-group list-group-flush">
                <div *ngFor="let course of courses" class="list-group-item px-0 py-3">
                  <div class="d-flex justify-content-between align-items-start">
                    <div class="flex-grow-1">
                      <h6 class="mb-1">{{course.title}}</h6>
                      <p class="mb-1 text-muted small">{{course.description}}</p>
                      <div class="d-flex align-items-center gap-3 mt-2">
                        <small class="text-muted">
                          <i class="fas fa-clock me-1"></i>{{course.duration}} min
                        </small>
                        <small class="text-muted">
                          <i class="fas fa-play-circle me-1"></i>{{course.lessonsCount}} aulas
                        </small>
                        <span class="badge" [ngClass]="getLevelClass(course.level)">
                          {{course.level}}
                        </span>
                      </div>
                    </div>
                    <div class="btn-group" role="group">
                      <button class="btn btn-sm btn-outline-primary">
                        <i class="fas fa-play"></i>
                      </button>
                      <button class="btn btn-sm btn-outline-secondary">
                        <i class="fas fa-edit"></i>
                      </button>
                    </div>
                  </div>
                  <div class="progress mt-2" style="height: 5px;">
                    <div class="progress-bar" [style.width.%]="course.completionRate"></div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="col-md-4">
          <div class="card border-0 shadow-sm mb-4">
            <div class="card-header bg-white">
              <h6 class="mb-0">Atividade Recente</h6>
            </div>
            <div class="card-body">
              <div *ngFor="let activity of recentActivity" class="d-flex align-items-center mb-3">
                <div class="avatar-sm bg-light rounded-circle d-flex align-items-center justify-content-center me-3">
                  <i class="fas fa-user text-muted"></i>
                </div>
                <div class="flex-grow-1">
                  <h6 class="mb-0 small">{{activity.userName}}</h6>
                  <small class="text-muted">{{activity.action}}</small>
                  <br>
                  <small class="text-muted">{{formatDate(activity.date)}}</small>
                </div>
              </div>
            </div>
          </div>

          <div class="card border-0 shadow-sm">
            <div class="card-header bg-white">
              <h6 class="mb-0">Categorias</h6>
            </div>
            <div class="card-body">
              <div *ngFor="let category of categories" class="d-flex justify-content-between align-items-center mb-2">
                <span class="small">{{category.name}}</span>
                <span class="badge bg-secondary">{{category.count}}</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class LearningComponent implements OnInit {
  stats = { totalCourses: 0, videoLessons: 0, pdfMaterials: 0, participants: 0 };
  courses: any[] = [];
  recentActivity: any[] = [];
  categories: any[] = [];
  loading = true;

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadLearningData();
  }

  loadLearningData() {
    this.loadCourses();
    this.loadStats();
    this.loadRecentActivity();
    this.loadCategories();
  }

  loadCourses() {
    this.apiService.get('/api/learning/courses').subscribe({
      next: (data: any) => {
        this.courses = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Erro ao carregar cursos:', error);
        this.loading = false;
      }
    });
  }

  loadStats() {
    this.apiService.get('/api/learning/stats').subscribe({
      next: (data: any) => {
        this.stats = data;
      },
      error: (error) => {
        console.error('Erro ao carregar estatísticas:', error);
      }
    });
  }

  loadRecentActivity() {
    this.apiService.get('/api/learning/activity').subscribe({
      next: (data: any) => {
        this.recentActivity = data;
      },
      error: (error) => {
        console.error('Erro ao carregar atividades:', error);
      }
    });
  }

  loadCategories() {
    this.apiService.get('/api/learning/categories').subscribe({
      next: (data: any) => {
        this.categories = data;
      },
      error: (error) => {
        console.error('Erro ao carregar categorias:', error);
      }
    });
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('pt-BR');
  }

  getLevelClass(level: string): string {
    const levelClasses: any = {
      'Iniciante': 'bg-success',
      'Intermediário': 'bg-warning',
      'Avançado': 'bg-danger'
    };
    return levelClasses[level] || 'bg-secondary';
  }
}