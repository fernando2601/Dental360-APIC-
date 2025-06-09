import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-packages',
  template: `
    <div class="container">
      <h2>Pacotes de Serviços</h2>
      <div class="packages-grid">
        <div class="package-card" *ngFor="let package of packages">
          <h4>{{ package.name }}</h4>
          <p>{{ package.description }}</p>
          <div class="package-price">R$ {{ package.price | number:'1.2-2' }}</div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .container { padding: 2rem; }
    .packages-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); gap: 1.5rem; }
    .package-card { background: white; padding: 1.5rem; border-radius: 8px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }
    .package-price { font-weight: 600; color: #3b82f6; font-size: 1.25rem; margin-top: 1rem; }
  `]
})
export class PackagesComponent implements OnInit {
  packages: any[] = [];

  constructor(private apiService: ApiService) { }

  ngOnInit() {
    this.apiService.get('/packages').subscribe({
      next: (data) => this.packages = data,
      error: () => {
        this.packages = [
          { id: 1, name: 'Pacote Básico', description: 'Consulta + Limpeza', price: 150.00 },
          { id: 2, name: 'Pacote Premium', description: 'Consulta + Limpeza + Clareamento', price: 550.00 }
        ];
      }
    });
  }
}