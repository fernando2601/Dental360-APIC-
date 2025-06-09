import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FinancialService {
  private apiUrl = 'http://localhost:5000/api/financial';

  constructor(private http: HttpClient) { }

  getSummary(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/summary`);
  }

  getTransactions(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/transactions`);
  }

  createTransaction(transaction: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/transactions`, transaction);
  }

  getRevenue(period: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/revenue/${period}`);
  }

  getExpenses(period: string): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/expenses/${period}`);
  }
}