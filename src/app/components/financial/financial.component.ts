import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-financial',
  template: `
    <div class="container-fluid p-4">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <h2><i class="fas fa-chart-line me-2"></i>Financeiro</h2>
        <div class="d-flex gap-2">
          <button class="btn btn-success">
            <i class="fas fa-plus me-2"></i>Nova Receita
          </button>
          <button class="btn btn-danger">
            <i class="fas fa-minus me-2"></i>Nova Despesa
          </button>
        </div>
      </div>
      
      <div class="row mb-4">
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-arrow-up text-success fa-2x mb-2"></i>
              <h6>Receitas do Mês</h6>
              <h4 class="text-success">{{formatCurrency(stats.monthlyRevenue)}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-arrow-down text-danger fa-2x mb-2"></i>
              <h6>Despesas do Mês</h6>
              <h4 class="text-danger">{{formatCurrency(stats.monthlyExpenses)}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-chart-line text-primary fa-2x mb-2"></i>
              <h6>Lucro Líquido</h6>
              <h4 class="text-primary">{{formatCurrency(stats.netProfit)}}</h4>
            </div>
          </div>
        </div>
        <div class="col-md-3 mb-3">
          <div class="card border-0 shadow-sm">
            <div class="card-body text-center">
              <i class="fas fa-clock text-warning fa-2x mb-2"></i>
              <h6>Contas a Receber</h6>
              <h4 class="text-warning">{{formatCurrency(stats.pendingReceivables)}}</h4>
            </div>
          </div>
        </div>
      </div>

      <div class="row">
        <div class="col-md-6 mb-4">
          <div class="card border-0 shadow-sm">
            <div class="card-header bg-white">
              <h5 class="mb-0">Receitas Recentes</h5>
            </div>
            <div class="card-body">
              <div *ngIf="loadingRevenue" class="text-center py-3">
                <div class="spinner-border spinner-border-sm" role="status"></div>
              </div>
              <div *ngIf="!loadingRevenue && revenues.length === 0" class="text-center py-3 text-muted">
                <i class="fas fa-receipt fa-2x mb-2"></i>
                <p class="mb-0">Nenhuma receita registrada</p>
              </div>
              <div *ngIf="!loadingRevenue && revenues.length > 0" class="list-group list-group-flush">
                <div *ngFor="let revenue of revenues" class="list-group-item px-0">
                  <div class="d-flex justify-content-between align-items-center">
                    <div>
                      <h6 class="mb-1">{{revenue.description}}</h6>
                      <small class="text-muted">{{formatDate(revenue.date)}}</small>
                    </div>
                    <span class="badge bg-success">{{formatCurrency(revenue.amount)}}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="col-md-6 mb-4">
          <div class="card border-0 shadow-sm">
            <div class="card-header bg-white">
              <h5 class="mb-0">Despesas Recentes</h5>
            </div>
            <div class="card-body">
              <div *ngIf="loadingExpenses" class="text-center py-3">
                <div class="spinner-border spinner-border-sm" role="status"></div>
              </div>
              <div *ngIf="!loadingExpenses && expenses.length === 0" class="text-center py-3 text-muted">
                <i class="fas fa-file-invoice fa-2x mb-2"></i>
                <p class="mb-0">Nenhuma despesa registrada</p>
              </div>
              <div *ngIf="!loadingExpenses && expenses.length > 0" class="list-group list-group-flush">
                <div *ngFor="let expense of expenses" class="list-group-item px-0">
                  <div class="d-flex justify-content-between align-items-center">
                    <div>
                      <h6 class="mb-1">{{expense.description}}</h6>
                      <small class="text-muted">{{formatDate(expense.date)}}</small>
                    </div>
                    <span class="badge bg-danger">{{formatCurrency(expense.amount)}}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div class="card border-0 shadow-sm">
        <div class="card-header bg-white d-flex justify-content-between align-items-center">
          <h5 class="mb-0">Histórico de Transações</h5>
          <div class="d-flex gap-2">
            <select class="form-select" style="width: auto;">
              <option value="">Todos os tipos</option>
              <option value="revenue">Receitas</option>
              <option value="expense">Despesas</option>
            </select>
            <input type="month" class="form-control" style="width: auto;">
          </div>
        </div>
        <div class="card-body">
          <div *ngIf="loadingTransactions" class="text-center py-4">
            <div class="spinner-border" role="status">
              <span class="visually-hidden">Carregando...</span>
            </div>
          </div>
          <div *ngIf="!loadingTransactions && transactions.length === 0" class="text-center py-4 text-muted">
            <i class="fas fa-chart-line fa-3x mb-3"></i>
            <p>Nenhuma transação encontrada</p>
          </div>
          <div *ngIf="!loadingTransactions && transactions.length > 0" class="table-responsive">
            <table class="table table-hover">
              <thead>
                <tr>
                  <th>Data</th>
                  <th>Descrição</th>
                  <th>Tipo</th>
                  <th>Valor</th>
                  <th>Ações</th>
                </tr>
              </thead>
              <tbody>
                <tr *ngFor="let transaction of transactions">
                  <td>{{formatDate(transaction.date)}}</td>
                  <td>{{transaction.description}}</td>
                  <td>
                    <span class="badge" [ngClass]="getTypeClass(transaction.type)">
                      {{transaction.type}}
                    </span>
                  </td>
                  <td>
                    <span [ngClass]="getAmountClass(transaction.type)">
                      {{formatCurrency(transaction.amount)}}
                    </span>
                  </td>
                  <td>
                    <button class="btn btn-sm btn-outline-primary me-1">
                      <i class="fas fa-edit"></i>
                    </button>
                    <button class="btn btn-sm btn-outline-danger">
                      <i class="fas fa-trash"></i>
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  `
})
export class FinancialComponent implements OnInit {
  stats = { 
    monthlyRevenue: 0, 
    monthlyExpenses: 0, 
    netProfit: 0, 
    pendingReceivables: 0 
  };
  revenues: any[] = [];
  expenses: any[] = [];
  transactions: any[] = [];
  loadingRevenue = true;
  loadingExpenses = true;
  loadingTransactions = true;

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadFinancialData();
  }

  loadFinancialData() {
    this.loadStats();
    this.loadRevenues();
    this.loadExpenses();
    this.loadTransactions();
  }

  loadStats() {
    this.apiService.get('/api/financial/stats').subscribe({
      next: (data: any) => {
        this.stats = data;
      },
      error: (error) => {
        console.error('Erro ao carregar estatísticas financeiras:', error);
      }
    });
  }

  loadRevenues() {
    this.apiService.get('/api/financial/revenues').subscribe({
      next: (data: any) => {
        this.revenues = data.slice(0, 5); // Mostrar apenas os 5 mais recentes
        this.loadingRevenue = false;
      },
      error: (error) => {
        console.error('Erro ao carregar receitas:', error);
        this.loadingRevenue = false;
      }
    });
  }

  loadExpenses() {
    this.apiService.get('/api/financial/expenses').subscribe({
      next: (data: any) => {
        this.expenses = data.slice(0, 5); // Mostrar apenas os 5 mais recentes
        this.loadingExpenses = false;
      },
      error: (error) => {
        console.error('Erro ao carregar despesas:', error);
        this.loadingExpenses = false;
      }
    });
  }

  loadTransactions() {
    this.apiService.get('/api/financial/transactions').subscribe({
      next: (data: any) => {
        this.transactions = data;
        this.loadingTransactions = false;
      },
      error: (error) => {
        console.error('Erro ao carregar transações:', error);
        this.loadingTransactions = false;
      }
    });
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('pt-BR');
  }

  getTypeClass(type: string): string {
    return type === 'Receita' ? 'bg-success' : 'bg-danger';
  }

  getAmountClass(type: string): string {
    return type === 'Receita' ? 'text-success fw-bold' : 'text-danger fw-bold';
  }
}