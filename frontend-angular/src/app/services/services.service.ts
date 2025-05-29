import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Service {
  id: number;
  name: string;
  category: string;
  description: string;
  duration: number;
  price: string;
  active: boolean;
}

export interface CreateService {
  name: string;
  category: string;
  description: string;
  duration: number;
  price: string;
  active?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class ServicesService {
  private apiUrl = '/api/services';

  constructor(private http: HttpClient) {}

  getServices(): Observable<Service[]> {
    return this.http.get<Service[]>(this.apiUrl);
  }

  getService(id: number): Observable<Service> {
    return this.http.get<Service>(`${this.apiUrl}/${id}`);
  }

  createService(service: CreateService): Observable<Service> {
    return this.http.post<Service>(this.apiUrl, service);
  }

  updateService(id: number, service: Partial<CreateService>): Observable<Service> {
    return this.http.put<Service>(`${this.apiUrl}/${id}`, service);
  }

  deleteService(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}