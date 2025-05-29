import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface BeforeAfterCase {
  id: number;
  patientId: number;
  title: string;
  description: string;
  procedure: string;
  beforeImages: string[];
  afterImages: string[];
  datePerformed: Date;
  isPublic: boolean;
  patientConsent: boolean;
  createdAt: Date;
}

export interface CreateBeforeAfterCase {
  patientId: number;
  title: string;
  description: string;
  procedure: string;
  beforeImages: string[];
  afterImages: string[];
  datePerformed: Date;
  isPublic?: boolean;
  patientConsent: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class BeforeAfterService {
  private apiUrl = '/api/before-after';

  constructor(private http: HttpClient) {}

  getCases(): Observable<BeforeAfterCase[]> {
    return this.http.get<BeforeAfterCase[]>(this.apiUrl);
  }

  getCase(id: number): Observable<BeforeAfterCase> {
    return this.http.get<BeforeAfterCase>(`${this.apiUrl}/${id}`);
  }

  createCase(caseData: CreateBeforeAfterCase): Observable<BeforeAfterCase> {
    return this.http.post<BeforeAfterCase>(this.apiUrl, caseData);
  }

  updateCase(id: number, caseData: Partial<CreateBeforeAfterCase>): Observable<BeforeAfterCase> {
    return this.http.put<BeforeAfterCase>(`${this.apiUrl}/${id}`, caseData);
  }

  deleteCase(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}