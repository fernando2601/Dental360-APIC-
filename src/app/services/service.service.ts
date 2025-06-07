import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ServiceService {
  private apiUrl = '/api/services';

  constructor(private http: HttpClient) {}

  getServices(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getService(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  createService(service: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, service);
  }

  updateService(id: number, service: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, service);
  }

  deleteService(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  getServiceStats(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/stats`);
  }
}