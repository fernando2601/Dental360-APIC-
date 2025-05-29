import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AppointmentService {
  private apiUrl = 'http://localhost:5000/api';

  constructor(private http: HttpClient) {}

  getAppointmentReports(
    startDate?: Date | null,
    endDate?: Date | null,
    statuses?: string[],
    professionalId?: number | null,
    clientId?: number | null,
    convenio?: string | null,
    sala?: string | null,
    page: number = 1,
    limit: number = 25
  ): Observable<any> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('limit', limit.toString());

    if (startDate) {
      params = params.set('startDate', startDate.toISOString());
    }
    if (endDate) {
      params = params.set('endDate', endDate.toISOString());
    }
    if (statuses && statuses.length > 0) {
      statuses.forEach(status => {
        params = params.append('status', status);
      });
    }
    if (professionalId) {
      params = params.set('professionalId', professionalId.toString());
    }
    if (clientId) {
      params = params.set('clientId', clientId.toString());
    }
    if (convenio) {
      params = params.set('convenio', convenio);
    }
    if (sala) {
      params = params.set('sala', sala);
    }

    return this.http.get(`${this.apiUrl}/appointments/reports`, { params });
  }

  downloadReport(format: string, filters: any): Observable<Blob> {
    let params = new HttpParams().set('format', format);

    if (filters.startDate) {
      params = params.set('startDate', filters.startDate.toISOString());
    }
    if (filters.endDate) {
      params = params.set('endDate', filters.endDate.toISOString());
    }
    if (filters.status && filters.status.length > 0) {
      filters.status.forEach((status: string) => {
        params = params.append('status', status);
      });
    }
    if (filters.professionalId) {
      params = params.set('professionalId', filters.professionalId.toString());
    }

    return this.http.get(`${this.apiUrl}/appointments/reports/download`, {
      params,
      responseType: 'blob'
    });
  }

  getAllAppointments(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/appointments`);
  }

  getAppointmentById(id: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/appointments/${id}`);
  }

  createAppointment(appointment: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/appointments`, appointment);
  }

  updateAppointment(id: number, appointment: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/appointments/${id}`, appointment);
  }

  deleteAppointment(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/appointments/${id}`);
  }
}