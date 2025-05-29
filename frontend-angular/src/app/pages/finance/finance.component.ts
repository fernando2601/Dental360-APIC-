import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

interface FinancialTransaction {
  id?: number;
  type: 'receita' | 'despesa';
  amount: number;
  description: string;
  category: string;
  date: string;
  paymentMethod: string;
  patientName?: string;
  reference?: string;
}

@Component({
  selector: 'app-finance',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="max-w-7xl mx-auto">
      <div class="flex justify-between items-center mb-6">
        <h2 class="text-2xl font-bold text-gray-900">Financeiro</h2>
        <button
          (click)="showNewTransactionForm = !showNewTransactionForm"
          class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md text-sm font-medium"
        >
          {{ showNewTransactionForm ? 'Cancelar' : 'Nova Transação' }}
        </button>
      </div>

      <!-- Resumo Financeiro -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-6">
        <div class="bg-white overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-green-500 rounded-md flex items-center justify-center">
                  <span class="text-white text-sm">💰</span>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">
                    Total Receitas
                  </dt>
                  <dd class="text-lg font-medium text-gray-900">
                    R$ {{ totalRevenue.toLocaleString('pt-BR', {minimumFractionDigits: 2}) }}
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-red-500 rounded-md flex items-center justify-center">
                  <span class="text-white text-sm">💸</span>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">
                    Total Despesas
                  </dt>
                  <dd class="text-lg font-medium text-gray-900">
                    R$ {{ totalExpenses.toLocaleString('pt-BR', {minimumFractionDigits: 2}) }}
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>

        <div class="bg-white overflow-hidden shadow rounded-lg">
          <div class="p-5">
            <div class="flex items-center">
              <div class="flex-shrink-0">
                <div class="w-8 h-8 bg-blue-500 rounded-md flex items-center justify-center">
                  <span class="text-white text-sm">📊</span>
                </div>
              </div>
              <div class="ml-5 w-0 flex-1">
                <dl>
                  <dt class="text-sm font-medium text-gray-500 truncate">
                    Saldo
                  </dt>
                  <dd class="text-lg font-medium text-gray-900">
                    R$ {{ (totalRevenue - totalExpenses).toLocaleString('pt-BR', {minimumFractionDigits: 2}) }}
                  </dd>
                </dl>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Formulário Nova Transação -->
      <div *ngIf="showNewTransactionForm" class="bg-white shadow rounded-lg p-6 mb-6">
        <h3 class="text-lg font-medium text-gray-900 mb-4">Nova Transação</h3>
        <form [formGroup]="transactionForm" (ngSubmit)="onSubmit()">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-medium text-gray-700">Tipo</label>
              <select
                formControlName="type"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="receita">Receita</option>
                <option value="despesa">Despesa</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Valor (R$)</label>
              <input
                type="number"
                step="0.01"
                formControlName="amount"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
                placeholder="0.00"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Categoria</label>
              <select
                formControlName="category"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="">Selecione uma categoria</option>
                <option value="Consultas">Consultas</option>
                <option value="Procedimentos">Procedimentos</option>
                <option value="Produtos">Produtos</option>
                <option value="Materiais">Materiais</option>
                <option value="Equipamentos">Equipamentos</option>
                <option value="Salários">Salários</option>
                <option value="Aluguel">Aluguel</option>
                <option value="Utilities">Utilities</option>
                <option value="Marketing">Marketing</option>
                <option value="Outros">Outros</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Método de Pagamento</label>
              <select
                formControlName="paymentMethod"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
              >
                <option value="">Selecione o método</option>
                <option value="Dinheiro">Dinheiro</option>
                <option value="Cartão de Crédito">Cartão de Crédito</option>
                <option value="Cartão de Débito">Cartão de Débito</option>
                <option value="PIX">PIX</option>
                <option value="Transferência">Transferência</option>
                <option value="Cheque">Cheque</option>
              </select>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Data</label>
              <input
                type="date"
                formControlName="date"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
              />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700">Paciente (opcional)</label>
              <input
                type="text"
                formControlName="patientName"
                class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
                placeholder="Nome do paciente"
              />
            </div>
          </div>
          <div class="mt-4">
            <label class="block text-sm font-medium text-gray-700">Descrição</label>
            <textarea
              formControlName="description"
              rows="3"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
              placeholder="Descrição da transação"
            ></textarea>
          </div>
          <div class="mt-4">
            <label class="block text-sm font-medium text-gray-700">Referência</label>
            <input
              type="text"
              formControlName="reference"
              class="mt-1 block w-full border-gray-300 rounded-md shadow-sm focus:ring-blue-500 focus:border-blue-500"
              placeholder="Número da nota fiscal, recibo, etc."
            />
          </div>
          <div class="mt-6 flex justify-end space-x-3">
            <button
              type="button"
              (click)="showNewTransactionForm = false"
              class="bg-gray-300 hover:bg-gray-400 text-gray-800 px-4 py-2 rounded-md text-sm font-medium"
            >
              Cancelar
            </button>
            <button
              type="submit"
              [disabled]="!transactionForm.valid || isLoading"
              class="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-md text-sm font-medium disabled:opacity-50"
            >
              {{ isLoading ? 'Salvando...' : 'Salvar Transação' }}
            </button>
          </div>
        </form>
      </div>

      <!-- Lista de Transações -->
      <div class="bg-white shadow rounded-lg">
        <div class="px-6 py-4 border-b border-gray-200">
          <h3 class="text-lg font-medium text-gray-900">Transações</h3>
        </div>
        <div class="overflow-x-auto">
          <table class="min-w-full divide-y divide-gray-200">
            <thead class="bg-gray-50">
              <tr>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Data
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Tipo
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Descrição
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Categoria
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Valor
                </th>
                <th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Ações
                </th>
              </tr>
            </thead>
            <tbody class="bg-white divide-y divide-gray-200">
              <tr *ngFor="let transaction of transactions">
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {{ formatDate(transaction.date) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap">
                  <span [class]="getTypeClass(transaction.type)" class="inline-flex px-2 py-1 text-xs font-semibold rounded-full">
                    {{ transaction.type === 'receita' ? 'Receita' : 'Despesa' }}
                  </span>
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                  {{ transaction.description }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                  {{ transaction.category }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium" [class]="transaction.type === 'receita' ? 'text-green-600' : 'text-red-600'">
                  {{ transaction.type === 'receita' ? '+' : '-' }} R$ {{ transaction.amount.toFixed(2) }}
                </td>
                <td class="px-6 py-4 whitespace-nowrap text-sm font-medium">
                  <button
                    (click)="editTransaction(transaction)"
                    class="text-blue-600 hover:text-blue-900 mr-3"
                  >
                    Editar
                  </button>
                  <button
                    (click)="deleteTransaction(transaction.id!)"
                    class="text-red-600 hover:text-red-900"
                  >
                    Excluir
                  </button>
                </td>
              </tr>
              <tr *ngIf="transactions.length === 0">
                <td colspan="6" class="px-6 py-4 text-center text-sm text-gray-500">
                  Nenhuma transação registrada
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `
})
export class FinanceComponent implements OnInit {
  transactions: FinancialTransaction[] = [];
  transactionForm: FormGroup;
  showNewTransactionForm = false;
  isLoading = false;
  
  totalRevenue = 0;
  totalExpenses = 0;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient
  ) {
    this.transactionForm = this.fb.group({
      type: ['receita', [Validators.required]],
      amount: [0, [Validators.required, Validators.min(0.01)]],
      description: ['', [Validators.required]],
      category: ['', [Validators.required]],
      date: ['', [Validators.required]],
      paymentMethod: ['', [Validators.required]],
      patientName: [''],
      reference: ['']
    });
  }

  ngOnInit(): void {
    this.loadTransactions();
  }

  private loadTransactions(): void {
    this.http.get<FinancialTransaction[]>('https://localhost:5001/api/financial').subscribe({
      next: (data) => {
        this.transactions = data;
        this.calculateTotals();
      },
      error: (error) => {
        console.error('Erro ao carregar transações:', error);
        this.transactions = [];
      }
    });
  }

  private calculateTotals(): void {
    this.totalRevenue = this.transactions
      .filter(t => t.type === 'receita')
      .reduce((sum, t) => sum + t.amount, 0);
    
    this.totalExpenses = this.transactions
      .filter(t => t.type === 'despesa')
      .reduce((sum, t) => sum + t.amount, 0);
  }

  onSubmit(): void {
    if (this.transactionForm.valid) {
      this.isLoading = true;
      const transactionData = this.transactionForm.value;

      this.http.post<FinancialTransaction>('https://localhost:5001/api/financial', transactionData).subscribe({
        next: (response) => {
          this.transactions.push(response);
          this.calculateTotals();
          this.transactionForm.reset();
          this.transactionForm.patchValue({ type: 'receita' });
          this.showNewTransactionForm = false;
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Erro ao criar transação:', error);
          this.isLoading = false;
        }
      });
    }
  }

  editTransaction(transaction: FinancialTransaction): void {
    this.transactionForm.patchValue(transaction);
    this.showNewTransactionForm = true;
  }

  deleteTransaction(id: number): void {
    if (confirm('Tem certeza que deseja excluir esta transação?')) {
      this.http.delete(`https://localhost:5001/api/financial/${id}`).subscribe({
        next: () => {
          this.transactions = this.transactions.filter(t => t.id !== id);
          this.calculateTotals();
        },
        error: (error) => {
          console.error('Erro ao excluir transação:', error);
        }
      });
    }
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR');
  }

  getTypeClass(type: string): string {
    return type === 'receita' ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800';
  }
}