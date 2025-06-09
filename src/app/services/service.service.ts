import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ServiceService {
  private apiUrl = 'http://localhost:5000/api/services';

  constructor(private http: HttpClient) { }

  getAll(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  create(service: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, service);
  }

  update(id: number, service: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, service);
  }

  delete(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }

  getByCategory(category: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/category/${category}`);
  }
}