import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-financial',
  template: `
    <div class="container">
      <h2>Controle Financeiro</h2>
      
      <div class="financial-summary">
        <div class="summary-card">
          <h4>Receita Mensal</h4>
          <div class="amount positive">R$ {{ monthlyRevenue | number:'1.2-2' }}</div>
        </div>
        <div class="summary-card">
          <h4>Despesas</h4>
          <div class="amount negative">R$ {{ monthlyExpenses | number:'1.2-2' }}</div>
        </div>
        <div class="summary-card">
          <h4>Lucro Líquido</h4>
          <div class="amount" [class.positive]="netProfit > 0" [class.negative]="netProfit < 0">
            R$ {{ netProfit | number:'1.2-2' }}
          </div>
        </div>
      </div>

      <div class="transactions-section">
        <h3>Transações Recentes</h3>
        <div class="transaction-list">
          <div class="transaction-item" *ngFor="let transaction of recentTransactions">
            <div class="transaction-info">
              <div class="transaction-description">{{ transaction.description }}</div>
              <div class="transaction-date">{{ transaction.date | date:'dd/MM/yyyy' }}</div>
            </div>
            <div class="transaction-amount" [class.positive]="transaction.type === 'income'" [class.negative]="transaction.type === 'expense'">
              {{ transaction.type === 'income' ? '+' : '-' }}R$ {{ transaction.amount | number:'1.2-2' }}
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .container { padding: 2rem; }
    .financial-summary { display: grid; grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); gap: 1.5rem; margin-bottom: 3rem; }
    .summary-card { background: white; padding: 1.5rem; border-radius: 8px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); text-align: center; }
    .summary-card h4 { margin: 0 0 1rem 0; color: #6b7280; }
    .amount { font-size: 2rem; font-weight: 700; }
    .amount.positive { color: #10b981; }
    .amount.negative { color: #ef4444; }
    .transactions-section { background: white; padding: 1.5rem; border-radius: 8px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }
    .transaction-item { display: flex; justify-content: space-between; align-items: center; padding: 1rem 0; border-bottom: 1px solid #f3f4f6; }
    .transaction-description { font-weight: 500; color: #1f2937; }
    .transaction-date { color: #6b7280; font-size: 0.875rem; }
    .transaction-amount { font-weight: 600; }
  `]
})
export class FinancialComponent implements OnInit {
  monthlyRevenue = 0;
  monthlyExpenses = 0;
  netProfit = 0;
  recentTransactions: any[] = [];

  constructor(private apiService: ApiService) { }

  ngOnInit() {
    this.loadFinancialData();
  }

  loadFinancialData() {
    this.apiService.get('/financial/summary').subscribe({
      next: (data) => {
        this.monthlyRevenue = data.revenue;
        this.monthlyExpenses = data.expenses;
        this.netProfit = this.monthlyRevenue - this.monthlyExpenses;
      },
      error: () => {
        this.monthlyRevenue = 15750.00;
        this.monthlyExpenses = 8200.00;
        this.netProfit = this.monthlyRevenue - this.monthlyExpenses;
      }
    });

    this.apiService.get('/financial/transactions').subscribe({
      next: (data) => this.recentTransactions = data,
      error: () => {
        this.recentTransactions = [
          { id: 1, description: 'Consulta - Maria Silva', amount: 80.00, type: 'income', date: new Date() },
          { id: 2, description: 'Clareamento - João Santos', amount: 450.00, type: 'income', date: new Date() },
          { id: 3, description: 'Material Odontológico', amount: 320.00, type: 'expense', date: new Date() },
          { id: 4, description: 'Aluguel da Clínica', amount: 2500.00, type: 'expense', date: new Date() }
        ];
      }
    });
  }
}