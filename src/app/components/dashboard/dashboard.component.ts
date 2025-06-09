import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  stats = {
    totalPatients: 0,
    todayAppointments: 0,
    totalServices: 0,
    monthlyRevenue: 0,
    occupancyRate: 85,
    averageTicket: 0,
    activeStaff: 0,
    satisfaction: 92
  };

  recentAppointments: any[] = [];
  loading = true;

  constructor(private apiService: ApiService) { }

  ngOnInit() {
    this.loadDashboardData();
  }

  loadDashboardData() {
    this.loading = true;
    
    // Load patients count
    this.apiService.get('/patients').subscribe({
      next: (patients: any[]) => {
        this.stats.totalPatients = patients.length;
      },
      error: (error) => console.error('Error loading patients:', error)
    });

    // Load services count
    this.apiService.get('/services').subscribe({
      next: (services: any[]) => {
        this.stats.totalServices = services.length;
      },
      error: (error) => console.error('Error loading services:', error)
    });

    // Load staff count
    this.apiService.get('/staff').subscribe({
      next: (staff: any[]) => {
        this.stats.activeStaff = staff.filter(s => s.is_active).length;
      },
      error: (error) => console.error('Error loading staff:', error)
    });

    // Load appointments with today's count
    this.apiService.get('/appointments').subscribe({
      next: (appointments: any[]) => {
        const today = new Date().toISOString().split('T')[0];
        this.stats.todayAppointments = appointments.filter(apt => 
          apt.appointment_date.startsWith(today)
        ).length;
        
        // Get recent appointments (next 5)
        this.recentAppointments = appointments
          .filter(apt => new Date(apt.appointment_date) >= new Date())
          .sort((a, b) => new Date(a.appointment_date).getTime() - new Date(b.appointment_date).getTime())
          .slice(0, 5);
      },
      error: (error) => console.error('Error loading appointments:', error)
    });

    // Load financial summary
    this.apiService.get('/financial/summary').subscribe({
      next: (summary: any) => {
        this.stats.monthlyRevenue = summary.monthlyRevenue || 0;
        this.stats.averageTicket = summary.averageTicket || 0;
      },
      error: (error) => console.error('Error loading financial summary:', error)
    });

    this.loading = false;
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('pt-BR', {
      day: '2-digit',
      month: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getStatusClass(status: string): string {
    return 'status-badge ' + status;
  }

  getStatusLabel(status: string): string {
    const statusLabels: { [key: string]: string } = {
      'scheduled': 'Agendado',
      'confirmed': 'Confirmado',
      'completed': 'Concluído',
      'cancelled': 'Cancelado'
    };
    return statusLabels[status] || status;
  }
}