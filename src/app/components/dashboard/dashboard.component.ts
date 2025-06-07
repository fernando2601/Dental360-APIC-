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
    
    // Load stats from the dedicated endpoint
    this.apiService.get<DashboardStats>('/dashboard/stats').subscribe({
      next: (stats) => {
        this.stats = stats;
      },
      error: (error) => console.error('Error loading dashboard stats:', error)
    });
    
    // Load database status
    this.loadDatabaseStatus();
    
    // Load upcoming appointments
    this.loadUpcomingAppointments();
  }

  private loadDatabaseStatus() {
    this.apiService.get<DatabaseStatus>('/database/status').subscribe({
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
    this.apiService.get<any[]>('/appointments/upcoming').subscribe({
      next: (appointments) => {
        this.upcomingAppointments = appointments.slice(0, 5);
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