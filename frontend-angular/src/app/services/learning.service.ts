import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface LearningContent {
  id: number;
  title: string;
  description: string;
  content: string;
  category: string;
  imageUrl: string | null;
  videoUrl: string | null;
  tags: string[];
  isPublic: boolean;
  createdAt: Date;
}

export interface CreateLearningContent {
  title: string;
  description: string;
  content: string;
  category: string;
  imageUrl?: string;
  videoUrl?: string;
  tags: string[];
  isPublic?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class LearningService {
  private apiUrl = '/api/learning';

  constructor(private http: HttpClient) {}

  getLearningContent(): Observable<LearningContent[]> {
    return this.http.get<LearningContent[]>(this.apiUrl);
  }

  getLearningContentById(id: number): Observable<LearningContent> {
    return this.http.get<LearningContent>(`${this.apiUrl}/${id}`);
  }

  createLearningContent(content: CreateLearningContent): Observable<LearningContent> {
    return this.http.post<LearningContent>(this.apiUrl, content);
  }

  updateLearningContent(id: number, content: Partial<CreateLearningContent>): Observable<LearningContent> {
    return this.http.put<LearningContent>(`${this.apiUrl}/${id}`, content);
  }

  deleteLearningContent(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}