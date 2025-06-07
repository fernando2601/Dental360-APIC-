import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

interface DashboardStats {
  totalClients: number;
  appointmentsToday: number;
  monthlyRevenue: number;
  totalServices: number;
}

interface DatabaseStatus {
  PostgreSQL: {
    Available: boolean;
    IsPrimary: boolean;
    ConnectionString: string;
  };
  SqlServer: {
    Available: boolean;
    IsPrimary: boolean;
    ConnectionString: string;
  };
  Recommendations: string[];
}

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  stats: DashboardStats = {
    totalClients: 0,
    appointmentsToday: 0,
    monthlyRevenue: 0,
    totalServices: 0
  };

  databaseStatus: DatabaseStatus | null = null;
  upcomingAppointments: any[] = [];
  loading = true;

  constructor(private apiService: ApiService) {}

  ngOnInit() {
    this.loadDashboardData();
  }

  loadDashboardData() {
    this.loading = true;
    
    // Load stats
    this.loadStats();
    
    // Load database status
    this.loadDatabaseStatus();
    
    // Load upcoming appointments
    this.loadUpcomingAppointments();
  }

  private loadStats() {
    // Load clients count
    this.apiService.get<any[]>('/api/clients').subscribe({
      next: (clients) => {
        this.stats.totalClients = clients.length;
      },
      error: (error) => console.error('Error loading clients:', error)
    });

    // Load services count
    this.apiService.get<any[]>('/api/services').subscribe({
      next: (services) => {
        this.stats.totalServices = services.filter(s => s.isActive).length;
      },
      error: (error) => console.error('Error loading services:', error)
    });

    // Load appointments today
    const today = new Date().toISOString().split('T')[0];
    this.apiService.get<any[]>('/api/appointments').subscribe({
      next: (appointments) => {
        this.stats.appointmentsToday = appointments.filter(a => 
          a.scheduledDate.startsWith(today)
        ).length;
      },
      error: (error) => console.error('Error loading appointments:', error)
    });

    // Load monthly revenue
    this.apiService.get<any[]>('/api/financial').subscribe({
      next: (transactions) => {
        const currentMonth = new Date().getMonth();
        const currentYear = new Date().getFullYear();
        
        this.stats.monthlyRevenue = transactions
          .filter(t => {
            const transactionDate = new Date(t.transactionDate);
            return transactionDate.getMonth() === currentMonth && 
                   transactionDate.getFullYear() === currentYear &&
                   t.type === 'income';
          })
          .reduce((sum, t) => sum + t.amount, 0);
      },
      error: (error) => console.error('Error loading financial data:', error)
    });
  }

  private loadDatabaseStatus() {
    this.apiService.get<DatabaseStatus>('/api/database/status').subscribe({
      next: (status) => {
        this.databaseStatus = status;
      },
      error: (error) => {
        console.error('Error loading database status:', error);
        this.databaseStatus = null;
      }
    });
  }

  private loadUpcomingAppointments() {
    this.apiService.get<any[]>('/api/appointments').subscribe({
      next: (appointments) => {
        const now = new Date();
        this.upcomingAppointments = appointments
          .filter(a => new Date(a.scheduledDate) > now && a.status !== 'cancelled')
          .sort((a, b) => new Date(a.scheduledDate).getTime() - new Date(b.scheduledDate).getTime())
          .slice(0, 5);
        
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading upcoming appointments:', error);
        this.loading = false;
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
    return new Date(dateString).toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getDatabaseStatusClass(available: boolean): string {
    return available ? 'text-success' : 'text-danger';
  }

  getDatabaseStatusIcon(available: boolean): string {
    return available ? 'fas fa-check-circle' : 'fas fa-times-circle';
  }
}