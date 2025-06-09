import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class InventoryService {
  private apiUrl = 'http://localhost:5000/api/inventory';

  constructor(private http: HttpClient) { }

  getAll(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  }

  getById(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }

  create(item: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, item);
  }

  update(id: number, item: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}`, item);
  }

  delete(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }

  getLowStock(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/low-stock`);
  }

  updateStock(id: number, quantity: number): Observable<any> {
    return this.http.patch<any>(`${this.apiUrl}/${id}/stock`, { quantity });
  }
}