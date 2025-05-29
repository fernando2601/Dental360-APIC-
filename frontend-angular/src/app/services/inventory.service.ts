import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface InventoryItem {
  id: number;
  name: string;
  category: string;
  description: string | null;
  price: string | null;
  quantity: number;
  unit: string;
  threshold: number | null;
  lastRestocked: Date | null;
}

export interface CreateInventoryItem {
  name: string;
  category: string;
  description?: string;
  price?: string;
  quantity: number;
  unit: string;
  threshold?: number;
  lastRestocked?: Date;
}

@Injectable({
  providedIn: 'root'
})
export class InventoryService {
  private apiUrl = '/api/inventory';

  constructor(private http: HttpClient) {}

  getInventoryItems(): Observable<InventoryItem[]> {
    return this.http.get<InventoryItem[]>(this.apiUrl);
  }

  getInventoryItem(id: number): Observable<InventoryItem> {
    return this.http.get<InventoryItem>(`${this.apiUrl}/${id}`);
  }

  createInventoryItem(item: CreateInventoryItem): Observable<InventoryItem> {
    return this.http.post<InventoryItem>(this.apiUrl, item);
  }

  updateInventoryItem(id: number, item: Partial<CreateInventoryItem>): Observable<InventoryItem> {
    return this.http.put<InventoryItem>(`${this.apiUrl}/${id}`, item);
  }

  deleteInventoryItem(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}