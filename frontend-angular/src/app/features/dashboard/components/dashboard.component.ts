import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subject, takeUntil } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';
import { DashboardMetrics, User, RecentActivity, UpcomingAppointment, QuickStat } from '../../../core/models/auth.model';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  currentUser: User | null = null;
  dashboardMetrics: DashboardMetrics | null = null;
  loading = true;
  error = '';

  // Métricas rápidas para exibição
  quickMetrics = {
    todayAppointments: 0,
    todayRevenue: 0,
    pendingAppointments: 0,
    lowStockAlerts: 0
  };

  recentActivities: RecentActivity[] = [];
  todayAppointments: UpcomingAppointment[] = [];
  quickStats: QuickStat[] = [];

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.loadCurrentUser();
    this.loadDashboardData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadCurrentUser(): void {
    this.authService.currentUser$
      .pipe(takeUntil(this.destroy$))
      .subscribe(user => {
        this.currentUser = user;
      });
  }

  private loadDashboardData(): void {
    this.loading = true;
    this.error = '';

    this.authService.getDashboardMetrics()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (metrics) => {
          this.dashboardMetrics = metrics;
          this.updateQuickMetrics(metrics);
          this.recentActivities = metrics.recentActivities || [];
          this.todayAppointments = metrics.todayAppointmentsList || [];
          this.quickStats = metrics.quickStats || [];
          this.loading = false;
        },
        error: (error) => {
          this.error = 'Erro ao carregar dados do dashboard';
          this.loading = false;
          console.error('Dashboard error:', error);
        }
      });
  }

  private updateQuickMetrics(metrics: DashboardMetrics): void {
    this.quickMetrics = {
      todayAppointments: metrics.todayAppointments,
      todayRevenue: metrics.todayRevenue,
      pendingAppointments: metrics.pendingAppointments,
      lowStockAlerts: metrics.lowStockAlerts
    };
  }

  refreshDashboard(): void {
    this.loadDashboardData();
  }

  getGreeting(): string {
    const hour = new Date().getHours();
    const name = this.currentUser?.fullName || 'Usuário';
    
    if (hour < 12) {
      return `Bom dia, ${name}!`;
    } else if (hour < 18) {
      return `Boa tarde, ${name}!`;
    } else {
      return `Boa noite, ${name}!`;
    }
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

  formatTime(dateString: string): string {
    return new Date(dateString).toLocaleTimeString('pt-BR', {
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getStatusBadgeClass(status: string): string {
    const statusClasses: { [key: string]: string } = {
      'confirmed': 'bg-green-100 text-green-800',
      'pending': 'bg-yellow-100 text-yellow-800',
      'cancelled': 'bg-red-100 text-red-800',
      'completed': 'bg-blue-100 text-blue-800'
    };
    return statusClasses[status] || 'bg-gray-100 text-gray-800';
  }

  getChangeTypeClass(changeType: string): string {
    const changeClasses: { [key: string]: string } = {
      'increase': 'text-green-600',
      'decrease': 'text-red-600',
      'neutral': 'text-gray-600'
    };
    return changeClasses[changeType] || 'text-gray-600';
  }

  getActivityIcon(type: string): string {
    const icons: { [key: string]: string } = {
      'appointment': 'calendar',
      'payment': 'dollar-sign',
      'patient': 'user-plus',
      'inventory': 'package'
    };
    return icons[type] || 'activity';
  }

  onQuickAction(action: string): void {
    switch (action) {
      case 'new-appointment':
        // Navegar para nova consulta
        break;
      case 'view-patients':
        // Navegar para pacientes
        break;
      case 'check-inventory':
        // Navegar para estoque
        break;
      case 'view-finances':
        // Navegar para financeiro
        break;
      default:
        break;
    }
  }

  hasPermission(permission: string): boolean {
    if (!this.currentUser) return false;
    
    // Verificações de permissão baseadas no role
    switch (permission) {
      case 'view-finances':
        return ['admin', 'manager'].includes(this.currentUser.role);
      case 'manage-users':
        return this.currentUser.role === 'admin';
      case 'manage-inventory':
        return ['admin', 'manager', 'staff'].includes(this.currentUser.role);
      default:
        return true;
    }
  }
}