import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../services/api.service';

declare var bootstrap: any;
declare var Chart: any;

interface Transaction {
  id?: number;
  description: string;
  amount: number;
  type: 'receita' | 'despesa';
  category: string;
  date: string;
  notes?: string;
  created_at?: string;
}

interface FinancialStats {
  monthlyRevenue: number;
  monthlyExpenses: number;
  netProfit: number;
  todayAppointments: number;
  todayRevenue: number;
}

@Component({
  selector: 'app-financial',
  templateUrl: './financial.component.html'
})
export class FinancialComponent implements OnInit {
  transactions: Transaction[] = [];
  filteredTransactions: Transaction[] = [];
  
  monthlyRevenue = 0;
  monthlyExpenses = 0;
  netProfit = 0;
  profitMargin = 0;
  todayAppointments = 0;
  todayRevenue = 0;
  monthlyGoal = 50000;
  averageTicket = '0';
  conversionRate = 85;
  
  transactionFilter = '';
  chartPeriod = 'month';
  loading = true;
  saving = false;
  isEditing = false;
  
  transactionForm: FormGroup;
  transactionModal: any;
  cashFlowChart: any;
  revenueChart: any;

  upcomingBills = [
    { description: 'Aluguel da Clínica', amount: 5000, daysUntilDue: 5 },
    { description: 'Materiais Dentários', amount: 2800, daysUntilDue: 10 },
    { description: 'Energia Elétrica', amount: 650, daysUntilDue: 15 }
  ];

  constructor(
    private apiService: ApiService,
    private fb: FormBuilder
  ) {
    this.transactionForm = this.fb.group({
      description: ['', Validators.required],
      amount: ['', [Validators.required, Validators.min(0)]],
      type: ['receita', Validators.required],
      category: ['', Validators.required],
      date: [new Date().toISOString().split('T')[0], Validators.required],
      notes: ['']
    });
  }

  ngOnInit() {
    this.loadFinancialData();
    this.generateMockTransactions();
    setTimeout(() => this.initializeCharts(), 500);
  }

  loadFinancialData() {
    this.loading = true;
    
    // Load appointments for today's revenue calculation
    this.apiService.get<any[]>('/appointments').subscribe({
      next: (appointments: any[]) => {
        const today = new Date().toISOString().split('T')[0];
        const todayAppts = appointments.filter(apt => 
          apt.appointment_date.startsWith(today)
        );
        this.todayAppointments = todayAppts.length;
        
        // Load services to calculate revenue
        this.apiService.get<any[]>('/services').subscribe({
          next: (services: any[]) => {
            this.calculateFinancialMetrics(appointments, services);
            this.loading = false;
          },
          error: () => this.loading = false
        });
      },
      error: () => this.loading = false
    });
  }

  calculateFinancialMetrics(appointments: any[], services: any[]) {
    const currentMonth = new Date().getMonth();
    const currentYear = new Date().getFullYear();
    
    // Calculate monthly revenue from appointments
    let monthlyRev = 0;
    appointments.forEach(apt => {
      const aptDate = new Date(apt.appointment_date);
      if (aptDate.getMonth() === currentMonth && aptDate.getFullYear() === currentYear) {
        const service = services.find(s => s.id === apt.service_id);
        if (service) {
          monthlyRev += parseFloat(service.price);
        }
      }
    });
    
    this.monthlyRevenue = monthlyRev;
    this.monthlyExpenses = monthlyRev * 0.65; // Estimate 65% expenses
    this.netProfit = this.monthlyRevenue - this.monthlyExpenses;
    this.profitMargin = this.monthlyRevenue > 0 ? Math.round((this.netProfit / this.monthlyRevenue) * 100) : 0;
    
    // Calculate today's revenue
    const today = new Date().toISOString().split('T')[0];
    let todayRev = 0;
    appointments.forEach(apt => {
      if (apt.appointment_date.startsWith(today)) {
        const service = services.find(s => s.id === apt.service_id);
        if (service) {
          todayRev += parseFloat(service.price);
        }
      }
    });
    this.todayRevenue = todayRev;
    
    // Calculate average ticket
    if (appointments.length > 0) {
      const totalRevenue = appointments.reduce((total, apt) => {
        const service = services.find(s => s.id === apt.service_id);
        return total + (service ? parseFloat(service.price) : 0);
      }, 0);
      this.averageTicket = this.formatCurrency(totalRevenue / appointments.length);
    }
  }

  generateMockTransactions() {
    const categories = {
      receita: ['consultas', 'procedimentos', 'produtos'],
      despesa: ['salarios', 'materiais', 'aluguel', 'utilidades']
    };
    
    this.transactions = [];
    
    // Generate last 30 days of transactions
    for (let i = 0; i < 30; i++) {
      const date = new Date();
      date.setDate(date.getDate() - i);
      
      // Add 2-4 transactions per day
      const dailyTransactions = Math.floor(Math.random() * 3) + 2;
      
      for (let j = 0; j < dailyTransactions; j++) {
        const isRevenue = Math.random() > 0.3; // 70% revenue, 30% expenses
        const type = isRevenue ? 'receita' : 'despesa';
        const categoryArray = categories[type];
        const category = categoryArray[Math.floor(Math.random() * categoryArray.length)];
        
        let amount: number;
        let description: string;
        
        if (type === 'receita') {
          amount = Math.floor(Math.random() * 800) + 200; // R$200-1000
          description = this.getRevenueDescription(category);
        } else {
          amount = Math.floor(Math.random() * 500) + 100; // R$100-600
          description = this.getExpenseDescription(category);
        }
        
        this.transactions.push({
          id: this.transactions.length + 1,
          description,
          amount,
          type,
          category,
          date: date.toISOString().split('T')[0],
          notes: '',
          created_at: date.toISOString()
        });
      }
    }
    
    this.filteredTransactions = [...this.transactions];
  }

  getRevenueDescription(category: string): string {
    const descriptions = {
      consultas: ['Consulta de Rotina', 'Avaliação Inicial', 'Retorno', 'Consulta de Emergência'],
      procedimentos: ['Limpeza Dental', 'Restauração', 'Canal', 'Extração', 'Implante'],
      produtos: ['Escova Dental', 'Pasta de Dente', 'Enxaguante', 'Fio Dental']
    };
    const options = descriptions[category as keyof typeof descriptions] || ['Receita'];
    return options[Math.floor(Math.random() * options.length)];
  }

  getExpenseDescription(category: string): string {
    const descriptions = {
      salarios: ['Salário Dr. Silva', 'Salário Recepcionista', 'Salário Auxiliar'],
      materiais: ['Materiais Dentários', 'Equipamentos', 'Descartáveis'],
      aluguel: ['Aluguel da Clínica', 'Condomínio'],
      utilidades: ['Energia Elétrica', 'Água', 'Internet', 'Telefone']
    };
    const options = descriptions[category as keyof typeof descriptions] || ['Despesa'];
    return options[Math.floor(Math.random() * options.length)];
  }

  initializeCharts() {
    this.createCashFlowChart();
    this.createRevenueDistributionChart();
  }

  createCashFlowChart() {
    const ctx = document.getElementById('cashFlowChart') as HTMLCanvasElement;
    if (!ctx) return;

    const labels = this.getLast6Months();
    const revenueData = this.getMonthlyRevenueData();
    const expenseData = this.getMonthlyExpenseData();

    this.cashFlowChart = new Chart(ctx, {
      type: 'line',
      data: {
        labels,
        datasets: [
          {
            label: 'Receitas',
            data: revenueData,
            borderColor: '#28a745',
            backgroundColor: 'rgba(40, 167, 69, 0.1)',
            tension: 0.4,
            fill: true
          },
          {
            label: 'Despesas',
            data: expenseData,
            borderColor: '#dc3545',
            backgroundColor: 'rgba(220, 53, 69, 0.1)',
            tension: 0.4,
            fill: true
          }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            position: 'top'
          }
        },
        scales: {
          y: {
            beginAtZero: true,
            ticks: {
              callback: function(value: any) {
                return 'R$ ' + value.toLocaleString();
              }
            }
          }
        }
      }
    });
  }

  createRevenueDistributionChart() {
    const ctx = document.getElementById('revenueDistributionChart') as HTMLCanvasElement;
    if (!ctx) return;

    const revenueByCategory = this.getRevenueByCategory();

    this.revenueChart = new Chart(ctx, {
      type: 'doughnut',
      data: {
        labels: Object.keys(revenueByCategory),
        datasets: [{
          data: Object.values(revenueByCategory),
          backgroundColor: [
            '#007bff',
            '#28a745',
            '#ffc107',
            '#17a2b8',
            '#6f42c1'
          ]
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            position: 'bottom'
          }
        }
      }
    });
  }

  getLast6Months(): string[] {
    const months = [];
    for (let i = 5; i >= 0; i--) {
      const date = new Date();
      date.setMonth(date.getMonth() - i);
      months.push(date.toLocaleDateString('pt-BR', { month: 'short', year: 'numeric' }));
    }
    return months;
  }

  getMonthlyRevenueData(): number[] {
    return [25000, 28000, 32000, 30000, 35000, this.monthlyRevenue];
  }

  getMonthlyExpenseData(): number[] {
    return [18000, 19000, 21000, 20000, 23000, this.monthlyExpenses];
  }

  getRevenueByCategory(): { [key: string]: number } {
    const categories = {
      'Consultas': 0,
      'Procedimentos': 0,
      'Produtos': 0
    };

    this.transactions
      .filter(t => t.type === 'receita')
      .forEach(transaction => {
        const categoryName = transaction.category === 'consultas' ? 'Consultas' :
                            transaction.category === 'procedimentos' ? 'Procedimentos' : 'Produtos';
        categories[categoryName] += transaction.amount;
      });

    return categories;
  }

  filterTransactions() {
    if (!this.transactionFilter) {
      this.filteredTransactions = [...this.transactions];
    } else {
      this.filteredTransactions = this.transactions.filter(t => 
        t.type === this.transactionFilter
      );
    }
  }

  openRevenueModal() {
    this.transactionForm.patchValue({ type: 'receita' });
    this.isEditing = false;
    this.openModal();
  }

  openExpenseModal() {
    this.transactionForm.patchValue({ type: 'despesa' });
    this.isEditing = false;
    this.openModal();
  }

  editTransaction(transaction: Transaction) {
    this.isEditing = true;
    this.transactionForm.patchValue(transaction);
    this.openModal();
  }

  viewTransactionDetails(transaction: Transaction) {
    console.log('Viewing transaction details:', transaction);
  }

  deleteTransaction(transaction: Transaction) {
    if (confirm('Tem certeza que deseja excluir esta transação?')) {
      this.transactions = this.transactions.filter(t => t.id !== transaction.id);
      this.filterTransactions();
    }
  }

  saveTransaction() {
    if (!this.transactionForm.valid) return;

    this.saving = true;
    const transactionData = this.transactionForm.value;

    if (this.isEditing) {
      const index = this.transactions.findIndex(t => t.id === transactionData.id);
      if (index !== -1) {
        this.transactions[index] = { ...transactionData };
      }
    } else {
      const newTransaction = {
        ...transactionData,
        id: this.transactions.length + 1,
        created_at: new Date().toISOString()
      };
      this.transactions.unshift(newTransaction);
    }

    this.filterTransactions();
    this.updateFinancialMetrics();
    this.saving = false;
    this.closeModal();
  }

  updateFinancialMetrics() {
    const currentMonth = new Date().getMonth();
    const currentYear = new Date().getFullYear();

    this.monthlyRevenue = this.transactions
      .filter(t => {
        const tDate = new Date(t.date);
        return t.type === 'receita' && 
               tDate.getMonth() === currentMonth && 
               tDate.getFullYear() === currentYear;
      })
      .reduce((sum, t) => sum + t.amount, 0);

    this.monthlyExpenses = this.transactions
      .filter(t => {
        const tDate = new Date(t.date);
        return t.type === 'despesa' && 
               tDate.getMonth() === currentMonth && 
               tDate.getFullYear() === currentYear;
      })
      .reduce((sum, t) => sum + t.amount, 0);

    this.netProfit = this.monthlyRevenue - this.monthlyExpenses;
    this.profitMargin = this.monthlyRevenue > 0 ? Math.round((this.netProfit / this.monthlyRevenue) * 100) : 0;
  }

  openModal() {
    this.transactionModal = new bootstrap.Modal(document.getElementById('transactionModal'));
    this.transactionModal.show();
  }

  closeModal() {
    if (this.transactionModal) {
      this.transactionModal.hide();
    }
  }

  setChartPeriod(period: string) {
    this.chartPeriod = period;
    // Update charts based on period
  }

  exportTransactions() {
    console.log('Exporting transactions...');
  }

  getGoalProgress(): number {
    return Math.min(Math.round((this.monthlyRevenue / this.monthlyGoal) * 100), 100);
  }

  getCategoryBadgeClass(category: string): string {
    const classes = {
      'consultas': 'bg-primary',
      'procedimentos': 'bg-success',
      'produtos': 'bg-info',
      'salarios': 'bg-warning',
      'materiais': 'bg-secondary',
      'aluguel': 'bg-danger',
      'utilidades': 'bg-dark'
    };
    return classes[category as keyof typeof classes] || 'bg-secondary';
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
}