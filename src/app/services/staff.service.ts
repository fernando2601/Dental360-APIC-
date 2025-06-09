import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class StaffService {
  private apiUrl = 'http://localhost:5000/api/staff';

  constructor(private http: HttpClient) { }

  getAll(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  create(staff: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, staff);
  }

  update(id: number, staff: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, staff);
  }

  delete(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }

  getByRole(role: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/role/${role}`);
  }
}