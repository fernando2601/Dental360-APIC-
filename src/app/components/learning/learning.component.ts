import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-learning',
  template: `
    <div class="container">
      <h2>Centro de Aprendizado</h2>
      <div class="learning-grid">
        <div class="course-card" *ngFor="let course of courses">
          <div class="course-header">
            <h4>{{ course.title }}</h4>
            <div class="course-duration">{{ course.duration }}</div>
          </div>
          <p class="course-description">{{ course.description }}</p>
          <div class="course-progress">
            <div class="progress-bar">
              <div class="progress-fill" [style.width.%]="course.progress"></div>
            </div>
            <span class="progress-text">{{ course.progress }}% concluído</span>
          </div>
          <button class="btn btn-primary">{{ course.progress > 0 ? 'Continuar' : 'Iniciar' }} Curso</button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .container { padding: 2rem; }
    .learning-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(350px, 1fr)); gap: 1.5rem; }
    .course-card { background: white; padding: 1.5rem; border-radius: 8px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }
    .course-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1rem; }
    .course-duration { background: #e5e7eb; color: #374151; padding: 0.25rem 0.75rem; border-radius: 20px; font-size: 0.75rem; }
    .course-description { color: #6b7280; margin-bottom: 1rem; line-height: 1.5; }
    .course-progress { margin-bottom: 1rem; }
    .progress-bar { width: 100%; height: 8px; background: #e5e7eb; border-radius: 4px; overflow: hidden; margin-bottom: 0.5rem; }
    .progress-fill { height: 100%; background: #3b82f6; transition: width 0.3s ease; }
    .progress-text { font-size: 0.875rem; color: #6b7280; }
    .btn { padding: 0.75rem 1.5rem; border: none; border-radius: 4px; font-weight: 500; cursor: pointer; width: 100%; }
    .btn-primary { background: #3b82f6; color: white; }
  `]
})
export class LearningComponent implements OnInit {
  courses: any[] = [];

  constructor(private apiService: ApiService) { }

  ngOnInit() {
    this.apiService.get('/learning/courses').subscribe({
      next: (data) => this.courses = data,
      error: () => {
        this.courses = [
          {
            id: 1,
            title: 'Fundamentos de Odontologia',
            description: 'Curso básico sobre anatomia oral e procedimentos fundamentais',
            duration: '8 horas',
            progress: 75
          },
          {
            id: 2,
            title: 'Harmonização Facial',
            description: 'Técnicas avançadas de estética facial e preenchimento',
            duration: '12 horas',
            progress: 30
          },
          {
            id: 3,
            title: 'Implantodontia Moderna',
            description: 'Procedimentos de implante dental com tecnologia atual',
            duration: '16 horas',
            progress: 0
          }
        ];
      }
    });
  }
}