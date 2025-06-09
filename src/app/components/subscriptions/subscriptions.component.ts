import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-subscriptions',
  template: `
    <div class="container">
      <h2>Planos de Assinatura</h2>
      <div class="subscriptions-grid">
        <div class="subscription-card" *ngFor="let plan of subscriptionPlans">
          <div class="plan-header">
            <h4>{{ plan.name }}</h4>
            <div class="plan-price">R$ {{ plan.price | number:'1.2-2' }}/mês</div>
          </div>
          <div class="plan-features">
            <ul>
              <li *ngFor="let feature of plan.features">{{ feature }}</li>
            </ul>
          </div>
          <button class="btn btn-primary btn-full">Assinar Plano</button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .container { padding: 2rem; }
    .subscriptions-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(300px, 1fr)); gap: 1.5rem; }
    .subscription-card { background: white; padding: 1.5rem; border-radius: 8px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); border: 2px solid #e5e7eb; }
    .plan-header { text-align: center; margin-bottom: 1.5rem; }
    .plan-price { font-size: 1.5rem; font-weight: 700; color: #3b82f6; margin-top: 0.5rem; }
    .plan-features ul { list-style: none; padding: 0; margin-bottom: 2rem; }
    .plan-features li { padding: 0.5rem 0; border-bottom: 1px solid #f3f4f6; }
    .btn { padding: 0.75rem 1.5rem; border: none; border-radius: 4px; font-weight: 500; cursor: pointer; }
    .btn-primary { background: #3b82f6; color: white; }
    .btn-full { width: 100%; }
  `]
})
export class SubscriptionsComponent implements OnInit {
  subscriptionPlans: any[] = [];

  constructor(private apiService: ApiService) { }

  ngOnInit() {
    this.apiService.get('/subscriptions/plans').subscribe({
      next: (data) => this.subscriptionPlans = data,
      error: () => {
        this.subscriptionPlans = [
          {
            id: 1,
            name: 'Plano Básico',
            price: 99.90,
            features: ['2 consultas por mês', '1 limpeza gratuita', 'Desconto de 10% em procedimentos']
          },
          {
            id: 2,
            name: 'Plano Premium',
            price: 199.90,
            features: ['Consultas ilimitadas', '2 limpezas gratuitas', 'Desconto de 20% em procedimentos', 'Atendimento prioritário']
          }
        ];
      }
    });
  }
}