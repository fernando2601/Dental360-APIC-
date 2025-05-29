import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface FinancialTransaction {
  id: number;
  date: Date;
  type: string;
  category: string;
  description: string | null;
  clientId: number | null;
  appointmentId: number | null;
  amount: string;
  paymentMethod: string | null;
}

export interface CreateFinancialTransaction {
  date: Date;
  type: string;
  category: string;
  description?: string;
  clientId?: number;
  appointmentId?: number;
  amount: string;
  paymentMethod?: string;
}

@Injectable({
  providedIn: 'root'
})
export class FinanceService {
  private apiUrl = '/api/financial-transactions';

  constructor(private http: HttpClient) {}

  getTransactions(): Observable<FinancialTransaction[]> {
    return this.http.get<FinancialTransaction[]>(this.apiUrl);
  }

  getTransaction(id: number): Observable<FinancialTransaction> {
    return this.http.get<FinancialTransaction>(`${this.apiUrl}/${id}`);
  }

  createTransaction(transaction: CreateFinancialTransaction): Observable<FinancialTransaction> {
    return this.http.post<FinancialTransaction>(this.apiUrl, transaction);
  }

  updateTransaction(id: number, transaction: Partial<CreateFinancialTransaction>): Observable<FinancialTransaction> {
    return this.http.put<FinancialTransaction>(`${this.apiUrl}/${id}`, transaction);
  }

  deleteTransaction(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}