import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Staff {
  id: number;
  userId: number;
  specialization: string;
  bio: string | null;
  available: boolean;
}

export interface CreateStaff {
  userId: number;
  specialization: string;
  bio?: string;
  available?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class StaffService {
  private apiUrl = '/api/staff';

  constructor(private http: HttpClient) {}

  getStaffMembers(): Observable<Staff[]> {
    return this.http.get<Staff[]>(this.apiUrl);
  }

  getStaffMember(id: number): Observable<Staff> {
    return this.http.get<Staff>(`${this.apiUrl}/${id}`);
  }

  createStaffMember(staff: CreateStaff): Observable<Staff> {
    return this.http.post<Staff>(this.apiUrl, staff);
  }

  updateStaffMember(id: number, staff: Partial<CreateStaff>): Observable<Staff> {
    return this.http.put<Staff>(`${this.apiUrl}/${id}`, staff);
  }

  deleteStaffMember(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}