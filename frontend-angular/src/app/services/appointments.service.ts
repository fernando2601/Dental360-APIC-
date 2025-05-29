import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Appointment {
  id: number;
  clientId: number;
  staffId: number;
  serviceId: number;
  startTime: Date;
  endTime: Date;
  status: string;
  notes: string | null;
  createdAt: Date;
}

export interface CreateAppointment {
  clientId: number;
  staffId: number;
  serviceId: number;
  startTime: Date;
  endTime: Date;
  status?: string;
  notes?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AppointmentsService {
  private apiUrl = '/api/appointments';

  constructor(private http: HttpClient) {}

  getAppointments(): Observable<Appointment[]> {
    return this.http.get<Appointment[]>(this.apiUrl);
  }

  getAppointment(id: number): Observable<Appointment> {
    return this.http.get<Appointment>(`${this.apiUrl}/${id}`);
  }

  createAppointment(appointment: CreateAppointment): Observable<Appointment> {
    return this.http.post<Appointment>(this.apiUrl, appointment);
  }

  updateAppointment(id: number, appointment: Partial<CreateAppointment>): Observable<Appointment> {
    return this.http.put<Appointment>(`${this.apiUrl}/${id}`, appointment);
  }

  deleteAppointment(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}