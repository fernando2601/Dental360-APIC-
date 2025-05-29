import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DashboardMetrics {
  totalPatients: number;
  appointmentsToday: number;
  monthlyRevenue: number;
  activeSubscriptions: number;
  inventoryAlerts: number;
  popularServices: Array<{
    name: string;
    count: number;
    revenue: string;
  }>;
  recentActivity: Array<{
    type: string;
    description: string;
    date: Date;
  }>;
}

export interface AppointmentReport {
  id: number;
  date: Date;
  patientName: string;
  serviceName: string;
  staffName: string;
  status: string;
  duration: number;
  revenue: string;
}

export interface FilterOptions {
  services: Array<{ id: number; name: string }>;
  staff: Array<{ id: number; name: string }>;
  statuses: string[];
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private apiUrl = '/api/dashboard';

  constructor(private http: HttpClient) {}

  getMetrics(): Observable<DashboardMetrics> {
    return this.http.get<DashboardMetrics>(`${this.apiUrl}/metrics`);
  }

  getAppointmentReports(params?: any): Observable<AppointmentReport[]> {
    return this.http.get<AppointmentReport[]>('/api/appointment-reports', { params });
  }

  getFilterOptions(): Observable<FilterOptions> {
    return this.http.get<FilterOptions>('/api/filter-options');
  }
}