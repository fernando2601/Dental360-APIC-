import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Patient {
  id: number;
  fullName: string;
  email: string;
  phone: string;
  address: string | null;
  birthday: string | null;
  notes: string | null;
  createdAt: Date;
}

export interface CreatePatient {
  fullName: string;
  email: string;
  phone: string;
  address?: string;
  birthday?: string;
  notes?: string;
}

@Injectable({
  providedIn: 'root'
})
export class PatientsService {
  private apiUrl = '/api/clients';

  constructor(private http: HttpClient) {}

  getPatients(): Observable<Patient[]> {
    return this.http.get<Patient[]>(this.apiUrl);
  }

  getPatient(id: number): Observable<Patient> {
    return this.http.get<Patient>(`${this.apiUrl}/${id}`);
  }

  createPatient(patient: CreatePatient): Observable<Patient> {
    return this.http.post<Patient>(this.apiUrl, patient);
  }

  updatePatient(id: number, patient: Partial<CreatePatient>): Observable<Patient> {
    return this.http.put<Patient>(`${this.apiUrl}/${id}`, patient);
  }

  deletePatient(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}