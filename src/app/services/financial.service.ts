import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Transaction {
  id?: number;
  description: string;
  amount: number;
  type: 'receita' | 'despesa';
  category: string;
  date: string;
  notes?: string;
  created_at?: string;
}

export interface FinancialSummary {
  monthlyRevenue: number;
  monthlyExpenses: number;
  netProfit: number;
  profitMargin: number;
  todayRevenue: number;
  averageTicket: number;
}

@Injectable({
  providedIn: 'root'
})
export class FinancialService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getTransactions(): Observable<Transaction[]> {
    return this.http.get<Transaction[]>(`${this.apiUrl}/transactions`);
  }

  getTransaction(id: number): Observable<Transaction> {
    return this.http.get<Transaction>(`${this.apiUrl}/transactions/${id}`);
  }

  createTransaction(transaction: Transaction): Observable<Transaction> {
    return this.http.post<Transaction>(`${this.apiUrl}/transactions`, transaction);
  }

  updateTransaction(id: number, transaction: Transaction): Observable<Transaction> {
    return this.http.put<Transaction>(`${this.apiUrl}/transactions/${id}`, transaction);
  }

  deleteTransaction(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/transactions/${id}`);
  }

  getFinancialSummary(): Observable<FinancialSummary> {
    return this.http.get<FinancialSummary>(`${this.apiUrl}/financial/summary`);
  }

  getMonthlyReport(year: number, month: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/financial/monthly-report?year=${year}&month=${month}`);
  }

  getCashFlowData(period: string = 'month'): Observable<any> {
    return this.http.get(`${this.apiUrl}/financial/cash-flow?period=${period}`);
  }

  getRevenueByCategory(): Observable<any> {
    return this.http.get(`${this.apiUrl}/financial/revenue-by-category`);
  }

  exportTransactions(startDate: string, endDate: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/financial/export?start=${startDate}&end=${endDate}`, {
      responseType: 'blob'
    });
  }
}