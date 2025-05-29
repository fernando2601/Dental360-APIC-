import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Subscription {
  id: number;
  clientId: number;
  packageId: number;
  startDate: Date;
  endDate: Date;
  status: string;
  renewalDate: Date | null;
  paymentMethod: string | null;
  createdAt: Date;
}

export interface CreateSubscription {
  clientId: number;
  packageId: number;
  startDate: Date;
  endDate: Date;
  status?: string;
  renewalDate?: Date;
  paymentMethod?: string;
}

@Injectable({
  providedIn: 'root'
})
export class SubscriptionsService {
  private apiUrl = '/api/subscriptions';

  constructor(private http: HttpClient) {}

  getSubscriptions(): Observable<Subscription[]> {
    return this.http.get<Subscription[]>(this.apiUrl);
  }

  getSubscription(id: number): Observable<Subscription> {
    return this.http.get<Subscription>(`${this.apiUrl}/${id}`);
  }

  createSubscription(subscription: CreateSubscription): Observable<Subscription> {
    return this.http.post<Subscription>(this.apiUrl, subscription);
  }

  updateSubscription(id: number, subscription: Partial<CreateSubscription>): Observable<Subscription> {
    return this.http.put<Subscription>(`${this.apiUrl}/${id}`, subscription);
  }

  deleteSubscription(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}