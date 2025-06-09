import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-before-after',
  template: `
    <div class="container">
      <h2>Galeria Antes e Depois</h2>
      <div class="gallery-grid">
        <div class="gallery-card" *ngFor="let case of beforeAfterCases">
          <div class="case-header">
            <h4>{{ case.title }}</h4>
            <div class="case-date">{{ case.date | date:'dd/MM/yyyy' }}</div>
          </div>
          <div class="images-container">
            <div class="image-box">
              <h5>Antes</h5>
              <div class="image-placeholder">📷 Antes</div>
            </div>
            <div class="image-box">
              <h5>Depois</h5>
              <div class="image-placeholder">📷 Depois</div>
            </div>
          </div>
          <div class="case-details">
            <p><strong>Procedimento:</strong> {{ case.procedure }}</p>
            <p><strong>Paciente:</strong> {{ case.patient }}</p>
            <p><strong>Profissional:</strong> {{ case.dentist }}</p>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .container { padding: 2rem; }
    .gallery-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(400px, 1fr)); gap: 1.5rem; }
    .gallery-card { background: white; padding: 1.5rem; border-radius: 8px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }
    .case-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1rem; }
    .case-date { color: #6b7280; font-size: 0.875rem; }
    .images-container { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; margin-bottom: 1rem; }
    .image-box h5 { margin: 0 0 0.5rem 0; text-align: center; color: #374151; }
    .image-placeholder { height: 150px; background: #f3f4f6; border: 2px dashed #d1d5db; border-radius: 8px; display: flex; align-items: center; justify-content: center; color: #9ca3af; font-size: 1.5rem; }
    .case-details p { margin: 0.5rem 0; color: #6b7280; }
  `]
})
export class BeforeAfterComponent implements OnInit {
  beforeAfterCases: any[] = [];

  constructor(private apiService: ApiService) { }

  ngOnInit() {
    this.apiService.get('/before-after').subscribe({
      next: (data) => this.beforeAfterCases = data,
      error: () => {
        this.beforeAfterCases = [
          {
            id: 1,
            title: 'Clareamento Dental',
            procedure: 'Clareamento com gel de peróxido',
            patient: 'M.S.',
            dentist: 'Dr. João Silva',
            date: new Date('2024-05-15')
          },
          {
            id: 2,
            title: 'Harmonização Facial',
            procedure: 'Preenchimento labial + Botox',
            patient: 'A.C.',
            dentist: 'Dra. Ana Costa',
            date: new Date('2024-05-20')
          }
        ];
      }
    });
  }
}