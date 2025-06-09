import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-staff',
  template: `
    <div class="container">
      <h2>Equipe da Clínica</h2>
      <div class="staff-grid">
        <div class="staff-card" *ngFor="let member of staff">
          <div class="staff-avatar">{{ member.name.charAt(0) }}</div>
          <h4>{{ member.name }}</h4>
          <p class="staff-role">{{ member.role }}</p>
          <p class="staff-specialty">{{ member.specialty }}</p>
          <div class="staff-contact">
            <p>📧 {{ member.email }}</p>
            <p>📱 {{ member.phone }}</p>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .container { padding: 2rem; }
    .staff-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(250px, 1fr)); gap: 1.5rem; }
    .staff-card { background: white; padding: 1.5rem; border-radius: 8px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); text-align: center; }
    .staff-avatar { width: 60px; height: 60px; background: #3b82f6; color: white; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-size: 1.5rem; font-weight: 600; margin: 0 auto 1rem; }
    .staff-role { color: #3b82f6; font-weight: 600; margin: 0.5rem 0; }
    .staff-specialty { color: #6b7280; margin: 0.5rem 0; }
    .staff-contact { margin-top: 1rem; }
    .staff-contact p { margin: 0.25rem 0; color: #6b7280; font-size: 0.875rem; }
  `]
})
export class StaffComponent implements OnInit {
  staff: any[] = [];

  constructor(private apiService: ApiService) { }

  ngOnInit() {
    this.apiService.get('/staff').subscribe({
      next: (data) => this.staff = data,
      error: () => {
        this.staff = [
          { id: 1, name: 'Dr. João Silva', role: 'Dentista', specialty: 'Clínica Geral', email: 'joao@dentalspa.com', phone: '(11) 99999-0001' },
          { id: 2, name: 'Dra. Ana Costa', role: 'Dentista', specialty: 'Ortodontia', email: 'ana@dentalspa.com', phone: '(11) 99999-0002' },
          { id: 3, name: 'Maria Santos', role: 'Auxiliar', specialty: 'Higienização', email: 'maria@dentalspa.com', phone: '(11) 99999-0003' }
        ];
      }
    });
  }
}